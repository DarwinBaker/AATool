using AATool.Data.Progress;
using AATool.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AATool.Data
{
    public class AdvancementManifest : IManifest
    {
        public Dictionary<string, Advancement> AllAdvancements              { get; private set; }
        public Dictionary<(string adv, string crit), Criterion> AllCriteria { get; private set; }
        public Dictionary<string, AdvancementGroup> AllGroups               { get; private set; }
        public int Completed                                                { get; private set; }
        
        public int AdvancementCount => this.AllAdvancements.Count;

        public bool TryGetGroup(string id, out AdvancementGroup value)  => this.AllGroups.TryGetValue(id, out value);

        public AdvancementManifest()
        {
            this.AllAdvancements = new ();
            this.AllCriteria     = new ();
            this.AllGroups       = new ();
        }

        public void UpdateReference()
        {
            //clear lists
            this.ClearProgress();

            //load lists of advancements to track for this version
            try
            {
                //iterate advancement reference files for current game version
                foreach (string file in Paths.AdvancementFiles)
                {
                    if (XmlObject.TryGetDocument(file, out XmlDocument document))
                    {
                        //add advancement group
                        var group = new AdvancementGroup(document);
                        this.AllGroups[Path.GetFileNameWithoutExtension(file)] = group;
                        group.CopyTo(this.AllAdvancements);

                        //add each sub-criterion in this advancement to the full list
                        foreach (Advancement advancement in group.Advancements.Values)
                            advancement.Criteria?.CopyCriteriaTo(this.AllCriteria);
                    }
                }
            }
            catch (Exception e)
            {
                Main.QuitBecause("Error loading advancement manifest reference files!", e); 
            }
        }

        public void Update(ProgressState progress)
        {
            this.Completed = 0;
            foreach (Advancement advancement in this.AllAdvancements.Values)
            {   
                //update advancement and increment counter if completed
                advancement.Update(progress);
                if (advancement.CompletedByAnyone())
                    this.Completed++;
            }  
        }

        public void ClearProgress()
        {
            this.AllAdvancements.Clear();
            this.AllCriteria.Clear();
            this.AllGroups.Clear();
        }
    }
}
