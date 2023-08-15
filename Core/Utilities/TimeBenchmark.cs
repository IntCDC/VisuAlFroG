using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



/*
 * Benchmark Timer
 * 
 * >>> Only executed in DEBUG build
 */
namespace Core
{
    namespace Utilities
    {
        public class TimeBenchmark
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public void Start()
            {
#if DEBUG
                if (!_started)
                {
                    _watch = System.Diagnostics.Stopwatch.StartNew();
                    _started = true;
                }
#endif
            }

            public void Stop(string calling_class)
            {
#if DEBUG
                if (_started)
                {
                    _watch.Stop();
                    var elapsed_ms = _watch.ElapsedMilliseconds;
                    Log.Default.Msg(Log.Level.Debug, "Elapsed Time: " + elapsed_ms.ToString(), new StackTrace(true));
                    _started = false;
                }
#endif
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private bool _started = false;
            private System.Diagnostics.Stopwatch _watch = new System.Diagnostics.Stopwatch();

        }
    }
}
