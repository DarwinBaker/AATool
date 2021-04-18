using AATool.DataStructures;
using AATool.Settings;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AATool.Trackers
{
    public class AdvancementTracker : Tracker
    {
        public Dictionary<string, Advancement> FullAdvancementList  { get; private set; }
        public Dictionary<string, Criterion> FullCriteriaList       { get; private set; }
        public Dictionary<string, AdvancementGroup> GroupList       { get; private set; }
        public int AdvancementCount                                 { get; private set; }
        public int CompletedCount                                   { get; private set; }
        public int CompletedPercent                                 { get; private set; }

        public Advancement Advancement(string id) => FullAdvancementList.TryGetValue(id, out var val) ? val : null;
        public AdvancementGroup Group(string id)  => GroupList.TryGetValue(id, out var val) ? val : null;
        public override bool VersionMismatch()    => TrackerSettings.IsPreExplorationUpdate;

        public AdvancementTracker() : base()
        {
            JSON = new AdvancementsJSON();
        }

        protected override void ParseReferences()
        {
            FullAdvancementList = new Dictionary<string, Advancement>();
            FullCriteriaList    = new Dictionary<string, Criterion>();
            GroupList           = new Dictionary<string, AdvancementGroup>();

            //skip loading if this version is before achievements were changed to advancements
            if (VersionMismatch())
                return;

            //load list of advancements to track
            try
            {
                foreach (string file in Paths.AdvancementFiles)
                {
                    var document = new XmlDocument();
                    using (var stream = File.OpenRead(file))
                    {
                        document.Load(stream);
                        var group = new AdvancementGroup(document);
                        GroupList[Path.GetFileNameWithoutExtension(file)] = group;

                        //iterate groups for individual advancements
                        foreach (var advancement in group.Advancements)
                        {
                            FullAdvancementList[advancement.Key] = advancement.Value;

                            //add sub-criteria
                            foreach (var criterion in advancement.Value.Criteria)
                                FullCriteriaList[criterion.Key] = criterion.Value;
                        }
                    }
                }
                AdvancementCount = FullAdvancementList.Count;
            }
            catch { Main.ForceQuit(this); }
        }

        protected override void ReadSave()
        {
            //update group/advancement states and completion count
            CompletedCount = 0;
            foreach (var group in GroupList.Values)
            {
                group.Update(JSON as AdvancementsJSON);
                CompletedCount += group.CompletedCount;
            }
            CompletedPercent = (int)((double)CompletedCount / FullAdvancementList.Count * 100);
        }
    }
}
