using System.Collections.Generic;
using System.Xml;
using AATool.Data.Categories;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public class AchievementManifest : AdvancementManifest
    {
        public Achievement Root { get; private set; }

        public override void RefreshObjectives()
        {
            this.ClearObjectives();

            if (Tracker.Category is not AllAchievements)
                return;

            //load lists of achievements to track for this version
            if (XmlObject.TryGetDocument(Paths.System.AchievementsFile, out XmlDocument document))
            {
                //recursively build achievement tree
                this.Root = new Achievement(document.DocumentElement);
                this.Root.GetAllChildrenRecursive(this.AllAdvancements);

                //add sub-criteria
                foreach (Advancement advancement in this.AllAdvancements.Values)
                {
                    if (!advancement.HasCriteria)
                        continue;

                    foreach (KeyValuePair<string, Criterion> criterion in advancement.Criteria.All)
                        this.AllCriteria[(advancement.Id, criterion.Key)] = criterion.Value;
                }
            }
        }
    }
}
