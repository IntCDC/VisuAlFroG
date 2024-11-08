using System;



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
            /* ------------------------------------------------------------------*/
            #region static functions

            /// <summary>
            /// [STATIC]
            /// </summary>
            /// <returns></returns>
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
                    else
                    {
                        data_leaf.AddValue("column_" + (j - 1).ToString());
                    }
                    data_branch.AddEntry(data_leaf);
                }
                sample_data.AddBranch(data_branch);

                for (int i = 0; i < 7; i++)
                {
                    data_branch = new GenericDataStructure();

                    var data_leaf = new GenericDataEntry();
                    data_leaf.AddValue(("row_" + i.ToString()));
                    data_branch.AddEntry(data_leaf);
                    for (int index = 0; index < 25; index++)
                    {
                        data_leaf = new GenericDataEntry();

                        var x_value = generator.Next(0, 50);
                        data_leaf.AddValue((double)x_value); // index | x_value

                        var y_value = generator.Next(0, 50);
                        data_leaf.AddValue((double)y_value);

                        data_branch.AddEntry(data_leaf);
                    }
                    sample_data.AddBranch(data_branch);
                }

                return sample_data;
            }

            #endregion
        }
    }
}
