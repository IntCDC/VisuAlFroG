using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data;

using Newtonsoft.Json.Linq;



/*
 * Example data for debugging in detached mode
 * 
 */
namespace Core
{
    namespace Data
    {
        public class TestData
        {
            public static GenericDataStructure Generate()
            {
                var generator = new Random();
                var sample_data = new GenericDataStructure();

                var data_branch = new GenericDataStructure();
                for (int j = 0; j < 26; j++)
                {
                    var data_leaf = new GenericDataEntry();

                    if (j == 0)
                    {
                        data_leaf.AddValue("row_headers");
                    }
                    else {
                        data_leaf.AddValue("column_" + (j-1).ToString());
                    }
                    data_branch.AddEntry(data_leaf);


                }
                sample_data.AddBranch(data_branch);


                for (int i = 0; i < 7; i++)
                {
                    var index_value = generator.Next(0, 50);

                    data_branch = new GenericDataStructure();

                    var data_leaf = new GenericDataEntry();
                    data_leaf.AddValue(("row_" + i.ToString()));
                    data_branch.AddEntry(data_leaf);
                    for (int j = 0; j < 25; j++)
                    {
                        var value = generator.Next(0, 50);
                        data_leaf = new GenericDataEntry();

                        data_leaf.AddValue((double)j); // index_value
                        data_leaf.AddValue((double)value);

                        data_branch.AddEntry(data_leaf);
                    }
                    sample_data.AddBranch(data_branch);
                }

                return sample_data;
            }
        }
    }
}
