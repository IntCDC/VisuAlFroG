﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizations.PythonInterface;
using Visualizations.WebAPI;
using Visualizations.SciChartInterface;
using Visualizations.Management;
using System.Net.Http;
using Microsoft.Owin.Hosting;
using Core.Utilities;
using Core.Abstracts;
using EntityFrameworkDatabase;
using System.Windows.Controls;
using Visualizations.Types;



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
            _servicemanager = new ServiceManager();
            _contentmanager = new ContentManager();
            _datamanager = new DataManager();
        }


        public override bool Initialize()
        {
            if (_initilized)
            {
                _servicemanager.Terminate();
                _contentmanager.Terminate();
                _datamanager.Terminate();
            }
            _timer.Start();

            // Content Manager
            bool initilized = _contentmanager.Initialize(_datamanager.RequestDataCallback());
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
            _timer.Start();

            bool executed = _servicemanager.Execute();
            executed &= _datamanager.Execute();
            executed &= _contentmanager.Execute();

            _timer.Stop();
            return executed;
        }


        public override bool Terminate()
        {
            bool terminated = true;
            if (_initilized)
            {
                terminated &= _servicemanager.Terminate();
                terminated &= _datamanager.Terminate();
                terminated &= _contentmanager.Terminate();
                _initilized = false;
            }
            return terminated;
        }


        public ContentCallbacks_Type GetContentCallbacks()
        {
            return new ContentCallbacks_Type(_contentmanager.ContentsCallback, _contentmanager.AttachContentCallback, _contentmanager.DetachContentCallback);
        }


        public DataManager.InputData_Delegate GetInputDataCallback()
        {
            return _datamanager.InputDataCallback;
        }


        public void RegisterOutputDataCallback(DataManager.OutputData_Delegate _outputdata_callback)
        {
            _datamanager.RegisterOutputDataCallback(_outputdata_callback);
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private ServiceManager _servicemanager = null;
        private ContentManager _contentmanager = null;
        private DataManager _datamanager = null;
    }
}

