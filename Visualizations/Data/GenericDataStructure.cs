using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;



/*
 * Generic data structure
 * 
 */
namespace Visualizations
{
    namespace Data
    {


        public class GenericDataEntry
        {
            /* ------------------------------------------------------------------*/
            // public functions

            public void AddValue(object value) { _values.Add(value); _dim = _values.Count; }
            public void AddValues<T>(List<T> values)
            {
                _values.Clear();
                foreach (var value in values)
                {
                    _values.Add((object)value);
                }
                _dim = _values.Count;
            }
            public List<object> Values { get { return _values; } }
            public MetaData MetaData { get { return _metadata; } set { _metadata = value; } }
            public int Dimension { get { return _dim; } }

            public bool HasIndex(int entry_index)
            {
                return (_metadata.Index == entry_index);
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private List<object> _values = new List<object>();
            private MetaData _metadata = new MetaData();
            private int _dim = 0;
        }



        public class GenericDataStructure
        {
            /* ------------------------------------------------------------------*/
            // public functions

            public void AddBranch(GenericDataStructure branch) { _branches.Add(branch); }
            public void AddEntry(GenericDataEntry entry) { _entries.Add(entry); }

            public List<GenericDataStructure> Branches { get { return _branches; } }
            public List<GenericDataEntry> Entries { get { return _entries; } }

            public GenericDataEntry EntryAtIndex(int entry_index)
            {
                GenericDataEntry entry = null;
                find_entry(this, entry_index, ref entry);
                return entry;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void find_entry(GenericDataStructure branch, int entry_index, ref GenericDataEntry out_entry)
            {
                if(out_entry != null) 
                {
                    return;
                }
                foreach(var b in branch.Branches)
                {
                    find_entry(b, entry_index, ref out_entry);
                }
                foreach(var l in branch.Entries)
                {
                    if (l.HasIndex(entry_index)) {
                        out_entry = l;
                        return;
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
