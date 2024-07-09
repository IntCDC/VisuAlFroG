using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Core.Abstracts;
using Core.Utilities;
using SciChart.Charting.Visuals;
using System.Windows;
using Core.Data;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChartInterface.Data;
using System.Windows.Media;
using System.Globalization;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace SciChartInterface
{
    namespace Abstracts
    {
        public abstract class AbstractSciChartVisualization<SurfaceType> : AbstractVisualization
            where SurfaceType : SciChartSurface, new()

        {
            /* ------------------------------------------------------------------*/
            #region public properties

            public sealed override bool _MultipleInstances { get { return true; } }
            public sealed override List<Type> _DependingServices { get { return new List<Type>() { typeof(SciChartInterfaceService) }; } }

            protected SurfaceType _Content { get { return _content_surface; } }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractSciChartVisualization(int uid) : base(uid) { }

            public override bool Initialize(DataManager.GetSpecificDataCallback_Delegate request_data_callback, DataManager.GetDataMenuCallback_Delegate request_menu_callback)
            {
                _timer.Start();

                if (base.Initialize(request_data_callback, request_menu_callback))
                {
                    _content_surface = new SurfaceType();
                    _content_surface.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
                    _content_surface.BorderThickness = new Thickness(0.0, 0.0, 0.0, 0.0);

                    attach_child_content(_content_surface);

                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }

                _timer.Stop();
                return _initialized;
            }

            public sealed override UIElement GetUI()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }

                _content_surface.ChartModifier.IsAttached = true;

                return base.GetUI();
            }

            public override bool Terminate()
            {
                if (_created)
                {
                    _content_surface.ChartModifier.IsAttached = false;
                    _content_surface.Dispose();
                    _content_surface = null;

                    _created = false;
                }
                return base.Terminate();
            }

            public override void Update(bool new_data)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                    return;
                }

                if (new_data)
                {
                    apply_data(_Content);
                    _Content.ZoomExtents();
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected abstract bool apply_data(SciChartSurface data_parent);

            protected sealed override bool apply_data<DataParentType>(out DataParentType data_parent)
            {
                throw new InvalidOperationException("Call apply_data(SciChartSurface data_parent) instead.");
            }

            protected void data_point_mouse_event(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {
                var hitTestPoint = e.GetPosition(_Content.GridLinesPanel as UIElement);

                var x_value = (double)_Content.XAxis.GetDataValue(hitTestPoint.X);
                var y_value = (double)_Content.YAxis.GetDataValue(hitTestPoint.Y);

                var text_anno = new SciChart.Charting.Visuals.Annotations.TextAnnotation();
                text_anno.FontSize = 12;
                text_anno.Text = string.Format(CultureInfo.InvariantCulture, "X: {0:N2} Y: {1:N2}", x_value, y_value);
                text_anno.X1 = x_value;
                text_anno.Y1 = y_value;
                text_anno.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33));
                text_anno.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x22, 0x22));
                text_anno.BorderThickness = new Thickness(1, 1, 1, 1);

                _Content.Annotations.Clear();
                _Content.Annotations.Add(text_anno);
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private SurfaceType _content_surface = null;

            #endregion
        }
    }
}
