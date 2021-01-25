using AATool.DataStructures;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AATool.Trackers
{
    public class AdvancementTracker : Tracker
    {
        public Dictionary<string, Advancement> AdvancementList  { get; private set; }
        public Dictionary<string, AdvancementGroup> GroupList   { get; private set; }

        public int AdvancementCount                             { get; private set; }
        public int CompletedCount                               { get; private set; }
        public int CompletedPercent                             { get; private set; }

        public Advancement Advancement(string id) => AdvancementList.TryGetValue(id, out var val) ? val : null;
        public AdvancementGroup Group(string id)  => GroupList.TryGetValue(id, out var val) ? val : null;

        public AdvancementTracker() : base()
        {
            JSON = new AdvancementsJSON(); 
        }

        protected override void ParseReferences()
        {
            AdvancementList = new Dictionary<string, Advancement>();
            GroupList = new Dictionary<string, AdvancementGroup>();

            //load list of advancements to check for
            foreach (string file in Directory.EnumerateFiles(Paths.DIR_ADVANCEMENTS, "*.xml", SearchOption.AllDirectories))
            {
                var document = new XmlDocument();
                using (var stream = File.OpenRead(file))
                {
                    document.Load(stream);
                    var group = new AdvancementGroup(document);
                    GroupList[Path.GetFileNameWithoutExtension(file)] = group;
                    foreach (var advancement in group.Advancements)
                        AdvancementList[advancement.Key] = advancement.Value;
                }
            }
            AdvancementCount = AdvancementList.Count;
        }

        protected override void ReadSave()
        {
            //update group/advancement states and count completed
            CompletedCount = 0;
            foreach (var group in GroupList.Values)
            {
                group.Update(JSON as AdvancementsJSON);
                CompletedCount += group.CompletedCount;
            }
            CompletedPercent = (int)((double)CompletedCount / AdvancementList.Count * 100);
        }
    }
}
