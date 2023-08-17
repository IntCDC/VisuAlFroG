﻿using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Core.Utilities;
using System.Windows.Documents;



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

            // BACKGROUND -----------------------------------------------------

            public static Brush BackgroundWhite { get { return Brushes.White; } }
            public static Brush BackgroundBlack { get { return Brushes.Black; } }


            // TEXT -----------------------------------------------------------

            public static Brush TextDisabled { get { return Brushes.DimGray; } }


            // GRID -----------------------------------------------------------

            public static Brush GridBackground { get { return Brushes.AliceBlue; } }



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


            // MENU -----------------------------------------------------------

            public static Brush MenuBackground { get { return Brushes.SteelBlue; } }
            public static Brush MenuForeground { get { return Brushes.GhostWhite; } }


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
                return style;
            }


            // MENU ITEM ------------------------------------------------------

            public static Brush MenuItemBackground { get { return Brushes.AliceBlue; } }
            public static Brush MenuItemForeground { get { return Brushes.Black; } }

            public static Style MenuItemStyle(string icon_path = null)
            {
                var style = new Style();
                style.TargetType = typeof(MenuItem);

                if (!String.IsNullOrEmpty(icon_path))
                {
                    Setter setter_icon = new Setter();
                    setter_icon.Property = MenuItem.IconProperty;
                    var image = new Image();
                    string path = Artefacts.Path() + "/resources/menu/" + icon_path;
                    try
                    {
                        image.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));

                        setter_icon.Value = image;
                        style.Setters.Add(setter_icon);
                    }
                    catch (Exception exc)
                    {
                        if ((exc is System.Net.WebException) || (exc is System.IO.FileNotFoundException))
                        {
                            Log.Default.Msg(Log.Level.Error, exc.Message);
                        }
                        else
                        {
                            throw;
                        }
                    }
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

                /*
                style.WindowColor = MenuColor;
                style.ContentAreaColorLight = MenuColor;
                style.ContentAreaColorDark = MenuColor;
                style.DisabledControlLightColor = MenuColor;
                style.DisabledControlDarkColor = MenuColor;
                style.DisabledForegroundColor = MenuColor;
                style.SelectedBackgroundColor = MenuColor;
                style.SelectedUnfocusedColor = MenuColor;
                style.ControlLightColor = MenuColor;
                style.ControlMediumColor = MenuColor;
                style.ControlDarkColor = MenuColor;
                style.ControlMouseOverColor = MenuColor;
                style.ControlPressedColor = MenuColor;
                style.GlyphColor = MenuColor;
                style.GlyphMouseOver = MenuColor
                style.BorderLightColor = MenuColor;
                style.BorderMediumColor = MenuColor;
                style.BorderDarkColor = MenuColor;
                style.PressedBorderLightColor = MenuColor;
                style.PressedBorderDarkColor = MenuColor;
                style.DisabledBorderLightColor = MenuColor;
                style.DisabledBorderDarkColor = MenuColor;
                style.DefaultBorderBrushDarkColor = MenuColor;
                */
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
