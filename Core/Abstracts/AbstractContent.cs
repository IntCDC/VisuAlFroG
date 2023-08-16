using System;
using System.Windows.Controls;

using Core.Utilities;



/*
 * Abstract Child Content
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractContent
        {

            /* ------------------------------------------------------------------*/
            // public types

            public delegate void ContentAttachedCall(bool attached);


            /* ------------------------------------------------------------------*/
            // abstract functions

            public abstract bool AttachContent(Grid content_grid);


            /* ------------------------------------------------------------------*/
            // public functions

            public AbstractContent(string header)
            {
                _header = header;
                _id = header.Replace(" ", "_") + UniqueStringID.Generate(); ;
            }


            public string ID()
            {
                return _id;
            }

            public string Header()
            {
                return _header;
            }


            public bool IsAttached()
            {
                return _attached;
            }

            public void ContendAttached(bool a)
            {
                _attached = a;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private readonly string _header;
            private readonly string _id;
            private bool _attached = false;
        }
    }
}
