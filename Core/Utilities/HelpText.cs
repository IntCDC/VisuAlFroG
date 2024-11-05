using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Collections.Generic;


/*
 * Questionmark showing help text on hovering
 * 
 */
namespace Core
{
    namespace Utilities
    {
        public class HelpText : TextBlock
        {
            /* ------------------------------------------------------------------*/
            #region public properties

            public string _HelpText { get { return _help_text.Text; } set { _help_text.Text = value; } }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public HelpText()
            {
                this.Text = " ? ";
                this.FontSize = 16;
                this.FontWeight = FontWeights.Bold;
                this.VerticalAlignment = VerticalAlignment.Center;
                this.HorizontalAlignment = HorizontalAlignment.Center;
                this.SetResourceReference(TextBlock.ForegroundProperty, "Brush_Foreground");

                this.MouseEnter += (object sender, System.Windows.Input.MouseEventArgs e) =>
                {
                    if (!_help_popup.IsOpen)
                    {
                        _help_popup.IsOpen = true;
                    }
                };
                this.MouseLeave += (object sender, System.Windows.Input.MouseEventArgs e) =>
                {
                    if (_help_popup.IsOpen)
                    {
                        _help_popup.IsOpen = false;
                    }
                };

                _help_text.Padding = new Thickness(5.0);
                _help_text.SetResourceReference(TextBlock.BackgroundProperty, "Brush_Foreground");
                _help_text.SetResourceReference(TextBlock.ForegroundProperty, "Brush_Background");

                _help_popup.Placement = PlacementMode.Mouse;
                _help_popup.Child = _help_text;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables

            private Popup _help_popup = new Popup();
            private TextBlock _help_text = new TextBlock();

            #endregion
        }
    }
}
