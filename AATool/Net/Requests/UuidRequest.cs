using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AATool.Net.Requests
{
    public sealed class UuidRequest : NetRequest
    {
        private readonly string name;
        private readonly bool requestAvatar;

        public UuidRequest(string name, bool requestAvatar = false) : base (Paths.Web.GetUuidUrl(name))
        {
            this.name = name;
            this.requestAvatar = requestAvatar;
        }

        public override async Task<bool> DownloadAsync()
        {
            using var client = new HttpClient() { 
                Timeout = TimeSpan.FromMilliseconds(Protocol.Requests.TimeoutMs) 
            };
            try
            {
                //get minecraft name and add to cache
                string response = await client.GetStringAsync(this.Url);
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

        private bool HandleResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            string uuid = JObject.Parse(response)["id"].ToString();
            if (Uuid.TryParse(uuid, out Uuid id))
            {
                Player.Cache(id, this.name);
                if (this.requestAvatar)
                    new AvatarRequest(id).EnqueueOnce();
                return true;
            }
            return false;
        }
    }
}
