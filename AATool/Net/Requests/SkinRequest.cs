using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Graphics;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace AATool.Net.Requests
{
    public sealed class SkinRequest : NetRequest
    {
        private readonly Uuid id;

        public SkinRequest(Uuid id) : base (Paths.GetUrlForPlayerHead(id.ToString()))
        {
            this.id = id;
        }

        public override async Task<bool> RunAsync()
        {
            string fileName = Path.Combine(Paths.DIR_SKIN_CACHE, this.id + ".png");
            if (!SpriteSheet.TryGetTextureFromFile(fileName, out Texture2D texture))
            {
                using (HttpClient client = new() { Timeout = TimeSpan.FromMilliseconds(Protocol.TIMEOUT_MS) })
                using (Stream stream = await client.GetStreamAsync(this.Url))
                {
                    //load texture and add to atlas
                    texture = Texture2D.FromStream(Main.Graphics.GraphicsDevice, stream);
                    texture.Tag = this.id.ToString();
                    SpriteSheet.AppendAtlas(texture);

                    //cache texture in local directory
                    Directory.CreateDirectory(Paths.DIR_SKIN_CACHE);
                    using (FileStream fileStream = File.Create(fileName))
                        texture.SaveAsPng(fileStream, texture.Width, texture.Height);
                }
            }

            //compute player accent color
            Player.Cache(this.id, ColorHelper.GetAccent(texture));
            return true;
        }
    }
}
