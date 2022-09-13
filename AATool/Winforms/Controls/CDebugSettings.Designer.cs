
namespace AATool.Winforms.Controls
{
    partial class CDebugSettings
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
            this.cacheDebug = new System.Windows.Forms.CheckBox();
            this.dumpAtlas = new System.Windows.Forms.Button();
            this.layoutDebug = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.hideRenderCache = new System.Windows.Forms.CheckBox();
            this.fastForward = new System.Windows.Forms.Button();
            this.refreshLeaderboards = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cacheDebug
            // 
            this.cacheDebug.AutoSize = true;
            this.cacheDebug.Location = new System.Drawing.Point(12, 58);
            this.cacheDebug.Name = "cacheDebug";
            this.cacheDebug.Size = new System.Drawing.Size(168, 17);
            this.cacheDebug.TabIndex = 11;
            this.cacheDebug.Text = "Show Render Cache Updates";
            this.cacheDebug.UseVisualStyleBackColor = true;
            this.cacheDebug.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // dumpAtlas
            // 
            this.dumpAtlas.Location = new System.Drawing.Point(12, 258);
            this.dumpAtlas.Name = "dumpAtlas";
            this.dumpAtlas.Size = new System.Drawing.Size(122, 23);
            this.dumpAtlas.TabIndex = 10;
            this.dumpAtlas.Text = "Dump Texture Atlas";
            this.dumpAtlas.UseVisualStyleBackColor = true;
            this.dumpAtlas.Click += new System.EventHandler(this.OnClicked);
            // 
            // layoutDebug
            // 
            this.layoutDebug.AutoSize = true;
            this.layoutDebug.Location = new System.Drawing.Point(12, 12);
            this.layoutDebug.Name = "layoutDebug";
            this.layoutDebug.Size = new System.Drawing.Size(119, 17);
            this.layoutDebug.TabIndex = 9;
            this.layoutDebug.Text = "Layout Debug View";
            this.layoutDebug.UseVisualStyleBackColor = true;
            this.layoutDebug.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label6.Location = new System.Drawing.Point(9, 32);
            this.label6.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(276, 13);
            this.label6.TabIndex = 36;
            this.label6.Text = "🛈 View wireframe-style bounds of user interface elements";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label1.Location = new System.Drawing.Point(9, 78);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(315, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "🛈 Flash the screen whenever the render cache must be re-drawn";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label2.Location = new System.Drawing.Point(9, 284);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(435, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "🛈 Save the texture atlas to an image file \"atlas_dump.png\" in the current workin" +
    "g directory ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label3.Location = new System.Drawing.Point(9, 124);
            this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(250, 13);
            this.label3.TabIndex = 40;
            this.label3.Text = "🛈 Show only the parts of the screen being re-drawn";
            // 
            // hideRenderCache
            // 
            this.hideRenderCache.AutoSize = true;
            this.hideRenderCache.Location = new System.Drawing.Point(12, 104);
            this.hideRenderCache.Name = "hideRenderCache";
            this.hideRenderCache.Size = new System.Drawing.Size(120, 17);
            this.hideRenderCache.TabIndex = 39;
            this.hideRenderCache.Text = "Hide Render Cache";
            this.hideRenderCache.UseVisualStyleBackColor = true;
            this.hideRenderCache.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // fastForward
            // 
            this.fastForward.Location = new System.Drawing.Point(12, 220);
            this.fastForward.Name = "fastForward";
            this.fastForward.Size = new System.Drawing.Size(122, 23);
            this.fastForward.TabIndex = 41;
            this.fastForward.Text = "Fast Forward Overlay";
            this.fastForward.UseVisualStyleBackColor = true;
            this.fastForward.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.fastForward.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.fastForward.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            // 
            // refreshLeaderboards
            // 
            this.refreshLeaderboards.Location = new System.Drawing.Point(12, 182);
            this.refreshLeaderboards.Name = "refreshLeaderboards";
            this.refreshLeaderboards.Size = new System.Drawing.Size(122, 23);
            this.refreshLeaderboards.TabIndex = 42;
            this.refreshLeaderboards.Text = "Refresh Leaderboards";
            this.refreshLeaderboards.UseVisualStyleBackColor = true;
            this.refreshLeaderboards.Click += new System.EventHandler(this.OnClicked);
            // 
            // CDebugSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.refreshLeaderboards);
            this.Controls.Add(this.fastForward);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.hideRenderCache);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cacheDebug);
            this.Controls.Add(this.dumpAtlas);
            this.Controls.Add(this.layoutDebug);
            this.Name = "CDebugSettings";
            this.Size = new System.Drawing.Size(538, 307);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox layoutDebug;
        private System.Windows.Forms.Button dumpAtlas;
        private System.Windows.Forms.CheckBox cacheDebug;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox hideRenderCache;
        private System.Windows.Forms.Button fastForward;
        private System.Windows.Forms.Button refreshLeaderboards;
    }
}
