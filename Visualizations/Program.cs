using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PythonInterface;
using WebAPI;
using SciChartInterface;


/*
 * Visualization Management
 * 
 */
namespace Visualizations
{
    public class Program
    {
        public void Main()
        {
            WebAPI.Program web_api = new WebAPI.Program();
            web_api.Main();


            PythonInterface.Program python_interface = new PythonInterface.Program();
            python_interface.Main();


        }
    }
}
