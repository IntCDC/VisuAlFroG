using System;
using System.Collections.Generic;
using Core.Utilities;
using Core.Abstracts;
using EntityFrameworkDatabase;
using Visualizations.SciChartInterface;



/*
 * Service Manager
 * 
 */
namespace Visualizations
{
    namespace Management
    {
        public class ServiceManager : AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }
                bool initilized = true;

                foreach (var m in _services)
                {
                    initilized &= m.Value.Initialize();
                }

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
                bool executed = true;
                foreach (var m in _services)
                {
                    executed &= m.Value.Execute();
                }
                return executed;
            }


            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {
                    foreach (var m in _services)
                    {
                        terminated &= m.Value.Terminate();
                    }
                    _services.Clear();
                    _initilized = false;
                }
                return terminated;
            }


            public void AddService(AbstractService service)
            {
                Type service_type = service.GetType();
                if (!_services.ContainsKey(service_type)) {
                    _services.Add(service_type, service);
                    Log.Default.Msg(Log.Level.Info, "Added Service: " + service_type.FullName);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Warn, "Service was already added: " + service_type.FullName);
                }
            }


            public T GetService<T>() where T : AbstractService
            {
                Type service_type = typeof(T);
                if (_services.ContainsKey(service_type))
                {
                    return (T)_services[service_type];
                }
                Log.Default.Msg(Log.Level.Warn, "Unable to find service: " + service_type.FullName);
                return null;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private Dictionary<Type, AbstractService> _services = new Dictionary<Type, AbstractService>();
        }
    }
}
