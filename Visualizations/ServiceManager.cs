using System;
using System.Collections.Generic;
using Core.Utilities;
using Core.Abstracts;



/*
 * Service Manager
 * 
 */
namespace Visualizations
{
    public class ServiceManager : AbstractService
    {

        /* ------------------------------------------------------------------*/
        // public functions

        /// <summary>
        /// Add new service.
        /// </summary>
        /// <param name="service">The service object.</param>
        public void AddService(AbstractService service)
        {
            if (_services == null)
            {
                _services = new Dictionary<Type, AbstractService>();
            }

            Type service_type = service.GetType();
            if (!_services.ContainsKey(service_type))
            {
                _services.Add(service_type, service);
                Log.Default.Msg(Log.Level.Info, "Added Service: " + service_type.FullName);
            }
            else
            {
                Log.Default.Msg(Log.Level.Warn, "Service has already been added: " + service_type.FullName);
            }
        }

        public override bool Initialize()
        {
            if (_initialized)
            {
                Terminate();
            }
            bool initialized = true;
            _timer.Start();

            if (_services != null)
            {
                foreach (var m in _services)
                {
                    initialized &= m.Value.Initialize();
                }
            }

            _timer.Stop();
            _initialized = initialized;
            return _initialized;
        }

        public override bool Terminate()
        {
            bool terminated = true;
            if (_initialized)
            {
                if (_services != null)
                {
                    foreach (var m in _services)
                    {
                        terminated &= m.Value.Terminate();
                    }
                    _services.Clear();
                    _services = null;
                }
                _initialized = false;
            }
            return terminated;
        }


        /* ------------------------------------------------------------------*/
        // private variables

        private Dictionary<Type, AbstractService> _services = null;
    }
}
