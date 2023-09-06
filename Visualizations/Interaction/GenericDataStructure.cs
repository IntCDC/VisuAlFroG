using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



/*
 * Generic data structure
 * 
 */
namespace Visualizations
{
    namespace Interaction
    {


        public class GenericDataEntry
        {
            /* ------------------------------------------------------------------*/
            // public functions

            public void AddValue(object value) { _values.Add(value); }
            public void AddValues<T>(List<T> values)
            {
                foreach (var value in values)
                {
                    _values.Add((object)value);
                }
            }
            public List<object> Values { get { return _values; } }
            public MetaData MetaData { get { return _metadata; } }

            /* ------------------------------------------------------------------*/
            // private variables

            private List<object> _values = new List<object>();
            private MetaData _metadata = new MetaData();
        }



        public class GenericDataBranch
        {
            /* ------------------------------------------------------------------*/
            // public functions

            public void AddBranch(GenericDataBranch branch) { _branches.Add(branch); }
            public void AddLeaf(GenericDataEntry leaf) { _leafs.Add(leaf); }

            public List<GenericDataBranch> Branches { get { return _branches; } }
            public List<GenericDataEntry> Leafs { get { return _leafs; } }

            /* ------------------------------------------------------------------*/
            // private variables

            private List<GenericDataBranch> _branches = new List<GenericDataBranch>();
            private List<GenericDataEntry> _leafs = new List<GenericDataEntry>();
        }
    }

}
