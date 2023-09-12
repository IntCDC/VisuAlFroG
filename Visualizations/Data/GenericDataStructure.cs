using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Utilities;

using Newtonsoft.Json.Linq;

using SciChart.Data.Model;

using Visualizations.Abstracts;



/*
 * Generic data structure
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class GenericDataStructure
        {
            /* ------------------------------------------------------------------*/
            // public properties

            public List<GenericDataStructure> Branches { get { return _branches; } }

            public List<GenericDataEntry> Entries { get { return _entries; } }


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

            public List<MetaData> ListMetaData()
            {
                List<MetaData> metadata_list = new List<MetaData>();
                list_metadata(this, ref metadata_list);
                return metadata_list;
            }

            public int DataDimension()
            {
                int dim = -1;
                if (!get_dimension(this, ref dim))
                {
                    dim = -1;
                }
                return dim;
            }

            public List<Type> ValueTypes()
            {
                var value_types = new List<Type>();
                get_valuetypes(this, ref value_types);
                return value_types;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="branch"></param>
            /// <param name="entry_index"></param>
            /// <param name="out_entry"></param>
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

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="branch"></param>
            /// <param name="out_metadatalist"></param>
            private void list_metadata(GenericDataStructure branch, ref List<MetaData> out_metadatalist)
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

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="branch"></param>
            /// <param name="out_metadatalist"></param>
            private bool get_dimension(GenericDataStructure branch, ref int out_dim)
            {
                bool retval = true;
                foreach (var entry in branch.Entries)
                {
                    if (out_dim == -1)
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

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="branch"></param>
            /// <param name="out_metadatalist"></param>
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


            /* ------------------------------------------------------------------*/
            // private variables

            private List<GenericDataStructure> _branches = new List<GenericDataStructure>();
            private List<GenericDataEntry> _entries = new List<GenericDataEntry>();
        }
    }
}
