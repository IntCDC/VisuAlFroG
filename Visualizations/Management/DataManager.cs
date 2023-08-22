using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Core.Abstracts;
using Core.Utilities;

using SciChart.Charting.Model.DataSeries;

using Visualizations.Interaction;



using AbstractData_Type = System.Collections.Generic.List<System.Collections.Generic.List<double>>;


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
            /// Callback for visualizations to request suiable data
            /// </summary>
            public delegate SciChart.Charting.Model.DataSeries.IDataSeries RequestDataCallback_Delegate();



            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                bool initilized = true;

                _data_series = new SciChart.Charting.Model.DataSeries.UniformXyDataSeries<double> { SeriesName = "data_series" };


                var yValues = new[] { 0.1, 0.2, 0.4, 0.8, 1.1, 1.5, 2.4, 4.6, 8.1, 11.7, 14.4, 16.0, 13.7, 10.1, 6.4, 3.5, 2.5, 1.4, 0.4, 0.1 };

                /// int data_count = yValues.Length;
                /// SelectedPointMetadata[] meta = Enumerable.Repeat(new SelectedPointMetadata() { IsSelected = false }, data_count).ToArray();
                /// new XyDataSeries<double, double>();

                for (int i = 0; i < yValues.Length; i++)
                {
                    // DataSeries for appending data
                    var selector = new SelectedPointMetadata() { IsSelected = false };
                    selector.PropertyChanged += SelectionChanged;
                    _data_series.Append(yValues[i], selector);
                }


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



                return executed;
            }


            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {



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


            private SciChart.Charting.Model.DataSeries.IDataSeries request_data()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                return _data_series;
            }


            protected void SelectionChanged(object sender, PropertyChangedEventArgs e)
            {
                var sender_selection = sender as SelectedPointMetadata;
                if (sender_selection != null)
                {
                    return;
                }
                string property_name = e.PropertyName;

                using (_data_series.SuspendUpdates())
                {
                    var data_count = _data_series.Count;
                    for (int i = 0; i < data_count; i++)
                    {
                        _data_series.Update(i, _data_series[i], _data_series.Metadata[i]);
                    }
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private OutputData_Delegate _outputdata_callback = null;
            private AbstractData_Type _input_data = null;

            /// DEBUG 
            private SciChart.Charting.Model.DataSeries.UniformXyDataSeries<double> _data_series = null;
        }
    }
}
