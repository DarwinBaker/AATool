using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AATool.Data.Players
{
    public abstract class Spreadsheet
    {
        private static Dictionary<string, string[][]> AllSheets = new ();
        private static Dictionary<string, string> AllRaw = new ();

        public const char LineBreak = '\n';
        public const char Delimiter = ',';
        public const int HeaderHeight = 2;

        public string Key { get; private set; }
        public string Header { get; private set; }
        public Point TopLeft { get; private set; }
        public bool IsValid  { get; protected set; }

        public string[][] Rows => AllSheets[this.Key];
        public string RawCsv => AllRaw[this.Key];

        protected Spreadsheet(string csv, string key, string header = null) 
        {
            this.Key = key;
            this.Header = header;

            var rows = csv
                .Split(LineBreak)
                .Select(x => x.Split(','))
                .ToArray();
            AllSheets[this.Key] = rows;
            AllRaw[this.Key] = csv;

            //find and lock onto a header anywhere on the sheet
            if (!string.IsNullOrEmpty(header))
                this.TopLeft = this.Find(this.Header);
        }

        protected bool TryGetRow(int row, out string[] cells)
        {
            int adjustedIndex = row + this.TopLeft.Y;
            if (adjustedIndex >= 0 && adjustedIndex < this.Rows.Length)
            {
                cells = this.Rows[adjustedIndex];
                return true;
            }
            cells = null;
            return false;
        }

        protected bool TryGetCell(int row, int col, out string value)
        {
            value = string.Empty;
            if (this.IsValid && col >= 0 && this.TryGetRow(row, out string[] cells))
                value = cells[col];
            return !string.IsNullOrEmpty(value);
        }

        protected Point Find(params string[] targets)
        {
            for (int r = this.TopLeft.Y; r < this.Rows.Length; r++)
            {
                if (r < 0)
                    break;

                string[] cells = this.Rows[r];
                for (int c = this.TopLeft.X; c < cells.Length; c++)
                {
                    if (targets.Contains(cells[c].Trim().ToLower()))
                        return new(c, r);
                }
            }
            return new (-1, -1);
        }

        public void SaveToCache()
        {
            try
            {
                //cache leaderboard so it loads instantly next launch
                //overwrite to keep leaderboard up to date
                Directory.CreateDirectory(Paths.System.LeaderboardsFolder);
                string path = Paths.System.LeaderboardFile(this.Key);
                File.WriteAllText(path, this.RawCsv);
            }
            catch
            {
                //couldn't save file. ignore and move on
            }
        }
    }
}
