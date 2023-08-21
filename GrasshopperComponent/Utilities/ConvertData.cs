using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Grasshopper.Kernel.Special;



using AbstractData_Type = System.Collections.Generic.List<System.Collections.Generic.List<double>>;
using GHData_Type = Grasshopper.Kernel.Data.GH_Structure<Grasshopper.Kernel.Types.IGH_Goo>;


/*
 * Global Messages Buffer 
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


                /*
                int i = 0;
                foreach (var branch in input_data.Branches)
                {
                Log.Default.Msg(Log.Level.Error, i.ToString() + " Branch Type: " + branch.GetType().FullName);

                foreach (var leaf in branch)
                {
                var type = leaf.GetType();
                //Log.Default.Msg(Log.Level.Error, i.ToString() + " Leaf Type: " + type.FullName);

                string data_s;
                if (leaf.CastTo<string>(out data_s))
                {
                    //Log.Default.Msg(Log.Level.Warn, i.ToString() + " Data String: " + data_s);
                }
                else {
                    Log.Default.Msg(Log.Level.Error, i.ToString() + " Data String: " + data_s);
                }

                i++;
                }
                }
                */


                return output;
            }


            public static GHData_Type list_to_gh(ref AbstractData_Type input)
            {
                var ouptut = new GHData_Type();





                return ouptut;
            }



        }
    }
}
