using System;

namespace AATool.Data.Players
{
    public class LeaderboardSheet : Spreadsheet
    {
        private readonly int placesCol;
        private readonly int runnersCol;
        private readonly int datesCol;
        private readonly int timesCol;
        private readonly int commentsCol;
        private readonly int statusCol;

        private LeaderboardSheet(string csv, string key, string header) : base (csv, key, header)
        {
            //find column headers
            this.runnersCol  = this.Find("runner", "player", "name").X;
            this.placesCol   = this.Find("#", "place", "rank").X;
            this.datesCol    = this.Find("date").X;
            this.timesCol    = this.Find("igt", "time", "ingametime").X;
            this.commentsCol = this.Find("comments", "comment", "notes", "note").X;
            this.statusCol   = this.Find("status").X;

            this.IsValid = this.runnersCol >= 0
                && this.placesCol >= 0
                && this.timesCol >= 0;
        }

        public static bool TryParse(string csv, string key, string header, out LeaderboardSheet sheet)
        {
            sheet = new LeaderboardSheet(csv, key, header);
            return sheet.IsValid;
        }

        public bool TryGetRunner(int index, out string runner) =>
            this.TryGetCell(index, this.runnersCol, out runner);

        public bool TryGetTime(int index, out TimeSpan time)
        {
            time = default;
            if (!this.TryGetCell(index, this.timesCol, out string timeString))
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

        public bool TryGetDate(int index, out DateTime date) =>
            this.TryGetCell(index, this.datesCol, out string dateString)
            & DateTime.TryParse(dateString, out date);

        public bool TryGetStatus(int index, out string status) =>
            this.TryGetCell(index, this.statusCol, out status);

        public bool TryGetComment(int index, out string comment) =>
            this.TryGetCell(index, this.commentsCol, out comment);
    }
}
