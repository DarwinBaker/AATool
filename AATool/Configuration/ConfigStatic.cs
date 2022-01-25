using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace AATool.Configuration
{
    public abstract partial class Config
    {
        public static TrackingConfig Tracking { get; private set; }
        public static MainConfig Main         { get; private set; }
        public static OverlayConfig Overlay   { get; private set; }
        public static NetworkConfig Net       { get; private set; }
        public static SftpConfig Sftp         { get; private set; }
        public static NotesConfig Notes       { get; private set; }

        private static readonly List<Config> All = new ();

        private static readonly Dictionary<Type, string> FileNames = new ()
        {
            { typeof(TrackingConfig), "config_tracking.json" },
            { typeof(MainConfig),     "config_main.json"     },
            { typeof(OverlayConfig),  "config_overlay.json"  },
            { typeof(NetworkConfig),  "config_network.json"  },
            { typeof(SftpConfig),     "config_sftp.json"     },
            { typeof(NotesConfig),    "config_notes.json"    },
        };

        public static void Initialize()
        {
            Load<TrackingConfig>();
            Load<MainConfig>();
            Load<OverlayConfig>();
            Load<NetworkConfig>();
            Load<SftpConfig>();
            Load<NotesConfig>();
            ArchiveOldSettings();
        }

        public static void SaveAll()
        {
            foreach (Config config in All)
                config.Save();
        }

        public static void ResetAllToDefaults()
        {
            foreach (Config config in All)
                config.ApplyDefaultValues();
            SaveAll();
        }

        public static void ClearAllFlags()
        {
            foreach (Config config in All)
                config.ClearFlags();
        }

        public static void Save(Config config)
        {
            try
            {
                Directory.CreateDirectory(Paths.System.ConfigFolder);
                string file = Path.Combine(Paths.System.ConfigFolder, config.FileName);
                using (StreamWriter stream = File.CreateText(file))
                {
                    new JsonSerializer {
                        Formatting = Newtonsoft.Json.Formatting.Indented
                    }.Serialize(stream, config);
                }
            }
            catch { }
        }

        private static void Load<T>() where T : Config, new()
        {
            T config = null;
            try
            {
                string file = Path.Combine(Paths.System.ConfigFolder, FileNames[typeof(T)]);
                using (StreamReader stream = File.OpenText(file))
                {
                    //deserialize config json
                    config = (T)new JsonSerializer {
                        Formatting = Newtonsoft.Json.Formatting.Indented
                    }.Deserialize(stream, typeof(T));
                }
            }
            catch (Exception e)
            {
                //couldn't deserialize, start from defaults
                config = new ();
                if (e is FileNotFoundException or DirectoryNotFoundException)
                {
                    //attempt to read depricated xml config format (for upgrading aatool from pre-1.4.0.0)
                    string file = Path.Combine(Paths.System.LegacySettingsFolder, config.LegacyFileName);
                    if (XmlObject.TryGetDocument(file, out XmlDocument document))
                        config.ApplyLegacy(document);
                }
                //overwrite missing/corrupt config file
                Save(config);
            }
            finally
            {
                RegisterConfig(config);
            }
        }

        private static bool TryParseLegacySetting(XmlNode setting, out string key, out object value)
        {
            //parse setting from old xml format
            string raw = XmlObject.Attribute(setting, "value", string.Empty);
            key = XmlObject.Attribute(setting, "key", string.Empty);
            value = null;

            bool valid = false;
            switch (setting?.Name.ToLower())
            {
                case "string":
                    valid = true;
                    value = raw;
                    break;

                case "bool":
                    valid = bool.TryParse(raw, out bool boolean);
                    value = boolean;
                    break;

                case "int":
                    valid = int.TryParse(raw, out int number);
                    value = number;
                    break;

                case "color":
                    string[] split = raw.Split(',');
                    valid = split.Length > 2;
                    valid &= int.TryParse(split[0], out int r);
                    valid &= int.TryParse(split[1], out int g);
                    valid &= int.TryParse(split[2], out int b);
                    value = new Color(r, g, b, 255);
                    break;
            }
            return valid;
        }

        private static void RegisterConfig(Config config)
        {
            switch (config)
            {
                case TrackingConfig:
                    Tracking = config as TrackingConfig;
                    break;
                case MainConfig:
                    Main = config as MainConfig;
                    break;
                case OverlayConfig:
                    Overlay = config as OverlayConfig;
                    break;
                case NetworkConfig:
                    Net = config as NetworkConfig;
                    break;
                case SftpConfig:
                    Sftp = config as SftpConfig;
                    break;
                case NotesConfig:
                    Notes = config as NotesConfig;
                    break;
                default:
                    return;
            }

            if (config is not null)
                All.Add(config);
        }

        private static void ArchiveOldSettings()
        {
            //rename and move outdated config format if present
            if (Directory.Exists(Paths.System.LegacySettingsFolder))
            {
                try
                {
                    Directory.Move(Paths.System.LegacySettingsFolder, Paths.System.ArchivedConfigFolder);
                }
                catch { }
            }
        }
    }
}
