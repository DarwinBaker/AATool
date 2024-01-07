using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Data.Speedrunning;
using AATool.UI.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.Net.Requests
{
    public sealed class SrcProfileRequest : NetRequest
    {
        public static int Downloads { get; private set; }

        private readonly string idOrName;

        public SrcProfileRequest(string idOrName) : base(Paths.Web.GetSpeedrunDotComProfileUrl(idOrName))
        {
        }

        public override async Task<bool> DownloadAsync()
        {
            //logging
            Debug.Log(Debug.RequestSection, $"{Outgoing} Requested profile for {this.idOrName} from speedrun.com");
            Downloads++;
            this.BeginTiming();

            using var client = new HttpClient() {
                Timeout = TimeSpan.FromMilliseconds(Protocol.Requests.TimeoutNormalMs)
            };
            try
            {
                string response = await client.GetStringAsync(this.Url);
                this.EndTiming();

                bool success = this.HandleResponse(response, out RunnerProfile profile);
                if (success)
                {
                    try
                    {
                        string avatarUrl = Paths.Web.GetSpeedrunDotComPictureUrl(profile.Id);
                        using (Stream imageStream = await client.GetStreamAsync(avatarUrl))
                        {
                            var picture = Texture2D.FromStream(Main.GraphicsManager.GraphicsDevice, imageStream);
                            string cacheFile = Paths.System.SpeedrunDotComProfilePicture(profile.Id);
                            Directory.CreateDirectory(Paths.System.ProfilePicturesCacheFolder);
                            using (FileStream fileStream = File.Create(cacheFile))
                                picture.SaveAsPng(fileStream, picture.Width, picture.Height);
                            profile.Picture = picture;
                        }
                    }
                    catch
                    {
                    }
                }
                return success;
            }
            catch (OperationCanceledException)
            {
                //request canceled, nothing left to do here
                Debug.Log(Debug.RequestSection, $"-- Profile request cancelled for \"{this.idOrName}\"");
            }
            catch (HttpRequestException e)
            {
                //error getting response, safely move on
                Debug.Log(Debug.RequestSection, $"-- Profile request failed for \"{this.idOrName}\": {e.Message}");
            }
            this.EndTiming();
            return false;
        }

        private bool HandleResponse(string response, out RunnerProfile profile)
        {
            profile = null;
            if (string.IsNullOrEmpty(response))
                return false;

            if (RunnerProfile.TryParseSrc(response, true, out profile))
            {
                Debug.Log(Debug.RequestSection, $"{Incoming} Received profile for \"{this.idOrName}\" ({response}) in {this.ResponseTime}");
                return true;
            }
            else
            {
                Debug.Log(Debug.RequestSection, $"{Incoming} Received invalid UUID for \"{this.idOrName}\" ({response}) in {this.ResponseTime}");
            }
            return false;
        }
    }
}
