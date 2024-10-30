using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using Core.Abstracts;
using System.Reflection;
using SciChart.Core.Extensions;
using Core.GUI;
using Core.Data;
using Core.Filter;
using System.Windows.Data;



/*
 * Content Manager
 * 
 */

// Arguments: <content name, flag: is content available, flag: are multiple instances allowed, content type>
using ReadContentMetaData_Type = System.Tuple<string, bool, bool, string>;
using ContentCallbacks_Type = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.CreateContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;
using AttachContentMetaData_Type = System.Tuple<int, string, System.Windows.UIElement, Core.Abstracts.AbstractVisualization.AttachWindowMenu_Delegate>;


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
            register_entry(typeof(WPF_LogConsole));
            register_entry(typeof(WPF_DataViewer));
            register_entry(typeof(WPF_FilterEditor));
            register_entry(typeof(SciChart_ScatterPlot));
            register_entry(typeof(SciChart_LinesPlot));
            register_entry(typeof(SciChart_BarPlot));
            register_entry(typeof(SciChart_ParallelCoordinatesPlot));
            /// >>> Register your new content/visualization here:        
            /// register_content(typeof(CustomWPFVisualization));


            _timer.Stop();
            _initialized = true;


            // Data Manager
            bool initialized = _datamanager.Initialize();

            // FilterManager
            initialized &= _filtermanager.Initialize(_datamanager.GetGenericDataCallback, _datamanager.UpdateSelectedDataCallback);

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
            bool terminated = base.Terminate();
            if (_initialized)
            {
                terminated &= _interfacemanager.Terminate();
                terminated &= _datamanager.Terminate();
                terminated &= _filtermanager.Terminate();

                _initialized = false;
                terminated &= true;
            }
            return terminated;
        }

        public string CollectConfigurations()
        {
            var configurations = new List<AbstractVisualization.Configuration>();
            foreach (var content_types in _entries)
            {
                foreach (var content_data in content_types.Value)
                {
                    configurations.Add(new AbstractVisualization.Configuration() { UID = content_data.Value._UID, Type = content_types.Key.FullName });
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
            bool success = false;

            var visualization_configurations = ConfigurationService.Deserialize<List<AbstractVisualization.Configuration>>(configurations);
            if (visualization_configurations != null)
            {
                if (!reset_entry())
                {
                    Log.Default.Msg(Log.Level.Warn, "Unable to clear content properly");
                    return false;
                }

                foreach (var content_configuration in visualization_configurations)
                {
                    var type = get_type(content_configuration.Type);
                    if (_entries.ContainsKey(type))
                    {
                        var id = content_configuration.UID;
                        if (id == UniqueID.InvalidInt)
                        {
                            Log.Default.Msg(Log.Level.Warn, "Invalid content id: " + id);
                            break;
                        }

                        if (!_entries[type].ContainsKey(id))
                        {
                            var new_content = create_content(type, id);
                            if (new_content == null)
                            {
                                return false;
                            }
                            _entries[type].Add(new_content._UID, new_content);
                            success &= true;
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Content " + content_configuration.Type + " with ID " + id + " already exists");
                        }
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Error, "Unregistered content type: " + type.ToString());
                    }
                }
            }

            return success;
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
            /// _servicemanager.AttachMenu(menu_bar);
            /// _filtermanager.AttachMenu(menu_bar);
            _datamanager.AttachMenu(menu_bar);
        }

        public FilterManager GetFilterManager()
        {
            return _filtermanager;
        }

        // Callback forwarding for DataManager
        public void UpdateInputData(GenericDataStructure input_data)
        {
            _datamanager.UpdateAllDataCallback(input_data);
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
            var uids = new List<ReadContentMetaData_Type>();

            if (!_initialized)
            {
                Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                return uids;
            }

            // Loop over registered types
            foreach (var content_types in _entries)
            {
                var content_type = content_types.Key;

                // Create temporary instance of content
                var tmp_content = (AbstractVisualization)Activator.CreateInstance(content_type, UniqueID.InvalidInt);
                string header = tmp_content._Name;
                bool multiple_instances = tmp_content._MultipleInstances;

                // Content is only available if multiple instance are allowed or has not been instantiated yet
                bool available = (multiple_instances || (content_types.Value.IsEmpty() && !multiple_instances));

                uids.Add(new ReadContentMetaData_Type(header, available, multiple_instances, content_type.FullName));
            }

            return uids;
        }

        /// <summary>
        /// Attach requested content to provided parent content element (called by window leaf).
        /// </summary>
        /// <param name="content_uid">The string ID of the content if present.</param>
        /// <param name="content_type">Using string for content type to allow cross project compatibility.</param> 
        /// <returns>Tuple of content ID and the WPF Control element holding the actual content.</returns>
        public AttachContentMetaData_Type CreateContentCallback(int content_uid, string content_type, Binding name_binding)
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

            if (_entries.ContainsKey(type))
            {
                if ((_entries[type].Count == 0) || ((_entries[type].Count >= 1) && (_entries[type].First().Value._MultipleInstances || (!_entries[type].First().Value._MultipleInstances && _entries[type].ContainsKey(content_uid)))))
                {
                    int temp_uid = content_uid;

                    if (!_entries[type].ContainsKey(temp_uid))
                    {
                        if (temp_uid != UniqueID.InvalidInt)
                        {
                            Log.Default.Msg(Log.Level.Warn, "Could not find requested content " + content_type + " with ID " + temp_uid);
                            return null;
                        }
                        else
                        {
                            var new_content = create_content(type, UniqueID.InvalidInt);
                            if (new_content == null)
                            {
                                return null;
                            }
                            temp_uid = new_content._UID;
                            _entries[type].Add(temp_uid, new_content);
                        }
                    }

                    // Only add content with valid data uid to filter list
                    int data_uid = _entries[type][temp_uid]._DataUID;
                    if (data_uid != UniqueID.InvalidInt)
                    {
                        var content_metadata = new AbstractFilter.ContentMetadata() { NameBinding = name_binding, DataUID = data_uid, ContentType = type };
                        _filtermanager.AddContentMetadataCallback(content_metadata);
                    }

                    return new AttachContentMetaData_Type(temp_uid, _entries[type][temp_uid]._Name, _entries[type][temp_uid].GetUI(), _entries[type][temp_uid].AttachMenu);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Content is single instance and already created: " + content_type.ToString());
                }
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
        /// <param name="uid">The id of the content to be deleted.</param>
        /// <return>True on success, false otherwise.</return>
        public bool DeleteContentCallback(int content_uid)
        {
            // Loop over registered types
            foreach (var content_types in _entries)
            {
                if (content_types.Value.ContainsKey(content_uid))
                {
                    int data_uid = content_types.Value[content_uid]._DataUID;
                    _filtermanager.DeleteContentMetadataCallback(data_uid);
                    _datamanager.UnregisterDataCallback(data_uid);
                    content_types.Value[content_uid].Terminate();
                    return content_types.Value.Remove(content_uid);
                }
            }
            Log.Default.Msg(Log.Level.Debug, "Content not available for deletion: " + content_uid);
            return false;
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region protected functions

        /// <summary>
        /// Reset specific content (only used in AbstractRegisterService)
        /// </summary>
        /// <param name="content_value"></param>
        /// <returns></returns>
        protected override bool reset_entry(AbstractVisualization content_value)
        {
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
            foreach (var content_types in _entries)
            {
                _timer.Start();

                // Create temporary instance of content
                Type content_type = content_types.Key;
                var tmp_content = (AbstractVisualization)Activator.CreateInstance(content_type, UniqueID.InvalidInt);
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
        private AbstractVisualization create_content(Type type, int uid)
        {
            var content = (AbstractVisualization)Activator.CreateInstance(type, uid);
            if (content.Initialize(_datamanager.GetSpecificDataCallback, _datamanager.GetDataMenuCallback))
            {
                content._DataUID = _datamanager.RegisterDataCallback(content._RequiredDataType, content.Update);
                // Do not check for invalid DATAUID because it might be intentional that no data should have been created

                // Filter Editor requires some more information
                if (type == typeof(WPF_FilterEditor))
                {
                    var filter_editor = content as WPF_FilterEditor;
                    filter_editor.FilterManagerCallbacks(_filtermanager.GetUI, _filtermanager.Reset);
                }

                if (content.CreateUI())
                {
                    content.Update(true);
                    return content;
                }
            }
            content = null;
            Log.Default.Msg(Log.Level.Error, "Unable to initialize or create content: " + type.FullName);
            return null;
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
