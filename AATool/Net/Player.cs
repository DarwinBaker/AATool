using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Graphics;
using AATool.Net.Requests;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace AATool.Net
{
    public static class Player
    {
        private static readonly Dictionary<string, Uuid> IdCache   = new ();
        private static readonly Dictionary<Uuid, string> NameCache = new ();
        private static readonly Dictionary<Uuid, Color> ColorCache = new ();

        public static bool TryGetName(Uuid id, out string name)  => NameCache.TryGetValue(id, out name);
        public static bool TryGetColor(Uuid id, out Color color) => ColorCache.TryGetValue(id, out color);

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

                    string uuid = JObject.Parse(response)["id"].ToString();
                    if (Uuid.TryParse(uuid, out id))
                    {
                        Cache(id, name);
                        NetRequest.Enqueue(new AvatarRequest(id));
                    }
                }
                catch { }
            }
            return id;
        }

        public static void Cache(Uuid id, string name)
        {
            if (!NameCache.ContainsKey(id) && !string.IsNullOrEmpty(name))
                NameCache[id] = name;
        }

        public static void Cache(Uuid id, Color color)
        {
            if (!ColorCache.ContainsKey(id))
                ColorCache[id] = color;
        }

        public static void FetchIdentity(Uuid id)
        {
            NetRequest.Enqueue(new NameRequest(id));
            NetRequest.Enqueue(new AvatarRequest(id));
        }
    }
}