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
        protected Dictionary<string, object> Entries    { get; private set; }
        protected Dictionary<string, bool> ChangedFlags { get; private set; }
        protected string FileName                       { get; private set; }
        protected string FilePath                       { get; private set; }

        public SettingsGroup()
        {
            this.Entries = new ();
            this.ChangedFlags   = new ();
        }

        public abstract void ResetToDefaults();

        public bool ValueChanged(string key) =>
            this.ChangedFlags.TryGetValue(key, out bool changed) && changed;

        protected T Get<T>(string key) => 
            this.Entries.TryGetValue(key, out object val) && val is T t ? t : default;

        protected void Set(string key, object newValue)
        {
            //if different, add key to list of values modified this frame
            if (this.Entries.TryGetValue(key, out object currentValue))
            {
                if (( currentValue is not null && !currentValue.Equals(newValue) ) || ( newValue != null && !newValue.Equals(currentValue) ))
                    this.ChangedFlags[key] = true;
            }
            this.Entries[key] = newValue;
        }

        public void ClearFlags()
        {
            //new frame is starting; clear list of values modified this frame
            foreach (string key in this.ChangedFlags.Keys.ToList())
                this.ChangedFlags[key] = false;
        }

        public void Load(string file)
        {
            this.FileName = file;
            this.FilePath = Path.Combine(Paths.DIR_SETTINGS, file + ".xml");
            this.ResetToDefaults();
            //attempt to read settings from xml file
            if (!this.TryLoadXml(this.FilePath))
            {
                //error parsing xml; restore defaults and overwrite bad settings file
                this.ResetToDefaults();
                this.Save();
            }
        }

        public void Save()
        {
            //create settings directory if it doesn't already exist
            Directory.CreateDirectory(Paths.DIR_SETTINGS);
            this.SaveXml(this.FilePath);
        }

        public override void ReadDocument(XmlDocument document)
        {
            foreach (XmlNode settingNode in document.SelectSingleNode("settings").ChildNodes)
            {
                //parse node to proper data type and add value with associated key
                string key   = settingNode.Attributes["key"].Value;
                string value = settingNode.Attributes["value"]?.Value;
                switch (settingNode.Name)
                {
                    case "string":
                        this.Set(key, value);
                        break;
                    case "bool":
                        this.Set(key, bool.Parse(value));
                        break;
                    case "int":
                        this.Set(key, int.Parse(value));
                        break;
                    case "color":
                        string[] split = value.Split(',');
                        this.Set(key, Color.FromNonPremultiplied(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), 255));
                        break;
                    case "list":
                        this.Set(key, ParseListNode<string>(settingNode));
                        break;
                }
            }
        }

        public override void WriteDocument(XmlWriter writer)
        {
            writer.WriteStartElement("settings");
            foreach (KeyValuePair<string, object> entry in this.Entries)
                this.WriteEntryElement(writer, entry);
            writer.WriteEndElement();
        }

        private void WriteEntryElement(XmlWriter writer, KeyValuePair<string, object> entry)
        {
            //convert type to user-friendly name
            string typeName = entry.Value switch {
                bool                => "bool",
                int                 => "int",
                Color               => "color",
                ICollection<string> => "list",
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
                var list = entry.Value as ICollection<string>;
                foreach (string item in list)
                    writer.WriteElementString("item", item);
            }
            else
            {
                if (entry.Value != null)
                    writer.WriteAttributeString("value", entry.Value?.ToString() ?? "");
            }
            writer.WriteEndElement();
        }
    }
}
