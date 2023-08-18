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
            // static variables

            // DECLARE IN DERIVED CLASS
            // -> public static new readonly ...
            public static readonly string name = "[uninitialized]";
            public static readonly bool multiple_instances = true;


            /* ------------------------------------------------------------------*/
            // public functions

            public AbstractContent()
            {
                _id = UniqueID.Generate();
            }


            /// <summary>
            /// Attach content to provided content_element.
            ///  Set "_attached=true" when attached successfully.
            /// </summary>
            public abstract bool AttachContent(Grid content_element);


            public virtual bool DetachContent()
            {
                _attached = false;
                return true;
            }


            public bool IsAttached()
            {
                return _attached;
            }

            public string ID()
            {
                return _id;
            }


            /* ------------------------------------------------------------------*/
            // protected functions

            protected virtual void setup_content()
            {
                _setup = true;
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            protected bool _attached = false;
            protected bool _setup = false;


            /* ------------------------------------------------------------------*/
            // private variables

            private readonly string _id = UniqueID.Invalid;
        }
    }
}
