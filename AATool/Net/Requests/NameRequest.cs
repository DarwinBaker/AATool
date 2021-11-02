using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AATool.Net.Requests
{
    public sealed class NameRequest : NetRequest
    {
        private readonly Uuid id;

        public NameRequest(Uuid id) : base (Paths.GetUrlForName(id.ToString()))
        {
            this.id = id;
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

        private bool HandleResponse(string nameHistory)
        {
            try
            {
                if (string.IsNullOrEmpty(nameHistory))
                    return false;

                string name = JArray.Parse(nameHistory).Last.First.Values().First().ToString();
                Player.Cache(this.id, name);
                return true;
            }
            catch (Exception e)
            {
                //safely ignore malformed reponse and move on
                if (e is ArgumentException or InvalidOperationException or JsonReaderException)  
                    return false;
                else
                    throw;
            }
        }
    }
}
