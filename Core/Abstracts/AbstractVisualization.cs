﻿using System;
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

            public AbstractVisualization(string name) : base(name)
            {
            }

            public override bool AttachContent(Grid content_grid) { return false; }

        }
    }
}
