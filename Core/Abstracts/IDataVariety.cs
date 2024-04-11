using System;
using System.Collections.Generic;
using Core.Data;



/*
 *  Data variety interface
 * 
 */
namespace Core
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
