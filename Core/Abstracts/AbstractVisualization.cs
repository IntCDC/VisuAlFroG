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
            public AbstractVisualization(string name) : base(name)
            {
            }

            public override void ProvideContent(Grid grid) { }

        }
    }
}
