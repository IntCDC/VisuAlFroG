using System;
using System.Windows;
using System.Windows.Controls;
using SciChart.Charting.Visuals;
using Core.Utilities;
using Core.Abstracts;
using SciChart.Charting.ChartModifiers;
using SciChart.Core.Utility.Mouse;



/*
 * SciChart custom chart modifier 
 * 
 */
namespace SciChartInterface
{
    public class SciChartCustomModifier : ChartModifierBase
    {

        public override void OnModifierDoubleClick(ModifierMouseArgs e)
        {
            base.OnModifierDoubleClick(e);

            base.ParentSurface.AnimateZoomExtents(TimeSpan.FromMilliseconds(500));
        }

        public override void OnModifierMouseUp(ModifierMouseArgs e)
        {
            base.OnModifierMouseUp(e);

            if (e.MouseButtons == MouseButtons.Left) {

                var surface = base.ParentSurface;
                var data_point = surface.RootGrid.TranslatePoint(e.MousePoint, surface.ModifierSurface);
                string text = "Relative Mouse Position: " + data_point.ToString();
                Log.Default.Msg(Log.Level.Warn, text);
            }
        }
    }
}
