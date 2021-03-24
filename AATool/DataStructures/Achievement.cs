using System.Collections.Generic;
using System.Xml;

namespace AATool.DataStructures
{
    public class Achievement : Advancement
    {
        public Achievement Parent                       { get; protected set; }
        public Dictionary<string, Achievement> Children { get; protected set; }

        public bool IsRoot       => Parent == null;
        public bool CanBeDoneYet => IsRoot || Parent.IsCompleted;

        public Achievement(XmlNode node, Achievement parent) : base(node)
        {
            Parent = parent;
            //initialize members from xml 
            ID = "achievement." + ID;

            Children = new Dictionary<string, Achievement>();
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == "achievement")
                {
                    var achievement = new Achievement(childNode, this);
                    Children[achievement.ID] = achievement;
                }
            }

            ParseCriteria(node);
        }

        public Dictionary<string, Achievement> GetAllChildrenRecursive(Dictionary<string, Achievement> children)
        {
            children[ID] = this;
            foreach (var child in Children.Values)
            {
                child.GetAllChildrenRecursive(children);
            }
            return children;
        }

        public override void Update(SaveJSON json)
        {
            var achievements = json as AchievementJSON;
            IsCompleted = achievements.IsCompleted(ID);
            if (HasCriteria)
            {
                var completedCriteria = achievements.GetCompletedCriteriaFor(this);
                CriteriaCompleted = completedCriteria.Count;
                foreach (var criterion in Criteria.Values)
                    criterion.Update(completedCriteria);
            }
        }
    }
}
