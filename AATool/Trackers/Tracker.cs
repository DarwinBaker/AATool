using AATool.DataStructures;
using AATool.Settings;
using AATool.Utilities;

namespace AATool.Trackers
{
    public abstract class Tracker
    {
        public SaveJSON JSON            { get; protected set; }
        public string CurrentSaveName   { get; protected set; }

        private Timer refreshTimer;

        public Tracker()
        {
            refreshTimer = new Timer(TrackerSettings.Instance.RefreshInterval / 1000.0);
            ParseReferences();
        }

        protected abstract void ParseReferences();
        protected abstract void ReadSave();
        public abstract bool VersionMismatch();
        
        public void Update(Time time)
        {
            //if user changes game version clear tracked data and load appropriate list for new version
            if (TrackerSettings.Instance.ValueChanged(TrackerSettings.GAME_VERSION))
            {
                ParseReferences();
                refreshTimer.Expire();
            }

            //skip updating if game version doesn't match version required for derived tracker
            if (VersionMismatch())
                return;

            //if user changes refresh rate create a new timer with the new refresh rate
            double targetRefresh = TrackerSettings.Instance.RefreshInterval / 1000.0;
            if (refreshTimer.Duration != targetRefresh)
                refreshTimer = new Timer(targetRefresh);
            refreshTimer.Update(time);

            JSON.Update();
            if (refreshTimer.IsExpired || CurrentSaveName != JSON.CurrentSaveName)
            {
                //time to refresh or current save changed; read json again
                JSON.Read();
                refreshTimer.Reset();
                ReadSave();
                CurrentSaveName = JSON.CurrentSaveName;
            }
        }
    }
}
