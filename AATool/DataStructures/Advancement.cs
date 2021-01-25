using System;
using System.Collections.Generic;
using System.Xml;

namespace AATool.DataStructures
{
    public class Advancement
    {
        public string ID                                { get; private set; }
        public string Name                              { get; private set; }
        public string Icon                              { get; private set; }
        public string CriteriaGoal                      { get; private set; }
        public int CriteriaCompleted                    { get; private set; }
        public bool IsCompleted                         { get; private set; }
        public string FrameComplete                     { get; private set; }
        public string FrameIncomplete                   { get; private set; }
        public Dictionary<string, Criterion> Criteria   { get; private set; }
        public AdvancementType Type                     { get; private set; }        

        public bool HasCriteria    => Criteria.Count > 0;
        public string CurrentFrame => IsCompleted ? FrameComplete : FrameIncomplete;
        public int CriteriaCount   => HasCriteria ? Criteria.Count : 0;
        public int CriteriaPercent => (int)(CriteriaCompleted / (double)Criteria.Count * 100);

        public Advancement(XmlNode node)
        {
            //initialize members from xml 
            ID   = node.Attributes["id"]?.Value;
            Name = node.Attributes["name"]?.Value ?? ID;
            Icon = node.Attributes["icon"]?.Value;
            if (Icon == null)
            {
                var idParts = ID.Split('/');
                if (idParts.Length > 0)
                    Icon = idParts[idParts.Length - 1];
            }
            if (Enum.TryParse(node.Attributes["type"]?.Value ?? "normal", true, out AdvancementType parsed)) 
                Type = parsed;

             FrameComplete   = "frame_" + Type.ToString().ToLower() + "_complete";
             FrameIncomplete = "frame_" + Type.ToString().ToLower() + "_incomplete";

            //parse criteria
            Criteria = new Dictionary<string, Criterion>();
            if (node.HasChildNodes)
            {
                XmlNode criteriaNode = node.SelectSingleNode("criteria");
                CriteriaGoal = criteriaNode.Attributes["goal"]?.Value ?? "Completed";
                foreach (XmlNode criterionNode in criteriaNode.ChildNodes)
                {
                    var criterion = new Criterion(criterionNode, ID);
                    Criteria.Add(criterion.ID, criterion);
                }
            }
        }

        public void Update(AdvancementsJSON advancements)
        {
            if (IsCompleted != advancements.IsCompleted(ID))
                IsCompleted = !IsCompleted;

            if (HasCriteria)
            {
                var completedCriteria = advancements.GetCompletedCriteriaFor(ID);
                CriteriaCompleted = completedCriteria.Count;
                foreach (var criterion in Criteria.Values)
                    criterion.Update(completedCriteria);
            }
        }
    }
}