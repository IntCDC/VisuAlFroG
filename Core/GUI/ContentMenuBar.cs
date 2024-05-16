using System.Windows.Controls;
using System.Windows.Documents;
using System;
using System.Windows.Navigation;
using System.Windows;



/*
 * Main Menu Bar
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class ContentMenuBar : AbstractMenuBar<ContentMenuBar.PredefinedMenuOption>
        {
            /* ------------------------------------------------------------------*/
            // public enum

            public enum PredefinedMenuOption
            {
                OPTIONS,
                DATA,
            }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (base.Initialize())
                {
                    add_main_menu("Options", PredefinedMenuOption.OPTIONS);
                    add_main_menu("Data", PredefinedMenuOption.DATA);

                }
                return _initialized;
            }
        }
    }
}
