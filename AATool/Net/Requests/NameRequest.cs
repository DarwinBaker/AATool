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
        public static int Downloads { get; private set; }

        private readonly Uuid id;
        private readonly string shortId;

        public NameRequest(Uuid id) : base (Paths.Web.GetNameUrl(id.ToString().Replace("-", "")))
        {
            this.id = id;
            this.shortId = this.id.ToString()?.Replace("-", "");
        }

        public override async Task<bool> DownloadAsync()
        {
            Debug.Log(Debug.RequestSection, $"Name requested for UUID: {this.shortId}");
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
                Debug.Log(Debug.RequestSection, $"-- Name request cancelled for UUID: {this.shortId}");
                //request canceled, nothing left to do here
            }
            catch (HttpRequestException e)
            {
                Debug.Log(Debug.RequestSection, $"-- Name request failed for UUID: {this.shortId}: {e.Message}");
                //error getting response, safely move on
            }
            this.EndTiming();
            return false;
        }

        private bool HandleResponse(string response)
        {
            response = response.Trim();
            if (string.IsNullOrEmpty(response) || response.Contains(" "))
                return false;

            Player.Cache(this.id, response);
            Debug.Log(Debug.RequestSection, $"{Incoming} Received name \"{response}\" for UUID: {this.shortId} in {this.ResponseTime}");
            return true;
        }
    }
}
