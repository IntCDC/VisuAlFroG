using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;



/*
 * Helper for parsing an validating command line arguments
 * 
 */
namespace Core
{
    namespace Utilities
    {
        public class CmdLineArguments
        {

            /* ------------------------------------------------------------------*/
            // public delegates

            public delegate void EvaluateArgument_Delegate(List<string> parameters);


            /* ------------------------------------------------------------------*/
            // public functions

            public bool Register(string cmd_long, string cmd_short, uint cmd_params, EvaluateArgument_Delegate cmd_evaluate_callback)
            {
                if (_registered_options.ContainsKey(cmd_long))
                {
                    Log.Default.Msg(Log.Level.Error, "Failed to register already existing option with the name: '" + cmd_long + "'");
                    return false;
                }
                foreach (var a in _registered_options)
                {
                    if (a.Value.ShortName == cmd_short)
                    {
                        Log.Default.Msg(Log.Level.Error, "Failed to register already existing option with the short name: '" + cmd_long + "'");
                        return false;
                    }
                }

                _registered_options.Add(cmd_long, new Option(cmd_short, cmd_params, cmd_evaluate_callback));
                return true;
            }

            public bool Parse(string arguments = "")
            {
                bool retval = true;
                _parsed_options.Clear();

                List<string> args = new List<string>();
                if (arguments != "")
                {
                    string arg_string = Regex.Replace(arguments, @"\s+", " ");
                    string[] arg_arr = arg_string.Split(' ');
                    args.AddRange(arg_arr);
                    args.RemoveAll(x => (x == ""));
                }
                else
                {
                    string[] arg_arr = Environment.GetCommandLineArgs();
                    args.AddRange(arg_arr);
                    // Remove first argument which is always the executable name
                    args.RemoveAt(0);
                }

                for (int index = 0; index < args.Count; index++)
                {
                    // Validate first argument
                    string first_arg = args[index];
                    // Remove trailing and leading spaces
                    first_arg = first_arg.Trim();
                    // Check if argument is an expected option
                    if (!first_arg.StartsWith("--") && !first_arg.StartsWith("-"))
                    {
                        Log.Default.Msg(Log.Level.Error, "Expected argument option starting with '--' or '-' but found: '" + first_arg + "'");
                        retval = false;
                        continue;
                    }
                    first_arg = first_arg.Replace("-", "");

                    string option_name = "";
                    if (_registered_options.ContainsKey(first_arg))
                    {
                        option_name = first_arg;
                    }
                    else
                    {
                        foreach (var a in _registered_options)
                        {
                            if (a.Value.ShortName == first_arg)
                            {
                                option_name = a.Key;
                            }
                        }
                    }
                    // Validate parameters of option
                    if (option_name != "")
                    {
                        List<string> parameters = new List<string>();
                        uint required_param_count = _registered_options[option_name].ParamCount;
                        bool missing_param = false;
                        for (int j = 0; j < required_param_count; j++)
                        {
                            if ((index + 1) < args.Count)
                            {
                                parameters.Add(args[index + 1]);
                                index++;
                            }
                            else
                            {
                                Log.Default.Msg(Log.Level.Error, "Missing required parameter for option: '" + option_name + "'.");
                                retval = false;
                                missing_param = true;
                                break;
                            }
                        }
                        if (missing_param)
                        {
                            _parsed_options.Remove(option_name);
                            Log.Default.Msg(Log.Level.Warn, "Ignoring option: '" + option_name + "'");
                        }
                        else
                        {
                            _parsed_options.Add(option_name, parameters);
                        }
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Error, "Unknown or unexpected command line option: '" + first_arg + "'.");
                        retval = false;
                    }
                }

                if (!retval)
                {
                    PrintHelp();
                }
                return retval;
            }

            public bool Evaluate()
            {
                foreach (var cmd in _parsed_options)
                {
                    Log.Default.Msg(Log.Level.Info, "Evaluating option '" + cmd.Key + "'");
                    _registered_options[cmd.Key].EvaluateCallback(cmd.Value);
                }
                return true;
            }

            public void PrintHelp()
            {
                Log.Default.Msg(Log.Level.Info,
                    "\n\n" +
                    "VisualFroG.exe [[OPTION] | [OPTION] [PARAMETER]]...\n " +
                    "Options: \n" +
                    "    -c, -config CONFIGURATION_FILE_PATH    Provide the absolute or relative file path to a configuration file that should be loaded on startup.\n" +
                    "    -h, --help                             Print this message.\n" +
                    "\n"
                    );
            }


            /* ------------------------------------------------------------------*/
            // private variables

            // List of registered command line options
            private Dictionary<string, Option> _registered_options = new Dictionary<string, Option>();

            // List of parsed options and their list of parameters
            private Dictionary<string, List<string>> _parsed_options = new Dictionary<string, List<string>>();

            private struct Option
            {
                public Option(string cmd_short, uint cmd_params, EvaluateArgument_Delegate cmd_evaluate_callback)
                {
                    ShortName = cmd_short;
                    ParamCount = cmd_params;
                    EvaluateCallback = cmd_evaluate_callback;
                }

                public string ShortName { get; }
                public uint ParamCount { get; }
                public EvaluateArgument_Delegate EvaluateCallback { get; }
            }
        }
    }
}
