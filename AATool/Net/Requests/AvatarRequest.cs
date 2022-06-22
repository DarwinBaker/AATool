using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Graphics;
using AATool.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.Net.Requests
{
    public sealed class AvatarRequest : NetRequest
    {
        public static int Downloads { get; private set; }

        private readonly Uuid id;
        private readonly string shortId;

        public AvatarRequest(Uuid id) : base (Paths.Web.GetAvatarUrl(id.ToString()))
        {
            this.id = id;
            this.shortId = this.id.ToString()?.Replace("-", "");
        }

        public override async Task<bool> DownloadAsync()
        {
            //logging
            if (Player.TryGetName(this.id, out string name))
                Debug.Log(Debug.RequestSection, $"{Outgoing} Requested avatar for \"{name}\"");
            else
                Debug.Log(Debug.RequestSection, $"{Outgoing} Requested avatar for {this.shortId}");
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
                Debug.Log(Debug.RequestSection, $"-- Avatar request cancelled for {this.shortId}");
            }
            catch (HttpRequestException e)
            {
                //error getting response, safely move on
                Debug.Log(Debug.RequestSection, $"-- Avatar request failed for {this.shortId}: {e.Message}");
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

                //save a copy for uuid
                string uuidSprite = $"avatar-{this.id}";
                texture.Tag = uuidSprite;
                if (SpriteSheet.ContainsSprite(uuidSprite))
                    SpriteSheet.Replace(uuidSprite, texture);
                else
                    SpriteSheet.Pack(texture);

                SaveToCache(texture, Path.Combine(Paths.System.AvatarCacheFolder, $"avatar-{this.id}.png"));
                
                //save a copy for player name
                if (Player.TryGetName(this.id, out string name))
                {
                    string nameSprite = $"avatar-{name.ToLower()}";
                    texture.Tag = nameSprite;
                    if (SpriteSheet.ContainsSprite(nameSprite))
                        SpriteSheet.Replace(nameSprite, texture);
                    else
                        SpriteSheet.Pack(texture);
                    SaveToCache(texture, Path.Combine(Paths.System.AvatarCacheFolder, $"avatar-{name.ToLower()}.png"));

                    Debug.Log(Debug.RequestSection, $"{Incoming} Received avatar for \"{name}\" in {this.ResponseTime}");
                }
                else
                {
                    Debug.Log(Debug.RequestSection, $"{Incoming} Received avatar for {this.id.String.Replace("-", "")} in {this.ResponseTime}");
                }
                return true;
            }
            catch (ArgumentException)
            {
                //safely ignore malformed stream and move on
                Debug.Log(Debug.RequestSection, $"{Incoming} Received invalid avatar for {this.id.String.Replace("-", "")} in {this.ResponseTime}");
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
