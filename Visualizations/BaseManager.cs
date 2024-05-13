using System;
using Visualizations.PythonInterface;
using Visualizations.WebAPI;
using Core.Abstracts;
using Core.Data;
using static Core.Data.DataManager;
using Core.GUI;



/*
 * Visualization Management
 * 
 */

using ContentCallbacks_Type = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.CreateContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;


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
            bool initialized = _contentmanager.Initialize(_datamanager.GetDataCallback, _datamanager.RegisterDataCallback, _datamanager.UnregisterDataCallback);
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

        // Callback forwarding for ContentManager
        public ContentCallbacks_Type GetContentCallbacks()
        {
            return new ContentCallbacks_Type(_contentmanager.AvailableContentsCallback, _contentmanager.CreateContentCallback, _contentmanager.DeleteContentCallback);
        }

        // Callback forwarding for DataManager
        public void UpdateInputData(GenericDataStructure input_data)
        {
            _datamanager.UpdateData(input_data);
        }
        public void SetOutputDataCallback(DataManager.SetDataCallback_Delegate _outputdata_callback)
        {
            _datamanager.SetOutputDataCallback(_outputdata_callback);
        }
        public MenuBar.MenuCallback_Delegate GetSaveDataCallback()
        {
            return _datamanager.SaveCSVData;
        }
        public MenuBar.MenuCallback_Delegate GetLoadDataCallback()
        {
            return _datamanager.LoadCSVData;
        }
        public MenuBar.MenuCallback_Delegate GetSendDataCallback()
        {
            return _datamanager.SendData;
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private ServiceManager _servicemanager = null;
        private ContentManager _contentmanager = null;
        private DataManager _datamanager = null;
    }
}

