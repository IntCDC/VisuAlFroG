using System;
using System.Linq;
using Core.Utilities;
using Core.Data;
using System.Collections.Generic;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Markup;
using System.IO;



/*
 * Convert Data from and to CSV data format 
 * 
 */
namespace Core
{
    namespace Data
    {
        public class CSV_DataHandling
        {
            /* ------------------------------------------------------------------*/
            // static functions

            /// <summary>
            /// [STATIC] Save data in CSV format to a file
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static bool SaveToFile(GenericDataStructure data)
            {
                if (ConvertToCSV(data, out string output_data))
                {
                    return FileDialogHelper.Save(output_data, "Save Data", "CSV files (*.csv)|*.csv", ResourcePaths.CreateFileName("data", "csv"));
                }
                return false;
            }

            /// <summary>
            /// [STATIC] Load CSV formatted data from a file
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static bool LoadFromFile(out GenericDataStructure data)
            {
                data = new GenericDataStructure();

                string data_file = FileDialogHelper.Load("Load Data", "CSV files (*.csv)|*.csv", ResourcePaths.CreateFileName("data", "csv"));
                try
                {
                    if (data_file != "")
                    {
                        var fileStream = new FileStream(data_file, FileMode.Open, FileAccess.Read, FileShare.Read);
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            Log.Default.Msg(Log.Level.Info, "Loading data from file: '" + data_file + "'");
                            string content = reader.ReadToEnd();
                            if (ConvertFromCSV(content, out GenericDataStructure input_data))
                            {
                                data = input_data;
                                return true;
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                return false;
            }

            /// <summary>
            /// [STATIC] Convert CSV formatted data to generic data type. 
            /// </summary>
            /// <param name="input_data">The CSV input data as string.</param>
            /// <param name="output_data">The output data converted to generic data structure.</param>
            /// <returns>The converted output data.</returns>
            public static bool ConvertFromCSV(string input_data, out GenericDataStructure output_data)
            {
                output_data = new GenericDataStructure();

                if ((input_data == null) || (input_data == ""))
                {
                    Log.Default.Msg(Log.Level.Info, "Empty input data. Skipping converting data to CSV format.");
                    return true;
                }

                var generic_input_data = new GenericDataStructure();
                var lines = input_data.Split('\n');
                foreach (var line in lines)
                {
                    // Skip empty lines
                    if (line == "")
                    {
                        continue;
                    }
                    var branch = new GenericDataStructure();

                    // Check if values are enclosed in quotes
                    var values = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                    if (values.Count() == 0)
                    {
                        values = line.Split(',');
                    }

                    foreach (var value in values)
                    {
                        var value_edit = value.Trim('"');

                        var entry = new GenericDataEntry();

                        char[] separators = new char[] { ' ', '|', ';' }; // Do check for '.' since this is comma for float
                        string[] subs = value_edit.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var sub in subs)
                        {
                            try
                            {
                                double value_double = Convert.ToDouble(sub, CultureInfo.InvariantCulture);
                                entry.AddValue(value_double);
                            }
                            catch // (Exception exc)
                            {
                                /// Log.Default.Msg(Log.Level.Error, exc.Message);

                                // Default: Add as string
                                entry.AddValue(value_edit);
                            }
                        }
                        if (subs.Length == 0)
                        {
                            ///Log.Default.Msg(Log.Level.Error, "Failed to parse values. ");

                            // Allow empty values
                            entry.AddValue(value_edit);
                        }

                        branch.AddEntry(entry);
                    }
                    generic_input_data.AddBranch(branch);
                }

                if (DataValidator.Convert(generic_input_data, out GenericDataStructure validated_data))
                {
                    output_data = validated_data;
                    return true;
                }
                else
                {
                    Log.Default.Msg(Log.Level.Info, "Empty input data. Skipping converting data from CSV format");
                    return false;
                }
            }

            /// <summary>
            /// [STATIC] Convert generic data type to CSV formatted data. 
            /// </summary>
            /// <param name="input_data">The input data as generic data structure.</param>
            /// <param name="output_data">The CSV output data as string.</param>
            /// <returns>The converted output data.</returns>
            public static bool ConvertToCSV(GenericDataStructure input_data, out string output_data)
            {
                output_data = "";

                if ((input_data == null) || (input_data.Empty()))
                {
                    Log.Default.Msg(Log.Level.Info, "Found no input data to convert");
                    return true;
                }

                if (!input_data.IsFlatRowBased())
                {
                    Log.Default.Msg(Log.Level.Warn, "Input data has wrong format to be converted to CSV format");
                    return false;
                }

                bool use_row_labels = true;
                bool use_column_labels = true;
                foreach (var b in input_data._Branches)
                {
                    use_row_labels &= (b._Label != "");

                    foreach (var e in b._Entries)
                    {
                        use_column_labels &= (e._Metadata._Label != "");
                    }
                }

                // First row with column labels
                if (use_column_labels)
                {
                    output_data += "\"\","; // First entry is empty!
                    foreach (var e in input_data._Branches[0]._Entries)
                    {
                        output_data += "\"" + e._Metadata._Label + "\"";
                        if (e != input_data._Branches[0]._Entries.Last())
                        {
                            output_data += ",";
                        }
                    }
                }
                output_data += "\n";

                foreach (var b in input_data._Branches)
                {
                    if (use_row_labels)
                    {
                        output_data += "\"" + b._Label + "\",";
                    }

                    foreach (var e in b._Entries)
                    {
                        output_data += "\"";
                        foreach (var v in e._Values)
                        {
                            output_data += v.ToString();
                            if (v != e._Values.Last())
                            {
                                output_data += ";";
                            }
                        }
                        output_data += "\"";
                        if (e != b._Entries.Last())
                        {
                            output_data += ",";
                        }
                    }
                    output_data += "\n";
                }
                return true;
            }
        }
    }
}
