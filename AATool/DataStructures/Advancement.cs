using System;
using System.Collections.Generic;
using System.Xml;

namespace AATool.DataStructures
{
    public class Advancement
    {
        //advancement frame textures
        protected static readonly Dictionary<FrameType, string> CompleteFrames = new Dictionary<FrameType, string>()  {
            { FrameType.Normal,    "frame_normal_complete"}, 
            { FrameType.Goal,      "frame_goal_complete"}, 
            { FrameType.Challenge, "frame_challenge_complete"}
        };
        protected static readonly Dictionary<FrameType, string> IncompleteFrames = new Dictionary<FrameType, string>()  {
            { FrameType.Normal,    "frame_normal_incomplete"}, 
            { FrameType.Goal,      "frame_goal_incomplete"}, 
            { FrameType.Challenge, "frame_challenge_incomplete"}
        };

        public string ID                                { get; protected set; }
        public string Name                              { get; protected set; }
        public string Icon                              { get; protected set; }
        public string CriteriaGoal                      { get; protected set; }
        public int CriteriaCompleted                    { get; protected set; }
        public bool Hidden                              { get; protected set; }
        public bool IsCompleted                         { get; protected set; }
        public Dictionary<string, Criterion> Criteria   { get; protected set; }
        public FrameType Type                           { get; protected set; }        

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
            if (Enum.TryParse(node.Attributes["type"]?.Value ?? "normal", true, out FrameType parsed)) 
                Type = parsed;

            if (bool.TryParse(node.Attributes["hidden"]?.Value, out bool hidden))
                Hidden = hidden;

            ParseCriteria(node);
        }

        protected void ParseCriteria(XmlNode node)
        {
            Criteria = new Dictionary<string, Criterion>();
            if (node.HasChildNodes)
            {
                XmlNode criteriaNode = node.SelectSingleNode("criteria");
                if (criteriaNode == null)
                    return;

                CriteriaGoal = criteriaNode.Attributes["goal"]?.Value ?? "Completed";
                foreach (XmlNode criterionNode in criteriaNode.ChildNodes)
                {
                    var criterion = new Criterion(criterionNode, this);
                    Criteria.Add(criterion.ID, criterion);
                }
            }
        }

        public virtual void Update(SaveJSON json)
        {
            var advancements = json as AdvancementsJSON;
            IsCompleted = advancements.IsCompleted(ID);
            if (HasCriteria)
            {
                var completedCriteria = advancements.GetCompletedCriteriaFor(this);
                CriteriaCompleted = completedCriteria.Count;
                foreach (var criterion in Criteria.Values)
                    criterion.Update(completedCriteria);
            }
        }
    }
}