using System;
using AATool.Net;
using AATool.Net.Requests;

namespace AATool.Data.Players
{
    public class PersonalBest
    {
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

        public static bool TryParse(LeaderboardSheet sheet, int rowIndex, out PersonalBest pb)
        {
            pb = null;
            try
            {
                string[] row = sheet.Rows[rowIndex];
                if (!sheet.TryGetTime(rowIndex, out TimeSpan time))
                    return false;
                if (!sheet.TryGetDate(rowIndex, out DateTime date))
                    return false;
                if (!sheet.TryGetRunner(rowIndex, out string runner))
                    return false;

                sheet.TryGetStatus(rowIndex, out string status);
                sheet.TryGetComment(rowIndex, out string comment);

                pb = new PersonalBest(rowIndex, time, date, runner, status, comment);
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
