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
            public delegate void InputData_Delegate(ref AbstractData_Type input_data);

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows pass output data to the interface
            /// </summary>
            public delegate void OutputData_Delegate(ref AbstractData_Type ouput_data);

            /// <summary>
            /// Callback for visualizations to request suitable data
            /// </summary>
            public delegate object RequestDataCallback_Delegate(Libraries library);



            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                bool initilized = true;


                var y_values = new[] { 0.1, 0.2, 0.4, 0.8, 1.1, 1.5, 2.4, 4.6, 8.1, 11.7, 14.4, 16.0, 13.7, 10.1, 6.4, 3.5, 2.5, 1.4, 0.4, 0.1 };


                // SciChart
                var data = new SciChartData_Type();
                data.SeriesName = "data_series";
                for (int i = 0; i < y_values.Length; i++)
                {
                    var selector = new Selection_PointMetadata() { Index = i, IsSelected = false };
                    selector.PropertyChanged += scichart_metadata_selection_changed;
                    data.Append(y_values[i], selector);
                }
                _data_series.Add(Libraries.SciChart, data);


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
                    foreach (var data in _data_series)
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
                    _data_series.Clear();

                    _initilized = false;
                }
                return terminated;
            }


            public void InputDataCallback(ref AbstractData_Type input_data)
            {
                _input_data = input_data;
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

                if (_data_series.ContainsKey(library))
                {
                    return _data_series[library];
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data not available for library: " + library.ToString());
                }

                return null;
            }


            private void scichart_metadata_selection_changed(object sender, PropertyChangedEventArgs e)
            {
                var sender_selection = sender as Selection_PointMetadata;
                if (sender_selection == null)
                {
                    return;
                }
                //string property_name = e.PropertyName; // == "IsSelected"

                int i = sender_selection.Index;
                var dataseries = (SciChartData_Type)_data_series[Libraries.SciChart];
                using (dataseries.SuspendUpdates())
                {
                    dataseries.Update(i, dataseries[i], dataseries.Metadata[i]);
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private OutputData_Delegate _outputdata_callback = null;

            private AbstractData_Type _input_data = null;

            Dictionary<Libraries, object> _data_series = new Dictionary<Libraries, object>();
        }
    }
}
