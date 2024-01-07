using System;

namespace AATool.Data.Speedrunning
{
    internal class HardcoreStreak : Run
    {
        public string Runs { get; set; }
        public string Deaths { get; set; }
        public int BestStreak { get; set; }
        public bool OnBestStreak { get; set; }

        public static bool TryParse(LeaderboardSheet sheet, int rowIndex, string gameVersion, out HardcoreStreak pb)
        {
            pb = null;
            try
            {
                string[] row = sheet.Rows[rowIndex];
                //required columns
                if (!sheet.TryGetRunner(rowIndex, out string runner))
                    return false;

                //optional columns
                sheet.TryGetIgt(rowIndex, out TimeSpan igt);
                sheet.TryGetDate(rowIndex, out DateTime date);
                sheet.TryGetRank(rowIndex, out int rank);
                sheet.TryGetRuns(rowIndex, out string runs);
                sheet.TryGetDeaths(rowIndex, out string deaths);
                sheet.TryGetStreak(rowIndex, out int streak);
                sheet.TryGetOnBestStreak(rowIndex, out bool onBestStreak);

                if (!sheet.TryGetLink(rowIndex, out string link))
                    Leaderboard.AALinks.TryGetValue((runner, date), out link);

                _= Version.TryParse(gameVersion, out Version version);

                pb = new HardcoreStreak() {
                    GameVersion = version,
                    InGameTime = igt,
                    Date = date,
                    Runner = runner,
                    Link = link,
                    Runs = runs,
                    Deaths = deaths,
                    BestStreak = streak,
                    OnBestStreak = onBestStreak,
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
