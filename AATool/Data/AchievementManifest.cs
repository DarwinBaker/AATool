using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using AATool.Data.Progress;
using AATool.Utilities;

namespace AATool.Data
{
    public class AchievementManifest : IManifest
    {
        public Achievement RootAdvancement                                  { get; private set; }
        public Dictionary<string, Advancement> AllAdvancements              { get; private set; }
        public Dictionary<(string adv, string crit), Criterion> AllCriteria { get; private set; }
        public int Completed                                                { get; private set; }
        public int Percent                                                  { get; private set; }

        public int Count => this.AllAdvancements.Count;

        public AchievementManifest()
        {
            this.AllAdvancements = new ();
            this.AllCriteria     = new ();
        }

        public void UpdateReference()
        {
            //clear lists
            this.ClearProgress();

            //load lists of advancements to track for this version
            try
            {
                if (!XmlObject.TryGetDocument(Paths.AchievementsFile, out XmlDocument document))
                    throw new IOException();

                //recursively build achievement tree
                XmlNode rootNode = document.DocumentElement.SelectSingleNode("root");
                this.RootAdvancement = new Achievement(rootNode);
                this.RootAdvancement.GetAllChildrenRecursive(this.AllAdvancements);

                //add sub-criteria
                foreach (Advancement advancement in this.AllAdvancements.Values)
                {
                    if (!advancement.TryGetCriteria(out CriteriaSet criteria))
                        continue;

                    foreach (KeyValuePair<string, Criterion> criterion in criteria.All)
                        this.AllCriteria[(advancement.Id, criterion.Key)] = criterion.Value;
                }
            }
            catch (Exception e)
            { 
                Main.QuitBecause("Error loading achievement manifest reference files!", e); 
            }
        }

        public void Update(ProgressState progress)
        {
            this.Completed = 0;
            foreach (Achievement advancement in this.AllAdvancements.Values)
            { 
                advancement.Update(progress);
                if (advancement.CompletedByAnyone())
                    this.Completed++;
            }      
        }

        public void ClearProgress()
        {
            this.AllAdvancements.Clear();
            this.AllCriteria.Clear();
        }
    }
}
