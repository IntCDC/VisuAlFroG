﻿using System;
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
        #region public functions

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
            bool initialized = _contentmanager.Initialize(_datamanager.GetDataCallback, _datamanager.GetDataMenuCallback, _datamanager.RegisterDataCallback, _datamanager.UnregisterDataCallback);

            // Data Manager
            initialized &= _datamanager.Initialize();

            // Service Manager
            var required_services = _contentmanager.DependingServices();
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

        public override void AttachMenu(MainMenuBar menu_bar)
        {
            ///_servicemanager.AttachMenu(menu_bar);
            ///_contentmanager.AttachMenu(menu_bar);
            _datamanager.AttachMenu(menu_bar);
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private variables

        private ServiceManager _servicemanager = null;
        private ContentManager _contentmanager = null;
        private DataManager _datamanager = null;

        #endregion
    }
}

