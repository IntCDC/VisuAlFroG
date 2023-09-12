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


            // DATA -----------------------------------------------------

            public static Color Color_StrokeSelected { get { return Colors.Red; } }
            public static Color Color_StrokeDefault { get { return Color.FromArgb(0x00, 0x00, 0x00, 0x00); } }

            public static Random random_generator = new Random();

            public static Color RandomColor()
            {
                byte alpha = 0xAF;
                int size = 2048; // in bytes
                byte[] b = new byte[size];
                random_generator.NextBytes(b);
                var r_index = random_generator.Next(0, size - 1);
                var g_index = random_generator.Next(0, size - 1);
                var b_index = random_generator.Next(0, size - 1);
                var r_byte = 0x0F | b[r_index];
                var g_byte = 0x0F | b[g_index];
                var b_byte = 0x0F | b[b_index];
                return Color.FromArgb(alpha, (byte)r_byte, (byte)g_byte, (byte)b_byte);
            }


            // GENERIC -----------------------------------------------------

            public static Brush Brush_DarkBackground { get { return Brushes.Black; } }
            public static Brush Brush_DarkForeground { get { return Brushes.Black; } }

            public static Brush Brush_LightBackground { get { return Brushes.AliceBlue; } }
            public static Brush Brush_LightForeground { get { return Brushes.AliceBlue; } }

            public static Color Color_LightForeground { get { return Colors.AliceBlue; } }



            // TEXT -----------------------------------------------------------

            public static Brush Brush_TextDisabled { get { return Brushes.DimGray; } }


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

            public static Brush Brush_LogMessageInfo { get { return Brushes.White; } }
            public static Brush Brush_LogMessageDebug { get { return Brushes.Gray; } }
            public static Brush Brush_LogMessageWarn { get { return Brushes.Yellow; } }
            public static Brush Brush_LogMessageError { get { return Brushes.IndianRed; } }
     

            //  GRID SPLITTER -------------------------------------------------

            public static Brush Brush_GridSplitterBackground { get { return Brushes.SteelBlue; } }
            public static Brush Brush_GridSplitterHovered { get { return Brushes.SkyBlue; } }
            public static double GridSplitterSize { get { return 5.0; } }

            public static Style GridSplitterStyle()
            {
                var style = new Style();
                style.TargetType = typeof(GridSplitter);

                Setter setter_background = new Setter();
                setter_background.Property = GridSplitter.BackgroundProperty;
                setter_background.Value = ColorTheme.Brush_GridSplitterBackground;
                style.Setters.Add(setter_background);

                // Create trigger for color change on mouse hover
                Trigger trigger = new Trigger();
                trigger.Property = GridSplitter.IsMouseOverProperty;
                trigger.Value = true;
                Setter setter_trigger = new Setter();
                setter_trigger.Property = GridSplitter.BackgroundProperty;
                setter_trigger.Value = ColorTheme.Brush_GridSplitterHovered;
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
                setter_background.Value = ColorTheme.Brush_LightBackground;
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
                setter_background.Value = ColorTheme.Brush_LightBackground;
                style.Setters.Add(setter_background);

                Setter setter_foreground = new Setter();
                setter_foreground.Property = TextBlock.ForegroundProperty;
                setter_foreground.Value = ColorTheme.Brush_DarkForeground;
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

            public static Brush Brush_MenuBackground { get { return Brushes.SteelBlue; } }
            public static Brush Brush_MenuForeground { get { return Brushes.White; } }


            public static Style MenuStyle()
            {
                var style = new Style();
                style.TargetType = typeof(Menu);

                Setter setter_background = new Setter();
                setter_background.Property = Menu.BackgroundProperty;
                setter_background.Value = ColorTheme.Brush_MenuBackground;
                style.Setters.Add(setter_background);

                Setter setter_foreground = new Setter();
                setter_foreground.Property = Menu.ForegroundProperty;
                setter_foreground.Value = ColorTheme.Brush_MenuForeground;
                style.Setters.Add(setter_foreground);

                Setter setter_height = new Setter();
                setter_height.Property = Menu.HeightProperty;
                setter_height.Value = 20.0;
                style.Setters.Add(setter_height);

                return style;
            }


            // MENU ITEM ------------------------------------------------------

            public static Brush Brush_MenuItemBackground { get { return Brushes.AliceBlue; } }
            public static Brush Brush_MenuItemForeground { get { return Brushes.Black; } }

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
                setter_foreground.Value = ColorTheme.Brush_MenuItemForeground;
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
