using System;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Core.Abstracts;
using Visualizations.Management;



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

            public sealed override bool MultipleIntances
            {
                get { return true; }
            }


            /* ------------------------------------------------------------------*/
            // public functions

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
