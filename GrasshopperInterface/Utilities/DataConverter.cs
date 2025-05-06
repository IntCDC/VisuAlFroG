using System;
using System.Linq;
using Core.Utilities;
using Core.Data;
using Grasshopper.Kernel.Types;
using System.Globalization;
using Grasshopper.Kernel.Data;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;



/*
 * Convert Data from and to Grasshopper data format 
 * 
 */
namespace GrasshopperInterface
{
    namespace Utilities
    {
        public class DataConverter
        {
            /* ------------------------------------------------------------------*/
            #region static functions

            /// <summary>
            /// [STATIC] Convert data provided by the interface to generic data type. 
            /// </summary>
            /// <param name="input_data">The input data.</param>
            /// <returns>The converted output data.</returns>
            public static GenericDataStructure ConvertFromGHStructure(GH_Structure<IGH_Goo> input_data, GenericDataStructure generic_data)
            {
                //var generic_data = new GenericDataStructure();

                foreach (var input_entries in input_data.Branches)
                {
                    var output_branch = new GenericDataStructure();
                    foreach (var input_value in input_entries)
                    {
                        var output_entry = new GenericDataEntry();
                        if (input_value.CastTo<string>(out string value_string))
                        {
                            char[] separators = new char[] { ' ', ',', '|', ';' };  // Do check for '.' since this is comma for float
                            string[] subs = value_string.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var sub in subs)
                            {
                                try
                                {
                                    double value_double = Convert.ToDouble(sub, CultureInfo.InvariantCulture);
                                    output_entry.AddValue(value_double);
                                }
                                catch // (Exception exc)
                                {
                                    /// Log.Default.Msg(Log.Level.Error, exc.Message);

                                    // Default: Add as string
                                    output_entry.AddValue(value_string);
                                }
                            }
                            if (subs.Length == 0)
                            {
                                ///Log.Default.Msg(Log.Level.Error, "Failed to parse values. ");

                                // Allow empty values
                                output_entry.AddValue(value_string);
                            }
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Can not convert raw input data to string from type: " + input_value.GetType());
                        }
                        output_branch.AddEntry(output_entry);
                    }
                    generic_data.AddBranch(output_branch);
                }
                return generic_data;
            }

            /// <summary>
            /// [STATIC] Convert from generic data type to interface specific data type. 
            /// </summary>
            /// <param name="input_data">The input data.</param>
            /// <returns>The converted output data.</returns>
            public static GH_Structure<IGH_Goo> ConvertToGHStructure(GenericDataStructure input_data)
            {
                var ghstructure_data = new GH_Structure<IGH_Goo>();

                int branch_index = 0;
                foreach (var input_branch in input_data._Branches) {

                    int entry_index = 0;
                    foreach (var input_entry in input_branch._Entries)
                    {
                        GH_Path path = new GH_Path(branch_index, entry_index);
                        foreach (var generic_value in input_entry._Values)
                        {
                            ghstructure_data.Append(new GH_String(generic_value.ToString()), path);
                        }
                        ghstructure_data.EnsurePath(path);

                        entry_index++;
                    }
                    branch_index++;
                }

                return ghstructure_data;
            }

            public static GenericDataStructure ConvertJsonSamplesToGenericData(List<Dictionary<string, object>> jsonSamples, GenericDataStructure generic_data)
            {
                char[] separators = new char[] { ' ', ',', '|', ';' };  // align with GH delimiter logic

                foreach (var dict in jsonSamples)
                {
                    var output_branch = new GenericDataStructure();

                    foreach (var topLevelKvp in dict)
                    {
                        string key = topLevelKvp.Key;
                        object value = topLevelKvp.Value;

                        if (value is Newtonsoft.Json.Linq.JObject nestedObject)
                        {
                            foreach (var nested in nestedObject)
                            {
                                var output_entry = new GenericDataEntry();
                                string value_string = nested.Value.ToString();
                                string[] subs = value_string.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                                foreach (var sub in subs)
                                {
                                    if (TryConvertToDouble(sub, out double numericValue))
                                    {
                                        output_entry.AddValue(numericValue);
                                    }
                                    else
                                    {
                                        output_entry.AddValue(sub);  // Keep string if not convertible
                                    }
                                }

                                if (subs.Length == 0)
                                {
                                    output_entry.AddValue(value_string);  // fallback for empty strings
                                }

                                output_branch.AddEntry(output_entry);
                            }
                        }
                        else
                        {
                            var output_entry = new GenericDataEntry();
                            string value_string = value.ToString();
                            string[] subs = value_string.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var sub in subs)
                            {
                                if (TryConvertToDouble(sub, out double numericValue))
                                {
                                    output_entry.AddValue(numericValue);
                                }
                                else
                                {
                                    output_entry.AddValue(sub);
                                }
                            }

                            if (subs.Length == 0)
                            {
                                output_entry.AddValue(value_string);
                            }

                            output_branch.AddEntry(output_entry);
                        }
                    }

                    generic_data.AddBranch(output_branch);
                }

                return generic_data;
            }

            private static bool TryConvertToDouble(object input, out double result)
            {
                if (input is double d)
                {
                    result = d;
                    return true;
                }

                return double.TryParse(input?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            }


            #endregion
        }
    }
}
