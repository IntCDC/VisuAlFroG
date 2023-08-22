using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Abstracts;
using Core.Utilities;



using AbstractData_Type = System.Collections.Generic.List<System.Collections.Generic.List<double>>;


/*
 * Data Manager
 * 
 */
namespace Visualizations
{
    namespace Management
    {
        public class DataManager : AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public delegates

            /// <summary>
            /// Function provided by the data manager for passng on the inpout data to the visualizations
            /// </summary>
            public delegate void InputData_Delegate(ref AbstractData_Type input_data);

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows pass output data to the interface
            /// </summary>
            public delegate void OutputData_Delegate(ref AbstractData_Type ouput_data);


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                bool initilized = true;



                _initilized = initilized;
                return _initilized;
            }


            public override bool Execute()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                bool executed = true;



                return executed;
            }


            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {



                    _initilized = false;
                }
                return terminated;
            }


            public void InputDataCallback(ref AbstractData_Type input_data)
            {
                _input_data = input_data;
            }


            public void RegisterOutputDataCallback(OutputData_Delegate outputdata_callback)
            {
                _outputdata_callback = outputdata_callback;
            }



            /* ------------------------------------------------------------------*/
            // private functions

            private OutputData_Delegate _outputdata_callback = null;
            private AbstractData_Type _input_data = null;
        }
    }
}
