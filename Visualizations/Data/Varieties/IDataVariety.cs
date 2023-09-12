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
        public enum Dimension
        {
            None = 0,
            Uniform = 1,
            TwoDimensional = 2,
            ThreeDimensional = 4,
            Multidimensional = 8,
        }


        public interface IDataVariety
        {
            /* ------------------------------------------------------------------*/
            // public properties

            Type Variety { get; }

            object Get { get; }

            List<Dimension> SupportedDimensions { get; }
            List<Type> SupportedValueTypes { get; }


            /* ------------------------------------------------------------------*/
            // public functions

            void Create(ref GenericDataStructure data, int data_dimension, List<Type> value_types);

            void UpdateEntryAtIndex(GenericDataEntry updated_entry);
        }
    }
}
