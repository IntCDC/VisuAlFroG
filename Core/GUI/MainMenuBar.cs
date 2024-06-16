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
        public class MainMenuBar : AbstractMenuBar<MainMenuBar.PredefinedMenuOption>
        {
            /* ------------------------------------------------------------------*/
            #region public enum

            public enum PredefinedMenuOption
            {
                FILE,
                DATA,
                STYLE,
                HELP,
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public override bool Initialize()
            {
                if (base.Initialize())
                {
                    add_main_menu("File", PredefinedMenuOption.FILE);
                    add_main_menu("Data", PredefinedMenuOption.DATA);
                    add_main_menu("Style", PredefinedMenuOption.STYLE);
                    add_main_menu("Help", PredefinedMenuOption.HELP);

                    add_usage_info();
                    add_repo_link();
                }
                return _initialized;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void add_repo_link()
            {
                var hyper_link = new Hyperlink();
                hyper_link.NavigateUri = new Uri("https://github.com/IntCDC/VisuAlFroG");
                hyper_link.RequestNavigate += event_hyperlink_requestnavigate;
                hyper_link.Inlines.Add("GitHub Repository");
                hyper_link.Style = ColorTheme.HyperlinkStyle();
                var repo_menu_item = new MenuItem();
                repo_menu_item.Style = ColorTheme.MenuItemIconStyle("github.png");
                repo_menu_item.Header = hyper_link;
                AddMenu(PredefinedMenuOption.HELP, repo_menu_item);
            }


            private void add_usage_info()
            {
                var usage_menu = GetDefaultMenuItem("Visualization Configuration");
                usage_menu.IsEnabled = true;
                AddMenu(PredefinedMenuOption.FILE, usage_menu);

                var usage_menu_text = GetDefaultMenuItem("> Open menu with [Right-Click] on surface.");
                usage_menu_text.IsEnabled = false;

                AddMenu(PredefinedMenuOption.FILE, usage_menu_text);

                AddSeparator(PredefinedMenuOption.FILE);
            }

            /// <summary>
            /// Called to open URL in new browser tab.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The request navigate event arguments.</param>
            private void event_hyperlink_requestnavigate(object sender, RequestNavigateEventArgs e)
            {
                System.Diagnostics.Process.Start(e.Uri.ToString());
                /// or Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }

            #endregion
        }
    }
}
