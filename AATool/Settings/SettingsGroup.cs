using AATool.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AATool.Settings
{
    public abstract class SettingsGroup
    {
        protected Dictionary<string, object> Entries;
        protected string FileName;

        protected string FilePath => Path.Combine(Paths.DIR_SETTINGS, FileName + ".xml");

        public abstract void ResetToDefaults();

        public void Load()
        {
            ResetToDefaults();
            try
            {
                //attempt to read settings from xml file
                var document = new XmlDocument();
                using (var stream = File.OpenRead(FilePath))
                {
                    document.Load(stream);
                    foreach (XmlNode settingNode in document.SelectSingleNode("settings").ChildNodes)
                    {
                        //parse node to proper data type and add value with associated key
                        string key = settingNode.Attributes["key"].Value;
                        string value = settingNode.Attributes["value"]?.Value;
                        switch (settingNode.Name)
                        {
                            case "string":
                                Entries[key] = value;
                                break;
                            case "bool":
                                Entries[key] = bool.Parse(value);
                                break;
                            case "int":
                                Entries[key] = int.Parse(value);
                                break;
                            case "color":
                                var split = value.Split(',');
                                Entries[key] = Color.FromNonPremultiplied(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), 255);
                                break;
                            case "list":
                                Entries[key] = XmlSerial.ParseListNode<string>(settingNode);
                                break;
                        }
                    }
                }
            }
            catch
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

            //create and write settings to xml file
            using (var stream = File.Create(FilePath))
            using (var writer = new XmlTextWriter(stream, Encoding.UTF8) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t' })
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("settings");

                foreach (var entry in Entries)
                    WriteEntryElement(writer, entry);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void WriteEntryElement(XmlTextWriter writer, KeyValuePair<string, object> entry)
        {
            //convert type to user-friendly name
            string typeName = entry.Value switch
            {
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
                writer.WriteAttributeString("value", entry.Value.ToString());

            writer.WriteEndElement();
        }
    }
}
