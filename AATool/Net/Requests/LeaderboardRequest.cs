using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Data;

namespace AATool.Net.Requests
{
    public sealed class LeaderboardRequest : NetRequest
    {
        public const int MaxShown = 6;
        public static List<PersonalBest> Runs = new ();

        public static int PlaceIndex = -1;
        public static int DateIndex = -1;
        public static int NameIndex = -1;
        public static int TimeIndex = -1;
        public static int StatusIndex = -1;
        public static int CommentIndex = -1;

        public static bool Downloaded = false;

        public LeaderboardRequest() : base (Paths.Web.UnofficialSpreadsheet)
        {
            this.TryLoadFromCache();
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
                return Downloaded = this.HandleResponse(response);
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

        private bool HandleResponse(string sheet)
        {
            if (string.IsNullOrEmpty(sheet))
                return false;

            string[] rows = sheet.Split('\n');
            if (rows.Length < 2)
                return false;

            string header = rows[1];
            Runs.Clear();
            for (int i = 2; i < rows.Length; i++)
            {
                if (PersonalBest.TryParse(rows[i], header, out PersonalBest pb))
                    Runs.Add(pb);

                if (Runs.Count <= MaxShown)
                    Player.FetchIdentity(pb.Runner);
            }
            SaveToCache(sheet);
            return Runs.Any();
        }

        private void TryLoadFromCache()
        {
            try
            {
                this.HandleResponse(File.ReadAllText(Paths.System.LeaderboardFile));
            }
            catch 
            { 
                //couldn't read cached leaderboard (probably not downloaded yet)
            }
        }

        private static void SaveToCache(string spreadsheet)
        {
            try
            {
                //cache leaderboard so it loads instantly next launch
                //overwrite to keep leaderboard up to date
                Directory.CreateDirectory(Paths.System.LeaderboardsFolder);
                File.WriteAllText(Paths.System.LeaderboardFile, spreadsheet);
            }
            catch
            {
                //couldn't save file. ignore and move on
            }
        }
    }
}
