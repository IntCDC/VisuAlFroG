using System;
using System.Drawing;
using System.Windows.Forms;



/*
 * Loading Progress Bar 
 * 
 */
namespace Core
{
    namespace GUI
    {
        public class LoadingProgressBar : System.Windows.Forms.ProgressBar
        {
            /* ------------------------------------------------------------------*/
            #region public functions

            public LoadingProgressBar(int width, int height)
            {
                int padding = 10;

                this.Minimum = 0;
                this.Maximum = 100;
                this.Step = 1;
                this.Value = 0;
                this.Location = new System.Drawing.Point(padding, padding);
                this.Name = "progress_bar";
                this.Width = width - (2 * padding);
                this.Height = height - (2 * padding);
                // this.Style = ProgressBarStyle.Blocks;
                this.SetStyle(ControlStyles.UserPaint, true);


            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected override void OnPaint(PaintEventArgs e)
            {
                Rectangle rec = e.ClipRectangle;

                rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
                if (ProgressBarRenderer.IsSupported)
                    ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
                rec.Height = rec.Height - 4;

                e.Graphics.FillRectangle(Brushes.DarkBlue, 2, 2, rec.Width, rec.Height);
            }

            #endregion
        }
    }
}
