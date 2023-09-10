using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizations.PythonInterface;
using Visualizations.WebAPI;
using Visualizations.SciChartInterface;
using Visualizations.Data;
using System.Net.Http;
using Microsoft.Owin.Hosting;
using Core.Utilities;
using Core.Abstracts;
using System.Windows.Controls;
using Visualizations.Types;
using Core.GUI;
using Visualizations.Abstracts;



/*
 * Visualization Management
 * 
 */
namespace Visualizations
{
    public class BaseManager : AbstractService
    {
        /* ------------------------------------------------------------------*/
        // public functions

        public BaseManager()
        {
            _servicemanager = new ServiceManager();
            _contentmanager = new VisualizationsManager();
            _datamanager = new DataManager();
        }

        public override bool Initialize()
        {
            if (_initialized)
            {
                _servicemanager.Terminate();
                _contentmanager.Terminate();
                _datamanager.Terminate();
            }
            _timer.Start();

            // Content Manager
            bool initilized = _contentmanager.Initialize(_datamanager.RequestDataCallback, _datamanager.RegisterUpdateCallback);
            var required_services = _contentmanager.DependingServices();

            // Data Manager
            initilized &= _datamanager.Initialize();

            // Service Manager
            foreach (Type service_type in required_services)
            {
                var new_service = (AbstractService)Activator.CreateInstance(service_type);
                _servicemanager.AddService(new_service);
            }
            initilized &= _servicemanager.Initialize();

            _timer.Stop();
            _initialized = initilized;
            return _initialized;
        }

        public override bool Terminate()
        {
            bool terminated = true;
            if (_initialized)
            {
                terminated &= _servicemanager.Terminate();
                terminated &= _datamanager.Terminate();
                terminated &= _contentmanager.Terminate();
                _initialized = false;
            }
            return terminated;
        }

        public string CollectConfigurations()
        {
            return _contentmanager.CollectConfigurations();
        }

        public bool ApplyConfigurations(string configuration)
        {
            return _contentmanager.ApplyConfigurations(configuration);
        }

        public ContentCallbacks_Type GetContentCallbacks()
        {
            return new ContentCallbacks_Type(_contentmanager.AvailableContents, _contentmanager.CreateContent, _contentmanager.DeleteContent);
        }

        public DataManager.InputData_Delegate GetInputDataCallback()
        {
            return _datamanager.UpdateInputData;
        }

        public void SetOutputDataCallback(DataManager.OutputData_Delegate _outputdata_callback)
        {
            _datamanager.SetOutputDataCallback(_outputdata_callback);
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private ServiceManager _servicemanager = null;
        private VisualizationsManager _contentmanager = null;
        private DataManager _datamanager = null;
    }
}

