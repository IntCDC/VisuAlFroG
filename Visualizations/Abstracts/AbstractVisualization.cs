using System;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Core.Abstracts;
using Visualizations.Management;
using Core.GUI;



/*
 * Abstract Visualization 
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {
        public abstract class AbstractVisualization : AbstractContent
        {

            /* ------------------------------------------------------------------*/
            // properties

            /// <summary>
            /// All visualizations should be able to be used multiple times.
            /// </summary>
            public sealed override bool MultipleInstances { get { return true; } }


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// Visualizations need access to data of specific type.
            /// </summary>
            /// <param name="request_data_callback">Callback from data manager to request data of specific type.</param>
            public void SetRequestDataCallback(DataManager.RequestDataCallback_Delegate request_data_callback)
            {
                _request_data_callback = request_data_callback;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            protected DataManager.RequestDataCallback_Delegate _request_data_callback = null;
        }
    }
}
