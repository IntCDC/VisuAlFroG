using System;
using System.Linq;
using Core.Utilities;
using Core.Data;
using System.Collections.Generic;



/*
 * Convert Data from and to CSV data format 
 * 
 */
namespace Core
{
    namespace Data
    {
        public class CSV_DataConverter
        {
            /* ------------------------------------------------------------------*/
            // static functions

            /// <summary>
            /// [STATIC] Convert CSV formatted data to generic data type. 
            /// </summary>
            /// <param name="input">Reference to the input data.</param>
            /// <returns>The converted output data.</returns>
            public static GenericDataStructure ConvertFromCSV(string input_data)
            {
                var generic_data = new GenericDataStructure();






                return generic_data;
            }

            /// <summary>
            /// [STATIC] Convert generic data type to CSV formatted data. 
            /// </summary>
            /// <param name="input">Reference to the input data.</param>
            /// <returns>The converted output data.</returns>
            public static string ConvertToCSV(GenericDataStructure input_data)
            {
                string csv_data = "";





                return csv_data;
            }
        }
    }
}
