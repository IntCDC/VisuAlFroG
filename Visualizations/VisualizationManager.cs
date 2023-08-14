using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizations.PythonInterface;
using Visualizations.WebAPI;
using Visualizations.SciChartInterface;
using System.Net.Http;
using Microsoft.Owin.Hosting;
using Core.Utilities;
using Core.Management;
using EntityFrameworkDatabase;



/*
 * Visualization Management
 * 
 */
namespace Visualizations
{
    public class VisualizationManager : AbstractService
    {

        /* ------------------------------------------------------------------*/
        // public functions

        public VisualizationManager()
        {
            _servicemanager.AddService(new WebAPIService());
            _servicemanager.AddService(new PythonInterfaceService());
            _servicemanager.AddService(new SciChartInterfaceService());
            _servicemanager.AddService(new DatabaseService());
        }


        public override bool Initialize()
        {
            if (_initilized)
            {
                _servicemanager.Terminate();
            }

            bool initilized = _servicemanager.Initialize();

            _initilized = initilized;
            return _initilized;

        }


        public override bool Execute()
        {
            if (!_initilized)
            {
                return false;
            }

            bool executed = _servicemanager.Execute();

            return executed;
        }


        public override bool Terminate()
        {
            bool terminated = true;
            if (_initilized)
            {
                terminated &= _servicemanager.Terminate();
                _initilized = false;
            }
            return terminated;
        }


        public T GetService<T>() where T : AbstractService
        {
            return _servicemanager.GetService<T>();
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private ServiceManager _servicemanager = new ServiceManager();
    }
}

