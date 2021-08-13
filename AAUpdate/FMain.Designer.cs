
namespace AAUpdate
{
    partial class FMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            this.background = new System.ComponentModel.BackgroundWorker();
            this.statusGeneric = new System.Windows.Forms.Label();
            this.statusSpecific = new System.Windows.Forms.Label();
            this.statusFile = new System.Windows.Forms.Label();
            this.labelCurrent = new System.Windows.Forms.Label();
            this.labelProgress = new System.Windows.Forms.Label();
            this.percentCurrent = new System.Windows.Forms.Label();
            this.percentOverall = new System.Windows.Forms.Label();
            this.totalCurrent = new System.Windows.Forms.Label();
            this.totalOverall = new System.Windows.Forms.Label();
            this.barOverall = new AAUpdate.MinecraftBar();
            this.barCurrent = new AAUpdate.MinecraftBar();
            this.SuspendLayout();
            // 
            // background
            // 
            this.background.WorkerReportsProgress = true;
            this.background.WorkerSupportsCancellation = true;
            this.background.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OnDoWork);
            this.background.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.OnReportProgress);
            this.background.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OnRunWorkerCompleted);
            // 
            // statusGeneric
            // 
            this.statusGeneric.Location = new System.Drawing.Point(12, 28);
            this.statusGeneric.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.statusGeneric.Name = "statusGeneric";
            this.statusGeneric.Size = new System.Drawing.Size(403, 13);
            this.statusGeneric.TabIndex = 1;
            this.statusGeneric.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // statusSpecific
            // 
            this.statusSpecific.Location = new System.Drawing.Point(12, 57);
            this.statusSpecific.Margin = new System.Windows.Forms.Padding(3, 16, 3, 0);
            this.statusSpecific.Name = "statusSpecific";
            this.statusSpecific.Size = new System.Drawing.Size(403, 13);
            this.statusSpecific.TabIndex = 2;
            this.statusSpecific.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // statusFile
            // 
            this.statusFile.Location = new System.Drawing.Point(12, 78);
            this.statusFile.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.statusFile.Name = "statusFile";
            this.statusFile.Size = new System.Drawing.Size(403, 13);
            this.statusFile.TabIndex = 3;
            this.statusFile.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelCurrent
            // 
            this.labelCurrent.Location = new System.Drawing.Point(12, 95);
            this.labelCurrent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this.labelCurrent.Name = "labelCurrent";
            this.labelCurrent.Size = new System.Drawing.Size(100, 13);
            this.labelCurrent.TabIndex = 5;
            this.labelCurrent.Text = "Current Task:";
            // 
            // labelProgress
            // 
            this.labelProgress.Location = new System.Drawing.Point(12, 141);
            this.labelProgress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(100, 13);
            this.labelProgress.TabIndex = 6;
            this.labelProgress.Text = "Overall Progress:";
            // 
            // percentCurrent
            // 
            this.percentCurrent.Location = new System.Drawing.Point(315, 95);
            this.percentCurrent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this.percentCurrent.Name = "percentCurrent";
            this.percentCurrent.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.percentCurrent.Size = new System.Drawing.Size(100, 13);
            this.percentCurrent.TabIndex = 7;
            this.percentCurrent.Text = "0%";
            // 
            // percentOverall
            // 
            this.percentOverall.Location = new System.Drawing.Point(315, 141);
            this.percentOverall.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this.percentOverall.Name = "percentOverall";
            this.percentOverall.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.percentOverall.Size = new System.Drawing.Size(100, 13);
            this.percentOverall.TabIndex = 8;
            this.percentOverall.Text = "0%";
            // 
            // totalCurrent
            // 
            this.totalCurrent.Location = new System.Drawing.Point(118, 95);
            this.totalCurrent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this.totalCurrent.Name = "totalCurrent";
            this.totalCurrent.Size = new System.Drawing.Size(191, 13);
            this.totalCurrent.TabIndex = 9;
            this.totalCurrent.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // totalOverall
            // 
            this.totalOverall.Location = new System.Drawing.Point(118, 141);
            this.totalOverall.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this.totalOverall.Name = "totalOverall";
            this.totalOverall.Size = new System.Drawing.Size(191, 13);
            this.totalOverall.TabIndex = 10;
            this.totalOverall.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // barOverall
            // 
            this.barOverall.Location = new System.Drawing.Point(12, 157);
            this.barOverall.Name = "barOverall";
            this.barOverall.Size = new System.Drawing.Size(400, 10);
            this.barOverall.TabIndex = 12;
            // 
            // barCurrent
            // 
            this.barCurrent.Location = new System.Drawing.Point(12, 111);
            this.barCurrent.Name = "barCurrent";
            this.barCurrent.Size = new System.Drawing.Size(400, 10);
            this.barCurrent.TabIndex = 11;
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 181);
            this.Controls.Add(this.barOverall);
            this.Controls.Add(this.barCurrent);
            this.Controls.Add(this.totalOverall);
            this.Controls.Add(this.totalCurrent);
            this.Controls.Add(this.percentOverall);
            this.Controls.Add(this.percentCurrent);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.labelCurrent);
            this.Controls.Add(this.statusFile);
            this.Controls.Add(this.statusSpecific);
            this.Controls.Add(this.statusGeneric);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FMain";
            this.Padding = new System.Windows.Forms.Padding(12, 16, 12, 12);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AATool Update Assistant";
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker background;
        private System.Windows.Forms.Label statusGeneric;
        private System.Windows.Forms.Label statusSpecific;
        private System.Windows.Forms.Label statusFile;
        private System.Windows.Forms.Label labelCurrent;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Label percentCurrent;
        private System.Windows.Forms.Label percentOverall;
        private System.Windows.Forms.Label totalCurrent;
        private System.Windows.Forms.Label totalOverall;
        private MinecraftBar barCurrent;
        private MinecraftBar barOverall;
    }
}

