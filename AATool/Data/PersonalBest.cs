using System;
using AATool.Net;
using AATool.Net.Requests;

namespace AATool.Data
{
    public struct PersonalBest
    {
        public static int RunnerIndex = -1;
        public static int PlaceIndex = -1;
        public static int DateIndex = -1;
        public static int TimeIndex = -1;
        public static int CommentIndex = -1;
        public static int StatusIndex = -1;

        private static bool HeaderValidated = false;

        public int Place;
        public TimeSpan InGameTime;
        public DateTime Date;
        public string Runner;
        public string Status;
        public string Comment;

        public PersonalBest(int place, TimeSpan inGameTime, DateTime date, string runner, string status, string comment)
        {
            this.Place = place;
            this.InGameTime = inGameTime;
            this.Date = date;
            this.Runner = runner;
            this.Comment = comment;
            this.Status = status switch {
                "verified" => "Verified",
                "notsubmitted" => "Not Submitted",
                _ => "Verifying",
            };
        }

        public static bool TryParse(string row, string header, out PersonalBest pb)
        {
            pb = default;
            if (string.IsNullOrEmpty(row) || !ValidateHeader(header))
                return false;

            try
            {
                string[] csv = row.Split(',');
                if (!int.TryParse(csv[PlaceIndex], out int place))
                    return false;
                if (!TimeSpan.TryParse(csv[TimeIndex], out TimeSpan igt))
                    return false;
                if (!DateTime.TryParse(csv[DateIndex], out DateTime date))
                    return false;

                string runner = csv[RunnerIndex];
                string status = csv[StatusIndex].Replace(" ", "").ToLower();
                string comment = csv[CommentIndex];
                pb = new PersonalBest(place, igt, date, runner, status, comment);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        private static bool ValidateHeader(string row)
        {
            if (HeaderValidated)
                return true;

            if (string.IsNullOrEmpty(row))
                return false;

            string[] header = row
                .Replace(" ", "")
                .Replace("-", "")
                .ToLower()
                .Split(',');

            for (int i = 0; i < header.Length; i++)
            {
                switch (header[i].Trim())
                {
                    case "#":
                    case "place":
                    case "rank":
                        PlaceIndex = i;
                        break;
                    case "date":
                        DateIndex = i;
                        break;
                    case "runner":
                    case "player":
                    case "name":
                        RunnerIndex = i;
                        break;
                    case "igt":
                    case "time":
                    case "ingametime":
                        TimeIndex = i;
                        break;
                    case "status":
                        StatusIndex = i;
                        break;
                    case "comments":
                    case "comment":
                    case "notes":
                    case "note":
                        CommentIndex = i;
                        break;
                }
            }
            return HeaderValidated = true;
        }
    }
}
