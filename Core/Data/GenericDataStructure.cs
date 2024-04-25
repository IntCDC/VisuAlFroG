using System;
using System.Collections.Generic;

using Core.Utilities;



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
            // public properties

            public List<GenericDataStructure> _Branches { get; private set; } = new List<GenericDataStructure>();
            public List<GenericDataEntry> _Entries { get; private set; } = new List<GenericDataEntry>();

            public string _Label { get; set; } = "";


            /* ------------------------------------------------------------------*/
            // public functions

            public void AddBranch(GenericDataStructure branch) { _Branches.Add(branch); }

            public void AddEntry(GenericDataEntry entry) { _Entries.Add(entry); }

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

            private void list_metadata(GenericDataStructure branch, ref List<MetaDataGeneric> out_metadatalist)
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

            private void get_valuetypes(GenericDataStructure branch, ref List<Type> out_valuetypes)
            {
                foreach (var b in branch._Branches)
                {
                    get_valuetypes(b, ref out_valuetypes);
                }
                foreach (var entry in branch._Entries)
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
                foreach (var b in branch._Branches)
                {
                    get_min(b, ref min);
                }
                foreach (var entry in branch._Entries)
                {
                    min = Math.Min(entry._Metadata._Min, min);
                }
            }

            private void get_max(GenericDataStructure branch, ref double max)
            {
                foreach (var b in branch._Branches)
                {
                    get_max(b, ref max);
                }
                foreach (var entry in branch._Entries)
                {
                    max = Math.Max(entry._Metadata._Max, max);
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
        }
    }
}
