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



/*
 * Convert Data from and to Grasshopper data format 
 * 
 */
namespace GrasshopperComponent
{
    namespace Utilities
    {
        public class ConvertData
        {

            /* ------------------------------------------------------------------*/
            // static functions

            /// <summary>
            /// [STATIC] Convert data provided by the interface to data type not depending on interface specific type. 
            /// </summary>
            /// <param name="input">Reference to the data.</param>
            /// <returns>The converted data.</returns>
            public static DefaultData_Type GH_to_List(ref GH_Structure<IGH_Goo> input)
            {
                var output = new DefaultData_Type();

                foreach (var branch in input.Branches)
                {
                    List<double> list = new List<double>();
                    foreach (var leaf in branch)
                    {
                        var type = leaf.GetType();
                        if (type == typeof(GH_String))
                        {
                            string value_str;
                            if (leaf.CastTo<string>(out value_str))
                            {
                                try
                                {
                                    double value = Convert.ToDouble(value_str, CultureInfo.InvariantCulture);
                                    list.Add(value);
                                }
                                catch (Exception exc)
                                {
                                    Log.Default.Msg(Log.Level.Error, exc.Message);
                                }
                            }
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Can not convert data from: " + type.FullName);
                        }
                    }
                    output.Add(list);
                }
                return output;
            }

            /// <summary>
            /// [STATIC] Convert data provided by the application to interface specific data type. 
            /// </summary>
            /// <param name="input">Reference to the data.</param>
            /// <returns>The converted data.</returns>
            public static GH_Structure<IGH_Goo> list_to_gh(ref DefaultData_Type input)
            {
                var ouptut = new GH_Structure<IGH_Goo>();

                int branch_index = 0;
                foreach (var branch in input)
                {
                    GH_Path path = new GH_Path(branch_index);
                    foreach (var leaf in branch)
                    {
                        ouptut.Append(new GH_String(leaf.ToString()), path);
                    }
                    ouptut.EnsurePath(path);
                    branch_index++;
                }
                return ouptut;
            }
        }
    }
}
