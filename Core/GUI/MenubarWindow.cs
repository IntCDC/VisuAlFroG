using System;
using System.Windows;

using Core.Utilities;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;



/*
 * Main Menu Bar
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class MenubarWindow : AbstractMenuBar<MenubarWindow.PredefinedMenuOption>
        {
            /* ------------------------------------------------------------------*/
            #region public enum

            public enum PredefinedMenuOption
            {
                VIEW,
                CONTENT,
                DATA,
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public override bool Initialize()
            {
                if (base.Initialize())
                {
                    add_main_menu("View", PredefinedMenuOption.VIEW);
                    add_main_menu("Content", PredefinedMenuOption.CONTENT);
                    add_main_menu("Data", PredefinedMenuOption.DATA);

                }
                return _initialized;
            }

            #endregion
        }
    }
}
