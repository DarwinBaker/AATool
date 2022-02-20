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
        private readonly Uuid id;

        public AvatarRequest(Uuid id) : base (Paths.Web.GetAvatarUrl(id.ToString()))
        {
            this.id = id;
        }

        private static void SaveToCache(Texture2D texture, string fileName)
        {
            try
            {
                //cache avatar so it loads instantly next launch (overwritten once available to keep skins up to date)
                Directory.CreateDirectory(Paths.System.AvatarCacheFolder);
                using (FileStream fileStream = File.Create(fileName))
                    texture.SaveAsPng(fileStream, texture.Width, texture.Height);
            }
            catch (IOException)
            {
                //couldn't save file. ignore and move on
            }
        }

        public override async Task<bool> DownloadAsync()
        {
            using var client = new HttpClient() {
                Timeout = TimeSpan.FromMilliseconds(Protocol.Requests.TimeoutMs)
            };
            try
            {
                //download texture and add to atlas
                using (Stream response = await client.GetStreamAsync(this.Url))
                    return this.HandleResponse(response);
            }
            catch (OperationCanceledException)
            {
                //request canceled, nothing left to do here
            }
            catch (HttpRequestException)
            {
                //error getting response, safely move on
            }
            return false;
        }

        private bool HandleResponse(Stream avatarStream)
        {
            Texture2D texture = null;
            try
            {
                texture = Texture2D.FromStream(Main.GraphicsManager.GraphicsDevice, avatarStream);
                texture.Tag = this.id.ToString();
                SpriteSheet.AppendAtlas(texture);

                string fileName = Path.Combine(Paths.System.AvatarCacheFolder, this.id + ".png");
                SaveToCache(texture, fileName);
                return true;
            }
            catch (ArgumentException)
            {
                //safely ignore malformed stream and move on
                return false;
            }
            finally
            {
                //compute average color for player-specific glow colors
                Player.Cache(this.id, ColorHelper.GetAccent(texture));
            }
        }
    }
}
