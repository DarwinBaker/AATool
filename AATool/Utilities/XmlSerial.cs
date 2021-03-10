using AATool.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AATool.Utilities
{
    public abstract class XmlSerial
    {
        //currently unused but very cool functions to return instances directly from xml
        public static T FromDocument<T>(XmlDocument document) where T : XmlSerial, new()
        {
            T value = new T();
            value.ReadDocument(document);
            return value;
        }

        public static T FromNode<T>(XmlNode node) where T : XmlSerial, new()
        {
            T value = new T();
            value.ReadNode(node);
            return value;
        }

        public virtual void ReadDocument(XmlDocument document) { }
        public virtual void ReadNode(XmlNode node) { }
        public virtual void WriteDocument(XmlWriter writer) { }
        public virtual void WriteNode(XmlWriter writer) { }
        public virtual void WriteNodeAttributes(XmlWriter writer) { }

        private XmlDocument GetXml()
        {
            //write this object into an xml doc
            var document = new XmlDocument();
            try
            {
                using (var stream = new MemoryStream())
                using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 1;
                    writer.IndentChar = '\t';

                    WriteDocument(writer);
                    writer.Flush();
                    stream.Position = 0;
                    document.Load(stream);
                }
            }
            catch { }
            return document;
        }

        public bool LoadXml(string file)
        {
            //read values from an xml doc into this object
            try
            {
                var document = new XmlDocument();
                document.Load(file);
                ReadDocument(document);
                return true;
            }
            catch { }
            return false;
        }

        public bool SaveXml(string file)
        {
            //save values from this object's xml representation into a file
            try
            {
                using (var stream = File.Create(file))
                using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 1;
                    writer.IndentChar = '\t';

                    writer.WriteStartDocument();
                    XmlDocument document = GetXml();
                    document.WriteTo(writer);
                    writer.WriteEndDocument();
                    writer.Flush();
                }
                return true;
            }
            catch { }
            return false;
        }

        public static T ParseAttribute<T>(XmlNode node, string key, T defaultValue)
        {
            return ParseValue(node?.Attributes[key]?.Value, defaultValue);
        }

        public static T ParseValue<T>(string value, T defaultValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                    return defaultValue;

                //split string into comma separated values
                string[] csv = value.Replace(" ", "").Split(',');

                //switch on type and parse accordingly
                object parsed = defaultValue switch {
                    string          _ => value,
                    char            _ => value[1],
                    int             _ => int.Parse(value),
                    float           _ => float.Parse(value),
                    double          _ => double.Parse(value),
                    bool            _ => bool.Parse(value),
                    Vector2         _ => new Vector2(float.Parse(csv[0]), float.Parse(csv[1])),
                    Point           _ => new Point(int.Parse(csv[0]), int.Parse(csv[1])),
                    Rectangle       _ => new Rectangle(int.Parse(csv[0]), int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3])),
                    Color           _ => Color.FromNonPremultiplied(int.Parse(csv[0]), int.Parse(csv[1]), int.Parse(csv[2]), csv.Length == 4 ? int.Parse(csv[3]) : 255),
                    HorizontalAlign _ => Enum.TryParse(value, true, out HorizontalAlign converted)                  ? converted : default,
                    VerticalAlign   _ => Enum.TryParse(value, true, out VerticalAlign converted)                    ? converted : default,
                    FlowDirection   _ => Enum.TryParse(value.Replace("_", ""), true, out FlowDirection converted)   ? converted : default,
                    DrawMode        _ => Enum.TryParse(value.Replace("_", ""), true, out DrawMode converted)        ? converted : default,
                    Size            _ => Size.Parse(value),
                    Margin          _ => Margin.Parse(value),
                                    _ => null
                };

                if (parsed != null)
                    return (T)Convert.ChangeType(parsed, typeof(T));
            }
            catch { }
            return defaultValue;
        }

        public static ICollection<string> ParseListNode<T>(XmlNode node)
        {
            //read list from xml
            var list = new List<string>();
            foreach (XmlNode itemNode in node.ChildNodes)
                if (!string.IsNullOrWhiteSpace(itemNode.InnerText))
                    list.Add(itemNode.InnerText);
            return list;
        }
    }
}
