using System;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Data.Speedrunning;

namespace AATool.Net.Requests
{
    public sealed class AASsgRequest : NetRequest
    {
        public const string SetSeed = "SSG";

        public AASsgRequest() : base (Paths.Web.AASsgRecord) 
        {
        }

        public override async Task<bool> DownloadAsync()
        {
            Debug.Log(Debug.RequestSection, $"Requested AA SSG (1.16) WR from speedrun.com");
            this.BeginTiming();

            using var client = new HttpClient() { 
                Timeout = TimeSpan.FromMilliseconds(Protocol.Requests.TimeoutNormalMs) 
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
                Debug.Log(Debug.RequestSection, $"-- AA SSG (1.16) WR request cancelled");
                //request canceled, nothing left to do here
            }
            catch (HttpRequestException e)
            {
                Debug.Log(Debug.RequestSection, $"-- AA SSG (1.16) WR request failed: {e.Message}");
                //error getting response, safely move on
            }
            this.EndTiming();
            return false;
        }

        private bool HandleResponse(string response)
        {
            response = response?.Trim();
            if (string.IsNullOrEmpty(response))
                return false;

            if (Leaderboard.SyncSpeedrunDotComRecord(response, false, true))
            {
                Leaderboard.SaveSpeedrunDotComRecordToCache(response, false, true);
                Debug.Log(Debug.RequestSection, $"{Incoming} Received AA SSG (1.16) WR from speedrun.com");
                return true;
            }
            else
            {
                Debug.Log(Debug.RequestSection, $"-- Received invalid Any% RSG (1.16) WR data");
                return false;
            }
        }
    }
}
