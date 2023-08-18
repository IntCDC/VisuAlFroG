using System;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using System.Windows.Media;



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
            // public functions

            public AbstractVisualization() : base()
            {

            }


            public override bool AttachContent(Grid content_element)
            {
                return false;
            }

        }
    }
}
