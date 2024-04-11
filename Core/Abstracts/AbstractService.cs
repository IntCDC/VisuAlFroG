using System;
using Core.Utilities;



/*
 * Abstract Service
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractService : IAbstractService
        {

            /* ------------------------------------------------------------------*/
            // public properties

            public string Name { get { return GetType().FullName; } }


            /* ------------------------------------------------------------------*/
            // public functions

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
                throw new InvalidOperationException("Call Initialize method of derived class");
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


            /* ------------------------------------------------------------------*/
            // protected variables

            protected bool _initialized = false;
            /// DEBUG
            protected TimeBenchmark _timer = null;
        }
    }
}
