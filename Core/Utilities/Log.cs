using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Utilities;



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
                public string caller_file;
                public string caller_method;
                public string caller_line;
                public string message;

            }

            public delegate void LogListenerCall(MessageData msgdata);


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


            public void Msg(Log.Level level, string message, StackTrace custom_stacktrace = null)
            {
                MessageData msgdata = new MessageData();

                var stacktrace = custom_stacktrace;
                if (stacktrace == null)
                {
                    stacktrace = new StackTrace(true);
                }
#if DEBUG
#else
                // Ignore debug messages on release build
                if (level != Level.Debug) {
#endif
                msgdata.caller_file = stacktrace.GetFrame(1).GetFileName();
                msgdata.caller_method = stacktrace.GetFrame(1).GetMethod().Name;
                msgdata.caller_line = stacktrace.GetFrame(1).GetFileLineNumber().ToString();
                msgdata.level = level;
                msgdata.message = message;
                _messages.Add(msgdata);

                // Call listeners
                foreach (var l in _listeners)
                {
                    l(msgdata);
                }
#if DEBUG
#else
                }
#endif
            }


            public void RegisterListener(LogListenerCall listener)
            {
                _listeners.Add(listener);
            }


            /* ------------------------------------------------------------------*/
            // private functions



            /* ------------------------------------------------------------------*/
            // private variables

            private static Log _instance = null;
            private static readonly object _padlock = new object();
            private List<MessageData> _messages = new List<MessageData>();
            private List<LogListenerCall> _listeners = new List<LogListenerCall>();
        }
    }

}
