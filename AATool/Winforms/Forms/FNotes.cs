using AATool.Configuration;
using System;
using System.IO;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FNotes : Form
    {
        private string lastSavedNotes;
        private string currentSaveName;

        private string CurrentNotesFile => Path.Combine(Paths.System.NotesFolder, $"{this.currentSaveName}.txt");

        public FNotes()
        {
            this.InitializeComponent();
            this.TopMost = Config.Notes.AlwaysOnTop;
            this.alwaysOnTop.Checked = Config.Notes.AlwaysOnTop;
            this.LoadNotes();
        }

        public void UpdateCurrentSave(string newSaveName)
        {
            if (newSaveName != this.currentSaveName)
            {
                if (!string.IsNullOrEmpty(newSaveName))
                {
                    this.TrySaveNotes();
                    this.currentSaveName = newSaveName;
                    this.LoadNotes();
                    this.Text = $"Notes - {newSaveName}";
                }
                else
                {
                    this.Text = "Notes - Not Currently Reading a Save";
                }
            }
        }

        public void LoadNotes()
        {
            try
            {
                this.notes.Clear();
                if (File.Exists(this.CurrentNotesFile))
                {
                    using (var stream = new StreamReader(this.CurrentNotesFile))
                        this.notes.Text = stream.ReadToEnd();
                }
                this.lastSavedNotes = this.notes.Text;
            }
            catch (Exception) 
            {
                //delete corrupt note
                File.Delete(this.CurrentNotesFile);
            }
        }

        public void TrySaveNotes()
        {
            if (this.lastSavedNotes != this.notes.Text)
            {
                try
                {
                    Directory.CreateDirectory(Paths.System.NotesFolder);
                    using (var stream = new StreamWriter(this.CurrentNotesFile))
                        stream.Write(this.notes.Text);
                    this.lastSavedNotes = this.notes.Text;
                }
                catch { }
            }
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if (sender == this.alwaysOnTop)
            {
                this.TopMost = this.alwaysOnTop.Checked;
                Config.Notes.AlwaysOnTop.Set(this.alwaysOnTop.Checked);
                Config.Notes.Save();
            }
        }

        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            Config.Notes.Enabled.Set(false);
            Config.Notes.Save();
        }

        private void OnTick(object sender, EventArgs e)
        {
            this.TrySaveNotes();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            this.saveTimer.Stop();
            this.saveTimer.Start();
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (sender == this.menuClearNotesFolder)
            {
                if (MessageBox.Show(this, "This will permanently delete all of your previous notes (the current one is preserved). Are you sure?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    try
                    {
                        Directory.Delete(Paths.System.NotesFolder, true);
                    }
                    catch (Exception) { }
                }
                this.lastSavedNotes = null;
                this.TrySaveNotes();
            }
            else if (sender == this.menuClear)
            { 
                if (MessageBox.Show(this, "Clear the current note?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    this.notes.Clear();
            }
            else if (sender == this.menuCopy)
            {
                this.notes.Copy();
            }
            else if (sender == this.menuPaste)
            {
                this.notes.Paste();
            }
            else if (sender == this.menuCut)
            {
                this.notes.Cut();
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            this.alwaysOnTop.Left = this.ClientSize.Width - this.alwaysOnTop.Width;
        }
    }
}
