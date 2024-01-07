using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.Net.Requests;
using Newtonsoft.Json;

namespace AATool.Data.Speedrunning
{
    public class LeaderboardSrcJson
    {
        public List<Run> Runs = new();

        private LeaderboardSrcJson(List<Run> runs)
        {
            this.Runs = runs;
        }

        public static bool TryParse(string json, string version, out LeaderboardSrcJson parsed)
        {
            parsed = null;
            try
            {
                dynamic data = JsonConvert.DeserializeObject(json);
                dynamic players = data["data"]["players"]["data"];
                //var playerNames = new Dictionary<string, string>();
                foreach (dynamic player in players)
                {
                    if (player["id"]?.Value is not string id)
                        continue;
                    if (player["names"]?["international"]?.Value is not string name)
                        continue;
                    RunnerProfile.NamesBySrcId[id] = name;
                    RunnerProfile.SrcIdsByName[name] = id;
                }

                dynamic runs = data["data"]["runs"];
                var runList = new List<Run>();
                foreach (dynamic runRoot in runs)
                {
                    try
                    {
                        dynamic run = runRoot["run"];
                        string id = run["players"][0]["id"].Value;
                        if (!RunnerProfile.NamesBySrcId.TryGetValue(id, out string name))
                            name = "<error>";

                        _= DateTime.TryParse(run["date"].Value, out DateTime date);

                        if (run["times"]["ingame_t"].Value is not double igtSeconds)
                            igtSeconds = ParseTimeString(run["times"]["ingame"].Value);

                        if (run["times"]["realtime_t"].Value is not double rtaSeconds)
                            rtaSeconds = ParseTimeString(run["times"]["realtime"].Value);

                        var igt = TimeSpan.FromSeconds(igtSeconds);
                        var rta = TimeSpan.FromSeconds(rtaSeconds);

                        string status = run["status"]["status"].Value;
                        string comment = run["comment"].Value;

                        /*
                        dynamic links = run["videos"]["links"];
                        string link = links.Count > 0
                        ? links[0]["uri"].Value
                        : string.Empty;
                        */

                        string link = run["weblink"].Value;

                        _= Version.TryParse(version, out Version gameVersion);

                        runList.Add(new Run() {
                            GameVersion = gameVersion, 
                            InGameTime = igt, 
                            RealTime = rta, 
                            Date = date, 
                            Runner = name,
                            RunnerSrcId = id, 
                            Verifiable = true, 
                            Status = status, 
                            Comment = comment, 
                            Link = link
                        });
                    }
                    catch (Exception e)
                    {

                    }
                }

                parsed = new(runList);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static double ParseTimeString(string value)
        {
            if (value is null)
                return 0;

            value = value.Replace("PT", "");
            int minuteEnd = value.IndexOf("M");
            string secondsString = value.Replace("S", "").Substring(minuteEnd + 1);
            string minutesString = value.Substring(0, minuteEnd);

            _= double.TryParse(secondsString, out double seconds);
            if (double.TryParse(minutesString, out double minutes))
                seconds += minutes * 60;

            return seconds;
        }
    }
}
