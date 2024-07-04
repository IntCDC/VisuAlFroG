using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Dynamic;
using static Core.GUI.ColorTheme;
using Core.GUI;
using Core.Utilities;
using Core.Abstracts;



/*
 * Abstract data filter
 * 
 */
namespace Core
{
    namespace Data
    {
        public abstract class AbstractFilter : IAbstractFilter
        {
            /* ------------------------------------------------------------------*/
            #region public classes

            /// <summary>
            /// Class defining the configuration required for restoring content.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public string _UID { get; set; }
                public string _Type { get; set; }
                public string _Name { get; set; }
                /// TODO Add additional configuration information that should be saved here...
                /// and adjust de-/serialization methods in ContenManager accordingly
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public enum

            [Flags]
            public enum Filters
            {

            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties

            public string _UID { get; } = UniqueID.GenerateString();
            public string _Name { get; set; }
            public bool _Attached { get; protected set; } = false;

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractFilter(string uid)
            {
                if (uid != UniqueID.InvalidString)
                {
                    _UID = uid;
                }
            }


            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions




            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions



            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions


            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables



            #endregion

        }
    }
}
