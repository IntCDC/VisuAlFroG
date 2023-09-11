using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciChart.Charting.Visuals;
using Visualizations.Data;
using System.Windows;



/*
 * Abstract data interface
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {

        public enum DataStyles
        {
            None,
            Lines,
            Columns,
            Points
        }


        public abstract class AbstractDataInterface
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public DataStyles DataStyle { get; set; }

            public DataManager.RequestCallback_Delegate RequestDataCallback { get; set; }


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// TODO
            /// </summary>
            public virtual bool Set(object data_parent)
            {
                throw new InvalidOperationException("Call Set() method of derived class");
            }
            public virtual bool Set(ref GenericDataStructure data_parent)
            {
                throw new InvalidOperationException("Call Set() method of derived class");
            }

            /// <summary>
            /// TODO 
            /// </summary>
            protected virtual Style GetDataStyle()
            {
                return null;
            }
        }
    }
}
