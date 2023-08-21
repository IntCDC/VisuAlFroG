using System.Diagnostics;



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

            public void Stop()
            {
#if DEBUG
                if (_started)
                {
                    _watch.Stop();
                    var elapsed_ms = _watch.ElapsedMilliseconds;
                    Log.Default.Msg(Log.Level.Debug, "Elapsed Time (ms): " + elapsed_ms.ToString(), new StackTrace(true));
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
