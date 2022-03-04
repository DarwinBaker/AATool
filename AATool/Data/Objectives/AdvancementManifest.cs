using System.Collections.Generic;
using System.IO;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public class AdvancementManifest : IManifest
    {
        public readonly Dictionary<string, Advancement> All = new ();
        public readonly Dictionary<string, HashSet<Advancement>> Groups = new ();
        public readonly Dictionary<(string adv, string crit), Criterion> Criteria = new ();

        public int CompletedCount { get; private set; }

        public int Count => this.All.Count;

        public AdvancementManifest()
        {
            this.All = new Dictionary<string, Advancement>();
            this.Groups = new Dictionary<string, HashSet<Advancement>>();
            this.Criteria = new Dictionary<(string adv, string crit), Criterion>();
        }

        public bool TryGet(string advId, out Advancement advancement) =>
            this.All.TryGetValue(advId, out advancement);

        public bool TryGet(string advId, string critId, out Criterion criterion) =>
            this.Criteria.TryGetValue((advId, critId), out criterion);

        public bool TryGet(string groupId, out HashSet<Advancement> group) =>
            this.Groups.TryGetValue(groupId, out group);

        public void ClearObjectives()
        {
            this.Groups.Clear();
            this.All.Clear();
            this.Criteria.Clear();
            this.CompletedCount = 0;
        }

        public virtual void RefreshObjectives()
        {
            this.ClearObjectives();

            if (Tracker.Category is AllAchievements)
                return;

            //try to get list of all advancement objective files
            bool filesExist = Paths.TryGetAllFiles(Paths.System.AdvancementsFolder, "*.xml",
                SearchOption.TopDirectoryOnly, out IEnumerable<string> files);

            if (filesExist)
            {
                //iterate advancement objective files for current game version
                foreach (string file in files)
                    this.ParseFile(file);
            }
        }

        private void ParseFile(string file)
        {
            if (XmlObject.TryGetDocument(file, out XmlDocument document))
            {
                //add advancement group
                var group = new HashSet<Advancement>();
                foreach (XmlNode node in document.DocumentElement?.ChildNodes)
                    this.RequireAdvancement(node, group);

                string id = Path.GetFileNameWithoutExtension(file);
                this.Groups[id] = group;
            }
        }

        private void RequireAdvancement(XmlNode node, HashSet<Advancement> group)
        {
            var advancement = new Advancement(node);
            this.All[advancement.Id] = advancement;
            group.Add(advancement);

            if (advancement.HasCriteria)
            {
                foreach (KeyValuePair<string, Criterion> criterion in advancement.Criteria.All)
                    this.Criteria[(advancement.Id, criterion.Key)] = criterion.Value;
            }
        }

        public void SetState(WorldState progress)
        {
            this.CompletedCount = 0;
            foreach (Advancement advancement in this.All.Values)
            {
                //update advancement and completion count
                advancement.UpdateState(progress);
                if (advancement.CompletedByAnyone())
                    this.CompletedCount++;
            }
        }
    }
}
