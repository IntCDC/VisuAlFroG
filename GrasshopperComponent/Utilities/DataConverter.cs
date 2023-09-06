using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Grasshopper.Kernel.Special;
using Core.Utilities;
using Grasshopper.Kernel.Types;
using System.Globalization;
using Grasshopper.Kernel.Data;
using GrasshopperComponent.Utilities;
using Visualizations.Interaction;



/*
 * Convert Data from and to Grasshopper data format 
 * 
 */
namespace GrasshopperComponent
{
    namespace Utilities
    {
        public class DataConverter
        {
            /* ------------------------------------------------------------------*/
            // static functions

            /// <summary>
            /// [STATIC] Convert data provided by the interface to data type not depending on interface specific type. 
            /// </summary>
            /// <param name="input">Reference to the input data.</param>
            /// <returns>The converted output data.</returns>
            public static GenericDataBranch ConvertFromGHStructure(ref GH_Structure<IGH_Goo> input_data)
            {
                var output_data = new GenericDataBranch();

                foreach (var input_leafs in input_data.Branches)
                {
                    var output_branch = new GenericDataBranch();
                    foreach (var input_value in input_leafs)
                    {
                        var output_leaf = new GenericDataEntry();

                        var type = input_value.GetType();
                        if (type == typeof(GH_String))
                        {
                            string value_string;
                            if (input_value.CastTo<string>(out value_string))
                            {
                                // TODO Add more conversions
                                try
                                {
                                    double value_double = Convert.ToDouble(value_string, CultureInfo.InvariantCulture);
                                    output_leaf.AddValue(value_double);
                                }
                                catch (Exception exc)
                                {
                                    ///Log.Default.Msg(Log.Level.Error, exc.Message);
                                    output_leaf.AddValue(value_string);
                                }
                            }
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Can not convert input data from type: " + type.FullName);
                        }
                        output_branch.AddLeaf(output_leaf);
                    }
                    output_data.AddBranch(output_branch);
                }
                return output_data;
            }

            /// <summary>
            /// [STATIC] Convert data provided by the application to interface specific data type. 
            /// </summary>
            /// <param name="input">Reference to the input data.</param>
            /// <returns>The converted output data.</returns>
            public static GH_Structure<IGH_Goo> ConvertToGHStructure(ref GenericDataBranch input_data)
            {
                var output_data = new GH_Structure<IGH_Goo>();

                /// TODO Support branches -> is this supported by GH_Structure nevertheless?
                if (input_data.Branches.Count > 0) {

                    Log.Default.Msg(Log.Level.Error, "output of branches is currently not supported... ");
                }

                int branch_index = 0;
                foreach (var input_leaf in input_data.Leafs)
                {
                    GH_Path path = new GH_Path(branch_index);
                    foreach (var generic_value in input_leaf.Values)
                    {
                        output_data.Append(new GH_String(generic_value.ToString()), path);
                    }
                    output_data.EnsurePath(path);
                    branch_index++;
                }

                return output_data;
            }
        }
    }
}
