using System;

namespace AATool.Data.Speedrunning
{
    public class Run
    {
        public Version GameVersion { get; set; }
        public TimeSpan InGameTime { get; set; }
        public TimeSpan RealTime { get; set; }
        public DateTime Date { get; set; }
        public string RunnerSrcId { get; set; }
        public string Runner { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public bool Verifiable { get; set; }
        public int ExtraStat { get; set; }

        public string Link { 
            get => this.validatedLink; 
            set {
                const string SpeedrunDotCom = "https://www.speedrun.com/";
                const string YouTubeFull = "https://www.youtube.com/";
                const string YouTubeShort = "https://youtu.be/";
                const string Twitch = "https://www.twitch.tv/";

                if (!string.IsNullOrEmpty(value))
                {
                    bool valid = value.StartsWith(SpeedrunDotCom)
                    || value.StartsWith(YouTubeFull)
                    || value.StartsWith(YouTubeShort)
                    || value.StartsWith(Twitch);

                    if (valid)
                        this.validatedLink = value;
                }
            } 
        }

        string validatedLink;
        Version parsedVersion;

        public Run()
        {
        }

        public Run(string name, TimeSpan igt)
        {
            this.Runner = name;
            this.InGameTime = igt;
        }

        public static bool TryParse(LeaderboardSheet sheet, int rowIndex, string gameVersion, out Run pb)
        {
            pb = null;
            try
            {   
                string[] row = sheet.Rows[rowIndex];
                //required columns
                if (!sheet.TryGetRunner(rowIndex, out string runner))
                    return false;
                if (!sheet.TryGetIgt(rowIndex, out TimeSpan igt) && sheet.Header is not ("1k no reset" or "hardcore no reset"))
                    return false;
                if (!sheet.TryGetDate(rowIndex, out DateTime date) && sheet.Header is not ("1k no reset" or "hardcore no reset"))
                    return false;

                //optional columns
                sheet.TryGetRank(rowIndex, out int rank);
                sheet.TryGetRta(rowIndex, out TimeSpan rta);
                sheet.TryGetStatus(rowIndex, out string status);
                sheet.TryGetComment(rowIndex, out string comment);
                sheet.TryGetVerifiability(rowIndex, out bool verifiable);
                sheet.TryGetExtraStat(rowIndex, out int extraStat);

                if (!sheet.TryGetLink(rowIndex, out string link))
                    Leaderboard.AALinks.TryGetValue((runner, date), out link);

                _= Version.TryParse(gameVersion, out Version version);

                pb = new Run() {
                    GameVersion = version, 
                    InGameTime = igt,
                    RealTime = rta, 
                    Date = date, 
                    Runner = runner, 
                    RunnerSrcId = "",
                    Verifiable = verifiable, 
                    Status = status,
                    Comment = comment, 
                    Link = link, 
                    ExtraStat = extraStat,
                };
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
