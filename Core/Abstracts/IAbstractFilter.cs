using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Data;
using System.Windows;



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

            /// <summary>
            /// Called when content element should be attached.
            /// </summary>
            /// <returns>The content to be attached by the caller.</returns>
            UIElement GetUI();

            /// <summary>
            /// Create the content. To be called only once.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool CreateUI();

            #endregion
        }
    }
}
