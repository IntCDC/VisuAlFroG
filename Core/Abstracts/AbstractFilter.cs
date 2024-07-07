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
using System.Runtime.Remoting.Contexts;



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
                public int _UID { get; set; }
                public string _Type { get; set; }
                public string _Name { get; set; }
                public List<int> _UIDList { get; set; } // Content or Data ?
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

            public int _UID { get; } = UniqueID.GenerateInt();
            public string _Name { get; set; }
            public bool _Attached { get; protected set; } = false;

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractFilter(int uid)
            {
                if (uid != UniqueID.InvalidInt)
                {
                    _UID = uid;
                }
            }

            /// <summary>
            /// Call in inherited class via base.CreateUI()
            /// </summary>
            public virtual bool CreateUI()
            {



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

                _Attached = true;
                return _content;
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

            private Grid _content = null;
            private bool _created = false;

            #endregion

        }
    }
}
