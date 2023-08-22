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



using AbstractData_Type = System.Collections.Generic.List<System.Collections.Generic.List<double>>;
using GHData_Type = Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.IGH_Goo>;


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

            public static AbstractData_Type GH_to_List(ref GHData_Type input)
            {
                var output = new AbstractData_Type();

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
                            Log.Default.Msg(Log.Level.Error, "Can not convert data from: " + type.Name);
                        }
                    }
                    output.Add(list);
                }
                return output;
            }


            public static GHData_Type list_to_gh(ref AbstractData_Type input)
            {
                var ouptut = new GHData_Type();

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
