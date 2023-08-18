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
            // static variables

            // DECLARE IN DERIVED CLASS
            //public static readonly string name = "...";

            public static readonly bool multiple_instances = true;


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool AttachContent(Grid content_element)
            {
                return false;
            }


            /* ------------------------------------------------------------------*/
            // protected variables


        }
    }
}
