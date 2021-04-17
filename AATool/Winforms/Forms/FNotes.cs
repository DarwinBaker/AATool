using AATool.Settings;
using System;
using System.IO;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FNotes : Form
    {
        private string lastSavedNotes;
        private string currentSaveName;

        private string CurrentNotesFile => Path.Combine(Paths.DIR_NOTES, $"{currentSaveName}.txt");

        public FNotes()
        {
            InitializeComponent();
            TopMost = NotesSettings.Instance.AlwaysOnTop;
            alwaysOnTop.Checked = NotesSettings.Instance.AlwaysOnTop;
            LoadNotes();
        }

        public void UpdateCurrentSave(string newSaveName)
        {
            if (newSaveName != currentSaveName)
            {
                if (!string.IsNullOrEmpty(newSaveName))
                {
                    TrySaveNotes();
                    currentSaveName = newSaveName;
                    LoadNotes();
                    Text = $"Notes - {newSaveName}";
                }
                else
                    Text = "Notes - Not Currently Reading a Save";
            }
        }

        public void LoadNotes()
        {
            try
            {
                notes.Clear();
                if (File.Exists(CurrentNotesFile))
                    using (var stream = new StreamReader(CurrentNotesFile))
                        notes.Text = stream.ReadToEnd();
                lastSavedNotes = notes.Text;
            }
            catch (Exception) 
            {
                //delete corrupt note
                File.Delete(CurrentNotesFile);
            }
        }

        public void TrySaveNotes()
        {
            if (lastSavedNotes != notes.Text)
            {
                try
                {
                    Directory.CreateDirectory(Paths.DIR_NOTES);
                    using (var stream = new StreamWriter(CurrentNotesFile))
                        stream.Write(notes.Text);
                    lastSavedNotes = notes.Text;
                }
                catch (Exception) { }
            }
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if (sender == alwaysOnTop)
            {
                TopMost = alwaysOnTop.Checked;
                NotesSettings.Instance.AlwaysOnTop = alwaysOnTop.Checked;
                NotesSettings.Instance.Save();
            }
        }

        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            NotesSettings.Instance.Enabled = false;
            NotesSettings.Instance.Save();
        }

        private void OnTick(object sender, EventArgs e)
        {
            TrySaveNotes();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            saveTimer.Stop();
            saveTimer.Start();
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (sender == menuClearNotesFolder)
            {
                if (MessageBox.Show(this, "This will permanently delete all of your previous notes (the current one is preserved). Are you sure?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    Directory.Delete(Paths.DIR_NOTES, true);
                lastSavedNotes = null;
                TrySaveNotes();
            }
            else if (sender == menuClear)
            { 
                if (MessageBox.Show(this, "Clear the current note?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    notes.Clear();
            }
            else if (sender == menuCopy)
                notes.Copy();
            else if (sender == menuPaste)
                notes.Paste();
            else if (sender == menuCut)
                notes.Cut();
        }

        private void OnResize(object sender, EventArgs e)
        {
            alwaysOnTop.Left = ClientSize.Width - alwaysOnTop.Width;
        }
    }
}
