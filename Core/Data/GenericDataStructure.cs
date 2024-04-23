using System;
using System.Collections.Generic;

using Core.Utilities;



/*
 * Generic data structure
 * 
 */
namespace Core
{
    namespace Data
    {
        public class GenericDataStructure
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public List<GenericDataStructure> Branches { get { return _branches; } }
            public List<GenericDataEntry> Entries { get { return _entries; } }

            public string Label { get; set; } = "";


            /* ------------------------------------------------------------------*/
            // public functions

            public void AddBranch(GenericDataStructure branch) { _branches.Add(branch); }

            public void AddEntry(GenericDataEntry entry) { _entries.Add(entry); }

            public GenericDataEntry EntryAtIndex(int entry_index)
            {
                GenericDataEntry entry = null;
                get_entry_at_index(this, entry_index, ref entry);
                return entry;
            }

            public List<MetaDataGeneric> ListMetaData()
            {
                List<MetaDataGeneric> metadata_list = new List<MetaDataGeneric>();
                list_metadata(this, ref metadata_list);
                return metadata_list;
            }

            public uint DataDimension()
            {
                uint dim = 0;
                get_dimension(this, ref dim);
                return dim;
            }

            public List<Type> ValueTypes()
            {
                var value_types = new List<Type>();
                get_valuetypes(this, ref value_types);
                return value_types;
            }

            public double Min()
            {
                double min = double.PositiveInfinity;
                get_min(this, ref min);
                return min;
            }
            public double Max()
            {
                double max = double.NegativeInfinity;
                get_max(this, ref max);
                return max;
            }

            public List<string> Labels()
            {
                List<string> labels = new List<string>();
                get_labels(this, ref labels);
                return labels;
            }

            /* ------------------------------------------------------------------*/
            // private functions

            private void get_entry_at_index(GenericDataStructure branch, int entry_index, ref GenericDataEntry out_entry)
            {
                if (out_entry != null)
                {
                    return;
                }
                foreach (var b in branch.Branches)
                {
                    get_entry_at_index(b, entry_index, ref out_entry);
                }
                foreach (var entry in branch.Entries)
                {
                    if (entry.HasIndex(entry_index))
                    {
                        out_entry = entry;
                        return;
                    }
                }
            }

            private void list_metadata(GenericDataStructure branch, ref List<MetaDataGeneric> out_metadatalist)
            {
                foreach (var entry in branch.Entries)
                {
                    out_metadatalist.Add(entry.MetaData);
                }
                foreach (var b in branch.Branches)
                {
                    list_metadata(b, ref out_metadatalist);
                }
            }

            private bool get_dimension(GenericDataStructure branch, ref uint out_dim)
            {
                bool retval = true;
                foreach (var entry in branch.Entries)
                {
                    if (out_dim == 0)
                    {
                        out_dim = entry.Dimension;
                    }
                    if (out_dim != entry.Dimension)
                    {
                        Log.Default.Msg(Log.Level.Warn, "Inconsistent data dimensions");
                        retval = false;
                    }
                }
                foreach (var b in branch.Branches)
                {
                    return (retval & get_dimension(b, ref out_dim));
                }
                return true;
            }

            private void get_valuetypes(GenericDataStructure branch, ref List<Type> out_valuetypes)
            {
                foreach (var b in branch.Branches)
                {
                    get_valuetypes(b, ref out_valuetypes);
                }
                foreach (var entry in branch.Entries)
                {
                    var value_types = entry.ValueTypes();
                    foreach (var t in value_types)
                    {
                        if (!out_valuetypes.Contains(t))
                        {
                            out_valuetypes.Add(t);
                        }
                    }
                }
            }

            private void get_min(GenericDataStructure branch, ref double min)
            {
                foreach (var b in branch.Branches)
                {
                    get_min(b, ref min);
                }
                foreach (var entry in branch.Entries)
                {
                    min = Math.Min(entry.Min, min);
                }
            }

            private void get_max(GenericDataStructure branch, ref double max)
            {
                foreach (var b in branch.Branches)
                {
                    get_max(b, ref max);
                }
                foreach (var entry in branch.Entries)
                {
                    max = Math.Max(entry.Max, max);
                }
            }

            private void get_labels(GenericDataStructure branch, ref List<string> labels)
            {
                foreach (var b in branch.Branches)
                {
                    labels.Add(branch.Label);
                    get_labels(b, ref labels);
                }
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private List<GenericDataStructure> _branches = new List<GenericDataStructure>();
            private List<GenericDataEntry> _entries = new List<GenericDataEntry>();
        }
    }
}
