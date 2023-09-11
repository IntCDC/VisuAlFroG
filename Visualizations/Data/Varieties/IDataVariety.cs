using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Visualizations.Data;



/*
 *  Data variety interface
 * 
 */
namespace Visualizations
{
    namespace Abstracts
    {

        [Flags]
        public enum DataDimensionality
        {
            None = 0,
            Uniform = 1,
            TwoDimensional = 2,
            ThreeDimensional = 4,
            Multidimensional = 8,
            All = (Uniform | TwoDimensional | ThreeDimensional | Multidimensional),
        }


        public interface IDataVariety
        {
            /* ------------------------------------------------------------------*/
            // public properties

            Type Variety { get; }

            object Get { get; }

            DataDimensionality SupportedDimensionality { get; }
            List<Type> SupportedValueTypes { get; }


            /* ------------------------------------------------------------------*/
            // public functions

            void Update(ref GenericDataStructure data, DataDimensionality dimensionality, List<Type> value_types);

            void UpdateEntryAtIndex(int index, GenericDataEntry updated_entry);
        }
    }
}
