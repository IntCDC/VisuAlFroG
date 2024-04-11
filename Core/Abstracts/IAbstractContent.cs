using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public interface IAbstractContent
        {
            /* ------------------------------------------------------------------*/
            // interface properties

            /// <summary>
            /// The name of the content.
            /// </summary>
            string Name { get; }

            /// <summary>
            /// True if multiple instances of the content would be created, false otherwise.
            /// </summary>
            bool MultipleInstances { get; }

            /// <summary>
            /// Services the content depends on.
            /// </summary>
            List<Type> DependingServices { get; }

            /// <summary>
            /// Returns whether WPF content is attached or not.
            /// </summary>
            bool IsAttached { get; }

            /// <summary>
            /// The id string of the content.
            /// </summary>
            string ID { get; }


            /* ------------------------------------------------------------------*/
            // interface functions

            /// <summary>
            /// Initialize the content.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool Initialize(DataManager.RequestCallback_Delegate request_callback);

            /// <summary>
            /// Create the content. To be called only once.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool ReCreate();

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
