using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.Net.Requests
{
    public sealed class AvatarRequest : NetRequest
    {
        public static int Downloads { get; private set; }

        private readonly Uuid id;
        private readonly string name;
        private readonly bool isFallback;

        public AvatarRequest(Uuid player, bool isFallback = false) : 
            base (isFallback ? Paths.Web.GetAvatarUrlFallback(player, 8) : Paths.Web.GetAvatarUrl(player, 8))
        {
            this.id = player;
            this.isFallback = isFallback;
            Player.TryGetName(this.id, out this.name);
            this.name = this.name?.ToLower();
        }

        public AvatarRequest(string name) : base(Paths.Web.GetAvatarUrl(Leaderboard.GetRealName(name).ToLower(), 8))
        {
            this.name = Leaderboard.GetRealName(name).ToLower();
            Player.TryGetUuid(this.name, out this.id);
        }

        public override async Task<bool> DownloadAsync()
        {
            //logging
            if (Player.TryGetName(this.id, out string name))
                Debug.Log(Debug.RequestSection, $"{Outgoing} Requested avatar for \"{name}\"");
            else
                Debug.Log(Debug.RequestSection, $"{Outgoing} Requested avatar for {this.id.ShortString ?? this.name}");
            Downloads++;
            this.BeginTiming();

            using var client = new HttpClient() {
                Timeout = TimeSpan.FromMilliseconds(Protocol.Requests.TimeoutMs)
            };
            try
            {
                //download texture and add to atlas
                using (Stream response = await client.GetStreamAsync(this.Url))
                {
                    this.EndTiming();
                    return this.HandleResponse(response);
                }
            }
            catch (OperationCanceledException)
            {
                //request canceled, nothing left to do here
                Debug.Log(Debug.RequestSection, $"-- Avatar request cancelled for {this.id.ShortString ?? this.name}");
            }
            catch (HttpRequestException e)
            {
                //error getting response, try other url
                Debug.Log(Debug.RequestSection, $"-- Avatar request failed for {this.id.ShortString ?? this.name}: {e.Message}");

                //try other api
                if (!this.isFallback && this.id != Uuid.Empty)
                    new AvatarRequest(this.id, true).EnqueueOnce();
            }
            this.EndTiming();
            return false;
        }

        private bool HandleResponse(Stream avatarStream)
        {
            Texture2D texture = null;
            try
            {
                texture = Texture2D.FromStream(Main.GraphicsManager.GraphicsDevice, avatarStream);

                if (!string.IsNullOrEmpty(this.name))
                {
                    Debug.Log(Debug.RequestSection, $"{Incoming} Received avatar for \"{this.name}\" in {this.ResponseTime}");

                    //save a copy for the player's name
                    string nameSprite = $"avatar-{this.name}";
                    texture.Tag = nameSprite;
                    if (SpriteSheet.ContainsSprite(nameSprite))
                        SpriteSheet.Replace(nameSprite, texture);
                    else
                        SpriteSheet.Pack(texture);
                    SaveToCache(texture, Path.Combine(Paths.System.AvatarCacheFolder, $"avatar-{this.name}.png"));
                }
                else
                {
                    Debug.Log(Debug.RequestSection, $"{Incoming} Received avatar for {this.id.ShortString ?? this.name} in {this.ResponseTime}");
                }

                if (this.id != Uuid.Empty)
                {
                    //save a copy for the player's uuid
                    string uuidSprite = $"avatar-{this.id}";
                    texture.Tag = uuidSprite;
                    if (SpriteSheet.ContainsSprite(uuidSprite))
                        SpriteSheet.Replace(uuidSprite, texture);
                    else
                        SpriteSheet.Pack(texture);

                    SaveToCache(texture, Path.Combine(Paths.System.AvatarCacheFolder, $"avatar-{this.id}.png"));

                }
                return true;
            }
            catch (ArgumentException)
            {
                //safely ignore malformed stream and move on
                Debug.Log(Debug.RequestSection, $"{Incoming} Received invalid avatar for {this.id.ShortString ?? this.name} in {this.ResponseTime}");
                return false;
            }
            finally
            {
                //compute average color for player-specific glow colors
                Player.Cache(this.id, ColorHelper.GetAccent(texture));
                texture?.Dispose();
            }
        }

        private static void SaveToCache(Texture2D texture, string fileName)
        {
            try
            {
                //cache avatar so it loads instantly next launch
                //overwrite to keep skins up to date
                Directory.CreateDirectory(Paths.System.AvatarCacheFolder);
                using (FileStream fileStream = File.Create(fileName))
                    texture.SaveAsPng(fileStream, texture.Width, texture.Height);
            }
            catch (IOException)
            {
                //couldn't save file. ignore and move on
            }
        }
    }
}
