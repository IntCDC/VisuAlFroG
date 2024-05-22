using System.Drawing;



namespace Core.GUI
{
    partial class LoadingProgressWindow
    {
        /* ------------------------------------------------------------------*/
        #region public variables

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private functions 

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Icon = SystemIcons.Information;
            this.Name = "loading_progress";
            this.Text = "Loading...";
            this.Load += new System.EventHandler(this.LoadingProgress_Load);

            this.ResumeLayout(false);
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private variables 

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #endregion
    }
}
