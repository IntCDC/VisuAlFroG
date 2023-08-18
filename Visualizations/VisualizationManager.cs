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
using Core.GUI;
using Visualizations.Types;



using ContentCallbacks = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.RequestContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;


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
            }

            bool initilized = _servicemanager.Initialize();
            initilized &= _contentmanager.Initialize();

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
            executed &= _contentmanager.Execute();

            return executed;
        }


        public override bool Terminate()
        {
            bool terminated = true;
            if (_initilized)
            {
                terminated &= _servicemanager.Terminate();
                terminated &= _contentmanager.Terminate();
                _initilized = false;
            }
            return terminated;
        }


        public ContentCallbacks GetContentCallbacks()
        {
            return new ContentCallbacks(_contentmanager.GetContentsCallback, _contentmanager.RequestContentCallback, _contentmanager.DeleteContentCallback);
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private ServiceManager _servicemanager = new ServiceManager();
        private ContentManager _contentmanager = new ContentManager();
    }
}

