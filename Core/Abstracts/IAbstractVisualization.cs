using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Core.Data;
using Core.GUI;



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
            bool Initialize(DataManager.GetSpecificDataCallback_Delegate request_data_callback, DataManager.GetDataMenuCallback_Delegate request_menu_callback);

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

            #endregion
        }
    }
}
