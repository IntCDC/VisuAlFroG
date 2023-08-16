using System;
using System.Collections.Generic;
using Core.Utilities;
using Core.Abstracts;



/*
 * Abstract Service Manager
 * 
 */
namespace Core
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
                    _initilized = false;
                }
                return terminated;
            }


            public void AddService(AbstractService service)
            {
                _services.Add(service.GetType(), service);
            }


            public T GetService<T>() where T : AbstractService
            {
                if (_services.ContainsKey(typeof(T)))
                {
                    return (T)_services[typeof(T)];
                }
                else
                {
                    return null;
                }
            }

            /* ------------------------------------------------------------------*/
            // private variables

            private Dictionary<Type, AbstractService> _services = new Dictionary<Type, AbstractService>();

        }
    }
}
