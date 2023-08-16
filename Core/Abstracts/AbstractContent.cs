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

            public delegate bool DetachContentCall();


            /* ------------------------------------------------------------------*/
            // abstract functions

            /// <summary>
            /// Attach content to provided content_grid.
            ///  Set "_attached=true" when attached successfully.
            /// </summary>
            public abstract bool AttachContent(Grid content_grid);


            /* ------------------------------------------------------------------*/
            // virtual functions

            public virtual bool DetachContent()
            {
                _attached = false;
                return true;
            }


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

            /* ------------------------------------------------------------------*/
            // private variables

            protected bool _attached = false;


            /* ------------------------------------------------------------------*/
            // private variables

            private readonly string _header;
            private readonly string _id;
            
        }
    }
}
