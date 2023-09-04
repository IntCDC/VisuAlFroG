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
            // public classes

            /// <summary>
            /// Class defining the settings required for restoring content.
            /// </summary>
            public class Settings : IAbstractSettingData
            {
                public string ID { get; set; }
                public string Type { get; set; }
                /// TODO Add additional settings that should be saved here...
            }


            /* ------------------------------------------------------------------*/
            // public properties

            public abstract string Name { get; }
            public abstract bool MultipleInstances { get; }
            public abstract List<Type> DependingServices { get; }
            public bool IsAttached { get { return _attached; } }
            public string ID { get { return _id; } set { _id = value; } }


            /* ------------------------------------------------------------------*/
            // public functions

            /// <summary>
            /// Ctor.
            /// </summary>
            public AbstractContent()
            {
                _id = UniqueID.Generate();
                _timer = new TimeBenchmark();
            }

            /// <summary>
            /// If derived class might requires additional data on initialization (declaring Initialize taking parameter(s)) ...
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            /// <exception cref="InvalidOperationException">...throw error when method of base class is called instead.</exception>
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

            /// <summary>
            /// Called to actually create the WPF content.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
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
            /// Called when content element is being attached to a parent element.
            /// </summary>
            /// <returns>The WPF control element holding the content.</returns>
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

            /// <summary>
            /// Called when content has been detached. 
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            public virtual bool Detach()
            {
                if (!_attached)
                {
                    /// PLACE YOUR STUFF HERE ...

                    _attached = false;
                }
                return true;
            }

            /// <summary>
            /// Called when content should be terminated. Should implement counterpart to Initialize().
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
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

            protected bool _initialized = false;
            protected bool _created = false;
            protected bool _attached = false;


            /* ------------------------------------------------------------------*/
            // private variables

            private string _id = null;
            /// DEBUG
            protected TimeBenchmark _timer = null;
        }
    }
}
