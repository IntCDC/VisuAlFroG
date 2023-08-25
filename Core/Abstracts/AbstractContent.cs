using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Windows.Controls;
using Core.Utilities;



/*
 * Abstract Content
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractContent : IAbstractService, IAbstractContent
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


            /// <summary>
            /// If derived class might requires additional data on initialization (declaring Initialize taking parameter(s)), 
            /// throw error when method of base class is called instead.
            /// </summary>
            public virtual bool Initialize()
            {
                throw new InvalidOperationException("Call Initialize method of derived class");
            }
            /* TEMPLATE
            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                _timer.Start();


                /// PLACE YOUR STUFF HERE ...

                /// ! REQUIRED:
                _content.Name = ID;


                _timer.Stop();
                _initilized = true;
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initilized;
            }
            */


            public abstract bool Create();
            /*
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Warn, "Content already created, skipping...");
                    return false;
                }
                _timer.Start();

                /// PLACE YOUR STUFF HERE ...

                _timer.Stop();
                _created = true;
                return true;
            }
            */


            /// <summary>
            /// Attach content to provided content_element.
            /// </summary>
            public abstract Control Attach();
            /* TEMPLATE
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }

                /// PLACE YOUR STUFF HERE ...

                _attached = true;
                return null;
            }
            */


            public virtual bool Detach()
            {
                if (!_attached)
                {
                    /// PLACE YOUR STUFF HERE ...

                    _attached = false;
                }
                return true;
            }


            public abstract bool Terminate();
            /* TEMPLATE
            {
                if (_initilized)
                {
                    /// PLACE YOUR STUFF HERE ...
                    
                    _initilized = false;
                }
                return true;
            }
            */


            /* ------------------------------------------------------------------*/
            // protected variables

            protected bool _initilized = false;
            protected bool _created = false;
            protected bool _attached = false;


            /* ------------------------------------------------------------------*/
            // private variables

            private readonly string _id = null;
            /// DEBUG
            protected TimeBenchmark _timer = null;
        }
    }
}
