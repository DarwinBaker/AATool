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
        private readonly int extraStatCol;
        private readonly int runsCol;
        private readonly int deathsCol;
        private readonly int streakCol;
        private readonly int onBestStreakCol;
        private readonly int isHardcoreCol;
        private readonly int noF3Col;
        private readonly int rangeCol;

        private LeaderboardSheet(string csv, string key, string header) : base (csv, key, header)
        {
            //find column headers
            this.rankCol    = this.Find("#", "place", "rank").X;
            this.runnerCol  = this.Find("runner", "player", "name").X;
            this.igtCol     = this.Find("igt", "ingametime", "average igt").X;
            this.rtaCol     = this.Find("rta", "realtime").X;
            this.datesCol   = this.Find("date").X;
            this.commentCol = this.Find("comments", "comment", "notes", "note").X;
            this.statuCol   = this.Find("status").X;
            this.linkCol    = this.Find("link").X;
            this.verifiableCol = this.Find("verifiable").X;
            this.extraStatCol = this.Find("blocks placed", "blocks", "total runs").X;
            this.runsCol = this.Find("total runs").X;
            this.deathsCol = this.Find("deaths").X;
            this.streakCol = this.Find("best streak").X;
            this.onBestStreakCol = this.Find("on best streak").X;
            this.isHardcoreCol = this.Find("hardcore").X;
            this.noF3Col = this.Find("no f3").X;
            this.rangeCol = this.Find("range").X;

            this.IsValid = this.runnerCol >= 0
                && this.datesCol >= 0
                && (this.igtCol >= 0 || header is "1k no reset" or "all versions");
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

        public bool TryGetExtraStat(int index, out int extraStat) =>
            this.TryGetCell(index, this.extraStatCol, out string extraStatString)
            & int.TryParse(extraStatString, out extraStat);

        public bool TryGetRuns(int index, out string runs) =>
            this.TryGetCell(index, this.runsCol, out runs);

        public bool TryGetDeaths(int index, out string deaths) =>
            this.TryGetCell(index, this.deathsCol, out deaths);

        public bool TryGetStreak(int index, out int streak) =>
            this.TryGetCell(index, this.streakCol, out string streakString)
            & int.TryParse(streakString, out streak);

        public bool TryGetOnBestStreak(int index, out bool onBestStreak) =>
            this.TryGetCell(index, this.onBestStreakCol, out string streakString)
            & bool.TryParse(streakString, out onBestStreak);

        public bool TryGetIsHardcore(int index, out bool isHardcore) =>
            this.TryGetCell(index, this.isHardcoreCol, out string isHardcoreString)
            & bool.TryParse(isHardcoreString, out isHardcore);

        public bool TryGetIsNoF3(int index, out bool isNoF3) =>
            this.TryGetCell(index, this.noF3Col, out string isNoF3String)
            & bool.TryParse(isNoF3String, out isNoF3);

        public bool TryGetRange(int index, out string range) =>
            this.TryGetCell(index, this.rangeCol, out range);

        private static bool TryParseTimeSpan(string timeString, out TimeSpan time)
        {
            time = default;
            if (string.IsNullOrEmpty(timeString))
                return false;

            string[] tokens = timeString.Trim().Split(':');

            int h = 0;
            int m = 0;
            int s = 0;
            if (tokens.Length is 3)
            {
                if (!int.TryParse(tokens[0], out h))
                    return false;
                if (!int.TryParse(tokens[1], out m))
                    return false;
                if (!int.TryParse(tokens[2], out s))
                    return false;
            }
            else if (tokens.Length is 2)
            {
                if (!int.TryParse(tokens[0], out m))
                    return false;
                if (!int.TryParse(tokens[1], out s))
                    return false;
            }

            time = new TimeSpan(h, m, s);
            return true;
        }
    }
}
