using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data;



/*
 * Example data for debugging in detached mode
 * 
 */
namespace Frontend
{
    class ExampleData
    {
        public static GenericDataStructure Get()
        {
            var generator = new Random();
            var sample_data = new GenericDataStructure();

            uint value_index = 0;
            for (int i = 0; i < 7; i++)
            {
                var data_branch = new GenericDataStructure();
                data_branch._Label = "labled_" + i.ToString();

                for (int j = 0; j < 25; j++)
                {
                    var value = generator.Next(0, 50);
                    var data_leaf = new GenericDataEntry();
                    data_leaf.AddValue((double)value);

                    data_leaf._Metadata._Index = value_index;
                    data_leaf._Metadata._Label = "entry_" + j.ToString();
                    value_index++;

                    data_branch.AddEntry(data_leaf);
                }
                sample_data.AddBranch(data_branch);
            }

            return sample_data;
        }
    }
}
