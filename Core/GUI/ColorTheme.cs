using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Documents;
using Core.Utilities;
using Core.Abstracts;



/*
 * Global GUI Color Theme for WPF
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class ColorTheme : AbstractService, IAbstractConfiguration
        {

            /* ------------------------------------------------------------------*/
            // public enum 

            public enum ColorStyle
            {
                LightBlue,
                DarkContrast
            }


            /* ------------------------------------------------------------------*/
            // public delegates

            /// <summary>
            /// Callback provided by the main WPF application on closing. 
            /// </summary>
            public delegate void SetColorStyle_Delegate(ColorStyle color_style);


            /* ------------------------------------------------------------------*/
            // static functions

            public static Color Color_StrokeSelected { get; set; }
            public static Color Color_StrokeDefault { get; set; }

            public static Color Color_LightForeground { get; set; }
            public static Color Color_LightBackground { get; set; }
            public static Color Color_DarkForeground { get; set; }
            public static Color Color_DarkBackground { get; set; }

            public static Brush Brush_TextDisabled { get; set; }

            public static Brush Brush_LogMessageInfo { get; set; }
            public static Brush Brush_LogMessageDebug { get; set; }
            public static Brush Brush_LogMessageWarn { get; set; }
            public static Brush Brush_LogMessageError { get; set; }

            public static Brush Brush_GridSplitterBackground { get; set; }
            public static Brush Brush_GridSplitterHovered { get; set; }

            public static Brush Brush_MenuBackground { get; set; }
            public static Brush Brush_MenuForeground { get; set; }

            public static double GridSplitterSize { get; set; }

            public static Brush Brush_LightForeground { get { return new SolidColorBrush(Color_LightForeground); } }
            public static Brush Brush_LightBackground { get { return new SolidColorBrush(Color_LightBackground); } }
            public static Brush Brush_DarkForeground { get { return new SolidColorBrush(Color_DarkForeground); } }
            public static Brush Brush_DarkBackground { get { return new SolidColorBrush(Color_DarkBackground); } }

            public static Brush Brush_StrokeSelected { get; set; } = new SolidColorBrush(Color_StrokeSelected);
            public static Brush Brush_StrokeDefault { get; set; } = new SolidColorBrush(Color_StrokeSelected);


            /* ------------------------------------------------------------------*/
            // static functions


            //  Random Color --------------------

            // Use one global random generator for colors for better random results
            public static Random RandomGenerator { get; } = new Random();

            public static Color RandomColor()
            {
                byte alpha = 0xAF;
                int size = 2048; // in bytes
                byte[] b = new byte[size];
                RandomGenerator.NextBytes(b);
                var r_index = RandomGenerator.Next(0, size - 1);
                var g_index = RandomGenerator.Next(0, size - 1);
                var b_index = RandomGenerator.Next(0, size - 1);
                var r_byte = 0x0F | b[r_index];
                var g_byte = 0x0F | b[g_index];
                var b_byte = 0x0F | b[b_index];
                return Color.FromArgb(alpha, (byte)r_byte, (byte)g_byte, (byte)b_byte);
            }

            //  HYPER LINK ----------------------

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

            //  GRID SPLITTER -------------------

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

            //  CONTENT MENU --------------------

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

            // MENU -----------------------------

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


            // MENU ITEM ------------------------

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

            // CONTEXT MENU ---------------------

            public static Style ContextMenuStyle()
            {
                var style = new Style();
                style.TargetType = typeof(ContextMenu);

                return style;
            }


            /* ------------------------------------------------------------------*/
            // public functions


            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                set_style_lightblue();

                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().Name);
                }
                return _initialized;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    /// PLACE YOUR STUFF HERE ...

                    _initialized = false;
                }
                return true;
            }

            public string CollectConfigurations()
            {
                /// TODO

                return "";
            }

            public bool ApplyConfigurations(string configurations)
            {
                /// TODO

                return false;
            }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="color_style"></param>
            public void SetColorStyle(ColorStyle color_style)
            {
                switch (color_style)
                {
                    case (ColorStyle.LightBlue):
                        set_style_lightblue();
                        break;
                    case (ColorStyle.DarkContrast):
                        set_style_darkcontrast();
                        break;
                    default:
                        Log.Default.Msg(Log.Level.Error, "Color style not implemented");
                        break;
                }
            }


            /* ------------------------------------------------------------------*/
            // private functions

            private void set_style_lightblue()
            {
                /* Color.FromArgb(0x00, 0x00, 0x00, 0x00); */

                /// TODO XXX HOW to apply afterwards???

                Color_StrokeSelected = Colors.Red;
                Color_StrokeDefault = Colors.LightYellow;
                Color_LightForeground = Colors.AliceBlue;
                Color_LightBackground = Colors.AliceBlue;
                Color_DarkForeground = Colors.Black;
                Color_DarkBackground = Colors.Black;
                Brush_TextDisabled = Brushes.DimGray;
                Brush_LogMessageInfo = Brushes.White;
                Brush_LogMessageDebug = Brushes.Gray;
                Brush_LogMessageWarn = Brushes.Yellow;
                Brush_LogMessageError = Brushes.IndianRed;
                Brush_GridSplitterBackground = Brushes.SteelBlue;
                Brush_GridSplitterHovered = Brushes.SkyBlue;
                Brush_MenuBackground = Brushes.SteelBlue;
                Brush_MenuForeground = Brushes.White;
                GridSplitterSize = 5.0;

                _color_style = ColorStyle.LightBlue;
            }

            private void set_style_darkcontrast()
            {
                /* Color.FromArgb(0x00, 0x00, 0x00, 0x00); */

                Color_StrokeSelected = Colors.Red;
                Color_StrokeDefault = Colors.LightYellow;
                Color_LightForeground = Colors.Black;
                Color_LightBackground = Colors.Black;
                Color_DarkForeground = Colors.AliceBlue;
                Color_DarkBackground = Colors.AliceBlue;
                Brush_TextDisabled = Brushes.DimGray;
                Brush_LogMessageInfo = Brushes.White;
                Brush_LogMessageDebug = Brushes.Gray;
                Brush_LogMessageWarn = Brushes.Yellow;
                Brush_LogMessageError = Brushes.IndianRed;
                Brush_GridSplitterBackground = Brushes.SteelBlue;
                Brush_GridSplitterHovered = Brushes.SkyBlue;
                Brush_MenuBackground = Brushes.SteelBlue;
                Brush_MenuForeground = Brushes.White;
                GridSplitterSize = 5.0;

                _color_style = ColorStyle.DarkContrast;
            }


            /* ------------------------------------------------------------------*/
            // private variables




            private ColorStyle _color_style = ColorStyle.LightBlue;

        }
    }
}
