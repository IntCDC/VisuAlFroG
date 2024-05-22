using System;
using System.Drawing;
using System.Windows.Forms;



/*
 * Loading Progress Bar Window
 * 
 */
namespace Core
{
    namespace GUI
    {
        public partial class LoadingProgressWindow : Form
        {
            /* ------------------------------------------------------------------*/
            #region public functions


            public LoadingProgressWindow()
            {
                int width = 500;
                int height = 50;

                InitializeComponent();
                this.ClientSize = new System.Drawing.Size(width, height);

                _progress_bar = new LoadingProgressBar(width, height);
                this.Controls.Add(_progress_bar);

                Show();
            }

            /// <summary>
            /// Provide value in range Minimum - Maximum 
            /// </summary>
            /// <param name="v"></param>
            public void SetValue(int v)
            {
                _progress_bar.Value = (v < _progress_bar.Minimum) ? (_progress_bar.Minimum) : ((v > _progress_bar.Maximum) ? (_progress_bar.Maximum) : (v));
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions

            private void LoadingProgress_Load(object sender, EventArgs e)
            {

            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region public variables

            private LoadingProgressBar _progress_bar = null;

            #endregion
        }
    }
}
