
namespace AATool.Winforms.Forms
{
    partial class FAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FAbout));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.gitHub = new System.Windows.Forms.LinkLabel();
            this.patreon = new System.Windows.Forms.Button();
            this.youtube = new System.Windows.Forms.Button();
            this.twitch = new System.Windows.Forms.Button();
            this.discord = new System.Windows.Forms.Button();
            this.picture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(168, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(404, 42);
            this.label1.TabIndex = 0;
            this.label1.Text = "Hiya! My name is Darwin Baker. I go by the online alias \"CTM\" and I\'m a programme" +
    "r and speedrunner. I\'ve been playing minecraft and experimenting with mods/shade" +
    "rs for nearly 10 years.";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(168, 64);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(404, 81);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 303);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(563, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "A special thanks to the moderators on Speedrun.com - none of this would be possib" +
    "le without you guys. Thank you ♥";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(168, 155);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(404, 70);
            this.label4.TabIndex = 4;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(168, 235);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(404, 55);
            this.label5.TabIndex = 5;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 177);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Find me on these platforms!";
            // 
            // gitHub
            // 
            this.gitHub.BackColor = System.Drawing.SystemColors.Control;
            this.gitHub.Location = new System.Drawing.Point(300, 248);
            this.gitHub.Margin = new System.Windows.Forms.Padding(0);
            this.gitHub.Name = "gitHub";
            this.gitHub.Size = new System.Drawing.Size(40, 15);
            this.gitHub.TabIndex = 11;
            this.gitHub.TabStop = true;
            this.gitHub.Text = "GitHub";
            this.gitHub.Click += new System.EventHandler(this.OnClick);
            // 
            // patreon
            // 
            this.patreon.Image = global::AATool.Properties.Resources.patreon;
            this.patreon.Location = new System.Drawing.Point(41, 196);
            this.patreon.Name = "patreon";
            this.patreon.Size = new System.Drawing.Size(44, 44);
            this.patreon.TabIndex = 0;
            this.patreon.UseVisualStyleBackColor = true;
            this.patreon.Click += new System.EventHandler(this.OnClick);
            // 
            // youtube
            // 
            this.youtube.Image = global::AATool.Properties.Resources.youtube;
            this.youtube.Location = new System.Drawing.Point(91, 246);
            this.youtube.Name = "youtube";
            this.youtube.Size = new System.Drawing.Size(44, 44);
            this.youtube.TabIndex = 2;
            this.youtube.UseVisualStyleBackColor = true;
            this.youtube.Click += new System.EventHandler(this.OnClick);
            // 
            // twitch
            // 
            this.twitch.Image = global::AATool.Properties.Resources.twitch;
            this.twitch.Location = new System.Drawing.Point(41, 246);
            this.twitch.Name = "twitch";
            this.twitch.Size = new System.Drawing.Size(44, 44);
            this.twitch.TabIndex = 2;
            this.twitch.UseVisualStyleBackColor = true;
            this.twitch.Click += new System.EventHandler(this.OnClick);
            // 
            // discord
            // 
            this.discord.Image = global::AATool.Properties.Resources.discord;
            this.discord.Location = new System.Drawing.Point(91, 196);
            this.discord.Name = "discord";
            this.discord.Size = new System.Drawing.Size(44, 44);
            this.discord.TabIndex = 1;
            this.discord.UseVisualStyleBackColor = true;
            this.discord.Click += new System.EventHandler(this.OnClick);
            // 
            // picture
            // 
            this.picture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture.Image = global::AATool.Properties.Resources.darwin;
            this.picture.Location = new System.Drawing.Point(12, 12);
            this.picture.Name = "picture";
            this.picture.Size = new System.Drawing.Size(150, 150);
            this.picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picture.TabIndex = 1;
            this.picture.TabStop = false;
            // 
            // FAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 327);
            this.Controls.Add(this.gitHub);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.patreon);
            this.Controls.Add(this.youtube);
            this.Controls.Add(this.twitch);
            this.Controls.Add(this.discord);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.picture);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FAbout";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About This Tool";
            ((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picture;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button discord;
        private System.Windows.Forms.Button twitch;
        private System.Windows.Forms.Button youtube;
        private System.Windows.Forms.Button patreon;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel gitHub;
    }
}