using AATool.DataStructures;
using AATool.Settings;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AATool.Trackers
{
    public class AchievementTracker : Tracker
    {
        public Dictionary<string, Achievement> FullAchievementList  { get; private set; }
        public Dictionary<string, Criterion> FullCriteriaList       { get; private set; }
        public Achievement RootAchievement                          { get; private set; }
        public int AchievementCount                                 { get; private set; }
        public int CompletedCount                                   { get; private set; }
        public int CompletedPercent                                 { get; private set; }

        public Achievement Achievement(string id) => FullAchievementList.TryGetValue(id, out var val) ? val : null;
        public override bool VersionMismatch()    => TrackerSettings.IsPostExplorationUpdate;

        public AchievementTracker() : base()
        {
            JSON = new AchievementJSON();
        }

        protected override void ParseReferences()
        {
            FullAchievementList = new Dictionary<string, Achievement>();
            FullCriteriaList    = new Dictionary<string, Criterion>();
            //skip loading if this version is after achievements were changed to advancements
            if (VersionMismatch())
                return;

            //load list of advancements to track
            try
            {
                var document = new XmlDocument();
                using (var stream = File.OpenRead(Paths.AchievementsFile))
                {
                    document.Load(stream);

                    //recursively instantiate achievement tree
                    RootAchievement = new Achievement(document.SelectSingleNode("achievements/root"), null);
                    RootAchievement.GetAllChildrenRecursive(FullAchievementList);
                    AchievementCount = FullAchievementList.Count;

                    //add sub-criteria
                    foreach (var achievement in FullAchievementList)
                        foreach (var criterion in achievement.Value.Criteria)
                            FullCriteriaList[criterion.Key] = criterion.Value;
                }
            }
            catch { Main.ForceQuit(this); }
        }

        protected override void ReadSave()
        {
            //update group/advancement states and completion count
            CompletedCount = 0;
            foreach (var achievement in FullAchievementList.Values)
            {
                achievement.Update(JSON as AchievementJSON);
                if (achievement.IsCompleted)
                    CompletedCount++;
            }
            CompletedPercent = (int)((double)CompletedCount / FullAchievementList.Count * 100);
        }
    }
}
