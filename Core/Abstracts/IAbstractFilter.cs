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
            /// </summary>
            string _UID { get; }

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



            #endregion
        }
    }
}
