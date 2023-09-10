using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizations.Data;


/*
 *  Data Types for Visualizations
 * 
 */
namespace Visualizations
{
    namespace Data
    {
        public class DataTypes
        {

            public int Dimension { get; set; }


            /* ------------------------------------------------------------------*/
            // static functions


            public static List<Type> SupportedDataTypes()
            {
                return _supported_types;
            }


            /* ------------------------------------------------------------------*/
            // public functions

            public DataTypes()
            {



            }



            /* ------------------------------------------------------------------*/
            // public variables


            private static List<Type> _supported_types = new List<Type>() {
                typeof(GenericDataStructure),
                typeof(SciChart.Charting.Model.DataSeries.UniformXyDataSeries<double>),
                typeof(SciChart.Charting.Model.DataSeries.XyDataSeries<double, double>)
            };

            /*
            /// <summary>
            /// Recursively create data series.
            /// </summary>
            private void check_data_type(GenericDataStructure data, ref DataDimensionality data_dim)
            {
                foreach (var entry in data.Entries)
                {
                    var value_count = entry.Values.Count;
                    data_dim.Uniform |= (value_count == 1);
                    data_dim.XY |= (value_count == 2;)
                    data_dim.XYZ |= (value_count == 3;)
                    data_dim.Multivariate |= (value_count > 3;)

                }
                foreach (var branch in data.Branches)
                {
                    check_data_type(branch, ref data_dim);
                }
            }
            */
            /*
            private class DataDimensionality
            {
                public DataDimensionality 
                {
                    Uniform = false;
                    XY = false;
                    XYZ = false;
                    Multivariate = false;
                }

            public bool Uniform { get; set; }
            public bool XY { get; set; }
            public bool XYZ { get; set; }
            public bool Multivariate { get; set; }
            }
            */
        }
    }
}
