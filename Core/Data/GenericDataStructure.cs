using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Utilities;
using Core.Abstracts;



/*
 * Generic data structure
 * 
 * Branches ~ Rows          ~ Data Series
 * Entries  ~ Column Values ~
 * 
 */
namespace Core
{
    namespace Data
    {
        public class GenericDataStructure
        {
            /* ------------------------------------------------------------------*/
            #region public properties

            public List<GenericDataStructure> _Branches { get; private set; } = new List<GenericDataStructure>();
            public List<GenericDataEntry> _Entries { get; private set; } = new List<GenericDataEntry>();

            public string _Label { get; set; } = "";

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            ///  DEBUG
            public GenericDataStructure() 
            {
                Console.WriteLine(" ----> Created new instance of GenericDataStructure");
                //Log.Default.Msg(Log.Level.Debug, " ----> Created new instance of GenericDataStructure");
            }
            ~GenericDataStructure()
            {
                Console.WriteLine(" --<<< Deleted instance of GenericDataStructure");
                //Log.Default.Msg(Log.Level.Debug, " --<<< Deleted instance of GenericDataStructure");
            }
            /// DEBUG

            public void AddBranch(GenericDataStructure branch) { _Branches.Add(branch); }

            public void AddEntry(GenericDataEntry entry) { _Entries.Add(entry); }

            public GenericDataEntry GetEntryAtIndex(uint entry_index)
            {
                GenericDataEntry entry = null;
                get_entry_at_index(this, entry_index, ref entry);
                return entry;
            }

            public List<GenericMetaData> GetListMetaData()
            {
                List<GenericMetaData> metadata_list = new List<GenericMetaData>();
                list_metadata(this, ref metadata_list);
                return metadata_list;
            }

            public uint GetDimension()
            {
                uint dim = 0;
                get_dimension(this, ref dim);
                return dim;
            }

            public List<Type> GetTypes()
            {
                var value_types = new List<Type>();
                get_types(this, ref value_types);
                return value_types;
            }

            public List<string> GetLabels()
            {
                List<string> labels = new List<string>();
                get_labels(this, ref labels);
                return labels;
            }

            public bool IsEmpty()
            {
                return ((_Branches.Count == 0) && (_Entries.Count == 0));
            }

            public bool IsFlatRowBased()
            {
                // Check if only branches (>0) exist on the top level
                if ((_Branches.Count == 0) || (_Entries.Count > 0))
                {
                    return false;
                }

                // Check if branches have at least one entry, all branches have the same number of entries, and no sub branches exist
                int entry_count = int.MinValue;
                for (int i = 0; i < _Branches.Count; i++)
                {
                    // Set entry_count for the first time
                    if (i == 0)
                    {
                        entry_count = _Branches[i]._Entries.Count;
                    }

                    if ((_Branches[i]._Branches.Count > 0) || (_Branches[i]._Entries.Count != entry_count) || (_Branches[i]._Entries.Count == 0))
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Apply transposed data in place
            /// </summary>
            /// <returns></returns>
            public bool Transpose()
            {
                if (!IsFlatRowBased())
                {
                    Log.Default.Msg(Log.Level.Error, "Transposing data is not possible since data is not flat and row based.");
                    return false;
                }

                var transposed_data = new GenericDataStructure();
                for (int b = 0; b < _Branches.Count; b++)
                {
                    var branch = _Branches[b];
                    for (int e = 0; e < branch._Entries.Count; e++)
                    {
                        var entry = branch._Entries[e];
                        if (b == 0)
                        {
                            var new_branch = new GenericDataStructure();
                            new_branch._Label = entry._Metadata._Label;
                            transposed_data._Branches.Add(new_branch);
                        }
                        entry._Metadata._Label = branch._Label;
                        transposed_data._Branches[e].AddEntry(entry);
                    }
                }

                _Branches.Clear();
                _Entries.Clear();
                _Branches = transposed_data._Branches;

                return true;
            }

            public GenericDataStructure DeepCopy()
            {
                var branch_clone = new GenericDataStructure();

                foreach (var branch in _Branches)
                {
                    branch_clone.AddBranch(branch.DeepCopy());
                }
                foreach (var entry in _Entries)
                {
                    var entry_clone = new GenericDataEntry();

                    foreach (var value in entry._Values)
                    {
                        entry_clone._Values.Add(value);
                    }
                    foreach (var type in entry._Types)
                    {
                        entry_clone._Types.Add(type);
                    }
                    // Do not clone metadata since this should be shared even with deep copies
                    entry_clone._Metadata = entry._Metadata;
                    branch_clone.AddEntry(entry_clone);
                }
                branch_clone._Label = _Label;

                return branch_clone;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void get_entry_at_index(GenericDataStructure branch, uint entry_index, ref GenericDataEntry out_entry)
            {
                if (out_entry != null)
                {
                    return;
                }
                foreach (var b in branch._Branches)
                {
                    get_entry_at_index(b, entry_index, ref out_entry);
                }
                foreach (var entry in branch._Entries)
                {
                    if (entry.HasIndex(entry_index))
                    {
                        out_entry = entry;
                        return;
                    }
                }
            }

            private void list_metadata(GenericDataStructure branch, ref List<GenericMetaData> out_metadatalist)
            {
                foreach (var entry in branch._Entries)
                {
                    out_metadatalist.Add(entry._Metadata);
                }
                foreach (var b in branch._Branches)
                {
                    list_metadata(b, ref out_metadatalist);
                }
            }

            private bool get_dimension(GenericDataStructure branch, ref uint out_dim)
            {
                bool retval = true;
                foreach (var entry in branch._Entries)
                {
                    if (out_dim == 0)
                    {
                        out_dim = entry._Metadata._Dimension;
                    }
                    if (out_dim != entry._Metadata._Dimension)
                    {
                        Log.Default.Msg(Log.Level.Warn, "Inconsistent data dimensions");
                        retval = false;
                    }
                }
                foreach (var b in branch._Branches)
                {
                    return (retval & get_dimension(b, ref out_dim));
                }
                return true;
            }

            private void get_types(GenericDataStructure branch, ref List<Type> out_valuetypes)
            {
                foreach (var b in branch._Branches)
                {
                    get_types(b, ref out_valuetypes);
                }
                foreach (var entry in branch._Entries)
                {
                    var value_types = entry._Types;
                    foreach (var t in value_types)
                    {
                        if (!out_valuetypes.Contains(t))
                        {
                            out_valuetypes.Add(t);
                        }
                    }
                }
            }

            private void get_labels(GenericDataStructure branch, ref List<string> labels)
            {
                foreach (var b in branch._Branches)
                {
                    labels.Add(branch._Label);
                    get_labels(b, ref labels);
                }
            }

            #endregion
        }
    }
}
