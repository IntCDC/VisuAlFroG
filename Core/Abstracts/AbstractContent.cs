using System;
using System.Windows.Controls;
using System.Collections.Generic;
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

            // DECLARE IN DERIVED CLASSES
            // public static readonly string name = "...";
            // public static readonly bool multiple_instances = false;
            // public static readonly List<Type> depending_services = new List<Type>() { typeof(<service>)};


            /* ------------------------------------------------------------------*/
            // public functions

            public AbstractContent()
            {
                _id = UniqueID.Generate();
            }


            /// <summary>
            /// Attach content to provided content_element.
            ///  Set "_attached=true" when attached successfully.
            ///  Only attach if "_setup=true" otherwise call setup_content()
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

            /// <summary>
            /// Setup content_element.
            ///  Set "_setup=true" when setup successfully.
            /// </summary>
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
