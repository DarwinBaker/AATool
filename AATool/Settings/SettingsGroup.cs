using AATool.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace AATool.Settings
{
    public abstract class SettingsGroup : XmlObject
    {
        protected Dictionary<string, object> Entries;
        protected Dictionary<string, bool> Changed;
        protected string FileName;

        protected string FilePath => Path.Combine(Paths.DIR_SETTINGS, FileName + ".xml");

        public abstract void ResetToDefaults();

        protected T Get<T>(string key)
        {
            if (Entries.TryGetValue(key, out var currentValue) && currentValue is T)
                return (T)currentValue;
            return default;
        }

        protected void Set(string key, object newValue)
        {
            //if different, add key to list of values modified this frame
            if (Entries.TryGetValue(key, out var currentValue))
            {
                if ((currentValue != null && !currentValue.Equals(newValue)) || (newValue != null && !newValue.Equals(currentValue)))
                    Changed[key] = true;
            }    
            Entries[key] = newValue;
        }

        public bool ValueChanged(string key)
        {
            return Changed.TryGetValue(key, out bool changed) ? changed : false;
        }

        public void Update()
        {
            //new frame is starting; clear list of values modified this frame
            foreach (var key in Changed.Keys.ToList())
                Changed[key] = false;
        }

        public void Load()
        {
            Changed = new Dictionary<string, bool>();
            ResetToDefaults();
            //attempt to read settings from xml file
            if (!LoadXml(FilePath))
            {
                //error parsing xml; restore defaults and overwrite bad settings file
                ResetToDefaults();
                Save();
            }
        }

        public void Save()
        {
            //create settings directory if it doesn't already exist
            Directory.CreateDirectory(Paths.DIR_SETTINGS);
            SaveXml(FilePath);
        }

        public override void ReadDocument(XmlDocument document)
        {
            foreach (XmlNode settingNode in document.SelectSingleNode("settings").ChildNodes)
            {
                //parse node to proper data type and add value with associated key
                string key = settingNode.Attributes["key"].Value;
                string value = settingNode.Attributes["value"]?.Value;
                switch (settingNode.Name)
                {
                    case "string":
                        Set(key, value);
                        break;
                    case "bool":
                        Set(key, bool.Parse(value));
                        break;
                    case "int":
                        Set(key, int.Parse(value));
                        break;
                    case "color":
                        var split = value.Split(',');
                        Set(key, Color.FromNonPremultiplied(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), 255));
                        break;
                    case "list":
                        Set(key, ParseListNode<string>(settingNode));
                        break;
                }
            }
        }

        public override void WriteDocument(XmlWriter writer)
        {
            writer.WriteStartElement("settings");
            foreach (var entry in Entries)
                WriteEntryElement(writer, entry);
            writer.WriteEndElement();
        }

        private void WriteEntryElement(XmlWriter writer, KeyValuePair<string, object> entry)
        {
            //convert type to user-friendly name
            string typeName = entry.Value switch {
                bool                _ => "bool",
                int                 _ => "int",
                Color               _ => "color",
                ICollection<string> _ => "list",
                                    _ => "string"
            };

            writer.WriteStartElement(typeName);
            writer.WriteAttributeString("key", entry.Key);

            //write value/inner xml
            if (typeName == "color")
            {
                var color = (Color)entry.Value;
                writer.WriteAttributeString("value", color.R + "," + color.G + "," + color.B);
            }
            else if (typeName == "list")
            {
                var list = (ICollection<string>)entry.Value;
                foreach (var item in list)
                    writer.WriteElementString("item", item);
            }
            else
                writer.WriteAttributeString("value", entry.Value.ToString().ToLower());

            writer.WriteEndElement();
        }
    }
}
