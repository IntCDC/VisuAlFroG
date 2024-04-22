﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        public interface IDataType
        {
            /* ------------------------------------------------------------------*/
            // public properties

            object Get { get; }

            List<Dimension> SupportedDimensions { get; }
            List<Type> SupportedValueTypes { get; }


            /* ------------------------------------------------------------------*/
            // public functions

            void Update(GenericDataStructure data);

            void UpdateMetaDataEntry(IMetaData updated_meta_data);
        }
    }
}
