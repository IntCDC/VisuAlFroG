using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using Core.Utilities;
using Core.Abstracts;
using System.Collections.Generic;
using System.Windows.Input;



/*
 * Rename Label
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class RenameLabel : TextBox
        {
            /* ------------------------------------------------------------------*/
            #region public classes 

            public RenameLabel() : base()
            {
                reset();
                this.IsEnabled = true;
                this.BorderThickness = new Thickness(0, 0, 0, 0);

                this.KeyUp += (object sender, KeyEventArgs e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        reset();
                        Keyboard.ClearFocus();
                    }
                };
                this.LostKeyboardFocus += (object sender, KeyboardFocusChangedEventArgs e) =>
                {
                    reset();
                };
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions 

            private void reset()
            {
                this.Focusable = false;
                this.Cursor = Cursors.Arrow;
                this.Style = ColorTheme.ContentCaptionStyle();
            }

            #endregion
        }
    }
}
