using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Utilities;
using Core.Data;
using Core.Abstracts;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace Visualizations
{
    namespace WPFInterface
    {
        public abstract class AbstractGenericVisualization<ContentType> : AbstractVisualization
            where ContentType : System.Windows.Controls.Control, new()
        {
            /* ------------------------------------------------------------------*/
            // properties

            public sealed override bool MultipleInstances { get { return false; } }
            public sealed override List<Type> DependingServices { get { return new List<Type>() { }; } }

            protected ContentType Content { get; } = new ContentType();


            /* ------------------------------------------------------------------*/
            // public functions

            public override Type GetDataType()
            {
                return typeof(DataTypeGeneric);
            }

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
            public override bool ReCreate()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    // Log Console does not depend on data
                    Log.Default.Msg(Log.Level.Debug, "Content already created. Skipping re-creating content.");
                    return false;
                }
                _timer.Start();

                /// PLACE YOUR STUFF HERE ...

                _timer.Stop();
                _created = true;
                return _created;
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

            public override void Update(bool new_data)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                    return;
                }
                if (new_data)
                {
                    ReCreate();
                }
                else
                {
                    UpdateData();
                }
            }

            /// <summary>
            /// --- Default implementation ---
            /// Called when existing data has been updated
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public virtual bool UpdateData()
            {
                return true;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data_parent"></param>
            /// <returns></returns>
            public override bool GetData(ref GenericDataStructure data_parent)
            {
                var data = (GenericDataStructure)RequestDataCallback(GetDataType());
                if (data == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing data for: " + typeof(GenericDataStructure).FullName);
                    return false;
                }

                data_parent = data;
                return true;
            }

        }
    }
}
