using System;

using Core.GUI;
using Core.Utilities;



/*
 * Abstract Service
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public properties

            public string _Name { get { return GetType().FullName; } }


            /* ------------------------------------------------------------------*/
            // protected functions

            protected AbstractService()
            {
                _timer = new TimeBenchmark();
            }


            /* ------------------------------------------------------------------*/
            // abstract functions

            /// <summary>
            /// If derived class might requires additional data on initialization (declaring Initialize taking parameter(s)), 
            /// throw error when method of base class is called instead.
            /// </summary>
            public virtual bool Initialize()
            {
                throw new InvalidOperationException("Call Initialize method of derived class with required parameters");
            }
            /* TEMPLATE
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                /// PLACE YOUR STUFF HERE ...

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initialized;
            }
            */

            public abstract bool Terminate();
            /* TEMPLATE
            {
                if (_initialized)
                {
                    /// PLACE YOUR STUFF HERE ...
                    
                    _initialized = false;
                }
                return true;
            }
            */

            public virtual void AttachMenu(MainMenuBar menu_bar)
            {
                throw new InvalidOperationException("This function has been called for derived class of AbstractService that does not implement this function");
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            protected bool _initialized = false;
            /// DEBUG
            protected TimeBenchmark _timer = null;
        }
    }
}
