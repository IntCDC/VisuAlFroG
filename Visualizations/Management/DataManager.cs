using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Abstracts;
using Core.Utilities;
using SciChart.Charting.Model.DataSeries;
using Visualizations.Interaction;
using Visualizations.SciChartInterface;
using Visualizations.WebAPI;
using Visualizations.PythonInterface;
using System.Windows.Markup;
using SciChart.Data.Model;



/*
 * Data Manager
 * 
 */
namespace Visualizations
{
    namespace Management
    {

        public class DataManager : AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public types

            public enum Libraries
            {
                SciChart,
                d3,
                Bokeh
            }


            /* ------------------------------------------------------------------*/
            // public delegates

            /// <summary>
            /// Function provided by the data manager for passng on the inpout data to the visualizations
            /// </summary>
            public delegate void InputData_Delegate(ref XYData_Type input_data);

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows pass output data to the interface
            /// </summary>
            public delegate void OutputData_Delegate(ref XYData_Type ouput_data);

            /// <summary>
            /// Callback for visualizations to request suitable data
            /// </summary>
            public delegate object RequestDataCallback_Delegate(Libraries library);



            /* ------------------------------------------------------------------*/
            // public functions

            public DataManager()
            {
                _data_x = new List<double>();
                _data_y = new List<double>();
                _data_meta = new List<Metadata>();
                _library_data = new Dictionary<Libraries, object>();
            }


            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                _timer.Start();

                bool initilized = true;

                // SciChart
                var data = new SciChartData_Type();
                data.SeriesName = "data_series";
                _library_data.Add(Libraries.SciChart, data);

                _timer.Stop();
                _initilized = initilized;
                return _initilized;
            }


            public override bool Execute()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                bool executed = true;

                // never called ...

                return executed;
            }


            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {
                    foreach (var data in _library_data)
                    {
                        switch (data.Key)
                        {
                            case (Libraries.SciChart):
                                ((SciChartData_Type)data.Value).Clear();
                                break;
                            case (Libraries.d3):
                                break;
                            case (Libraries.Bokeh):
                                ; break;
                        }
                    }
                    _library_data.Clear();

                    _initilized = false;
                }
                return terminated;
            }


            public void InputDataCallback(ref XYData_Type input_data)
            {
                _data_x.Clear();
                _data_y.Clear();
                _data_meta.Clear();


                /// DEBUG Taking only first value row
                var count_x = input_data.Count;
                if (count_x == 0)
                {
                    return;
                }
                ///for (int x = 0; x < count_x; x++)
                ///{
                int x = 0;
                var count_y = input_data[x].Count;
                for (int y = 0; y < count_y; y++)
                {
                    _data_x.Add(y);
                    _data_y.Add(input_data[x][y]);

                    var metadata = new Metadata() { Index = y, IsSelected = false };
                    metadata.PropertyChanged += metadata_changed;
                    _data_meta.Add(metadata);
                }
                ///}

                foreach (var data in _library_data)
                {
                    switch (data.Key)
                    {
                        case (Libraries.SciChart):
                            var data_series = (SciChartData_Type)data.Value;
                            data_series.Clear();
                            for (int i = 0; i < _data_y.Count; i++)
                            {
                                data_series.Append(_data_y[i], _data_meta[i]); // _data_x[i], for XyDataSeries<double, double>
                            }
                            break;
                        case (Libraries.d3):
                            break;
                        case (Libraries.Bokeh):
                            ; break;
                    }
                }
            }


            public void RegisterOutputDataCallback(OutputData_Delegate outputdata_callback)
            {
                _outputdata_callback = outputdata_callback;
            }


            public RequestDataCallback_Delegate RequestDataCallback()
            {
                return request_data;
            }


            private object request_data(Libraries library)
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                if (_library_data.ContainsKey(library))
                {
                    return _library_data[library];
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data not available for library: " + library.ToString());
                }
                return null;
            }


            private void metadata_changed(object sender, PropertyChangedEventArgs e)
            {
                var sender_selection = sender as Metadata;
                if (sender_selection == null)
                {
                    return;
                }
                //string property_name = e.PropertyName; // == "IsSelected"
                int i = sender_selection.Index;
                var dataseries = (SciChartData_Type)_library_data[Libraries.SciChart];
                using (dataseries.SuspendUpdates())
                {
                    dataseries.Update(i, _data_y[i], dataseries.Metadata[i]);
                }

                // Send changed output data
                if (_outputdata_callback != null)
                {
                    var out_data = new XYData_Type();
                    var list = new List<double>();
                    foreach (var metadata in _data_meta)
                    {
                        list.Add(metadata.IsSelected ? 1.0 : 0.0);
                    }
                    out_data.Add(list);
                    _outputdata_callback(ref out_data);
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private OutputData_Delegate _outputdata_callback = null;
            private List<double> _data_x = null;
            private List<double> _data_y = null;
            private List<Metadata> _data_meta = null;
            private Dictionary<Libraries, object> _library_data = null;
        }
    }
}
