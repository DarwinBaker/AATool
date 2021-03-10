using System;
using System.Collections.Generic;
using System.Xml;

namespace AATool.DataStructures
{
    public class Advancement
    {
        //advancement frame textures
        public static readonly Dictionary<AdvancementType, string> CompleteFrames = new Dictionary<AdvancementType, string>()  {
            { AdvancementType.Normal,    "frame_normal_complete"}, 
            { AdvancementType.Goal,      "frame_goal_complete"}, 
            { AdvancementType.Challenge, "frame_challenge_complete"}
        };
        public static readonly Dictionary<AdvancementType, string> IncompleteFrames = new Dictionary<AdvancementType, string>()  {
            { AdvancementType.Normal,    "frame_normal_incomplete"}, 
            { AdvancementType.Goal,      "frame_goal_incomplete"}, 
            { AdvancementType.Challenge, "frame_challenge_incomplete"}
        };

        public string ID                                { get; private set; }
        public string Name                              { get; private set; }
        public string Icon                              { get; private set; }
        public string CriteriaGoal                      { get; private set; }
        public int CriteriaCompleted                    { get; private set; }
        public bool IsCompleted                         { get; private set; }
        public Dictionary<string, Criterion> Criteria   { get; private set; }
        public AdvancementType Type                     { get; private set; }        

        public bool HasCriteria    => Criteria.Count > 0;
        public string CurrentFrame => IsCompleted ? CompleteFrames[Type] : IncompleteFrames[Type];
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
            IsCompleted = advancements.IsCompleted(ID);
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