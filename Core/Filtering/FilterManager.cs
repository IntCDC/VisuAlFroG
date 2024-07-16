using System;
using System.Windows;
using System.Collections.Generic;
using Core.Abstracts;
using Core.GUI;
using Core.Utilities;
using Core.Filter;
using Core.Data;
using static Core.Abstracts.AbstractFilter;



/*
 * Data Manager
 * 
 */
namespace Core
{
    namespace Filter
    {
        public class FilterManager : AbstractRegisterService<AbstractFilter>
        {
            /* ------------------------------------------------------------------*/
            #region public classes

            /// <summary>
            /// Class defining the configuration required for restoring filters.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public List<AbstractFilter.Configuration> FilterList { get; set; }
            }

            public class FilterListMetadata
            {
                public int ID { get; set; }
                public string Name { get; set; }
                public Type Type { get; set; }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public delegates

            public delegate bool CreateFilterCallback_Delegate(Type filter_type);
            public delegate bool DeleteFilterCallback_Delegate(int filter_uid);
            public delegate void ModifyUIFilterList_Delegate(UIElement element, AbstractFilter.ListModification mod);
            public delegate void FilterChanged_Delegate(int filter_uid, List<int> data_uids);

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public bool Initialize(DataManager.GetGenericDataCallback_Delegate get_selected_data_callback, DataManager.UpdateSelectedDataCallback_Delegate update_selected_data_callback)
            {
                if (!base.Initialize())
                {
                    return false;
                }
                if ((update_selected_data_callback == null) || (get_selected_data_callback == null))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing callback(s)");
                }
                _timer.Start();


                _contents_metadata = new List<AbstractFilter.ContentMetadata>();
                _ordered_filter_list = new List<int>();

                _get_selected_data_callback = get_selected_data_callback;
                _update_selected_data_callback = update_selected_data_callback;

                register_content(typeof(TransposeFilter));


                _timer.Stop();
                _initialized = true;
                return _initialized;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _contents_metadata = null;
                    _ordered_filter_list.Clear();
                    _ordered_filter_list = null;
                    _update_selected_data_callback = null;
                    _get_selected_data_callback = null;
                    _modify_ui_filter_list_callback = null;

                    _initialized = false;
                }
                return true;
            }

            public List<FilterListMetadata> GetFilterTypeList()
            {
                int id = 0;
                var list = new List<FilterListMetadata>();
                foreach (var filter_data in _contents)
                {
                    var filter_metadata = new FilterListMetadata();
                    ///filter_metadata.ID = id; /// required for enumeration in combobox?
                    filter_metadata.Name = filter_data.Key.Name;
                    filter_metadata.Type = filter_data.Key;
                    list.Add(filter_metadata);
                    id++;
                }
                return list;
            }

            public void SetModifyUIFilterList(ModifyUIFilterList_Delegate modify_ui_filter_list_callback)
            {
                _modify_ui_filter_list_callback = modify_ui_filter_list_callback;
            }

            public bool CreateFilterCallback(Type filter_type)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (filter_type == null)
                {
                    Log.Default.Msg(Log.Level.Error, "Unable to find type: " + filter_type);
                    return false;
                }

                if (_contents.ContainsKey(filter_type))
                {
                    var filter = (AbstractFilter)Activator.CreateInstance(filter_type);
                    if (filter.Initialize(_get_selected_data_callback, _update_selected_data_callback, this.filter_changed, DeleteFilterCallback))
                    {
                        if (filter.CreateUI())
                        {
                            filter.ContentMetadataListCallback(_contents_metadata);
                            _contents[filter_type].Add(filter._UID, filter);
                            _ordered_filter_list.Add(filter._UID);
                            // Add filter to UI list
                            _modify_ui_filter_list_callback(filter.GetUI(), ListModification.ADD);
                            return true;
                        }
                        else
                        {
                            Log.Default.Msg(Log.Level.Error, "Unable to create the UI of the new filter: " + filter_type.ToString());
                        }
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Error, "Unable to initialize the new filter: " + filter_type.ToString());
                    }
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Unregistered filter type: " + filter_type.ToString());
                }
                return false;
            }

            public bool DeleteFilterCallback(int filter_uid)
            {
                // Loop over registered types
                foreach (var filter_types in _contents)
                {
                    if (filter_types.Value.ContainsKey(filter_uid))
                    {
                        // Remove filter from UI list
                        _modify_ui_filter_list_callback(filter_types.Value[filter_uid].GetUI(), ListModification.DELETE);
                        filter_types.Value[filter_uid].Terminate();
                        // Call after terminate
                        _ordered_filter_list.Remove(filter_types.Value[filter_uid]._UID);
                        return filter_types.Value.Remove(filter_uid);
                    }
                }
                Log.Default.Msg(Log.Level.Debug, "Filter not available for deletion: " + filter_uid);
                return false;
            }

            public string CollectConfigurations()
            {
                var filtermanager_configuration = new FilterManager.Configuration();

                foreach (var filter_metadata in _contents)
                {
                    /// TODO collect filter configurations
                }

                return ConfigurationService.Serialize<FilterManager.Configuration>(filtermanager_configuration);
            }

            public bool ApplyConfigurations(string configurations)
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }

                var filtermanager_configuration = ConfigurationService.Deserialize<FilterManager.Configuration>(configurations);
                if (filtermanager_configuration != null)
                {
                    foreach (var filtermetadata in filtermanager_configuration.FilterList)
                    {
                        // TODO Apply configuration and create filters
                    }
                    return true;
                }
                return false;
            }

            public void AddContentMetadataCallback(AbstractFilter.ContentMetadata content_metadata)
            {
                _contents_metadata.Add(content_metadata);

                foreach (var filter_type in _contents)
                {
                    foreach (var filter in filter_type.Value)
                    {
                        filter.Value.ContentMetadataListCallback(_contents_metadata);
                    }
                }
            }

            public void DeleteContentMetadataCallback(int data_uid)
            {
                foreach (var cm in _contents_metadata)
                {
                    if (cm.DataUID == data_uid)
                    {
                        _contents_metadata.Remove(cm);
                        break;
                    }
                }
                foreach (var filter_type in _contents)
                {
                    foreach (var filter in filter_type.Value)
                    {
                        filter.Value.ContentMetadataListCallback(_contents_metadata);
                    }
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            /// <summary>
            /// Reset specific filter (only used in AbstractRegisterService)
            /// </summary>
            /// <param name="filter_value"></param>
            /// <returns></returns>
            protected override bool reset_content(AbstractFilter filter_value)
            {
                return filter_value.Terminate();
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            void filter_changed(int filter_uid, List<int> data_uids)
            {
                // Notify all subsequent filters that previous filter has changed and that their original data has changed
                if (!_ordered_filter_list.Contains(filter_uid))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing filter UID");

                }

                var index = _ordered_filter_list.FindIndex(f => (f == filter_uid));
                for (int i = index+1; i < _ordered_filter_list.Count; i++)
                {
                    foreach (var filter_type in _contents)
                    {
                        foreach (var filter in filter_type.Value)
                        {
                            if (filter.Value._UID == _ordered_filter_list[i])
                            {
                                filter.Value.SetDirty(data_uids);
                            }
                        }
                    }
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            // Required to provide new filters with content metadata
            private List<AbstractFilter.ContentMetadata> _contents_metadata = null;

            private List<int> _ordered_filter_list = null;

            private DataManager.UpdateSelectedDataCallback_Delegate _update_selected_data_callback = null;
            private DataManager.GetGenericDataCallback_Delegate _get_selected_data_callback = null;
            private ModifyUIFilterList_Delegate _modify_ui_filter_list_callback = null;

            #endregion
        }
    }
}
