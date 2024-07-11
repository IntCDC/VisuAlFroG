using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Core.GUI;
using Core.Utilities;
using Core.Abstracts;
using Core.Filter;
using Core.Data;
using static Core.Filter.FilterManager;
using System.Runtime.Remoting.Contexts;



/*
 * Abstract data filter
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractFilter : AbstractService, IAbstractFilter
        {
            /* ------------------------------------------------------------------*/
            #region public classes

            /// <summary>
            /// Class defining the configuration required for restoring content.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public int UID { get; set; }
                public string Type { get; set; }
                public string Name { get; set; }
                public List<int> ContentUIDList { get; set; }
            }

            public class ContentMetadata
            {
                public string Name { get; set; }
                public int DataUID { get; set; }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public enum

            // Define in Core for overall availability
            [Flags]
            public enum ListModification
            {
                DELETE,
                ADD,
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties

            public int _UID { get; } = UniqueID.GenerateInt();
            public string _Name { get; set; }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public bool Initialize(DataManager.GetGenericDataCallback_Delegate get_selected_data_callback, DataManager.UpdateSelectedDataCallback_Delegate update_selected_data_callback, ModifyUIFilterList_Delegate modify_ui_filter_list_callback)
            {
                if (_initialized)
                {
                    Terminate();
                }
                if ((get_selected_data_callback == null) || (update_selected_data_callback == null) || (modify_ui_filter_list_callback == null))
                {
                    Log.Default.Msg(Log.Level.Error, "Missing callback(s)");
                }
                _timer.Start();


                _update_selected_data_callback = update_selected_data_callback;
                _get_selected_data_callback = get_selected_data_callback;
                _modify_ui_filter_list_callback = modify_ui_filter_list_callback;


                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initialized;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _modify_ui_filter_list_callback(_content, ListModification.DELETE);

                    _content = null;
                    _update_selected_data_callback = null;
                    _get_selected_data_callback = null;
                    _modify_ui_filter_list_callback = null;

                    _created = false;
                    _initialized = false;
                }
                return true;
            }

            /// <summary>
            /// Call in inherited class via base.CreateUI()
            /// </summary>
            public virtual bool CreateUI()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Debug, "Content already created. Skipping re-creating content.");
                    return false;
                }
                _timer.Start();


                _content = new Grid();

                                var rename_button = new Button();
                var endisable_button = new Button();
                var delete_button = new Button();
                var apply_button = new Button(); // Disable on hit, but enable when anything changes

                // Subscribe to list update (add/delete content with own data) List Visualization Name -> binding/ Data UID 

                // Get generic data of DataType -> apply changed data to DataType

                // Add custom UI of inheriting filter
                var child = create_ui();



                // Attach filter UI to filter list of Filter Editor
                _modify_ui_filter_list_callback(_content, ListModification.ADD);


                _timer.Stop();
                _created = true;
                return _created;
            }

            public UIElement GetUI()
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }
                return _content;
            }

            /// <summary>
            /// Called on creation of filter
            /// </summary>
            /// <param name="content_metadata"></param>
            public void ProvideContentDataCallback(List<ContentMetadata> content_metadata)
            {
                // Update check boxes in content list


            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected virtual UIElement create_ui()
            {
                throw new InvalidOperationException("Should be implemented by inheriting class.");
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void event_apply_filter()
            {

                // call _get_selected_data_callback, modify all data and the call _update_selected_data_callback for all selected contents
            }


            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private Grid _content = null;
            private bool _created = false;
            private DataManager.UpdateSelectedDataCallback_Delegate _update_selected_data_callback = null;
            private DataManager.GetGenericDataCallback_Delegate _get_selected_data_callback = null;
            private ModifyUIFilterList_Delegate _modify_ui_filter_list_callback = null;

            #endregion
        }
    }
}
