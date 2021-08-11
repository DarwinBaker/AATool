using AATool.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AATool.Utilities
{
    public interface IXmlObject
    {
        public abstract void ReadDocument(XmlDocument document);
        public abstract void ReadNode(XmlNode node);
        public abstract void WriteDocument(XmlWriter writer);
        public abstract void WriteNode(XmlWriter writer);
        public abstract void WriteNodeAttributes(XmlWriter writer);
    }

    public abstract class XmlObject : IXmlObject
    {
        //create and return xmlobject instance from .xml file
        public static T Instantiate<T>(string file) where T : IXmlObject, new()
        {
            var value = new T();
            if (TryGetDocument(file, out XmlDocument document))
                value.ReadDocument(document);
            return value;
        }

        public static T Instantiate<T>(XmlDocument document) where T : IXmlObject, new()
        {
            //create and return xmlobject instance from document
            var value = new T();
            value.ReadDocument(document);
            return value;
        }

        public static T Instantiate<T>(XmlNode node) where T : IXmlObject, new()
        {
            //create and return xmlobject instance from node
            var value = new T();
            value.ReadNode(node);
            return value;
        }

        public static bool TryGetDocument(string file, out XmlDocument document)
        {
            //try to load xml file
            document = new XmlDocument();
            try
            {
                if (File.Exists(file))
                {
                    document.Load(file);
                    return true;
                }
            }
            catch { }
            return false;
        }

        private static void SetWriterFormatting(XmlTextWriter writer)
        {
            writer.Formatting  = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar  = '\t';
        }

        public static T ParseAttribute<T>(XmlNode node, string key, T defaultValue) => 
            ParseValue(node?.Attributes[key]?.Value, defaultValue);

        public static T ParseValue<T>(string value, T defaultValue)
        {
            //try to parse string into object of same type as default
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                    return defaultValue;

                //split string into comma separated values
                string[] csv = value.Replace(" ", string.Empty).Split(',');

                //switch on type and parse accordingly
                object parsed = defaultValue switch {
                    string          => value,
                    char            => value[1],
                    int             => int.Parse(value),
                    float           => float.Parse(value),
                    double          => double.Parse(value),
                    bool            => bool.Parse(value),
                    Vector2         => new Vector2(float.Parse(csv[0]), float.Parse(csv[1])),
                    Point           => new Point(int.Parse(csv[0]), int.Parse(csv[1])),
                    Rectangle       => new Rectangle(int.Parse(csv[0]), int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3])),
                    Color           => new Color(int.Parse(csv[0]), int.Parse(csv[1]), int.Parse(csv[2]), csv.Length is 4 ? int.Parse(csv[3]) : 255),
                    HorizontalAlign => Enum.TryParse(value, true, out HorizontalAlign e) ? e : default,
                    VerticalAlign   => Enum.TryParse(value, true, out VerticalAlign e) ? e : default,
                    FlowDirection   => Enum.TryParse(value.Replace("_", string.Empty), true, out FlowDirection e) ? e : default,
                    DrawMode        => Enum.TryParse(value.Replace("_", string.Empty), true, out DrawMode e) ? e : default,
                    Layer           => Enum.TryParse(value, true, out Layer e) ? e : default,
                    Size            => Size.Parse(value),
                    Margin          => Margin.Parse(value),
                    _ => null
                };

                if (parsed != null)
                    return (T)Convert.ChangeType(parsed, typeof(T));
            }
            catch { }
            //couldn't parse, return default
            return defaultValue;
        }

        public static ICollection<string> ParseListNode<T>(XmlNode node)
        {
            //read list from xml
            var list = new List<string>();
            foreach (XmlNode itemNode in node.ChildNodes)
            {
                if (!string.IsNullOrWhiteSpace(itemNode.InnerText))
                    list.Add(itemNode.InnerText);
            }
            return list;
        }

        //try to read values from xml doc into this object and return success
        public bool TryLoadXml(string file)
        {
            if (TryGetDocument(file, out XmlDocument document))
            {
                this.ReadDocument(document);
                return true;
            }
            return false;
        }

        public bool SaveXml(string file)
        {
            //save values from this object's xml representation into a file
            try
            {
                using (FileStream stream = File.Create(file))
                using (XmlTextWriter writer = new(stream, Encoding.UTF8))
                {
                    if (!this.TryGetXml(out XmlDocument document))
                        return false;

                    SetWriterFormatting(writer);
                    writer.WriteStartDocument();
                        document.WriteTo(writer);
                    writer.WriteEndDocument();
                    writer.Flush();
                }
                return true;
            }
            catch { }
            return false;
        }

        private bool TryGetXml(out XmlDocument document)
        {
            //write this object into an xml doc
            document = new XmlDocument();
            try
            {
                using var stream = new MemoryStream();
                using var writer = new XmlTextWriter(stream, Encoding.UTF8);
                SetWriterFormatting(writer);
                this.WriteDocument(writer);
                writer.Flush();
                stream.Position = 0;
                document.Load(stream);
                return true;
            }
            catch { }
            return false;
        }

        public virtual void ReadDocument(XmlDocument document)    { }
        public virtual void ReadNode(XmlNode node)                { }
        public virtual void WriteDocument(XmlWriter writer)       { }
        public virtual void WriteNode(XmlWriter writer)           { }
        public virtual void WriteNodeAttributes(XmlWriter writer) { }
    }
}
