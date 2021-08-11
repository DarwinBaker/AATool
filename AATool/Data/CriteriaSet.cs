using AATool.Data.Progress;
using AATool.Net;
using AATool.UI.Controls;
using AATool.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace AATool.Data
{
    public class CriteriaSet
    {
        public Advancement ParentAdvancement            { get; private set; }
        public Dictionary<string, Criterion> All        { get; private set; }
        public Dictionary<Uuid, int> CompletedByPlayer  { get; private set; }
        public Uuid ClosestToCompletion                 { get; private set; }
        public string Goal                              { get; private set; }

        public int Count => this.All.Count;

        public bool Contains(string criterion) => 
            this.All.ContainsKey(criterion);

        public int NumberCompletedBy(Uuid id) => 
            this.CompletedByPlayer.TryGetValue(id, out int i) ? i : 0;

        public int PercentCompletedBy(Uuid id) =>
            (int)((double)this.NumberCompletedBy(id) / this.Count * 100);

        public CriteriaSet(XmlNode node, Advancement advancement)
        {
            this.All                 = new ();
            this.CompletedByPlayer   = new ();
            this.ParentAdvancement   = advancement;
            this.ClosestToCompletion = Uuid.Empty;

            this.Goal = XmlObject.ParseAttribute(node, "goal", "Completed");
            foreach (XmlNode criterionNode in node.ChildNodes)
            {
                var criterion = new Criterion(criterionNode, advancement);
                this.All.Add(criterion.ID, criterion);
            }
        }

        public void Update(ProgressState progress)
        {
            //update all criteria in this group and count them
            this.CompletedByPlayer.Clear();
            foreach (Criterion criterion in this.All.Values)
            {
                criterion.Update(progress);
                foreach (Uuid id in criterion.Completionists)
                {
                    this.CompletedByPlayer.TryGetValue(id, out int current);
                    this.CompletedByPlayer[id] = current + 1;
                }
            }

            //figure out which player is closest to completing all criteria
            KeyValuePair<Uuid, int> closest = new (Uuid.Empty, 0);
            foreach (KeyValuePair<Uuid, int> player in this.CompletedByPlayer)
            {
                if (player.Value >= closest.Value)
                    closest = player;
            }

            if (closest.Key == Uuid.Empty && progress.Players.Any())
            {
                //nobody has completed any criteria, just pick someone
                closest = new (progress.Players.First().Key, 0);
            }
            this.ClosestToCompletion = closest.Key;
        }

        public void CopyCriteriaTo(Dictionary<(string, string), Criterion> dictionary)
        {
            //copy criteria to passed list
            foreach (KeyValuePair<string, Criterion> criterion in this.All)
                dictionary[(this.ParentAdvancement.Id, criterion.Key)] = criterion.Value;
        }
    }
}
