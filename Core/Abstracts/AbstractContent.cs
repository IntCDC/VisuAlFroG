using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
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

        // INTERFACE 
        public interface IAbstractContent
        {
            /* ------------------------------------------------------------------*/
            // interface properties

            string Name { get; }

            bool MultipleIntances { get; }

            List<Type> DependingServices { get; }

            bool IsAttached { get; }

            string ID { get; }


            /* ------------------------------------------------------------------*/
            // interface functions

            bool Create();

            Control Attach();

            bool Detach();
        }



        // ABSTRACT CLASS
        public abstract class AbstractContent : IAbstractContent
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
                _timer = new TimeBenchmark();
            }


            public virtual bool Create()
            {
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Warn, "Content already created");
                    return false;
                }
                _timer.Start();
                //_content.Name = ID;


                _timer.Stop();
                _created = true;
                return true;
            }


            /// <summary>
            /// Attach content to provided content_element.
            /// </summary>
            public virtual Control Attach()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }

                //_attached = true;
                return null;
            }


            public virtual bool Detach()
            {
                _attached = false;
                return true;
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            protected bool _attached = false;
            protected bool _created = false;


            /* ------------------------------------------------------------------*/
            // private variables

            private readonly string _id = null;
            protected TimeBenchmark _timer = null;
        }
    }
}
