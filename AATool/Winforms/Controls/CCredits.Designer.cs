
namespace AATool.Winforms.Controls
{
    partial class CCredits
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
            this.mainGroup = new System.Windows.Forms.GroupBox();
            this.patreon = new System.Windows.Forms.LinkLabel();
            this.flow = new System.Windows.Forms.FlowLayoutPanel();
            this.mainGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainGroup
            // 
            this.mainGroup.Controls.Add(this.flow);
            this.mainGroup.Controls.Add(this.patreon);
            this.mainGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainGroup.Location = new System.Drawing.Point(0, 0);
            this.mainGroup.Name = "mainGroup";
            this.mainGroup.Size = new System.Drawing.Size(226, 401);
            this.mainGroup.TabIndex = 25;
            this.mainGroup.TabStop = false;
            this.mainGroup.Text = "Credits";
            // 
            // patreon
            // 
            this.patreon.AutoSize = true;
            this.patreon.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.patreon.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.patreon.Location = new System.Drawing.Point(3, 370);
            this.patreon.Name = "patreon";
            this.patreon.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.patreon.Size = new System.Drawing.Size(216, 28);
            this.patreon.TabIndex = 27;
            this.patreon.TabStop = true;
            this.patreon.Tag = "https://www.patreon.com/_ctm";
            this.patreon.Text = "If you\'d like to see your name featured here, \r\nvisit the AATool Patreon!";
            this.patreon.Click += new System.EventHandler(this.OnClick);
            // 
            // flow
            // 
            this.flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flow.Location = new System.Drawing.Point(3, 16);
            this.flow.Name = "flow";
            this.flow.Size = new System.Drawing.Size(220, 354);
            this.flow.TabIndex = 28;
            // 
            // CCredits
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainGroup);
            this.Name = "CCredits";
            this.Size = new System.Drawing.Size(226, 401);
            this.mainGroup.ResumeLayout(false);
            this.mainGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox mainGroup;
        private System.Windows.Forms.FlowLayoutPanel flow;
        private System.Windows.Forms.LinkLabel patreon;
    }
}
