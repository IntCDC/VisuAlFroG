using System.Diagnostics;



/*
 * Benchmark Timer
 * 
 * Only executed in DEBUG build
 * 
 */
namespace Core
{
    namespace Utilities
    {
        public class TimeBenchmark
        {
            /* ------------------------------------------------------------------*/
            #region public functions

            /// <summary>
            /// Ctor.
            /// </summary>
            public TimeBenchmark()
            {
                _started = false;
                _watch = new System.Diagnostics.Stopwatch();
            }

            /// <summary>
            /// Start benchmark timer.
            /// </summary>
            public void Start()
            {
#if DEBUG
                if (!_started)
                {
                    _watch = System.Diagnostics.Stopwatch.StartNew();
                    _started = true;
                    _level++;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Timer already started");

                }
#endif
            }

            /// <summary>
            ///  Stop the benchmark timer and log the elapsed time.
            /// </summary>
            public void Stop()
            {
#if DEBUG
                if (_started)
                {
                    _watch.Stop();
                    var elapsed_ms = _watch.ElapsedMilliseconds;
                    Log.Default.Msg(Log.Level.Debug, "[level " + _level.ToString() + "]Elapsed Time (ms): " + elapsed_ms.ToString(), new StackTrace(true));
                    _started = false;
                    _level--;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Start timer before stopping it");

                }
#endif
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private bool _started = false;
            private System.Diagnostics.Stopwatch _watch = null;
            private static int _level = 0;

            #endregion
        }
    }
}
