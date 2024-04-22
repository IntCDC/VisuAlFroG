using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Utilities;
using Core.Data;
using Core.Abstracts;
using System.Windows;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace Visualizations
{
    namespace WPFInterface
    {
        public abstract class AbstractWPFVisualization<ContentType> : AbstractVisualization
            where ContentType : UIElement, new()
        {
            /* ------------------------------------------------------------------*/
            // properties

            public sealed override List<Type> DependingServices { get { return new List<Type>() { }; } }
            public sealed override Type RequiredDataType { get; } = typeof(DataTypeGeneric);

            protected ContentType Content { get; } = new ContentType();


            /* ------------------------------------------------------------------*/
            // public functions


            public override bool Initialize(DataManager.RequestCallback_Delegate request_callback)
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                this.RequestDataCallback = request_callback;
                AttachChildContent(Content);

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initialized;
            }

            /* TEMPLATE
            public override bool Create()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                _timer.Start();

                /// PLACE YOUR STUFF HERE ...

                _timer.Stop();
                _created = true;
                return _created;
            }
            */

            /* TEMPLATE
            public override void Update(bool new_data)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                    return;
                }

                if (new_data)
                {
                    // Re-creation of content is required for new data
                    Create();
                }
                else
                {
                    /// PLACE YOUR STUFF HERE ...
                }
            }
            */

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _initialized = false;
                }
                return base.Terminate();
            }
        }
    }
}
