using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        public override async Task<bool> TryRunAsync()
        {
            try
            {
                using (var client = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(Protocol.CONNECTION_TIMEOUT_MS) })
                {
                    //try to pull minecraft name from mojang api
                    string response = await client.GetStringAsync(this.Url);
                    if (string.IsNullOrEmpty(response))
                        return false;

                    string name = JArray.Parse(response).Last.First.Values().First().ToString();
                    if (string.IsNullOrEmpty(name))
                        return false;

                    Player.Cache(this.id, name);
                    return true;
                }
            }
            catch (OperationCanceledException)
            {
                //request canceled
                return false;
            }
        }
    }
}
