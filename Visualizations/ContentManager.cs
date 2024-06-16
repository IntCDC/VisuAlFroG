using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using Core.Abstracts;
using System.Reflection;
using SciChart.Core.Extensions;
using Core.GUI;
using Core.Data;



/*
 * Content Manager
 * 
 */

// Arguments: <content name, flag: is content available, flag: are multiple instances allowed, content type>
using ReadContentMetaData_Type = System.Tuple<string, bool, bool, string>;

using AttachContentMetaData_Type = System.Tuple<string, System.Windows.Controls.Panel>;


namespace Visualizations
{
    public class ContentManager : AbstractService
    {
        /* ------------------------------------------------------------------*/
        #region public functions

        public bool Initialize(DataManager.GetDataCallback_Delegate getdata_callback,
                DataManager.GetDataMenuCallback_Delegate getmenu_callback,
                DataManager.RegisterDataCallback_Delegate register_callback,
                DataManager.UnregisterUpdateCallback_Delegate unregister_callback)
        {
            if (_initialized)
            {
                Terminate();
            }
            if ((getdata_callback == null) || (getmenu_callback == null) || (register_callback == null) || (unregister_callback == null))
            {
                Log.Default.Msg(Log.Level.Error, "Missing callback(s)");
                return false;
            }
            _timer.Start();

            _contents = new Dictionary<Type, Dictionary<string, AbstractVisualization>>();
            _content_getdata_callback = getdata_callback;
            _content_getmenu_callback = getmenu_callback;
            _register_data_callback = register_callback;
            _unregister_data_callback = unregister_callback;


            // Register new visualizations here:
            register_content(typeof(WPF_LogConsole));
            register_content(typeof(WPF_DataViewer));
            register_content(typeof(SciChart_ScatterPlot));
            register_content(typeof(SciChart_Lines));
            register_content(typeof(SciChart_Columns));
            register_content(typeof(SciChart_ParallelCoordinatesPlot));
            /// DEBUG             register_content(typeof(CustomWPFVisualization));



            _timer.Stop();
            _initialized = true;
            return _initialized;
        }

        public override bool Terminate()
        {
            bool terminated = true;
            if (_initialized)
            {
                terminated &= clear_contents();

                _content_getdata_callback = null;
                _content_getmenu_callback = null;
                _register_data_callback = null;
                _unregister_data_callback = null;

                _initialized = false;
            }
            return terminated;
        }

        public string CollectConfigurations()
        {
            var configurations = new List<AbstractVisualization.Configuration>();
            foreach (var content_types in _contents)
            {
                foreach (var content_data in content_types.Value)
                {
                    configurations.Add(new AbstractVisualization.Configuration() { _ID = content_data.Value._ID, _Type = content_types.Key.FullName });
                }
            }
            return ConfigurationService.Serialize<List<AbstractVisualization.Configuration>>(configurations);
        }

        public bool ApplyConfigurations(string configurations)
        {
            if (!_initialized)
            {
                Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                return false;
            }

            var visualization_configurations = ConfigurationService.Deserialize<List<AbstractVisualization.Configuration>>(configurations);
            if (visualization_configurations != null)
            {
                if (!clear_contents())
                {
                    Log.Default.Msg(Log.Level.Warn, "Unable to clear content properly");
                    return false;
                }

                foreach (var content_configuration in visualization_configurations)
                {
                    var type = get_type(content_configuration._Type);
                    if (_contents.ContainsKey(type))
                    {
                        var id = content_configuration._ID;
                        if (id == UniqueID.InvalidString)
                        {
                            Log.Default.Msg(Log.Level.Warn, "Invalid content id: " + id);
                            break;
                        }

                        if (!_contents[type].ContainsKey(id))
                        {
                            var new_content = create_content(type);
                            if (new_content == null)
                            {
                                return false;
                            }
                            new_content._ID = id;
                            _contents[type].Add(id, new_content);
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Content " + content_configuration._Type + " with ID " + id + " already exists");
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

            if (_initialized)
            {
                // Loop over registered types
                foreach (var content_types in _contents)
                {
                    _timer.Start();

                    // Create temporary instance of content
                    Type content_type = content_types.Key;
                    var tmp_content = (AbstractVisualization)Activator.CreateInstance(content_type);
                    var lservice_types = tmp_content._DependingServices;
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
        public List<ReadContentMetaData_Type> AvailableContentsCallback()
        {
            var content_ids = new List<ReadContentMetaData_Type>();

            if (!_initialized)
            {
                Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                return content_ids;
            }

            // Loop over registered types
            foreach (var content_types in _contents)
            {
                var content_type = content_types.Key;

                // Create temporary instance of content
                var tmp_content = (AbstractVisualization)Activator.CreateInstance(content_type);
                string header = tmp_content._Name;
                bool multiple_instances = tmp_content._MultipleInstances;

                // Content is only available if multiple instance are allowed or has not been instantiated yet
                bool available = (multiple_instances || (content_types.Value.IsEmpty() && !multiple_instances));

                content_ids.Add(new ReadContentMetaData_Type(header, available, multiple_instances, content_type.FullName));
            }

            return content_ids;
        }

        /// <summary>
        /// Attach requested content to provided parent content element (called by window leaf).
        /// </summary>
        /// <param name="content_id">The string ID of the content if present.</param>
        /// <param name="content_type">Using string for content type to allow cross project compatibility.</param> 
        /// <returns>Tuple of content ID and the WPF Control element holding the actual content.</returns>
        public AttachContentMetaData_Type CreateContentCallback(string content_id, string content_type)
        {
            if (!_initialized)
            {
                Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                return null;
            }
            var type = get_type(content_type);
            if (type == null)
            {
                Log.Default.Msg(Log.Level.Error, "Unable to find type: " + content_type);
                return null;
            }

            if (_contents.ContainsKey(type))
            {
                string id = content_id;
                if (!_contents[type].ContainsKey(id))
                {
                    if (content_id != UniqueID.InvalidString)
                    {
                        Log.Default.Msg(Log.Level.Warn, "Could not find requested content " + content_type + " with ID " + id);
                        return null;
                    }
                    else
                    {
                        var new_content = create_content(type);
                        if (new_content == null)
                        {
                            return null;
                        }
                        id = new_content._ID;
                        _contents[type].Add(id, new_content);
                    }
                }

                return new AttachContentMetaData_Type(id, _contents[type][id].Attach());
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
        /// <return>True on success, false otherwise.</return>
        public bool DeleteContentCallback(string content_id)
        {
            // Loop over registered types
            foreach (var content_types in _contents)
            {
                if (content_types.Value.ContainsKey(content_id))
                {
                    _unregister_data_callback(content_types.Value[content_id]._DataUID);
                    content_types.Value[content_id].Detach();
                    content_types.Value[content_id].Terminate();
                    return content_types.Value.Remove(content_id);
                }
            }
            Log.Default.Msg(Log.Level.Debug, "Content not available for deletion: " + content_id);
            return false;
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions

        /// <summary>
        /// Register new content type.
        /// </summary>
        /// <param name="content_type">The content type.</param>
        private void register_content(Type content_type)
        {
            // Check for required base type
            Type type = content_type;
            Type required_basetype = typeof(AbstractVisualization);
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
                    _contents.Add(content_type, new Dictionary<string, AbstractVisualization>());
                    Log.Default.Msg(Log.Level.Info, "Registered content type: " + content_type.FullName);
                }
            }
            else
            {
                Log.Default.Msg(Log.Level.Error, "Incompatible content type: " + content_type.FullName);
            }
        }

        /// <summary>
        /// Initialize and create new instance of content of given type
        /// </summary>
        /// <param name="type">The content type.</param>
        /// <returns>Return instance of new content.</returns>
        private AbstractVisualization create_content(Type type)
        {
            var content = (AbstractVisualization)Activator.CreateInstance(type);
            if (content.Initialize(_content_getdata_callback, _content_getmenu_callback))
            {
                content._DataUID = _register_data_callback(content._RequiredDataType, content.Update);
                // XXX Do not check for invalid DATAUID because it might be intentional that no data should have been created...
                if (content.Create())
                {
                    content.Update(true);
                    return content;
                }
            }
            content = null;
            Log.Default.Msg(Log.Level.Error, "Unable to initialize or create content: " + type.FullName);
            return null;
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

        /// <summary>
        /// Delete all contents.
        /// </summary>
        /// <returns>True on success, false otherwise.</returns>
        private bool clear_contents()
        {
            bool terminated = true;
            foreach (var content_types in _contents)
            {
                foreach (var content_data in content_types.Value)
                {
                    _unregister_data_callback(content_data.Value._DataUID);
                    content_data.Value.Detach();
                    terminated &= content_data.Value.Terminate();
                }
                content_types.Value.Clear();
            }
            return terminated;
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private variables

        // Separate dictionary for each content type
        private Dictionary<Type, Dictionary<string, AbstractVisualization>> _contents = null;

        private DataManager.GetDataCallback_Delegate _content_getdata_callback = null;
        private DataManager.GetDataMenuCallback_Delegate _content_getmenu_callback = null;
        private DataManager.RegisterDataCallback_Delegate _register_data_callback = null;
        private DataManager.UnregisterUpdateCallback_Delegate _unregister_data_callback = null;

        #endregion
    }
}
