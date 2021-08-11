using AATool.Data.Progress;
using AATool.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AATool.Data
{
    public class StatisticsManifest : IManifest
    {
        public Dictionary<string, Statistic> Items { get; private set; }

        //try to get values by reference and return success
        public bool TryGetItem(string id, out Statistic value) => Items.TryGetValue(id, out value);

        public StatisticsManifest()
        {
            this.Items = new ();
        }
        
        public void UpdateReference()
        {
            //clear list     
            this.ClearProgress();

            //load list of items to count for this version
            try
            {
                if (XmlObject.TryGetDocument(Paths.StatisticsFile, out XmlDocument document))
                {
                    foreach (XmlNode itemNode in document.SelectSingleNode("items").ChildNodes)
                        this.Items.Add(itemNode.Attributes["id"]?.Value, new Statistic(itemNode));
                }
            }
            catch (Exception e)
            {
                Main.QuitBecause("Error loading statistics manifest reference files!", e);
            }
        }

        public void Update(ProgressState progress)
        {
            foreach (Statistic item in this.Items.Values)
                item.Update(progress);
        }

        public void ClearProgress()
        {
            this.Items.Clear();
        }
    }
}
