
namespace AATool.Winforms.Forms
{
    partial class FNotes
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FNotes));
            this.notes = new System.Windows.Forms.RichTextBox();
            this.saveTimer = new System.Windows.Forms.Timer(this.components);
            this.alwaysOnTop = new System.Windows.Forms.CheckBox();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClearNotesFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuClear = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notes
            // 
            this.notes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.notes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notes.Location = new System.Drawing.Point(0, 24);
            this.notes.Name = "notes";
            this.notes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.notes.Size = new System.Drawing.Size(404, 357);
            this.notes.TabIndex = 4;
            this.notes.Text = "";
            this.notes.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // saveTimer
            // 
            this.saveTimer.Interval = 1000;
            this.saveTimer.Tick += new System.EventHandler(this.OnTick);
            // 
            // alwaysOnTop
            // 
            this.alwaysOnTop.AutoSize = true;
            this.alwaysOnTop.BackColor = System.Drawing.SystemColors.MenuBar;
            this.alwaysOnTop.Location = new System.Drawing.Point(306, 4);
            this.alwaysOnTop.Name = "alwaysOnTop";
            this.alwaysOnTop.Size = new System.Drawing.Size(98, 17);
            this.alwaysOnTop.TabIndex = 0;
            this.alwaysOnTop.Text = "Always On Top";
            this.alwaysOnTop.UseVisualStyleBackColor = false;
            this.alwaysOnTop.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuClearNotesFolder});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = "File";
            // 
            // menuClearNotesFolder
            // 
            this.menuClearNotesFolder.Name = "menuClearNotesFolder";
            this.menuClearNotesFolder.Size = new System.Drawing.Size(157, 22);
            this.menuClearNotesFolder.Text = "Clear Old Notes";
            this.menuClearNotesFolder.Click += new System.EventHandler(this.OnClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCopy,
            this.menuPaste,
            this.menuCut,
            this.toolStripSeparator1,
            this.menuClear});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // menuCopy
            // 
            this.menuCopy.Name = "menuCopy";
            this.menuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuCopy.Size = new System.Drawing.Size(157, 22);
            this.menuCopy.Text = "Copy";
            this.menuCopy.Click += new System.EventHandler(this.OnClick);
            // 
            // menuPaste
            // 
            this.menuPaste.Name = "menuPaste";
            this.menuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.menuPaste.Size = new System.Drawing.Size(157, 22);
            this.menuPaste.Text = "Paste";
            this.menuPaste.Click += new System.EventHandler(this.OnClick);
            // 
            // menuCut
            // 
            this.menuCut.Name = "menuCut";
            this.menuCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.menuCut.Size = new System.Drawing.Size(157, 22);
            this.menuCut.Text = "Cut";
            this.menuCut.Click += new System.EventHandler(this.OnClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(154, 6);
            // 
            // menuClear
            // 
            this.menuClear.Name = "menuClear";
            this.menuClear.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Delete)));
            this.menuClear.Size = new System.Drawing.Size(157, 22);
            this.menuClear.Text = "Clear";
            this.menuClear.Click += new System.EventHandler(this.OnClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(404, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FNotes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 381);
            this.Controls.Add(this.notes);
            this.Controls.Add(this.alwaysOnTop);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(256, 256);
            this.Name = "FNotes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Notes";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            this.ResizeEnd += new System.EventHandler(this.OnResizeEnd);
            this.Resize += new System.EventHandler(this.OnResize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox notes;
        private System.Windows.Forms.Timer saveTimer;
        private System.Windows.Forms.CheckBox alwaysOnTop;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuClearNotesFolder;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuCopy;
        private System.Windows.Forms.ToolStripMenuItem menuPaste;
        private System.Windows.Forms.ToolStripMenuItem menuCut;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuClear;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}