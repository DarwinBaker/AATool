using System.Collections.Generic;
using System.Xml;

namespace AATool.DataStructures
{
    public struct Potion
    {
        public string Name                          { get; private set; }
        public string Icon                          { get; private set; }
        public List<PotionIngredient> Ingredients   { get; private set; }

        public Potion(XmlNode node)
        {
            //initialize members from xml 
            Icon = node.Attributes["icon"]?.Value;
            Name = node.Attributes["name"]?.Value;
            Ingredients = new List<PotionIngredient>();
            foreach (XmlNode ingredientNode in node.SelectNodes("ingredient"))
                Ingredients.Add(new PotionIngredient(ingredientNode));
        }
    }
}
