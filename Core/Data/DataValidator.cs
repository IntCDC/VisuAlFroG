using Core.Utilities;



/*
 * Validation and analyses of input data
 * 
 */
namespace Core
{
    namespace Data
    {
        public class DataValidator
        {

            /* ------------------------------------------------------------------*/
            #region static functions

            /// <summary>
            /// [STATIC] Validate and convert input data into known format. 
            /// Extracts labels.
            /// </summary>
            /// <param name="input_data"></param>
            /// <param name="validated_data"></param>
            /// <returns></returns>
            public static bool Convert(GenericDataStructure input_data, out GenericDataStructure output_data)
            {
                output_data = new GenericDataStructure();

                // Check validity of input data
                bool valid = true;
                valid &= check_dimension(input_data);
                valid &= check_branches(input_data);
                uint entry_count = uint.MaxValue;
                valid &= check_entries(input_data, ref entry_count);

                // Copy data and remove label entries
                output_data = input_data;
                extract_column_labels(ref output_data);
                extract_row_labels(ref output_data);

                uint value_count = uint.MaxValue;
                valid &= check_values(input_data, ref value_count);

                convert_value_type(ref output_data);
                Log.Default.Msg(Log.Level.Info, "    >>> Successfully aligned labels.");

                if (!valid)
                {
                    return false;
                }
                Log.Default.Msg(Log.Level.Info, "    >>> Successfully validated data.");

                uint index = 0;
                init_index(input_data, ref index);
                Log.Default.Msg(Log.Level.Info, "    >>> Successfully initialized data.");

                return true;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            // Check required dimension
            private static bool check_dimension(GenericDataStructure data)
            {
                if (data.GetDimension() == 0)
                {
                    Log.Default.Msg(Log.Level.Error, "Data is required to have at least values for one dimension.");
                    return false;
                }
                return true;
            }

            // Check if only main branch contains branches and that no sub-branches exist
            private static bool check_branches(GenericDataStructure data)
            {
                foreach (var b in data._Branches)
                {
                    if (b._Branches.Count > 0)
                    {
                        Log.Default.Msg(Log.Level.Error, "Sub-branches are currently not supported.");
                        return false;
                    }
                }
                return true;
            }

            // Check if all branches contain equal number of entries
            /// XXX This must not be true if sub-branches are allowed?
            private static bool check_entries(GenericDataStructure data, ref uint entry_count)
            {
                bool retval = true;
                foreach (var branch in data._Branches)
                {
                    var tmp_entry_count = (uint)data._Entries.Count;
                    // Set entry count once and then only compare to this value
                    if (entry_count == uint.MaxValue)
                    {
                        entry_count = tmp_entry_count;
                    }
                    else
                    {
                        if (entry_count != tmp_entry_count)
                        {
                            Log.Default.Msg(Log.Level.Error, "Entry count mismatch. Expected " + entry_count.ToString() + " entries but found " + tmp_entry_count.ToString() + ".");
                            retval &= false;
                        }
                    }
                    retval &= check_entries(branch, ref entry_count);
                }
                return retval;
            }

            // Check if all entries contain equal number of values
            private static bool check_values(GenericDataStructure data, ref uint value_count)
            {
                bool retval = true;
                foreach (var entry in data._Entries)
                {
                    var tmp_value_count = (uint)entry._Values.Count;
                    // Set value count once and then only compare to this value
                    if (value_count == uint.MaxValue)
                    {
                        value_count = tmp_value_count;
                    }
                    else
                    {
                        if (value_count != tmp_value_count)
                        {
                            Log.Default.Msg(Log.Level.Error, "Values count mismatch. Expected " + value_count.ToString() + " values but found " + tmp_value_count.ToString() + ".");
                            retval &= false;
                        }
                    }
                }
                foreach (var branch in data._Branches)
                {
                    retval &= check_values(branch, ref value_count);
                }
                return retval;
            }

            // Check if first branch contains only entries with one string value that can be used as column labels
            // if not, generate column labels 'Column_[X]'
            private static bool extract_column_labels(ref GenericDataStructure data)
            {
                bool retval = false;
                if (data._Branches.Count > 0)
                {
                    if (data._Branches[0]._Entries.Count > 0)
                    {
                        var t_list = data._Branches[0].GetTypes();
                        bool only_strings = ((t_list.Count == 1) && (t_list[0] == typeof(string)));
                        bool only_one = true;
                        foreach (var e in data._Branches[0]._Entries)
                        {
                            only_one &= (e._Values.Count == 1);
                        }
                        if (only_one && only_strings)
                        {
                            Log.Default.Msg(Log.Level.Info, "        >>> Extracting column labels.");
                            var column_labels = data._Branches[0]._Entries;
                            data._Branches.RemoveAt(0);
                            foreach (var b in data._Branches)
                            {
                                for (int i = 0; i < b._Entries.Count; i++)
                                {
                                    var column_label = column_labels[i]._Values[0] as string;
                                    if (column_label == null)
                                    {
                                        Log.Default.Msg(Log.Level.Error, "BUG: Value should be string.");
                                        return false;
                                    }
                                    b._Entries[i]._Metadata._Label = column_label;
                                }
                            }
                            return true;
                        }
                    }
                }

                return retval;
            }

            // Check if all branches start with one string value for each entry that can be used as row label
            // if not, generate row labels 'Row_[X]'
            private static bool extract_row_labels(ref GenericDataStructure data)
            {
                bool retval = false;
                if (data._Branches.Count > 0)
                {
                    bool first_string = true;
                    foreach (var b in data._Branches)
                    {
                        if (b._Entries.Count > 0)
                        {
                            if (b._Entries[0]._Values.Count > 0)
                            {
                                if (b._Entries[0]._Values[0].GetType() != typeof(string))
                                {
                                    first_string = false;
                                }
                            }
                            else
                            {
                                first_string = false;
                            }
                        }
                        else
                        {
                            first_string = false;
                        }
                    }
                    if (first_string)
                    {
                        Log.Default.Msg(Log.Level.Info, "        >>> Extracting row labels.");
                        foreach (var b in data._Branches)
                        {
                            var row_label = b._Entries[0]._Values[0] as string;
                            if (row_label == null)
                            {
                                Log.Default.Msg(Log.Level.Error, "BUG: Value should be string.");
                                return false;
                            }
                            b._Label = row_label;

                            var row_entries = b._Entries.GetRange(1, b._Entries.Count - 1);
                            b._Entries.Clear();
                            foreach (var e in row_entries)
                            {
                                b._Entries.Add(e);
                            }
                        }
                        return true;
                    }
                }

                return retval;
            }

            // Check if all other values can be converted to double
            private static bool convert_value_type(ref GenericDataStructure data)
            {
                if (!((data.GetTypes().Count == 1) && (data.GetTypes()[0] == typeof(double))))
                {
                    Log.Default.Msg(Log.Level.Error, "Not all values are of type 'double'.");
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Recursively initialize meta data index.
            /// </summary>
            /// <param name="data">The data branch.</param>
            /// <param name="index">Reference to a index declared outside</param>
            private static void init_index(GenericDataStructure data, ref uint index)
            {
                foreach (var entry in data._Entries)
                {
                    entry._Metadata._Index = index;
                    index++;
                }
                foreach (var branch in data._Branches)
                {
                    init_index(branch, ref index);
                }
            }

            #endregion
        }
    }
}
