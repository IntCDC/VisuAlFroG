using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Data;



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
            // interface properties

            /// <summary>
            /// The name of the content.
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

            /// <summary>
            /// Returns whether WPF content is attached or not.
            /// </summary>
            bool _Attached { get; }

            /// <summary>
            /// The id string of the content.
            /// </summary>
            string _ID { get; }


            /* ------------------------------------------------------------------*/
            // interface functions

            /// <summary>
            /// Initialize the content.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool Initialize(DataManager.GetDataCallback_Delegate request_data_callback, DataManager.GetDataMenuCallback_Delegate request_menu_callback);

            /// <summary>
            /// Create the content. To be called only once.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool Create();

            /// <summary>
            /// Called when content element should be attached.
            /// </summary>
            /// <returns>The content to be attached by the caller.</returns>
            Panel Attach();

            /// <summary>
            /// Called when the content element has been detached. Should implement counterpart to Attach().
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool Detach();

            /// <summary>
            /// Terminate the content. Should implement counterpart to Initialize().
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool Terminate();
        }
    }
}
