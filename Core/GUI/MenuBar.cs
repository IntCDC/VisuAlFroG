using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using Core.GUI;



/*
 * Menu Bar
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class MenuBar
        {
            /* ------------------------------------------------------------------*/
            // public delegates

            /// <summary>
            /// Function provided by the interface (= Grasshopper) which allows to trigger relaoding of the interface
            /// </summary>
            public delegate void CloseCallback();


            /* ------------------------------------------------------------------*/
            // public functions

            public void Create(Menu content_element)
            {
                var item_file = new MenuItem();
                item_file.Header = "File";

                var item_close = new MenuItem();
                item_close.Header = "Close";
                item_close.Name = _item_id_close;
                item_close.Click += menuitem_click;
                item_file.Items.Add(item_close);

                /*
                var item_help = new MenuItem();
                item_help.Header = "Help";
                item_help.Name = _item_id_help;
                item_help.Click += menuitem_click;
                item_file.Items.Add(item_help);
                */

                content_element.Height = 20.0;
                content_element.Style = ColorTheme.MenuStyle();
                content_element.Items.Add(item_file);
            }


            public void RegisterCloseCallback(CloseCallback close_callback)
            {
                _close_callback = close_callback;
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void menuitem_click(object sender, RoutedEventArgs e)
            {
                var sender_content = sender as MenuItem;
                if (sender_content == null)
                {
                    return;
                }

                string content_id = sender_content.Name;
                if (content_id == _item_id_close)
                {
                    if (_close_callback != null)
                    {
                        _close_callback();
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Warn, "Missing close callback");
                    }
                }
                else if (content_id == _item_id_help)
                {


                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private CloseCallback _close_callback = null;

            private readonly string _item_id_close = "item_close_" + UniqueStringID.Generate();
            private readonly string _item_id_help = "item_help_" + UniqueStringID.Generate();
        }
    }
}
