
namespace AATool.Winforms.Forms
{
    partial class FDebug
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.atlas = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.atlas)).BeginInit();
            this.SuspendLayout();
            // 
            // atlas
            // 
            this.atlas.Location = new System.Drawing.Point(12, 12);
            this.atlas.Name = "atlas";
            this.atlas.Size = new System.Drawing.Size(512, 512);
            this.atlas.TabIndex = 0;
            this.atlas.TabStop = false;
            // 
            // FDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1482, 1098);
            this.Controls.Add(this.atlas);
            this.Name = "FDebug";
            this.Text = "Debug Info";
            ((System.ComponentModel.ISupportInitialize)(this.atlas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox atlas;
    }
}