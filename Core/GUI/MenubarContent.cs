using System;



/*
 * Main Menu Bar
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class MenubarContent : AbstractMenuBar<MenubarContent.PredefinedMenuOption>
        {
            /* ------------------------------------------------------------------*/
            #region public enum

            public enum PredefinedMenuOption
            {
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
                    add_main_menu("Content", PredefinedMenuOption.CONTENT);
                    /// XXX UNUSED add_main_menu("Data", PredefinedMenuOption.DATA);

                }
                return _initialized;
            }

            #endregion
        }
    }
}
