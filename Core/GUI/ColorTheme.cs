using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Core.Utilities;



/*
 * Global GUI Color Theme
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class ColorTheme
        {

            // TEXT -----------------------------------------------------------

            public static Brush TextDisabled { get { return Brushes.DimGray; } }



            // MENU -----------------------------------------------------------

            public static Brush MenuBackground { get { return Brushes.LightSkyBlue; } }


            public static Style MenuStyle()
            {
                var style = new Style();
                style.TargetType = typeof(Menu);

                Setter setter_background = new Setter();
                setter_background.Property = Menu.BackgroundProperty;
                setter_background.Value = ColorTheme.MenuBackground;
                style.Setters.Add(setter_background);

                return style;
            }


            // GRID -----------------------------------------------------------

            public static Brush GridBackground { get { return Brushes.AliceBlue; } }


            //  GRID SPLITTER -------------------------------------------------

            public static Brush GridSplitterBackground { get { return Brushes.LightSkyBlue; } }
            public static Brush GridSplitterHovered { get { return Brushes.DeepSkyBlue; } }
            public static double GridSplitterSize { get { return 5.0; } }

            public static Style GridSplitterStyle()
            {
                var style = new Style();
                style.TargetType = typeof(GridSplitter);

                Setter setter_background = new Setter();
                setter_background.Property = GridSplitter.BackgroundProperty;
                setter_background.Value = ColorTheme.GridSplitterBackground;
                style.Setters.Add(setter_background);

                // Create trigger for color change on mouse hover
                Trigger trigger = new Trigger();
                trigger.Property = GridSplitter.IsMouseOverProperty;
                trigger.Value = true;
                Setter setter_trigger = new Setter();
                setter_trigger.Property = GridSplitter.BackgroundProperty;
                setter_trigger.Value = ColorTheme.GridSplitterHovered;
                trigger.Setters.Add(setter_trigger);
                style.Triggers.Add(trigger);

                return style;
            }


            // MENU ITEM ------------------------------------------------------

            public static Brush MenuItemBackground { get { return Brushes.AliceBlue; } }

            public static Style MenuItemStyle(string icon_path = "")
            {
                var style = new Style();
                style.TargetType = typeof(MenuItem);

                if (!String.IsNullOrEmpty(icon_path))
                {
                    Setter setter_icon = new Setter();
                    setter_icon.Property = MenuItem.IconProperty;

                    var image = new Image();
                    string path = Artefacts.Path() + icon_path;
                    image.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
                    setter_icon.Value = image;

                    style.Setters.Add(setter_icon);
                }
                /*
                Setter setter_background = new Setter();
                setter_background.Property = MenuItem.BackgroundProperty;
                setter_background.Value = MenuItemBackground;
                style.Setters.Add(setter_background);
                */
                return style;
            }

            public static Style ContextMenuStyle()
            {
                var style = new Style();
                style.TargetType = typeof(ContextMenu);
                /*
                Setter setter_background = new Setter();
                setter_background.Property = MenuItem.BackgroundProperty;
                setter_background.Value = ColorTheme.MenuItemBackground;
                style.Setters.Add(setter_background);
                */
                return style;
            }
        }
    }
}
