
namespace AATool.Winforms.Controls
{
    partial class CCreditsGroup
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label = new System.Windows.Forms.Label();
            this.divider = new System.Windows.Forms.GroupBox();
            this.flow = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(6, -2);
            this.label.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(180, 15);
            this.label.TabIndex = 38;
            this.label.Text = "Group Title";
            this.label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // divider
            // 
            this.divider.Location = new System.Drawing.Point(6, 5);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(180, 10);
            this.divider.TabIndex = 37;
            this.divider.TabStop = false;
            // 
            // flow
            // 
            this.flow.AutoScroll = true;
            this.flow.AutoSize = true;
            this.flow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flow.Location = new System.Drawing.Point(3, 16);
            this.flow.Name = "flow";
            this.flow.Size = new System.Drawing.Size(180, 0);
            this.flow.TabIndex = 36;
            this.flow.WrapContents = false;
            // 
            // CCreditsGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.label);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.flow);
            this.Name = "CCreditsGroup";
            this.Size = new System.Drawing.Size(200, 19);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.GroupBox divider;
        private System.Windows.Forms.FlowLayoutPanel flow;
    }
}
