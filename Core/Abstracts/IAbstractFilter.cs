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
        public interface IAbstractFilter
        {
            /* ------------------------------------------------------------------*/
            #region interface properties

            /// <summary>
            /// The id string of the content.
            /// </summary>S
            int _UID { get; }

            /// <summary>
            /// The variable name of a specific content instance.
            /// </summary>
            string _Name { get; set; }

            /// <summary>
            /// Returns whether WPF content is attached or not.
            /// </summary>
            bool _Attached { get; }

            #endregion

            /* ------------------------------------------------------------------*/
            #region interface functions

/*
            /// <summary>
            /// Initialize the content.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool Initialize();

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
*/
            #endregion
        }
    }
}
