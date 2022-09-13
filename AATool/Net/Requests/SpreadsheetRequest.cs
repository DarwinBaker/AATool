using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Data.Speedrunning;

namespace AATool.Net.Requests
{
    public sealed class SpreadsheetRequest : NetRequest
    { 
        public static int Downloads { get; private set; }

        public static HashSet<(string sheetId, string pageId)> DownloadedPages = new ();
        private readonly string sheet;
        private readonly string page;
        private readonly string name;

        public SpreadsheetRequest(string name, string sheet, string page = "0") : base (Paths.Web.GetSpreadsheetUrl(sheet, page))
        {
            this.name = name;
            this.sheet = sheet;
            this.page = page;
        }

        public override async Task<bool> DownloadAsync()
        {
            //logging
            Debug.Log(Debug.RequestSection, $"{Outgoing} Requested spreadsheet for {this.name}");
            this.BeginTiming();
            Downloads++;

            using var client = new HttpClient() { 
                Timeout = TimeSpan.FromMilliseconds(Protocol.Requests.TimeoutMs) 
            };
            try
            {
                //download leaderboard spreadsheet as csv string
                string response = await client.GetStringAsync(this.Url);
                this.EndTiming();
                if (this.HandleResponse(response))
                {
                    DownloadedPages.Add((this.sheet, this.page));
                    return true;
                }
                return false;
            }
            catch (OperationCanceledException)
            {
                //request canceled, nothing left to do here
                Debug.Log(Debug.RequestSection, $"-- Spreadsheet request cancelled for {this.sheet}:{this.page}");
            }
            catch (HttpRequestException e)
            {
                //error getting response, safely move on
                Debug.Log(Debug.RequestSection, $"-- Spreadsheet request failed for {this.sheet}:{this.page}: {e.Message}");
            }
            this.EndTiming();
            return false;
        }

        private bool HandleResponse(string csv)
        {
            if (this.sheet is Paths.Web.NicknameSheet)
            {
                Debug.Log(Debug.RequestSection, $"{Incoming} Received nickname spreadsheet in {this.ResponseTime}");
                return Leaderboard.SyncNicknames(csv);
            }
            else if (this.page == Paths.Web.PrimaryAAHistory)
            {
                Debug.Log(Debug.RequestSection, $"{Incoming} Received submission history spreadsheet in {this.ResponseTime}");
                return Leaderboard.SyncHistory(csv);
            }
            else
            {
                Debug.Log(Debug.RequestSection, $"{Incoming} Received spreadsheet {this.name} in {this.ResponseTime}");
                return Leaderboard.SyncRecords(this.sheet, this.page, csv);
            }
        }
    }
}
