using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Documents;
using Core.Utilities;



/*
 * Global GUI Color Theme for WPF
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class ColorTheme
        {
            /* ------------------------------------------------------------------*/
            // static functions


            // GENERIC -----------------------------------------------------

            public static Brush DarkBackground { get { return Brushes.Black; } }
            public static Brush DarkForeground { get { return Brushes.Black; } }

            public static Brush LightBackground { get { return Brushes.AliceBlue; } }
            public static Brush LightForeground { get { return Brushes.WhiteSmoke; } }


            // TEXT -----------------------------------------------------------

            public static Brush TextDisabled { get { return Brushes.DimGray; } }


            //  HYPER LINK ----------------------------------------------------

            public static Style HyperlinkStyle()
            {
                var style = new Style();
                style.TargetType = typeof(Hyperlink);

                Setter setter_deco = new Setter();
                setter_deco.Property = Hyperlink.TextDecorationsProperty;
                setter_deco.Value = null;
                style.Setters.Add(setter_deco);

                return style;
            }


            // LOG MESSAGES ---------------------------------------------------

            public static Brush LogMessageInfo { get { return Brushes.White; } }
            public static Brush LogMessageDebug { get { return Brushes.Gray; } }
            public static Brush LogMessageWarn { get { return Brushes.Yellow; } }
            public static Brush LogMessageError { get { return Brushes.IndianRed; } }
     

            //  GRID SPLITTER -------------------------------------------------

            public static Brush GridSplitterBackground { get { return Brushes.SteelBlue; } }
            public static Brush GridSplitterHovered { get { return Brushes.SkyBlue; } }
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


            //  CONTENT MENU --------------------------------------------------

            public static Style ContentMenuStyle()
            {
                var style = new Style();
                style.TargetType = typeof(Grid);

                Setter setter_background = new Setter();
                setter_background.Property = Grid.BackgroundProperty;
                setter_background.Value = ColorTheme.LightBackground;
                style.Setters.Add(setter_background);

                Setter setter_height = new Setter();
                setter_height.Property = Grid.HeightProperty;
                setter_height.Value = 20.0;
                style.Setters.Add(setter_height);

                return style;
            }

            public static Style CaptionStyle()
            {
                var style = new Style();
                style.TargetType = typeof(TextBlock);

                Setter setter_background = new Setter();
                setter_background.Property = TextBlock.BackgroundProperty;
                setter_background.Value = ColorTheme.LightBackground;
                style.Setters.Add(setter_background);

                Setter setter_foreground = new Setter();
                setter_foreground.Property = TextBlock.ForegroundProperty;
                setter_foreground.Value = ColorTheme.DarkForeground;
                style.Setters.Add(setter_foreground);

                Setter setter_horiz = new Setter();
                setter_horiz.Property = TextBlock.HorizontalAlignmentProperty;
                setter_horiz.Value = HorizontalAlignment.Left;
                style.Setters.Add(setter_horiz);

                Setter setter_vert = new Setter();
                setter_vert.Property = TextBlock.VerticalAlignmentProperty;
                setter_vert.Value = VerticalAlignment.Center;
                style.Setters.Add(setter_vert);

                Setter setter_margin = new Setter();
                setter_margin.Property = Label.MarginProperty;
                setter_margin.Value = new Thickness(2.0, 0.0, 2.0, 0.0);
                style.Setters.Add(setter_margin);

                return style;
            }


            // MENU -----------------------------------------------------------

            public static Brush MenuBackground { get { return Brushes.SteelBlue; } }
            public static Brush MenuForeground { get { return Brushes.White; } }


            public static Style MenuStyle()
            {
                var style = new Style();
                style.TargetType = typeof(Menu);

                Setter setter_background = new Setter();
                setter_background.Property = Menu.BackgroundProperty;
                setter_background.Value = ColorTheme.MenuBackground;
                style.Setters.Add(setter_background);

                Setter setter_foreground = new Setter();
                setter_foreground.Property = Menu.ForegroundProperty;
                setter_foreground.Value = ColorTheme.MenuForeground;
                style.Setters.Add(setter_foreground);

                Setter setter_height = new Setter();
                setter_height.Property = Menu.HeightProperty;
                setter_height.Value = 20.0;
                style.Setters.Add(setter_height);

                return style;
            }


            // MENU ITEM ------------------------------------------------------

            public static Brush MenuItemBackground { get { return Brushes.AliceBlue; } }
            public static Brush MenuItemForeground { get { return Brushes.Black; } }

            public static Style MenuItemStyle(string icon_filename = null)
            {
                var style = new Style();
                style.TargetType = typeof(MenuItem);

                if (!String.IsNullOrEmpty(icon_filename))
                {
                    Setter setter_icon = new Setter();
                    setter_icon.Property = MenuItem.IconProperty;
                    setter_icon.Value = ImageHelper.ImageFromFile(WorkingDirectory.Locations.MenuIcons, icon_filename);
                    style.Setters.Add(setter_icon);
                }
                Setter setter_foreground = new Setter();
                setter_foreground.Property = MenuItem.ForegroundProperty;
                setter_foreground.Value = ColorTheme.MenuItemForeground;
                style.Setters.Add(setter_foreground);

                return style;
            }


            // CONTEXT MENU ---------------------------------------------------

            public static Style ContextMenuStyle()
            {
                var style = new Style();
                style.TargetType = typeof(ContextMenu);

                return style;
            }
        }
    }
}
