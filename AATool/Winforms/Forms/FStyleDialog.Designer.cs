
namespace AATool.Winforms.Forms
{
    partial class FStyleDialog
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
            this.frames = new System.Windows.Forms.FlowLayoutPanel();
            this.closeOnSelect = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // frames
            // 
            this.frames.AutoScroll = true;
            this.frames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frames.Location = new System.Drawing.Point(0, 0);
            this.frames.Margin = new System.Windows.Forms.Padding(0);
            this.frames.Name = "frames";
            this.frames.Size = new System.Drawing.Size(809, 681);
            this.frames.TabIndex = 0;
            // 
            // closeOnSelect
            // 
            this.closeOnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeOnSelect.BackColor = System.Drawing.Color.Transparent;
            this.closeOnSelect.Location = new System.Drawing.Point(350, 302);
            this.closeOnSelect.Name = "closeOnSelect";
            this.closeOnSelect.Size = new System.Drawing.Size(140, 17);
            this.closeOnSelect.TabIndex = 2;
            this.closeOnSelect.Text = "Close Upon Selection";
            this.closeOnSelect.UseVisualStyleBackColor = false;
            this.closeOnSelect.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // FStyleDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 681);
            this.Controls.Add(this.closeOnSelect);
            this.Controls.Add(this.frames);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FStyleDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FFrameDesigner";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel frames;
        private System.Windows.Forms.CheckBox closeOnSelect;
    }
}