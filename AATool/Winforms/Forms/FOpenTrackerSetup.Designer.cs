
namespace AATool.Winforms.Forms
{
    partial class FOpenTrackerSetup
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
            this.label8 = new System.Windows.Forms.Label();
            this.url = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.aaKey = new System.Windows.Forms.TextBox();
            this.toggleKey = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.done = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(50, 58);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 61;
            this.label8.Text = "Upload URL:";
            // 
            // url
            // 
            this.url.Location = new System.Drawing.Point(53, 74);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(192, 20);
            this.url.TabIndex = 60;
            this.url.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(50, 13);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 13);
            this.label7.TabIndex = 59;
            this.label7.Text = "AAKey:";
            // 
            // aaKey
            // 
            this.aaKey.Location = new System.Drawing.Point(53, 29);
            this.aaKey.Name = "aaKey";
            this.aaKey.Size = new System.Drawing.Size(106, 20);
            this.aaKey.TabIndex = 58;
            this.aaKey.UseSystemPasswordChar = true;
            this.aaKey.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // toggleKey
            // 
            this.toggleKey.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.toggleKey.Location = new System.Drawing.Point(165, 28);
            this.toggleKey.Name = "toggleKey";
            this.toggleKey.Size = new System.Drawing.Size(80, 22);
            this.toggleKey.TabIndex = 62;
            this.toggleKey.Text = "Show AAKey";
            this.toggleKey.UseVisualStyleBackColor = true;
            this.toggleKey.Click += new System.EventHandler(this.OnClick);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 66);
            this.label1.TabIndex = 63;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // done
            // 
            this.done.Location = new System.Drawing.Point(122, 141);
            this.done.Name = "done";
            this.done.Size = new System.Drawing.Size(75, 23);
            this.done.TabIndex = 64;
            this.done.Text = "Done";
            this.done.UseVisualStyleBackColor = true;
            this.done.Click += new System.EventHandler(this.OnClick);
            // 
            // FOpenTrackerSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 177);
            this.Controls.Add(this.done);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.toggleKey);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.url);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.aaKey);
            this.Name = "FOpenTrackerSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OpenTracker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox url;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox aaKey;
        private System.Windows.Forms.Button toggleKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button done;
    }
}