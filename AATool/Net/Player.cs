using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Configuration;
using AATool.Net.Requests;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace AATool.Net
{
    public static class Player
    {
        private static readonly Dictionary<string, Uuid> IdCache = new ();
        private static readonly Dictionary<Uuid, string> NameCache = new ();

        private static readonly Dictionary<Uuid, Color> IdColorCache = new ();
        private static readonly Dictionary<string, Color> NameColorCache = new ();

        private static readonly HashSet<string> NamesAlreadyRequested = new ();
        private static readonly HashSet<Uuid> IdentitiesAlreadyRequested = new ();

        public static bool TryGetUuid(string name, out Uuid id) => IdCache.TryGetValue(name ?? "", out id);
        public static bool TryGetName(Uuid id, out string name) => NameCache.TryGetValue(id, out name);
        public static bool TryGetColor(Uuid id, out Color color) => IdColorCache.TryGetValue(id, out color);
        public static bool TryGetColor(string name, out Color color) => NameColorCache.TryGetValue(name ?? "", out color);

        public static bool ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length is < 3 or > 16)
                return false;

            //can only contain "a-z", "0-9", and "_"
            char character;
            for (int i = 0; i < name.Length; i++)
            {
                character = name[i];
                if (!char.IsLetter(character) && !char.IsNumber(character) && character is not '_')
                    return false;
            }
            return true;
        }

        public static async Task<Uuid> FetchUuidAsync(string name)
        {
            if (!ValidateName(name))
                return Uuid.Empty;
            if (IdCache.TryGetValue(name, out Uuid id))
                return id;

            using (HttpClient client = new() { Timeout = TimeSpan.FromMilliseconds(Protocol.Requests.TimeoutMs) })
            {
                try
                {
                    //try to pull uuid from mojang api
                    string response = await client.GetStringAsync(Paths.Web.GetUuidUrl(name));
                    if (string.IsNullOrEmpty(response))
                        return Uuid.Empty;

                    if (Uuid.TryParse(response, out id))
                    {
                        Cache(id, name);
                        new AvatarRequest(id).EnqueueOnce();
                    }
                }
                catch { }
            }
            return id;
        }

        public static void Cache(Uuid id, string name)
        {
            if (id == Uuid.Empty)
                return;

            if (!NameCache.ContainsKey(id) && !string.IsNullOrEmpty(name))
                NameCache[id] = name;
            if (name is not null && !IdCache.ContainsKey(name) && id != Uuid.Empty)
                IdCache[name] = id;

            if (name == Config.Tracking.SoloFilterName)
                Config.Tracking.SoloFilterName.InvokeChange();
        }

        public static void Cache(Uuid id, Color color)
        {
            IdColorCache[id] = color;
        }

        public static void Cache(string name, Color color)
        {
            NameColorCache[name] = color;
        }

        public static void FetchIdentityAsync(Uuid id)
        {
            if (IdentitiesAlreadyRequested.Contains(id))
                return;

            IdentitiesAlreadyRequested.Add(id);
            new NameRequest(id).EnqueueOnce();
            new AvatarRequest(id).EnqueueOnce();
        }

        public static void FetchIdentityAsync(string name)
        {
            if (NamesAlreadyRequested.Contains(name))
                return;
            
            NamesAlreadyRequested.Add(name);
            new UuidRequest(name, true).EnqueueOnce();
        }
    }
}