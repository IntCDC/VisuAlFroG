using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Core.Utilities;
using System.Runtime.Remoting.Contexts;
using Core.GUI;



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
            // public properties

            public abstract string Name { get; }
            public abstract bool MultipleIntances { get; }
            public abstract List<Type> DependingServices { get; }


            public bool IsAttached { get { return _attached; } }
            public string ID { get { return _id; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public AbstractContent()
            {
                _id = UniqueID.Generate();
            }


            /// <summary>
            /// Attach content to provided content_element.
            /// </summary>
            public virtual bool Attach(Grid content_element)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return false;
                }

                //content_element.Children.Add(_content);
                _attached = true;
                return true;
            }

            public virtual bool Detach()
            {
                _attached = false;
                return true;
            }


            public virtual bool Create()
            {
                _created = true;
                return true;
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            protected bool _attached = false;
            protected bool _created = false;


            /* ------------------------------------------------------------------*/
            // private variables

            private readonly string _id = UniqueID.Invalid;
            protected TimeBenchmark _timer = new TimeBenchmark();
        }
    }
}
