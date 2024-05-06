using System;
using System.Linq;
using Core.Utilities;
using Core.Data;
using Grasshopper.Kernel.Types;
using System.Globalization;
using Grasshopper.Kernel.Data;



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
            public static GenericDataStructure ConvertFromGHStructure(GH_Structure<IGH_Goo> input_data)
            {
                var output_data = new GenericDataStructure();

                foreach (var input_entries in input_data.Branches)
                {
                    var output_branch = new GenericDataStructure();
                    foreach (var input_value in input_entries)
                    {
                        var output_entry = new GenericDataEntry();

                        var type = input_value.GetType();
                        if (type == typeof(GH_String))
                        {
                            if (input_value.CastTo<string>(out string value_string))
                            {
                                // TODO Add more conversions
                                try
                                {
                                    double value_double = Convert.ToDouble(value_string, CultureInfo.InvariantCulture);
                                    output_entry.AddValue(value_double);
                                }
                                catch (Exception exc)
                                {
                                    //Log.Default.Msg(Log.Level.Error, exc.Message);
                                    
                                    // Default: Add as string
                                    output_entry.AddValue(value_string);
                                }
                            }
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Can not convert input data from type: " + type.FullName);
                        }
                        output_branch.AddEntry(output_entry);
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
            public static GH_Structure<IGH_Goo> ConvertToGHStructure(GenericDataStructure input_data)
            {
                var output_data = new GH_Structure<IGH_Goo>();

                /// TODO Support branches -> is this supported by GH_Structure nevertheless?
                if (input_data._Branches.Count > 0) {

                    Log.Default.Msg(Log.Level.Error, "output of branches is currently not supported... ");
                }

                int branch_index = 0;
                foreach (var input_entry in input_data._Entries)
                {
                    GH_Path path = new GH_Path(branch_index);
                    foreach (var generic_value in input_entry._Values)
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
