using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AATool.Data.Progress
{
    public struct Event
    {
        public string Label;
        public DateTime When;

        public Event(string label, DateTime when)
        {
            this.Label = label;
            this.When = when;
        }
    }

    class Timeline
    {
        const string Trident = "minecraft:trident";
        const string Debris = "minecraft:ancient_debris";
        const string Shell = "minecraft:nautillus_shell";
        const string Skull = "minecraft:wither_skeleton_skull";
        const string Egap = "minecraft:enchanted_golden_apple";

        public readonly List<Event> Events;
        private Dictionary<string, int> currentPickups;
        private string currentSaveName;

        private string CurrentTimelineFile => Path.Combine(Paths.System.DataFolder, $"{this.currentSaveName}.json");

        public Timeline()
        {
            this.Events = new ();
        }

        public void UpdateState(WorldState state)
        {
            this.UpdatePickups(state);
        }

        private void UpdatePickups(WorldState state)
        {
            foreach (KeyValuePair<string, int> latest in state.PickupTotals)
            {
                this.currentPickups.TryGetValue(latest.Key, out int current);
                int difference = current - latest.Value;
                if (difference > 0)
                {
                    string item = latest.Key switch {
                        Trident => "Trident",
                        Shell => "Shell",
                        Skull => "Skull",
                        Egap => "God Apple",
                        Debris => "Debris",
                        _ => latest.Key
                    };
                    if (difference > 1)
                        item += $" x{difference}";
                    this.Events.Add(new Event(item, DateTime.Now));
                }
            }
        }

        public void TryLoad()
        {
            /*
            try
            {
                this.events.Clear();
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
            */
        }

        public void TrySave()
        {
            /*
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
            */
        }
    }
}
