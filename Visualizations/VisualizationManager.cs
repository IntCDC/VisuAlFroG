using System;
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



using ContentCallbacks = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.RequestContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;
using static Visualizations.Management.DataManager;


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

        public override bool Initialize()
        {
            if (_initilized)
            {
                _servicemanager.Terminate();
                _contentmanager.Terminate();
                _datamanager.Terminate();
            }

            bool initilized = _datamanager.Initialize();
            initilized &= _contentmanager.Initialize(_datamanager.RequestDataCallback());

            // Check for services the contents rely on
            var depending_services = _contentmanager.DependingServices();
            foreach (Type service_type in depending_services)
            {
                // Create new instance from type
                var new_service = (AbstractService)Activator.CreateInstance(service_type);
                _servicemanager.AddService(new_service);
            }
            initilized &= _servicemanager.Initialize();

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

            bool executed = _servicemanager.Execute();
            executed &= _datamanager.Execute();
            executed &= _contentmanager.Execute();

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


        public ContentCallbacks GetContentCallbacks()
        {
            return new ContentCallbacks(_contentmanager.ContentsCallback, _contentmanager.AttachContentCallback, _contentmanager.DetachContentCallback);
        }


        public InputData_Delegate GetInputDataCallback()
        {
            return _datamanager.InputDataCallback;
        }


        public void RegisterOutputDataCallback(DataManager.OutputData_Delegate _outputdata_callback)
        {
            _datamanager.RegisterOutputDataCallback(_outputdata_callback);
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private ServiceManager _servicemanager = new ServiceManager();
        private ContentManager _contentmanager = new ContentManager();
        private DataManager _datamanager = new DataManager();
    }
}

