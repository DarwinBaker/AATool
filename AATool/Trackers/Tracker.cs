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
            refreshTimer = new Timer(TrackerSettings.Instance.RefreshInterval);
            ParseReferences();
        }

        protected abstract void ParseReferences();
        protected abstract void ReadSave();

        public void Update(Time time)
        {
            //update save file check refresh rate
            double targetRefresh = TrackerSettings.Instance.RefreshInterval / 1000.0;
            if (refreshTimer.Duration != targetRefresh)
                refreshTimer = new Timer(targetRefresh);
            refreshTimer.Update(time);

            JSON.Update();
            if (refreshTimer.IsExpired || CurrentSaveName != JSON.CurrentSaveName)
            {
                JSON.Read();
                refreshTimer.Reset();
                ReadSave();
                CurrentSaveName = JSON.CurrentSaveName;
            }
        }

        
    }
}
