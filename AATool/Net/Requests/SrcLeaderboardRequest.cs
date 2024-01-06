using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Data.Speedrunning;

namespace AATool.Net.Requests
{
    public sealed class SrcLeaderboardRequest : NetRequest
    {
        public static HashSet<(string category, string version)> DownloadedLeaderboards = new ();
        readonly string category;
        readonly string version;

        public SrcLeaderboardRequest(string category, string version) : base(GetLeaderboardUrl(category, version))
        {
            this.category = category;
            this.version = version;
        }

        public override async Task<bool> DownloadAsync()
        {
            Debug.Log(Debug.RequestSection, $"Requested {this.category} {this.version} leaderboard from speedrun.com");
            this.BeginTiming();

            using var client = new HttpClient() {
                Timeout = TimeSpan.FromMilliseconds(Protocol.Requests.TimeoutLongerMs)
            };
            try
            {
                //download leaderboard from speedrun.com as json string
                string response = await client.GetStringAsync(this.Url);
                this.EndTiming();
                if (this.HandleResponse(response))
                {
                    DownloadedLeaderboards.Add((this.category, this.version));
                    return true;
                }
                return false;
            }
            catch (OperationCanceledException)
            {
                Debug.Log(Debug.RequestSection, $"-- {this.category} {this.version} leaderboard request cancelled");
                //request canceled, nothing left to do here
            }
            catch (HttpRequestException e)
            {
                Debug.Log(Debug.RequestSection, $"-- {this.category} {this.version} leaderboard request failed: {e.Message}");
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

            if (Leaderboard.SyncSpeedrunDotComLeaderboard(response, this.category, this.version))
            {
                Leaderboard.SaveSpeedrunDotComLeaderboardToCache(response, this.category, this.version);
                Debug.Log(Debug.RequestSection, $"{Incoming} Received {this.category} ({this.version}) leaderboard from speedrun.com");
                return true;
            }
            else
            {
                Debug.Log(Debug.RequestSection, $"-- Received invalid {this.category} ({this.version}) leaderboard data");
                return false;
            }
        }

        public static string GetLeaderboardUrl(string category, string version, int maxRuns = 100)
        {
            category = category.ToLower();
            version = version.ToLower();
            if (!SeedTypes.TryGetValue(category, out string seedTypeKey))
                return string.Empty;

            if (category is "any% rsg" or "any% ssg")
            {
                if (!AnyPercentVersions.TryGetValue(version, out string versionKey))
                    return string.Empty;

                return $"{Api}/category/{AnyPercentCategoryVar}?top={maxRuns}&embed=players" +
                    $"&var-{AnyPercentVersionVar}={versionKey}" +
                    $"&var-{AnyPercentSeedTypeVar}={seedTypeKey}";
            }
            else if (category is "aadv rsg" or "aadv ssg")
            {
                if (!AADVVersions.TryGetValue(version, out string versionKey))
                    return string.Empty;

                return $"{Api}/category/{AllAdvancementsCategoryVar}?top={maxRuns}&embed=players" +
                    $"&var-{AADVVersionVar}={versionKey}" +
                    $"&var-{AADVSeedTypeVar}={seedTypeKey}";
            }
            else if (category is "aach rsg" or "aach ssg")
            {
                if (!AACHVersions.TryGetValue(version, out string versionKey))
                    return string.Empty;

                return $"{Api}/category/{AllAchievementsCategoryVar}?top={maxRuns}&embed=players" +
                    $"&var-{AACHVersionVar}={versionKey}" +
                    $"&var-{AACHSeedTypeVar}={seedTypeKey}";
            }
            return string.Empty;
        }

        const string Api = "https://www.speedrun.com/api/v1/leaderboards/mc";

        const string AnyPercentCategoryVar = "mkeyl926";
        const string AnyPercentVersionVar = "wl33kewl";
        const string AnyPercentSeedTypeVar = "r8rg67rn";

        const string AllAdvancementsCategoryVar = "xk9gz16d";
        const string AADVVersionVar = "789je4qn";
        const string AADVSeedTypeVar = "p853vv0n";

        const string AllAchievementsCategoryVar = "wk63eek1";
        const string AACHVersionVar = "0nw2y7xn";
        const string AACHSeedTypeVar = "38do09zl";

        public static readonly Dictionary<string, string> AADVVersions = new()
        {
            { "1.20", "1gn9yrnl" },
            { "1.19", "013z703q" },
            { "1.18", "4qy24521" },
            { "1.17", "mlnno7nl" },
            { "1.16", "klrw35m1" },
            { "1.15", "81p687gq" },
            { "1.14", "gq7r9kdl" },
            { "1.13", "xqkm97kl" },
            { "1.12", "81pd4881" },
        };

        public static readonly Dictionary<string, string> AACHVersions = new()
        {
            { "1.8-1.11", "klrzp521" },
            { "1.0-1.6", "jqz6p0m1" },
        };

        public static readonly Dictionary<string, string> AnyPercentVersions = new()
        {
            { "1.16+", "4qye4731" },
            { "1.13-1.15", "21go6e6q" },
            { "1.9-1.12", "jq6j9571" },
            { "1.8", "q5v9ev2l" },
            { "pre-1.8", "gq7zo9p1" },
        };

        public static readonly Dictionary<string, string> SeedTypes = new()
        {
            { "any% rsg", "21d4zvp1" },
            { "any% ssg", "klrzpjo1" },

            { "aadv rsg", "01343grl" },
            { "aadv ssg", "81wr3yml" },
            { "aadv rs", "zqo2d45l" },
            { "aadv ss", "5lmwz58q" },

            { "aach rsg", "4qy8je21" },
            { "aach ssg", "5q8rd731" },
            { "aach rs", "9qjgyzgq" },
            { "aach ss", "810xe551" },
        };
    }
}
