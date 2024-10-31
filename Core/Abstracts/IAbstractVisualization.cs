using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Core.Data;
using Core.GUI;

using static Core.Abstracts.AbstractVisualization;



/*
 * Abstract Content Interface
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public interface IAbstractVisualization
        {
            /* ------------------------------------------------------------------*/
            #region interface properties

            /// <summary>
            /// The id string of the content.
            /// </summary>
            int _UID { get; }

            /// <summary>
            /// The unique type name of the content, e.g. used in the context menu to select new content type.
            /// </summary>
            string _Name { get; }

            /// <summary>
            /// True if multiple instances of the content would be created, false otherwise.
            /// </summary>
            bool _MultipleInstances { get; }

            /// <summary>
            /// Services the content depends on.
            /// </summary>
            List<Type> _DependingServices { get; }

            #endregion

            /* ------------------------------------------------------------------*/
            #region interface functions

            /// <summary>
            /// Initialize the content.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool Initialize(DataManager.GetSpecificDataCallback_Delegate request_data_callback, DataManager.GetDataMenuCallback_Delegate request_menu_callback,
                UpdateSeriesSelection_Delegate update_selection_series);

            /// <summary>
            /// Create the content. To be called only once.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool CreateUI();

            /// <summary>
            /// Called when content element should be attached.
            /// </summary>
            /// <returns>The content to be attached by the caller.</returns>
            UIElement GetUI();

            /// <summary>
            /// Called when menu of content should be attached.
            /// </summary>
            /// <param name="menubar"></param>
            void AttachMenu(MenubarWindow menubar);

            /// <summary>
            /// Terminate the content. Should implement counterpart to Initialize().
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool Terminate();

            /// <summary>
            /// Called when (partially: new_data=false) updated data is available
            /// </summary>
            /// <param name="new_data">True if new data is available, false if existing data has been updated.</param>
            void Update(bool new_data);

            /// <summary>
            /// Update selection of specific data entry
            /// </summary>
            /// <param name="updated_meta_data"></param>
            void UpdateEntrySelection(IMetaData updated_meta_data);

            /// <summary>
            /// Update selection of data series
            /// </summary>
            /// <param name="updated_series_indexes"></param>
            void UpdateSeriesSelection(List<int> updated_series_indexes);

            #endregion
        }
    }
}
