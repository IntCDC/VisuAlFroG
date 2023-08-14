using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;



/*
 * Abstract Service
 * 
 */
namespace Core
{
    namespace Management
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
