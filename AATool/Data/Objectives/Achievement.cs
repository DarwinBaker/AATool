using System.Collections.Generic;
using System.Xml;

namespace AATool.Data.Objectives
{
    public class Achievement : Advancement
    {
        public bool IsRoot => this.Parent is null;
        public bool IsLocked => !this.IsRoot && !this.Parent.CompletedByAnyone();

        public readonly Dictionary<string, Achievement> Children;
        public readonly Achievement Parent;

        public Achievement(XmlNode node, Achievement parent = null) : base (node)
        {
            this.Children = new Dictionary<string, Achievement>();
            this.Parent = parent;
            this.Id = $"achievement.{this.Id}";

            //recursively build nested structure of pre-1.12 achievements
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name is "achievement")
                {
                    var achievement = new Achievement(childNode, this);
                    this.Children[achievement.Id] = achievement;
                }
            }
            this.ParseCriteria(node);
        }

        public Dictionary<string, Advancement> GetAllChildrenRecursive(Dictionary<string, Advancement> children)
        {
            children[this.Id] = this;
            foreach (Achievement child in this.Children.Values)
                child.GetAllChildrenRecursive(children);
            return children;
        }
    }
}
