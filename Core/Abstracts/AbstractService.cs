using System;
using System.Collections.Generic;
using System.Windows.Controls;
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
                if (_initilized)
                {
                    Terminate();
                }
                _timer.Start();

                /// PLACE YOUR STUFF HERE ...

                _timer.Stop();
                _initilized = true;
                if (_initilized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initilized;
            }
            */


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
            /// DEBUG
            protected TimeBenchmark _timer = null;
        }
    }
}
