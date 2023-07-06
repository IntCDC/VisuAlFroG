using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityFrameworkDatabase;
using Visualizations;


/*
 * Core functionality
 * 
 */
namespace Core
{
    public class Program
    {
        public void Main()
        {

            Visualizations.Program visualization = new Visualizations.Program();
            visualization.Main();

            DatabaseManagement database_management = new DatabaseManagement();
            database_management.Initialize();




        }
    }
}
