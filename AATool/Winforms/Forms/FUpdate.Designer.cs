
namespace AATool.Winforms.Forms
{
    partial class FUpdate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FUpdate));
            this.patchNotes = new System.Windows.Forms.RichTextBox();
            this.yes = new System.Windows.Forms.Button();
            this.no = new System.Windows.Forms.Button();
            this.browser = new System.Windows.Forms.Button();
            this.icon = new System.Windows.Forms.PictureBox();
            this.label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
            this.SuspendLayout();
            // 
            // patchNotes
            // 
            this.patchNotes.Location = new System.Drawing.Point(12, 54);
            this.patchNotes.Name = "patchNotes";
            this.patchNotes.ReadOnly = true;
            this.patchNotes.Size = new System.Drawing.Size(400, 220);
            this.patchNotes.TabIndex = 0;
            this.patchNotes.Text = "";
            // 
            // yes
            // 
            this.yes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.yes.Location = new System.Drawing.Point(12, 280);
            this.yes.Name = "yes";
            this.yes.Size = new System.Drawing.Size(75, 23);
            this.yes.TabIndex = 1;
            this.yes.Text = "Yes";
            this.yes.UseVisualStyleBackColor = true;
            // 
            // no
            // 
            this.no.DialogResult = System.Windows.Forms.DialogResult.No;
            this.no.Location = new System.Drawing.Point(93, 280);
            this.no.Name = "no";
            this.no.Size = new System.Drawing.Size(75, 23);
            this.no.TabIndex = 2;
            this.no.Text = "No";
            this.no.UseVisualStyleBackColor = true;
            // 
            // browser
            // 
            this.browser.Location = new System.Drawing.Point(313, 280);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(99, 23);
            this.browser.TabIndex = 3;
            this.browser.Text = "Open in Browser";
            this.browser.UseVisualStyleBackColor = true;
            this.browser.Click += new System.EventHandler(this.OnClick);
            // 
            // icon
            // 
            this.icon.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.icon.Location = new System.Drawing.Point(12, 12);
            this.icon.Name = "icon";
            this.icon.Size = new System.Drawing.Size(36, 36);
            this.icon.TabIndex = 5;
            this.icon.TabStop = false;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(50, 23);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(365, 13);
            this.label.TabIndex = 6;
            this.label.Text = "A new version of CTM\'s AATool is available! Would you like to update now?";
            // 
            // FUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 315);
            this.Controls.Add(this.label);
            this.Controls.Add(this.icon);
            this.Controls.Add(this.browser);
            this.Controls.Add(this.no);
            this.Controls.Add(this.yes);
            this.Controls.Add(this.patchNotes);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FUpdate";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Updates are Available";
            ((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox patchNotes;
        private System.Windows.Forms.Button yes;
        private System.Windows.Forms.Button no;
        private System.Windows.Forms.Button browser;
        private System.Windows.Forms.PictureBox icon;
        private System.Windows.Forms.Label label;
    }
}