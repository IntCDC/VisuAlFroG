using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Core.Utilities;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Charting.Visuals;
using Visualizations.Abstracts;
using System.Windows.Controls;
using System.Xml.Linq;
using Core.GUI;
using System.Runtime.Remoting.Contexts;
using SciChart.Core.Extensions;
using SciChart.Charting.Visuals.PointMarkers;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.PaletteProviders;



/*
 * SciChart renderable series interface
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class DataInterfaceSciChartSeries<DataType> : AbstractDataInterface
            where DataType : BaseRenderableSeries
        {
            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Set(object data_parent)
            {
                var parent = data_parent as SciChartSurface;
                if (parent == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Can not convert data parent parameter to required type");
                    return false;
                }

                var data = (List<DataType>)RequestDataCallback(typeof(List<DataType>));
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(DataType).FullName);
                    return false;
                }
                if (data.Count == 0)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data");
                    return false;
                }

                foreach (var data_series in data)
                {
                    data_series.Name = UniqueID.Generate();
                    parent.RenderableSeries.Add(data_series);
                }
                return true;
            }
        }
    }
}
