using AATool.DataStructures;
using AATool.Settings;
using AATool.Utilities;
using System;

namespace AATool.Trackers
{
    public abstract class Tracker
    {
        public SaveJSON JSON            { get; protected set; }
        public string CurrentSaveName   { get; protected set; }

        protected DateTime lastModified;
        private Timer refreshTimer;

        public Tracker()
        {
            refreshTimer = new Timer(1.0 / 4);
            ParseReferences();
        }

        protected abstract void ParseReferences();
        protected abstract void ReadSave();
        public abstract bool VersionMismatch();
        
        public void Update(Time time)
        {
            bool versionHasChanged = TrackerSettings.Instance.ValueChanged(TrackerSettings.GAME_VERSION);

            //if user changes game version clear tracked data and load appropriate list for new version
            if (versionHasChanged)
            {
                ParseReferences();
                refreshTimer.Expire();
            }

            //skip updating if game version doesn't match version required for derived tracker
            if (VersionMismatch())
                return;

            refreshTimer.Update(time);
            if (refreshTimer.IsExpired)
            {
                //time to refresh or current save changed; read json again
                if (JSON.TryRead() || versionHasChanged)
                {
                    ReadSave();
                    CurrentSaveName = JSON.CurrentSaveName;
                }
                refreshTimer.Reset();
            }
        }
    }
}
