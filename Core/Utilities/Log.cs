using System.Collections.Generic;
using System.Diagnostics;
using System;
using Core.GUI;
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
            // public types

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

            public delegate void LogListener_Delegate(List<MessageData> msglist);


            /* ------------------------------------------------------------------*/
            // public functions

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


            public void Msg(Log.Level level, string log, StackTrace custom_stacktrace = null)
            {
#if DEBUG
#else
                // Ignore debug messages on release build
                if (level != Level.Debug) {
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
                trace_meta = trace_meta.PadRight(75, ' ');
                string message = (trace_meta + " > " + log);

                MessageData msgdata = new MessageData();
                msgdata.level = level;
                msgdata.message = message;
                _messages.Add(msgdata);

                List<MessageData> listener_messages = new List<MessageData>();
                listener_messages.Add(msgdata);

                // Call listeners
                foreach (var l in _listeners)
                {
                    l(listener_messages);
                }
                Console.WriteLine(message);
#if DEBUG
#else
                }
#endif
            }


            public void RegisterListener(LogListener_Delegate listener)
            {
                _listeners.Add(listener);
                // Send all previous messages to newly registered listener
                listener(_messages);
            }


            public bool UnRegisterListener(LogListener_Delegate listener)
            {
                return _listeners.Remove(listener);
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private static Log _instance = null;
            private static readonly object _padlock = new object();
            private List<MessageData> _messages = new List<MessageData>();
            private List<LogListener_Delegate> _listeners = new List<LogListener_Delegate>();
        }
    }

}
