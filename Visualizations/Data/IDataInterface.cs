using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciChart.Charting.Visuals;
using Visualizations.Data;



namespace Visualizations
{
    namespace Data
    {

        public enum DataStyles
        {
            Default
        }


        public interface IDataInterface
        {
            /// <summary>
            /// TODO
            /// </summary>
            bool Set(object data_parent);

            /// <summary>
            /// TODO Add parameter for style
            /// </summary>
            void SetDataStyle(DataStyles style);

            /// <summary>
            /// Visualizations need to be able to request data
            /// </summary>
            DataManager.RequestCallback_Delegate RequestDataCallback { get; set; }
        }
    }
}
