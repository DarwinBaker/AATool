using System;

namespace AATool.Data.Speedrunning
{
    public class LeaderboardSheet : Spreadsheet
    {
        private readonly int runnerCol;
        private readonly int datesCol;
        private readonly int rankCol;
        private readonly int igtCol;
        private readonly int rtaCol;
        private readonly int commentCol;
        private readonly int statuCol;
        private readonly int linkCol;
        private readonly int verifiableCol;
        private readonly int blocksCol;

        private LeaderboardSheet(string csv, string key, string header) : base (csv, key, header)
        {
            //find column headers
            this.rankCol    = this.Find("#", "place", "rank").X;
            this.runnerCol  = this.Find("runner", "player", "name").X;
            this.igtCol     = this.Find("igt", "ingametime").X;
            this.rtaCol     = this.Find("rta", "realtime").X;
            this.datesCol   = this.Find("date").X;
            this.commentCol = this.Find("comments", "comment", "notes", "note").X;
            this.statuCol   = this.Find("status").X;
            this.linkCol    = this.Find("link").X;
            this.verifiableCol = this.Find("verifiable").X;
            this.blocksCol = this.Find("blocks placed", "blocks").X;

            this.IsValid = this.runnerCol >= 0
                && this.datesCol >= 0
                && this.igtCol >= 0;
        }

        public static bool TryParse(string csv, string key, string header, out LeaderboardSheet sheet)
        {
            sheet = new LeaderboardSheet(csv, key, header);
            return sheet.IsValid;
        }

        public bool TryGetIgt(int index, out TimeSpan igt)
        {
            igt = default;
            return this.TryGetCell(index, this.igtCol, out string timeString)
                && TryParseTimeSpan(timeString, out igt);
        }

        public bool TryGetRta(int index, out TimeSpan rta)
        {
            rta = default;
            return this.TryGetCell(index, this.rtaCol, out string timeString)
                && TryParseTimeSpan(timeString, out rta);
        }
        public bool TryGetRunner(int index, out string runner) =>
            this.TryGetCell(index, this.runnerCol, out runner);

        public bool TryGetRank(int index, out int rank) =>
            this.TryGetCell(index, this.rankCol, out string rankString)
            & int.TryParse(rankString, out rank);

        public bool TryGetDate(int index, out DateTime date) =>
            this.TryGetCell(index, this.datesCol, out string dateString)
            & DateTime.TryParse(dateString, out date);

        public bool TryGetStatus(int index, out string status) =>
            this.TryGetCell(index, this.statuCol, out status);

        public bool TryGetLink(int index, out string status) =>
            this.TryGetCell(index, this.linkCol, out status);

        public bool TryGetComment(int index, out string comment) =>
            this.TryGetCell(index, this.commentCol, out comment);

        public bool TryGetVerifiability(int index, out bool isVerifiable) =>
            this.TryGetCell(index, this.verifiableCol, out string verifiableString)
            & bool.TryParse(verifiableString, out isVerifiable);

        public bool TryGetBlocks(int index, out int blocksPlaced) =>
            this.TryGetCell(index, this.blocksCol, out string blocksString)
            & int.TryParse(blocksString, out blocksPlaced);

        private static bool TryParseTimeSpan(string timeString, out TimeSpan time)
        {
            time = default;
            if (string.IsNullOrEmpty(timeString))
                return false;

            string[] tokens = timeString.Trim().Split(':');
            if (tokens.Length < 3)
                return false;

            if (!int.TryParse(tokens[0], out int hours))
                return false;
            if (!int.TryParse(tokens[1], out int minutes))
                return false;
            if (!int.TryParse(tokens[2], out int seconds))
                return false;

            time = new TimeSpan(hours, minutes, seconds);
            return true;
        }
    }
}
