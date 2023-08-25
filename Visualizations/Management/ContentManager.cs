using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Core.Utilities;
using Core.Abstracts;
using Visualizations.Types;
using System.Reflection;
using System.Windows;
using SciChart.Core.Extensions;
using SciChart.Charting.Visuals;
using Core.GUI;
using Visualizations.Abstracts;
using Visualizations.Management;



/*
 * Content Manager
 * 
 */
namespace Visualizations
{
    namespace Management
    {
        public class ContentManager : AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public bool Initialize(DataManager.RequestDataCallback_Delegate request_data_callback)
            {
                if (_initilized)
                {
                    Terminate();
                }
                if (request_data_callback == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Missing request data callback");
                    return false;
                }
                _timer.Start();

                _request_data_callback = request_data_callback;

                _contents = new Dictionary<Type, Dictionary<string, AbstractContent>>();

                register_content(typeof(LogContent));
                register_content(typeof(DEBUGLines));
                register_content(typeof(DEBUGColumns));

                _timer.Stop();
                _initilized = true;
                return _initilized;
            }


            public override bool Terminate()
            {
                if (_initilized)
                {
                    foreach (var c_data in _contents)
                    {
                        foreach (var c in c_data.Value)
                        {
                            c.Value.Terminate();
                        }
                        c_data.Value.Clear();
                    }
                    _contents.Clear();
                    _initilized = false;
                }
                return true;
            }


            /// <summary>
            ///  Returns distinct list of vaild services required by the registered contents
            /// </summary>
            public List<Type> DependingServices()
            {
                var depending_services = new List<Type>();

                if (_initilized)
                {
                    // Loop over registered types
                    foreach (var c_data in _contents)
                    {
                        Type c_type = c_data.Key;

                        _timer.Start();

                        // Create temporary instance of content
                        var tmp_content = (AbstractContent)Activator.CreateInstance(c_type);
                        var lservice_types = tmp_content.DependingServices;
                        foreach (Type lservice_type in lservice_types)
                        {
                            // Only consider valid services
                            if ((lservice_type != null) && recursive_basetype(lservice_type, typeof(AbstractService)))
                            {
                                depending_services.Add(lservice_type);
                            }
                        }

                        _timer.Stop();
                    }
                    // Remove duplicates
                    depending_services = depending_services.Distinct().ToList();
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to requesting depending services");
                }
                return depending_services;
            }


            /// <summary>
            ///  Provide necessary information of available window content
            /// >> Called by WindowLeaf
            /// </summary>
            public AvailableContentList_Type ContentsCallback()
            {
                var content_ids = new AvailableContentList_Type();

                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return content_ids;
                }

                // Loop over registered types
                foreach (var c_data in _contents)
                {
                    Type c_type = c_data.Key;

                    // Create temporary instance of content
                    var tmp_content = (AbstractContent)Activator.CreateInstance(c_type);
                    string header = tmp_content.Name;
                    bool multiple_instances = tmp_content.MultipleIntances;

                    // Content is only available if multiple instance are allowed or has not been instanciated yet
                    bool available = (multiple_instances || (c_data.Value.IsEmpty() && !multiple_instances));

                    content_ids.Add(new AvailableContent_Type(header, available, multiple_instances, c_type));
                }

                return content_ids;
            }


            /// <summary>
            /// Attach requested content to provided parent content element.
            /// >> Called by WindowLeaf
            /// </summary>
            public Control AttachContentCallback(string content_id, Type content_type)
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                // Loop over registered types
                if (_contents.ContainsKey(content_type))
                {
                    string id = content_id;
                    if (!_contents[content_type].ContainsKey(id))
                    {
                        // Create new instance from type
                        var new_content = (AbstractContent)Activator.CreateInstance(content_type);
                        if (recursive_basetype(new_content.GetType(), typeof(AbstractVisualization)))
                        {
                            ((AbstractVisualization)new_content).SetRequestDataCallback(_request_data_callback);
                        }
                        new_content.Initialize();
                        new_content.Create();
                        id = new_content.ID;
                        _contents[content_type].Add(id, new_content);
                    }

                    return _contents[content_type][id].Attach();

                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Unregistered content type: " + content_type.ToString());
                }
                return null;
            }


            /// <summary>
            /// Delete content.
            /// >> Called by WindowLeaf
            /// </summary>
            public void DetachContentCallback(string content_id)
            {
                // Loop over registered types
                foreach (var c_data in _contents)
                {
                    if (c_data.Value.ContainsKey(content_id))
                    {
                        c_data.Value[content_id].Detach();
                        c_data.Value.Remove(content_id);
                    }
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void register_content(Type content_type)
            {
                // Check for required base type
                Type type = content_type;
                Type required_basetype = typeof(AbstractContent);
                bool valid_type = false;
                while (!recursive_basetype(type, typeof(object)))
                {
                    if (recursive_basetype(type, required_basetype))
                    {
                        valid_type = true;
                        break;
                    }
                    type = type.BaseType;
                    if (type == null)
                    {
                        break;
                    }
                }
                if (valid_type)
                {
                    _contents.Add(content_type, new Dictionary<string, AbstractContent>());
                    Log.Default.Msg(Log.Level.Info, "Registered content type: " + content_type.Name);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Incompatible content type: " + content_type.Name);
                }
            }


            bool recursive_basetype(Type check_type, Type reference_base_type)
            {
                Type base_type = check_type;
                bool valid_base_type = false;
                while (base_type != typeof(object))
                {
                    if (base_type == reference_base_type)
                    {
                        valid_base_type = true;
                        break;
                    }
                    base_type = base_type.BaseType;
                    if (base_type == null)
                    {
                        break;
                    }
                }
                return valid_base_type;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            // separate dict for each content type
            private Dictionary<Type, Dictionary<string, AbstractContent>> _contents = null;
            private DataManager.RequestDataCallback_Delegate _request_data_callback = null;
        }
    }
}
