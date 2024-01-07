using System;
using System.Collections.Generic;
using System.IO;
using AATool.Configuration;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace AATool.Data.Speedrunning
{
    public class RunnerProfile
    {
        public static Dictionary<string, RunnerProfile> ProfilesByIdOrName = new();
        public static Dictionary<string, string> NamesBySrcId = new();
        public static Dictionary<string, string> SrcIdsByName = new();

        public static RunnerProfile GetCurrent()
            => ProfilesByIdOrName.TryGetValue(Config.Tracking.CurrentRunnerProfileNameOrId.ToLower(), 
                out RunnerProfile profile) ? profile : null;

        public string Id { get; set; }
        public string Name { get; set; }
        public string Pronouns { get; set; }
        public string Link { get; set; }
        public Texture2D Picture { get; set; }

        public static void Initialize()
        {
            if (!string.IsNullOrEmpty(Config.Tracking.CurrentRunnerProfileId))
                SetCurrentId(Config.Tracking.CurrentRunnerProfileId);
            else if (!string.IsNullOrEmpty(Config.Tracking.CurrentRunnerProfileName))
                SetCurrentName(Config.Tracking.CurrentRunnerProfileName);
        }

        public static void SetCurrentId(string id)
        {
            Config.Tracking.CurrentRunnerProfileId.Set(id);
            Config.Tracking.CurrentRunnerProfileName.Set(string.Empty);
            Config.Tracking.TrySave();
            //new SrcProfileRequest(id).EnqueueOnce();

            if (GetCurrent() is not null)
                return;

            if (TryReadCached(id, out RunnerProfile cached))
            {
                ProfilesByIdOrName[cached.Id] = ProfilesByIdOrName[cached.Name.ToLower()] = cached;
                return;
            }

            var newProfile = new RunnerProfile() {
                Id = id
            };
            if (!string.IsNullOrEmpty(id)) 
            {
                ProfilesByIdOrName[id] = newProfile;
                if (NamesBySrcId.TryGetValue(id, out string cachedName))
                    newProfile.Name = cachedName;
            }
        }

        public static void SetCurrentName(string name)
        {
            Config.Tracking.CurrentRunnerProfileName.Set(name);
            Config.Tracking.CurrentRunnerProfileId.Set(string.Empty);
            Config.Tracking.TrySave();
            //new SrcProfileRequest(name).EnqueueOnce();

            if (GetCurrent() is not null)
                return;

            if (TryReadCached(name, out RunnerProfile cached))
            {
                ProfilesByIdOrName[cached.Id] = ProfilesByIdOrName[cached.Name.ToLower()] = cached;
                return;
            }

            var newProfile = new RunnerProfile() { 
                Name = name
            };
            if (!string.IsNullOrEmpty(name))
            {
                ProfilesByIdOrName[name.ToLower()] = newProfile;
                if (SrcIdsByName.TryGetValue(name, out string cachedId))
                    newProfile.Id = cachedId;
            }
        }

        public static bool TryParseSrc(string json, bool cache, out RunnerProfile profile)
        {
            profile = null;
            try
            {
                dynamic data = JsonConvert.DeserializeObject(json);
                string id = data["data"]["id"].Value;
                string name = data["data"]["names"]["international"].Value;
                string pronouns = data["data"]["pronouns"].Value;
                string link = data["data"]["weblink"].Value;

                ProfilesByIdOrName[id] = ProfilesByIdOrName[name.ToLower()] = profile = new RunnerProfile() {
                    Id = id,
                    Name = name,
                    Pronouns = pronouns,
                    Link = link,
                };

                if (cache)
                {
                    Cache(json, id);
                    Cache(json, name);
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        private static void Cache(string json, string idOrName)
        {
            try
            {
                Directory.CreateDirectory(Paths.System.ProfileDetailsCacheFolder);
                string fileName = Paths.System.SpeedrunDotComProfileJson(idOrName);
                File.WriteAllText(fileName, json);
            }
            catch
            {
            }
        }

        public static bool TryReadCached(string idOrName, out RunnerProfile profile)
        {
            profile = null;
            if (string.IsNullOrEmpty(idOrName))
                return false;

            try
            {
                string jsonName = Paths.System.SpeedrunDotComProfileJson(idOrName);
                if (!File.Exists(jsonName))
                    return false;

                string json = File.ReadAllText(jsonName);
                if (!TryParseSrc(json, false, out profile))
                    return false;

                string pictureName = Paths.System.SpeedrunDotComProfilePicture(profile.Id);
                if (File.Exists(pictureName))
                {
                    using (FileStream stream = File.OpenRead(pictureName))
                        profile.Picture = Texture2D.FromStream(Main.GraphicsManager.GraphicsDevice, stream);
                }
                return true;
            }
            catch
            {
                return false;
            }
        } 
    }
}
