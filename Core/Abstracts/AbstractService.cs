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
        public abstract class AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public abstract bool Initialize();


            public abstract bool Execute();


            public virtual bool Terminate()
            {
                return true;
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            protected bool _initilized = false;
            protected TimeBenchmark _timer = new TimeBenchmark();

        }
    }
}
