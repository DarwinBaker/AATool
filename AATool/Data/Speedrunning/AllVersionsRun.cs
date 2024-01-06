using System;

namespace AATool.Data.Speedrunning
{
    internal class AllVersionsRun : Run
    {
        public string Range { get; set; }
        public bool Hardcore { get; set; }
        public bool NoF3 { get; set; }

        public static new bool TryParse(LeaderboardSheet sheet, int rowIndex, string gameVersion, out Run pb)
        {
            pb = null;
            try
            {
                string[] row = sheet.Rows[rowIndex];
                //required columns
                if (!sheet.TryGetRunner(rowIndex, out string runner))
                    return false;

                //optional columns
                sheet.TryGetRta(rowIndex, out TimeSpan rta);
                sheet.TryGetDate(rowIndex, out DateTime date);
                sheet.TryGetRank(rowIndex, out int rank);

                sheet.TryGetRange(rowIndex, out string range);
                sheet.TryGetIsNoF3(rowIndex, out bool noF3);
                sheet.TryGetIsHardcore(rowIndex, out bool hardcore);

                if (!sheet.TryGetLink(rowIndex, out string link))
                    Leaderboard.AALinks.TryGetValue((runner, date), out link);

                _= Version.TryParse(gameVersion, out Version version);

                pb = new AllVersionsRun() {
                    GameVersion = version,
                    RealTime = rta,
                    Date = date,
                    Runner = runner,
                    Link = link,
                    Range = range,
                    NoF3 = noF3,
                    Hardcore = hardcore,
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
