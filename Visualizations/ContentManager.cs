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
using ContentCallbacks_Type = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.CreateContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;


namespace Visualizations
{
    public class ContentManager : AbstractRegisterService<AbstractVisualization>
    {
        /* ------------------------------------------------------------------*/
        #region public functions

        public override bool Initialize()
        {
            if (!base.Initialize())
            {
                return false;
            }
            _timer.Start();

            // Register new visualizations here:
            register_content(typeof(WPF_LogConsole));
            register_content(typeof(WPF_DataViewer));
            register_content(typeof(WPF_FilterEditor));
            register_content(typeof(SciChart_ScatterPlot));
            register_content(typeof(SciChart_Lines));
            register_content(typeof(SciChart_Columns));
            register_content(typeof(SciChart_ParallelCoordinatesPlot));
            /// DEBUG             
            /// register_content(typeof(CustomWPFVisualization));


            _timer.Stop();
            _initialized = true;


            // Data Manager
            bool initialized = _datamanager.Initialize();

            // Service Manager
            /// after registering all contents
            foreach (Type service_type in depending_services())
            {
                var new_service = (AbstractService)Activator.CreateInstance(service_type);
                _interfacemanager.AddService(new_service);
            }
            /// DEBUG
            //_servicemanager.AddService(new PythonInterfaceService());
            //_servicemanager.AddService(new WebAPIService());
            initialized &= _interfacemanager.Initialize();


            _initialized = initialized;
            return _initialized;
        }

        public override bool Terminate()
        {
            bool terminated = true;
            if (_initialized)
            {
                terminated &= _interfacemanager.Terminate();
                terminated &= _datamanager.Terminate();
                terminated &= _filtermanager.Terminate();

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
                    configurations.Add(new AbstractVisualization.Configuration() { _UID = content_data.Value._UID, _Type = content_types.Key.FullName, _Name = content_data.Value._Name });
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
                        var id = content_configuration._UID;
                        if (id == UniqueID.InvalidString)
                        {
                            Log.Default.Msg(Log.Level.Warn, "Invalid content id: " + id);
                            break;
                        }

                        if (!_contents[type].ContainsKey(id))
                        {
                            var new_content = create_content(type, id);
                            new_content._Name = content_configuration._Name;
                            if (new_content == null)
                            {
                                return false;
                            }
                            _contents[type].Add(new_content._UID, new_content);
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
        /// Callback forwarding for ContentManager
        /// </summary>
        /// <returns></returns>
        public ContentCallbacks_Type GetContentCallbacks()
        {
            return new ContentCallbacks_Type(AvailableContentsCallback, CreateContentCallback, DeleteContentCallback);
        }

        public override void AttachMenu(MenubarMain menu_bar)
        {
            ///_servicemanager.AttachMenu(menu_bar);
            ///_filtermanager.AttachMenu(menu_bar);
            _datamanager.AttachMenu(menu_bar);
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
                var tmp_content = (AbstractVisualization)Activator.CreateInstance(content_type, UniqueID.InvalidString);
                string header = tmp_content._TypeName;
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
                        var new_content = create_content(type, UniqueID.InvalidString);
                        if (new_content == null)
                        {
                            return null;
                        }
                        id = new_content._UID;
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
                    content_types.Value[content_id].Detach();
                    _datamanager.UnregisterDataCallback(content_types.Value[content_id]._DataUID);
                    content_types.Value[content_id].Terminate();
                    return content_types.Value.Remove(content_id);
                }
            }
            Log.Default.Msg(Log.Level.Debug, "Content not available for deletion: " + content_id);
            return false;
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region protected functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content_data"></param>
        /// <returns></returns>
        protected override bool reset_content(AbstractVisualization content_value)
        {
            content_value.Detach();
            _datamanager.UnregisterDataCallback(content_value._DataUID);
            return content_value.Terminate();
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions

        /// <summary>
        /// Returns distinct list of valid services required by the registered contents.
        /// </summary>
        /// <returns>List of service types.</returns>
        private List<Type> depending_services()
        {
            var depending_services = new List<Type>();

            // Loop over registered types
            foreach (var content_types in _contents)
            {
                _timer.Start();

                // Create temporary instance of content
                Type content_type = content_types.Key;
                var tmp_content = (AbstractVisualization)Activator.CreateInstance(content_type, UniqueID.InvalidString);
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

            return depending_services;
        }

        /// <summary>
        /// Initialize and create new instance of content of given type
        /// </summary>
        /// <param name="type">The content type.</param>
        /// <returns>Return instance of new content.</returns>
        private AbstractVisualization create_content(Type type, string uid)
        {
            var content = (AbstractVisualization)Activator.CreateInstance(type, uid);
            if (content.Initialize(_datamanager.GetDataCallback, _datamanager.GetDataMenuCallback))
            {
                content._DataUID = _datamanager.RegisterDataCallback(content._RequiredDataType, content.Update);
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
        /// Convert string to type.
        /// </summary>
        /// <param name="type_string">The type as string.</param>
        /// <returns>The requested type, default(?) otherwise.</returns>
        private Type get_type(string type_string)
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

        #endregion

        /* ------------------------------------------------------------------*/
        #region private variables

        private InterfaceManager _interfacemanager = new InterfaceManager();
        private DataManager _datamanager = new DataManager();
        private FilterManager _filtermanager = new FilterManager();


        #endregion
    }
}
