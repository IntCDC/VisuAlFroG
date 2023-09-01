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

        // data types
        public class SciChartUniformData_Type : SciChart.Charting.Model.DataSeries.UniformXyDataSeries<double> { }
        public class SciChartData_Type : SciChart.Charting.Model.DataSeries.XyDataSeries<double, double> { }


        public class DataManager : AbstractService
        {
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
            public delegate object RequestDataCallback_Delegate(Type t);



            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                _timer.Start();

                bool initilized = true;

                _data_x = new List<double>();
                _data_y = new List<double>();
                _data_meta = new List<Metadata>();
                _library_data = new Dictionary<Type, object>();

                // SciChart
                var scichart_uniformxy = new SciChartUniformData_Type();
                scichart_uniformxy.SeriesName = "SciChartUniformData_Type";
                _library_data.Add(typeof(SciChartUniformData_Type), scichart_uniformxy);

                var scichart_xy = new SciChartData_Type();
                scichart_xy.SeriesName = "SciChartData_Type";
                _library_data.Add(typeof(SciChartData_Type), scichart_xy);

                /// TODO Add more library data formats here ...


                _timer.Stop();
                _initilized = initilized;
                return _initilized;
            }


            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {
                    foreach (var data_type in _library_data)
                    {
                        if (data_type.Key == typeof(SciChartUniformData_Type))
                        {
                            ((SciChartUniformData_Type)data_type.Value).Clear();
                        }
                        else if (data_type.Key == typeof(SciChartData_Type))
                        {
                            ((SciChartData_Type)data_type.Value).Clear();
                        }
                        else
                        {
                            /// TODO Add more library data formats here ...
                            Log.Default.Msg(Log.Level.Warn, "Unknown data type: " + data_type.Key.FullName);
                        }
                    }
                    _library_data.Clear();

                    _initilized = false;
                }
                return terminated;
            }


            public void InputData(ref XYData_Type input_data)
            {
                _data_x.Clear();
                _data_y.Clear();
                _data_meta.Clear();
                if (input_data.Count == 0)
                {
                    Log.Default.Msg(Log.Level.Warn, "No input data available");
                    return;
                }
                _timer.Start();
                Log.Default.Msg(Log.Level.Debug, "Reading input data ...");

                // Copy input data

                //for (int x = 0; x < count_x; x++)
                //{
                /// DEBUG Taking only first value row
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
                //}
                if ((_data_x.Count != _data_y.Count) || (_data_x.Count != _data_meta.Count) || (_data_y.Count != _data_meta.Count))
                {
                    Log.Default.Msg(Log.Level.Warn, "Data count does not match");
                    return;
                }


                // Create library dependent data
                foreach (var data_type in _library_data)
                {
                    Log.Default.Msg(Log.Level.Debug, data_type.Key.FullName);

                    if (data_type.Key == typeof(SciChartUniformData_Type))
                    {
                        var data_series = (SciChartUniformData_Type)data_type.Value;
                        data_series.Clear();
                        for (int i = 0; i < _data_x.Count; i++)
                        {
                            data_series.Append(_data_y[i], _data_meta[i]);
                        }
                    }
                    else if (data_type.Key == typeof(SciChartData_Type))
                    {
                        var data_series = (SciChartData_Type)data_type.Value;
                        data_series.Clear();
                        for (int i = 0; i < _data_x.Count; i++)
                        {
                            data_series.Append(_data_x[i], _data_y[i], _data_meta[i]);
                        }
                    }
                    else
                    {
                        /// TODO Add more library data formats here ...
                        Log.Default.Msg(Log.Level.Warn, "Unknown data type: " + data_type.Key.FullName);
                    }
                }

                _timer.Stop();
                Log.Default.Msg(Log.Level.Debug, "... done.");
            }


            public void SetOutputDataCallback(OutputData_Delegate outputdata_callback)
            {
                _outputdata_callback = outputdata_callback;
            }


            public RequestDataCallback_Delegate GetRequestDataCallback()
            {
                return request_data;
            }


            private object request_data(Type t)
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                if (_library_data.ContainsKey(t))
                {
                    return _library_data[t];
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Requested data not available for given data type: " + t.FullName);
                }
                return null;
            }


            private void metadata_changed(object sender, PropertyChangedEventArgs e)
            {
                var sender_selection = sender as Metadata;
                if ((sender_selection == null) || (e.PropertyName != "IsSelected"))
                {
                    return;
                }
                int i = sender_selection.Index;

                foreach (var data_type in _library_data)
                {
                    if (data_type.Key == typeof(SciChartUniformData_Type))
                    {
                        var dataseries = (SciChartUniformData_Type)_library_data[data_type.Key];
                        using (dataseries.SuspendUpdates())
                        {
                            dataseries.Update(i, _data_y[i], dataseries.Metadata[i]);
                        }
                    }
                    else if (data_type.Key == typeof(SciChartData_Type))
                    {
                        var dataseries = (SciChartData_Type)_library_data[data_type.Key];
                        using (dataseries.SuspendUpdates())
                        {
                            dataseries.Update(i, _data_y[i], dataseries.Metadata[i]);
                        }
                    }
                    else
                    {
                        /// TODO Add more library data formats here ...
                        Log.Default.Msg(Log.Level.Warn, "Unknown data type: " + data_type.Key.FullName);
                    }
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
            private Dictionary<Type, object> _library_data = null;
        }
    }
}
