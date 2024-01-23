﻿using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Core.Utilities;
using Core.Abstracts;
using Core.GUI;



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

            public enum PredefinedThemes
            {
                LightBlue,
                DarkContrast
            }


            /* ------------------------------------------------------------------*/
            // public classes 

            /// <summary>
            /// Configuration data.
            /// </summary>
            public class Configuration : IAbstractConfigurationData
            {
                public ColorTheme.PredefinedThemes theme { get; set; }
            }


            /* ------------------------------------------------------------------*/
            // public properties

            public static ColorTheme.PredefinedThemes DefaultColorTheme { get { return ColorTheme.PredefinedThemes.LightBlue; } }


            /* ------------------------------------------------------------------*/
            // public delegates

            /// <summary>
            /// .... 
            /// </summary>
            public delegate void SetColorStyle_Delegate(PredefinedThemes theme);


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

            public static double GridSplitterSize { get { return 5.0; } }

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

            public static Style MenuBarStyle()
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

                return style;
            }

            public static Style ContentMenuCaptionStyle()
            {
                var style = new Style();
                style.TargetType = typeof(TextBlock);

                Setter setter_foreground = new Setter();
                setter_foreground.Property = TextBlock.ForegroundProperty;
                setter_foreground.Value = new DynamicResourceExtension("Brush_MenuBarForeground");
                style.Setters.Add(setter_foreground);

                Setter setter_background = new Setter();
                setter_background.Property = TextBlock.BackgroundProperty;
                setter_background.Value = new DynamicResourceExtension("Brush_MenuBarBackground");
                style.Setters.Add(setter_background);

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

                Setter setter_fontweight = new Setter();
                setter_fontweight.Property = Label.FontWeightProperty;
                setter_fontweight.Value = FontWeights.Bold;
                style.Setters.Add(setter_fontweight);

                return style;
            }

            public static Style MenuItemStyle()
            {
                var style = new Style();
                style.TargetType = typeof(MenuItem);

                Setter setter_foreground = new Setter();
                setter_foreground.Property = Menu.ForegroundProperty;
                setter_foreground.Value = new DynamicResourceExtension("Brush_MenuItemForeground");
                style.Setters.Add(setter_foreground);

                /*
                Setter setter_background = new Setter();
                setter_background.Property = Menu.BackgroundProperty;
                setter_background.Value = new DynamicResourceExtension("Brush_MenuItemBackground");
                style.Setters.Add(setter_background);

                Setter setter_border = new Setter();
                setter_border.Property = Menu.BorderBrushProperty;
                setter_border.Value = new DynamicResourceExtension("Brush_MenuBarBorder");
                style.Setters.Add(setter_border);
                */

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
                    setter_icon.Value = ImageHelper.ImageFromFile(WorkingDirectory.Locations.MenuIcons, icon_filename);
                    style.Setters.Add(setter_icon);
                }

                Setter setter_foreground = new Setter();
                setter_foreground.Property = MenuItem.ForegroundProperty;
                setter_foreground.Value = new DynamicResourceExtension("Brush_MenuItemForeground");
                style.Setters.Add(setter_foreground);

                /*
                Setter setter_background = new Setter();
                setter_background.Property = MenuItem.BackgroundProperty;
                setter_background.Value = new DynamicResourceExtension("Brush_MenuItemBackground");
                style.Setters.Add(setter_background);

                Setter setter_border = new Setter();
                setter_border.Property = Menu.BorderBrushProperty;
                setter_border.Value = new DynamicResourceExtension("Brush_MenuItemBorder");
                style.Setters.Add(setter_border);

                Setter setter_trigger_background = new Setter();
                setter_trigger_background.Property = MenuItem.BackgroundProperty;
                setter_trigger_background.Value = new DynamicResourceExtension("Brush_MenuItemBackgroundHighlight");

                Setter setter_trigger_border = new Setter();
                setter_trigger_border.Property = MenuItem.BorderBrushProperty;
                setter_trigger_border.Value = new DynamicResourceExtension("Brush_MenuItemBorderdHighlight");

                Trigger trigger = new Trigger();
                trigger.Property = MenuItem.IsHighlightedProperty;
                trigger.Value = true;
                trigger.Setters.Add(setter_trigger_background);
                trigger.Setters.Add(setter_trigger_border);

                ControlTemplate control_template = new ControlTemplate();
                control_template.TargetType = typeof(MenuItem);
                control_template.Triggers.Add(trigger);

                Setter setter_highlight = new Setter();
                setter_highlight.Property = MenuItem.TemplateProperty;
                setter_highlight.Value = control_template;

                style.Setters.Add(setter_highlight);
                */
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

            public bool Initialize(ResourceDictionary app_resources, MenuBar.MarkColorTheme_Delegate mark_color_theme_callback)
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();

                _app_resource = app_resources;
                _mark_color_theme_callback = mark_color_theme_callback;
                SetColorStyle(DefaultColorTheme);

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

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="theme"></param>
            public void SetColorStyle(PredefinedThemes theme)
            {
                _app_resource.Clear();

                ResourceDictionary theme_resource = null;
                switch (theme)
                {
                    case (PredefinedThemes.LightBlue):
                        theme_resource = new ResourceDictionary() { Source = new Uri("T:/02-Management/09_Datamanagement/RSE/VisuAlFrog/repository/VisuAlFroG/Core/GUI/Themes/LightBlue.xaml", UriKind.Absolute) };
                        break;
                    case (PredefinedThemes.DarkContrast):
                        theme_resource = new ResourceDictionary() { Source = new Uri("T:/02-Management/09_Datamanagement/RSE/VisuAlFrog/repository/VisuAlFroG/Core/GUI/Themes/DarkContrast.xaml", UriKind.Absolute) };
                        break;
                    default:
                        Log.Default.Msg(Log.Level.Error, "Unknown predefined color theme");
                        return;
                }

                _mark_color_theme_callback(theme);
                _app_resource.MergedDictionaries.Add(theme_resource);
                _color_theme = theme;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private PredefinedThemes _color_theme = PredefinedThemes.LightBlue;
            private ResourceDictionary _app_resource = null;
            private MenuBar.MarkColorTheme_Delegate _mark_color_theme_callback = null;

        }
    }
}