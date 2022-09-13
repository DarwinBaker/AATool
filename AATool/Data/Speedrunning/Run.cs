using System;

namespace AATool.Data.Speedrunning
{
    public class Run
    {
        public Version GameVersion;
        public TimeSpan InGameTime;
        public TimeSpan RealTime;
        public DateTime Date;
        public string Runner;
        public string Status;
        public string Comment;
        public string Link;
        public bool Verifiable;
        public int Blocks;

        public Run(string gameVersion, TimeSpan inGameTime, TimeSpan realTime, DateTime date, 
            string runner, bool verifiable = true, string status = null, string comment = null, string link = null, int blocks = 0)
        {
            Version.TryParse(gameVersion, out this.GameVersion);
            this.InGameTime = inGameTime;
            this.RealTime = realTime;
            this.Date = date;
            this.Runner = runner;
            this.Comment = comment;
            this.Status = status;
            this.Link = link;
            this.Verifiable = verifiable;
            this.Blocks = blocks;
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
                if (!sheet.TryGetIgt(rowIndex, out TimeSpan igt))
                    return false;
                if (!sheet.TryGetDate(rowIndex, out DateTime date))
                    return false;

                //optional columns
                sheet.TryGetRank(rowIndex, out int rank);
                sheet.TryGetRta(rowIndex, out TimeSpan rta);
                sheet.TryGetStatus(rowIndex, out string status);
                sheet.TryGetComment(rowIndex, out string comment);
                sheet.TryGetVerifiability(rowIndex, out bool verifiable);
                sheet.TryGetBlocks(rowIndex, out int blocks);

                pb = new Run(gameVersion, igt, rta, date, runner, verifiable, status, comment, null, blocks);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
