using System;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;



/*
 * Abstract Visualization 
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractVisualization : AbstractContent
        {

            /* ------------------------------------------------------------------*/
            // properties

            public override bool MultipleIntances
            {
                get { return true; }
            }


            /* ------------------------------------------------------------------*/
            // public functions




            /* ------------------------------------------------------------------*/
            // protected variables



        }
    }
}
