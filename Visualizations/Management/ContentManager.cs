﻿using System;
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
using System.Runtime.InteropServices;
using Visualizations.Interaction;



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


                /// TODO Register new visualizations here:
                register_content(typeof(LogContent));
                register_content(typeof(FilterContent));
                register_content(typeof(ScatterPlotVisualization));
                register_content(typeof(ParallelCoordinatesPlotVisualization));
                register_content(typeof(LinesVisualization));
                register_content(typeof(ColumnsVisualization));
                //register_content(typeof(...));

                _timer.Stop();
                _initilized = true;
                return _initilized;
            }

            public override bool Terminate()
            {
                if (_initilized)
                {
                    foreach (var content_types in _contents)
                    {
                        foreach (var content_data in content_types.Value)
                        {
                            content_data.Value.Terminate();
                        }
                        content_types.Value.Clear();
                    }
                    _contents.Clear();
                    _initilized = false;
                }
                return true;
            }

            public string CollectSettings()
            {
                var settings = new List<AbstractContent.Settings>();
                foreach (var content_types in _contents)
                {
                    foreach (var content_data in content_types.Value)
                    {
                        settings.Add(new AbstractContent.Settings() { ID = content_data.Value.ID, Type = content_types.Key.FullName });
                    }
                }
                return SettingsService.Serialize<List<AbstractContent.Settings>>(settings);
            }

            public bool ApplySettings(string settings)
            {
                var visualizations_settings = SettingsService.Deserialize<List<AbstractContent.Settings>>(settings);
                if (visualizations_settings != null)
                {
                    foreach (var content_settings in visualizations_settings)
                    {
                        var type = get_type(content_settings.Type);
                        if (_contents.ContainsKey(type))
                        {
                            var id = content_settings.ID;
                            if (id == UniqueID.Invalid)
                            {
                                Log.Default.Msg(Log.Level.Warn, "Invalid content id: " + id);
                            }

                            if (!_contents[type].ContainsKey(id))
                            {
                                // Create new instance from type
                                var new_content = (AbstractContent)Activator.CreateInstance(type);
                                if (recursive_basetype(new_content.GetType(), typeof(AbstractVisualization)))
                                {
                                    ((AbstractVisualization)new_content).SetRequestDataCallback(_request_data_callback);
                                }

                                new_content.ID = id;
                                new_content.Initialize();
                                new_content.Create();

                                _contents[type].Add(id, new_content);
                            }
                            else
                            {
                                Log.Default.Msg(Log.Level.Error, "Content " + content_settings.Type + " with ID " + id + " already exists");
                            }
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Unregistered content type: " + type.ToString());
                        }
                    }
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Returns distinct list of valid services required by the registered contents.
            /// </summary>
            /// <returns>List of service types.</returns>
            public List<Type> DependingServices()
            {
                var depending_services = new List<Type>();

                if (_initilized)
                {
                    // Loop over registered types
                    foreach (var content_types in _contents)
                    {
                        _timer.Start();

                        // Create temporary instance of content
                        Type content_type = content_types.Key;
                        var tmp_content = (AbstractContent)Activator.CreateInstance(content_type);
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
            /// Provide necessary information of available window content (called by window leaf).
            /// </summary>
            /// <returns>List of available content meta data.</returns>
            public AvailableContentList_Type AvailableContents()
            {
                var content_ids = new AvailableContentList_Type();

                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return content_ids;
                }

                // Loop over registered types
                foreach (var content_types in _contents)
                {
                    var content_type = content_types.Key;

                    // Create temporary instance of content
                    var tmp_content = (AbstractContent)Activator.CreateInstance(content_type);
                    string header = tmp_content.Name;
                    bool multiple_instances = tmp_content.MultipleInstances;

                    // Content is only available if multiple instance are allowed or has not been instantiated yet
                    bool available = (multiple_instances || (content_types.Value.IsEmpty() && !multiple_instances));

                    content_ids.Add(new AvailableContent_Type(header, available, multiple_instances, content_type.FullName));
                }

                return content_ids;
            }

            /// <summary>
            /// Attach requested content to provided parent content element (called by window leaf).
            /// </summary>
            /// <param name="content_id">The string ID of the content if present.</param>
            /// <param name="content_type">Using string for content type to allow cross project compatibility.</param> 
            /// <returns>The WPF Control element holding the actual content.</returns>
            public Control CreateContent(string content_id, string content_type)
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }

                var type = get_type(content_type);
                if (_contents.ContainsKey(type))
                {
                    string id = content_id;
                    if (!_contents[type].ContainsKey(id))
                    {
                        if (content_id != UniqueID.Invalid)
                        {
                            Log.Default.Msg(Log.Level.Warn, "Could not find requested content " + content_type + " with ID " + id);
                        }
                        else
                        {
                            // Create new instance from type
                            var new_content = (AbstractContent)Activator.CreateInstance(type);
                            if (recursive_basetype(new_content.GetType(), typeof(AbstractVisualization)))
                            {
                                ((AbstractVisualization)new_content).SetRequestDataCallback(_request_data_callback);
                            }

                            new_content.Initialize();
                            new_content.Create();

                            id = new_content.ID;
                            _contents[type].Add(id, new_content);
                        }
                    }

                    return _contents[type][id].Attach();
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Unregistered content type: " + content_type.ToString());
                }
                return null;
            }

            /// <summary>
            /// Delete the content requested by id (called by window leaf).
            /// </summary>
            /// <param name="content_id">The id of the content to be deleted.</param>
            public void DeleteContent(string content_id)
            {
                // Loop over registered types
                foreach (var content_types in _contents)
                {
                    if (content_types.Value.ContainsKey(content_id))
                    {
                        content_types.Value[content_id].Detach();
                        content_types.Value.Remove(content_id);
                    }
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            /// <summary>
            /// Register new content type.
            /// </summary>
            /// <param name="content_type">The content type.</param>
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
                    if (_contents.ContainsKey(content_type))
                    {
                        Log.Default.Msg(Log.Level.Warn, "Content type already added: " + content_type.FullName);
                    }
                    else
                    {
                        _contents.Add(content_type, new Dictionary<string, AbstractContent>());
                        Log.Default.Msg(Log.Level.Info, "Registered content type: " + content_type.FullName);
                    }
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Incompatible content type: " + content_type.FullName);
                }
            }

            /// <summary>
            /// Check recursively for base type.
            /// </summary>
            /// <param name="check_type">The type to look into.</param>
            /// <param name="reference_base_type">The base type to check for.</param>
            /// <returns>True on success, false otherwise.</returns>
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

            /// <summary>
            /// Convert string to type.
            /// </summary>
            /// <param name="type_string">The type as string.</param>
            /// <returns>The requested type, default(?) otherwise.</returns>
            public Type get_type(string type_string)
            {
                Type type = default(Type);
                try
                {
                    // Try to load type from current assembly (suppress errors -> return null on error)
                    type = Type.GetType(type_string);
                    if (type == null)
                    {
                        // Try to load type from Core assembly - trow error if this is also not possible
                        type = Assembly.Load("Core").GetType(type_string, true);
                    }
                }
                catch (TypeLoadException e)
                {
                    Log.Default.Msg(Log.Level.Error, e.Message);
                }
                return type;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            // Separate dictionary for each content type
            private Dictionary<Type, Dictionary<string, AbstractContent>> _contents = null;
            private DataManager.RequestDataCallback_Delegate _request_data_callback = null;
        }
    }
}
