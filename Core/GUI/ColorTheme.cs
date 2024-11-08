using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using Core.Utilities;
using Core.Abstracts;
using System.Collections.Generic;



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
            #region public enum 

            public enum PredefinedThemes
            {
                LightBlue,
                Dark
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties

            public static ColorTheme.PredefinedThemes _DefaultColorTheme { get { return ColorTheme.PredefinedThemes.Dark; } }
            public static double _GridSplitterSize { get { return 7.0; } }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public classes 

            /// <summary>
            /// Configuration data.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public ColorTheme.PredefinedThemes theme { get; set; }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region static functions

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

            public static Color SinglePaletteColor()
            {
                byte alpha = 0xAF;
                var r = 34;
                var g = 168;
                var b = 132;
                return Color.FromArgb(alpha, (byte)r, (byte)g, (byte)b);
            }
                public static Color PaletteColor(double palette_index)
            {
                if ((palette_index < 0.0) || (palette_index > 1.0))
                {
                    Log.Default.Msg(Log.Level.Error, "Palette index out of range");
                    return Colors.Transparent;
                }

                byte alpha = 0xAF;
                int[,] palette_colors = new int[16, 3] {
                    { 253, 231, 37 },
                    { 210, 226, 27 },
                    { 165, 219, 54 },
                    { 122, 209, 81 },
                    { 84, 197, 104 },
                    { 53, 183, 121 },
                    { 34, 168, 132 },
                    { 31, 152, 139 },
                    { 35, 136, 142 },
                    { 42, 120, 142 },
                    { 49, 104, 142 },
                    { 57, 86, 140 },
                    { 65, 68, 135 },
                    { 71, 47, 125 },
                    { 72, 26, 108 },
                    { 68, 1, 84 }};

                double max_color_index = 12.0;
                int color_index_floor = (int)Math.Floor(palette_index * max_color_index);
                double color_fraction = (palette_index * max_color_index) - (double)color_index_floor;

                double r = palette_colors[color_index_floor, 0] + (palette_colors[color_index_floor + 1, 0] - palette_colors[color_index_floor, 0]) * color_fraction;
                double g = palette_colors[color_index_floor, 1] + (palette_colors[color_index_floor + 1, 1] - palette_colors[color_index_floor, 1]) * color_fraction;
                double b = palette_colors[color_index_floor, 2] + (palette_colors[color_index_floor + 1, 2] - palette_colors[color_index_floor, 2]) * color_fraction;

                return Color.FromArgb(alpha, (byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b));
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
                setter_background.Value = new DynamicResourceExtension("Brush_GridSplitterBackground");
                style.Setters.Add(setter_background);

                // Create trigger for color change on mouse hover
                Trigger trigger = new Trigger();
                trigger.Property = GridSplitter.IsMouseOverProperty;
                trigger.Value = true;
                Setter setter_trigger = new Setter();
                setter_trigger.Property = GridSplitter.BackgroundProperty;
                setter_trigger.Value = new DynamicResourceExtension("Brush_GridSplitterHovered");
                trigger.Setters.Add(setter_trigger);
                style.Triggers.Add(trigger);

                return style;
            }

            //  MENU --------------------

            public static Style MainMenuBarStyle()
            {
                var style = new Style();
                style.TargetType = typeof(Menu);

                Setter setter_foreground = new Setter();
                setter_foreground.Property = TextBlock.ForegroundProperty;
                setter_foreground.Value = new DynamicResourceExtension("Brush_MenuBarForeground");
                style.Setters.Add(setter_foreground);

                Setter setter_background = new Setter();
                setter_background.Property = Menu.BackgroundProperty;
                setter_background.Value = new DynamicResourceExtension("Brush_MenuBarBackground");
                style.Setters.Add(setter_background);

                Setter setter_border = new Setter();
                setter_border.Property = Menu.BorderBrushProperty;
                setter_border.Value = new DynamicResourceExtension("Brush_MenuBarBorder");
                style.Setters.Add(setter_border);

                Setter setter_thickness = new Setter();
                setter_thickness.Property = Menu.BorderThicknessProperty;
                setter_thickness.Value = new Thickness(0, 0, 0, _GridSplitterSize);
                style.Setters.Add(setter_thickness);

                return style;
            }

            public static Style ContentMenuBarStyle()
            {
                var style = new Style();
                style.TargetType = typeof(Menu);

                Setter setter_foreground = new Setter();
                setter_foreground.Property = TextBlock.ForegroundProperty;
                setter_foreground.Value = new DynamicResourceExtension("Brush_MenuBarForeground");
                style.Setters.Add(setter_foreground);

                Setter setter_background = new Setter();
                setter_background.Property = Menu.BackgroundProperty;
                setter_background.Value = new DynamicResourceExtension("Brush_MenuBarBackground");
                style.Setters.Add(setter_background);

                Setter setter_thickness = new Setter();
                setter_thickness.Property = Menu.BorderThicknessProperty;
                setter_thickness.Value = new Thickness(0, 0, 0, 0); ;
                style.Setters.Add(setter_thickness);

                return style;
            }

            public static Style ContentCaptionStyle()
            {
                var style = new Style();
                style.TargetType = typeof(TextBox);

                Setter setter_foreground = new Setter();
                setter_foreground.Property = TextBox.ForegroundProperty;
                setter_foreground.Value = new DynamicResourceExtension("Brush_MenuBarForeground");
                style.Setters.Add(setter_foreground);

                Setter setter_background = new Setter();
                setter_background.Property = TextBox.BackgroundProperty;
                setter_background.Value = new DynamicResourceExtension("Brush_MenuBarBackground");
                style.Setters.Add(setter_background);

                Setter setter_horiz = new Setter();
                setter_horiz.Property = TextBox.HorizontalAlignmentProperty;
                setter_horiz.Value = HorizontalAlignment.Left;
                style.Setters.Add(setter_horiz);

                Setter setter_vert = new Setter();
                setter_vert.Property = TextBox.VerticalAlignmentProperty;
                setter_vert.Value = VerticalAlignment.Center;
                style.Setters.Add(setter_vert);

                Setter setter_margin = new Setter();
                setter_margin.Property = Label.MarginProperty;
                setter_margin.Value = new Thickness(2.0, 0.0, 2.0, 0.0);
                style.Setters.Add(setter_margin);

                Setter setter_fontweight = new Setter();
                setter_fontweight.Property = Label.FontWeightProperty;
                setter_fontweight.Value = FontWeights.Bold;
                style.Setters.Add(setter_fontweight);

                return style;
            }

            public static Style MenuItemIconStyle(string icon_filename = null)
            {
                var style = new Style();
                style.TargetType = typeof(MenuItem);

                if (!String.IsNullOrEmpty(icon_filename))
                {
                    Setter setter_icon = new Setter();
                    setter_icon.Property = MenuItem.IconProperty;
                    setter_icon.Value = ImageLoader.ImageFromFile(ResourcePaths.Locations.MenuIcons, icon_filename);
                    style.Setters.Add(setter_icon);
                }

                Setter setter_foreground = new Setter();
                setter_foreground.Property = MenuItem.ForegroundProperty;
                setter_foreground.Value = new DynamicResourceExtension("Brush_MenuItemForeground");
                style.Setters.Add(setter_foreground);

                Setter setter_background = new Setter();
                setter_background.Property = MenuItem.BackgroundProperty;
                setter_background.Value = new DynamicResourceExtension("Brush_MenuItemBackground");
                style.Setters.Add(setter_background);

                return style;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public bool Initialize(ResourceDictionary app_resources)
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                _app_resource = app_resources;
                SetColorStyle(_DefaultColorTheme);

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
                var theme_configurations = new ColorTheme.Configuration();
                theme_configurations.theme = _color_theme;
                return ConfigurationService.Serialize<ColorTheme.Configuration>(theme_configurations);
            }

            public bool ApplyConfigurations(string configurations)
            {
                var theme_configurations = ConfigurationService.Deserialize<ColorTheme.Configuration>(configurations);
                if (theme_configurations != null)
                {
                    SetColorStyle(theme_configurations.theme);
                    return true;
                }
                return false;
            }

            public void SetColorStyle(PredefinedThemes theme)
            {
                _app_resource.Clear();
                Uri theme_uri = null;
                switch (theme)
                {
                    case (PredefinedThemes.LightBlue):
                        theme_uri = ResourcePaths.GetResourcePath(ResourcePaths.Locations.Themes, "LightBlue.xaml");
                        break;
                    case (PredefinedThemes.Dark):
                        theme_uri = ResourcePaths.GetResourcePath(ResourcePaths.Locations.Themes, "Dark.xaml");
                        break;
                    default:
                        Log.Default.Msg(Log.Level.Error, "Unknown predefined color theme");
                        return;
                }
                ResourceDictionary theme_resource = new ResourceDictionary() { Source = theme_uri };

                _app_resource.MergedDictionaries.Add(theme_resource);
                _color_theme = theme;

                update_menu_item_selection();
            }

            public override void AttachMenu(MenubarMain menu_bar)
            {
                var themes_names = Enum.GetNames(typeof(PredefinedThemes));
                foreach (var theme_name in themes_names)
                {
                    var theme_item = MenubarMain.GetDefaultMenuItem(theme_name);
                    theme_item.Click += (object sender, RoutedEventArgs e) =>
                    {
                        var sender_content = sender as MenuItem;
                        if (sender_content == null)
                        {
                            return;
                        }
                        SetColorStyle((PredefinedThemes)Enum.Parse(typeof(PredefinedThemes), theme_name));

                    };
                    menu_bar.AddMenu(MenubarMain.PredefinedMenuOption.STYLE, theme_item);
                    _theme_menu_items.Add(theme_item);
                }
                update_menu_item_selection();
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions 

            private void update_menu_item_selection()
            {
                foreach (var theme_meu_item in _theme_menu_items)
                {
                    theme_meu_item.IsChecked = (_color_theme == (PredefinedThemes)Enum.Parse(typeof(PredefinedThemes), (string)theme_meu_item.Header));
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private PredefinedThemes _color_theme = PredefinedThemes.LightBlue;
            private ResourceDictionary _app_resource = null;
            private List<MenuItem> _theme_menu_items = new List<MenuItem>();

            #endregion
        }
    }
}
