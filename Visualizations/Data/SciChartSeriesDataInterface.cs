using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

using Core.Utilities;
using SciChart.Charting.Visuals.RenderableSeries;

using Visualizations.Interaction;
using SciChart.Charting.Visuals;



/*
 * SciChart Renderable Series Interface
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class SciChartSeriesDataInterface<DataType> : IDataInterface
            where DataType : BaseRenderableSeries, new()
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public DataManager.RequestCallback_Delegate RequestDataCallback { get; set; }


            /* ------------------------------------------------------------------*/
            // public functions

            public SciChartSeriesDataInterface()
            {
                _data = new List<DataType>();
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            public bool Set(object data_parent)
            {
                var parent = data_parent as SciChartSurface;
                if (parent == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing valid data parent");
                    return false;
                }

                _data = (List<DataType>)RequestDataCallback(typeof(List<DataType>));
                if (_data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing valid data");
                    return false;
                }
                foreach (var data_series in _data)
                {
                    parent.RenderableSeries.Add(data_series);
                }

                /// TODO Set style

                return true;
            }

            public void SetDataStyle(DataStyles style)
            {
                switch (style)
                {
                    case (DataStyles.Default):
                        {
                            foreach (var data_series in _data)
                            {
                                data_series.Name = UniqueID.Generate();

                                data_series.PaletteProvider = new Selection_StrokePaletteProvider();
                                data_series.PointMarker = Selection_PointMarker.Default;
                                data_series.SelectedPointMarker = Selection_PointMarker.Selected;

                                data_series.Stroke = Colors.Aquamarine;
                                data_series.StrokeThickness = 2;

                                /*
                                data_series.DataPointWidth = 0.5;
    
                                var gradient = new LinearGradientBrush();
                                gradient.StartPoint = new Point(0.0, 0.0);
                                gradient.EndPoint = new Point(1.0, 1.0);
                                gradient.GradientStops = new GradientStopCollection();
                                var gs1 = new GradientStop() { Color = Colors.Blue, Offset = 0.2 };
                                gradient.GradientStops.Add(gs1);
                                var gs2 = new GradientStop() { Color = Colors.Green, Offset = 0.8 };
                                gradient.GradientStops.Add(gs2);
                                data_series.Fill = gradient;
                                */
                            }
                        }
                        break;
                    default: break;
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private List<DataType> _data = null;
        }
    }
}
