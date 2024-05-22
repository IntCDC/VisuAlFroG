using System.Collections.Generic;
using System.Diagnostics;
using System;
using Core.Utilities;
using System.IO;



/*
 * Global Messages Buffer 
 * 
 * Access SINGELTON via e.g.: Log.Default.Msg(Log.Level.Info, ...)
 * 
 */
namespace Core
{
    namespace Utilities
    {



        public class Log
        {

            /* ------------------------------------------------------------------*/
            #region public types

            public enum Level
            {
                Info,
                Warn,
                Error,
                Debug,
            }

            public struct MessageData
            {
                public Level level;
                public string message;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public delegates

            public delegate void LogListener_Delegate(List<MessageData> msglist);

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties

            public bool DisableDebug { get; set; } = false;

            /// <summary>
            /// Dump log messages to a file. Default is 'false'.
            /// </summary>
            public bool DumpFile
            {
                get { return _dump_file; }
                set
                {
                    if (value != _dump_file)
                    {
                        _dump_file = value;
                        if (_dump_file)
                        {
                            if (!File.Exists(log_file))
                            {
                                using (StreamWriter sw = File.CreateText(log_file))
                                {
                                    sw.WriteLine("---------- NEW LOG (" + DateTime.Now.ToString() + ") ----------");
                                }
                                Log.Default.Msg(Log.Level.Info, "Starting to write log messages to file: '" + log_file + "'");
                            }
                        } else {
                            Log.Default.Msg(Log.Level.Info, "Stopped writing log messages to file.");
                        }
                    }
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            /// <summary>
            /// [STATIC] Create singelton when Log.Default is called the first time.
            /// </summary>
            public static Log Default
            {
                get
                {
                    // simple thread safety
                    lock (_padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Log();
                        }
                        return _instance;
                    }
                }
            }

            /// <summary>
            /// Ctor.
            /// </summary>
            public Log()
            {
                _messages = new List<MessageData>();
                _listeners = new List<LogListener_Delegate>();
                if (File.Exists(log_file))
                {
                    File.Delete(log_file);
                }
            }

            /// <summary>
            /// Log new message.
            /// </summary>
            /// <param name="level">Log level of message.</param>
            /// <param name="log">The actual log message.</param>
            /// <param name="custom_stacktrace">Pass separate stack trace instance.</param>
            public void Msg(Log.Level level, string log, StackTrace custom_stacktrace = null)
            {
#if DEBUG
                if ((level == Level.Debug) && DisableDebug)
                {
                    return;
                }
#else
                // Ignore debug messages on release build
                if (level == Level.Debug) {
                    return;
                }
#endif

                var stacktrace = custom_stacktrace;
                if (stacktrace == null)
                {
                    stacktrace = new StackTrace(true);
                }

                string caller_file = stacktrace.GetFrame(1).GetFileName();
                string caller_method = stacktrace.GetFrame(1).GetMethod().Name;
                string caller_line = stacktrace.GetFrame(1).GetFileLineNumber().ToString();
                string timestamp = "[" + DateTime.Now.ToString() + "]";

                string trace_file = "[" + Path.GetFileName(caller_file) + "]";
                string trace_method = "[" + caller_method + "]";
                string trace_line = "[" + caller_line + "]";

                string level_prefix = " <INFO>  ";
                switch (level)
                {
                    case (Log.Level.Warn):
                        level_prefix = " <WARN> ";
                        break;
                    case (Log.Level.Error):
                        level_prefix = " <ERROR> ";
                        break;
                    case (Log.Level.Debug):
                        level_prefix = " <DEBUG> ";
                        break;
                }

                // Fixed padding
                string trace_meta = timestamp + level_prefix + trace_file + trace_method + trace_line;
                trace_meta = trace_meta.PadRight(80, ' ');
                string message = (trace_meta + " > " + log);

                MessageData msgdata = new MessageData();
                msgdata.level = level;
                msgdata.message = message;
                _messages.Add(msgdata);

                // Call listeners
                var listener_messages = new List<MessageData>();
                listener_messages.Add(msgdata);
                foreach (var listener in _listeners)
                {
                    listener(listener_messages);
                }

                if (_dump_file)
                {
                    using (StreamWriter sw = File.AppendText(log_file))
                    {
                        sw.WriteLine(message);
                    }
                }
                Console.WriteLine(message);
            }

            /// <summary>
            /// Register new log message listener.
            /// </summary>
            /// <param name="listener">The log listener to register.</param>
            public void RegisterListener(LogListener_Delegate listener)
            {
                _listeners.Add(listener);
                // Send all previous messages to newly registered listener
                listener(_messages);
            }

            /// <summary>
            /// Unregister log listener.
            /// </summary>
            /// <param name="listener">The listener that should be removed.</param>
            /// <returns>True on success, false otherwise.</returns>
            public bool UnRegisterListener(LogListener_Delegate listener)
            {
                return _listeners.Remove(listener);
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private static Log _instance = null;
            private static readonly object _padlock = new object();

            private List<MessageData> _messages = null;
            private List<LogListener_Delegate> _listeners = null;

            private bool _dump_file = false;
            private readonly string log_file = ResourcePaths.CreateFileName("console_log", "txt");

            #endregion
        }
    }
}
