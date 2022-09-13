using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AATool.Net.Requests
{
    public sealed class UuidRequest : NetRequest
    {
        public static int Downloads { get; private set; }

        private readonly string name;
        private readonly bool requestAvatar;

        public UuidRequest(string name, bool requestAvatar = false) : base (Paths.Web.GetUuidUrl(name))
        {
            this.name = name;
            this.requestAvatar = requestAvatar;
        }

        public override async Task<bool> DownloadAsync()
        {
            //logging
            Debug.Log(Debug.RequestSection, $"{Outgoing} Requested UUID for \"{this.name}\"");
            Downloads++;
            this.BeginTiming();

            using var client = new HttpClient() { 
                Timeout = TimeSpan.FromMilliseconds(Protocol.Requests.TimeoutMs) 
            };
            try
            {
                //get minecraft name and add to cache
                string response = await client.GetStringAsync(this.Url);
                this.EndTiming();
                return this.HandleResponse(response);
            }
            catch (OperationCanceledException)
            {
                //request canceled, nothing left to do here
                Debug.Log(Debug.RequestSection, $"-- UUID request cancelled for \"{this.name}\"");
            }
            catch (HttpRequestException e)
            {
                //error getting response, safely move on
                Debug.Log(Debug.RequestSection, $"-- UUID request failed for \"{this.name}\": {e.Message}");
            }
            this.EndTiming();
            return false;
        }

        private bool HandleResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            if (Uuid.TryParse(response, out Uuid id))
            {
                Debug.Log(Debug.RequestSection, $"{Incoming} Received UUID for \"{this.name}\" ({response}) in {this.ResponseTime}");

                Player.Cache(id, this.name);
                if (this.requestAvatar)
                    new AvatarRequest(id).EnqueueOnce();
                return true;
            }
            else
            {
                Debug.Log(Debug.RequestSection, $"{Incoming} Received invalid UUID for \"{this.name}\" ({response}) in {this.ResponseTime}");
            }
            return false;
        }
    }
}
