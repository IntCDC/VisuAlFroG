using System;
using Visualizations.PythonInterface;
using Visualizations.WebAPI;
using Core.Abstracts;
using Core.Data;



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
            _contentmanager = new ContentManager();
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
            bool initialized = _contentmanager.Initialize(_datamanager.RequestDataCallback, _datamanager.RegisterUpdateTypeCallback, _datamanager.UnregisterUpdateCallback);
            var required_services = _contentmanager.DependingServices();

            // Data Manager
            initialized &= _datamanager.Initialize();

            // Service Manager
            foreach (Type service_type in required_services)
            {
                var new_service = (AbstractService)Activator.CreateInstance(service_type);
                _servicemanager.AddService(new_service);
            }

            /// DEBUG
            //_servicemanager.AddService(new PythonInterfaceService());
            //_servicemanager.AddService(new WebAPIService());
            
            
            initialized &= _servicemanager.Initialize();

            _timer.Stop();
            _initialized = initialized;
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
            return new ContentCallbacks_Type(_contentmanager.AvailableContentsCallback, _contentmanager.CreateContentCallback, _contentmanager.DeleteContentCallback);
        }

        public DataManager.InputData_Delegate GetInputDataCallback()
        {
            return _datamanager.GetInputDataCallback;
        }

        public void SetOutputDataCallback(DataManager.OutputData_Delegate _outputdata_callback)
        {
            _datamanager.SetOutputDataCallback(_outputdata_callback);
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private ServiceManager _servicemanager = null;
        private ContentManager _contentmanager = null;
        private DataManager _datamanager = null;
    }
}

