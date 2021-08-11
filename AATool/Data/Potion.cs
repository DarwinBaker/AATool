using System.Collections.Generic;
using System.Xml;

namespace AATool.Data
{
    public struct Potion
    {
        public string Name              { get; private set; }
        public string Icon              { get; private set; }
        public List<string> Ingredients { get; private set; }

        public Potion(XmlNode node)
        {
            //initialize members from xml 
            this.Icon = node.Attributes["icon"]?.Value;
            this.Name = node.Attributes["name"]?.Value;
            this.Ingredients = new ();
            foreach (XmlNode ingredient in node.ChildNodes)
                this.Ingredients.Add(ingredient.Attributes["icon"]?.Value);
        }
    }
}
