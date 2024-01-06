using System.Collections.Generic;
using AATool.Data.Speedrunning;
using AATool.Net;

namespace AATool.Data
{
    class SupporterSheet : Spreadsheet
    {
        private readonly int activeCol;
        private readonly int currentTierCol;
        private readonly int highestTierCol;
        private readonly int nameCol;
        private readonly int uuidsCol;
        private readonly int altNamesCol;

        public SupporterSheet(string csv) 
            : base(csv, "supporters")
        {
            this.nameCol = this.Find("name").X;
            this.activeCol = this.Find("active").X;
            this.currentTierCol = this.Find("current tier").X;
            this.highestTierCol = this.Find("highest tier").X;
            this.uuidsCol = this.Find("uuid").X;
            this.altNamesCol = this.Find("alt names").X;

            this.IsValid = this.nameCol >= 0
                && this.activeCol >= 0
                && this.currentTierCol >= 0
                && this.highestTierCol >= 0;
        }

        public static bool TryParse(string csv, out SupporterSheet sheet)
        {
            sheet = new SupporterSheet(csv);
            return sheet.IsValid;
        }

        public void GetCredits(out HashSet<Credit> credits)
        {
            credits = new HashSet<Credit>();
            for (int i = 1; i < this.Rows.Length; i++)
            {
                if (!this.TryGetName(i, out string name))
                    continue;
                if (!this.TryGetActive(i, out bool active))
                    continue;
                if (!this.TryGetHighestTier(i, out string highestTier))
                    continue;

                this.TryGetCurrentTier(i, out string currentTier);
                this.TryGetUuids(i, out List<Uuid> uuids);
                this.TryGetAltNames(i, out List<string> altNames);

                credits.Add(new Credit(highestTier, currentTier, name, altNames, uuids));
            }
        }

        public bool TryGetName(int index, out string name)
        {
            if (this.TryGetCell(index, this.nameCol, out name))
                name = name.Trim();
            return !string.IsNullOrEmpty(name);
        }

        public bool TryGetActive(int index, out bool active)
        {
            if (this.TryGetCell(index, this.activeCol, out string activeString))
                activeString = activeString.Trim();
            return bool.TryParse(activeString, out active);
        }

        public bool TryGetCurrentTier(int index, out string currentTier)
        {
            if (this.TryGetCell(index, this.currentTierCol, out currentTier))
                currentTier = currentTier.Trim();
            return !string.IsNullOrEmpty(currentTier);
        }

        public bool TryGetHighestTier(int index, out string highestTier)
        {
            if (this.TryGetCell(index, this.highestTierCol, out highestTier))
                highestTier = highestTier.Trim();
            return !string.IsNullOrEmpty(highestTier);
        }

        public bool TryGetUuids(int index, out List<Uuid> uuids)
        {
            uuids = new List<Uuid>();

            if (!this.TryGetCell(index, this.uuidsCol, out string uuidListString))
                return false;

            foreach (string item in uuidListString.Split(' '))
            {
                if (Uuid.TryParse(item, out Uuid uuid))
                    uuids.Add(uuid);
            }
            return uuids.Count > 0;
        }

        public bool TryGetAltNames(int index, out List<string> altNames)
        {
            altNames = new List<string>();

            if (!this.TryGetCell(index, this.altNamesCol, out string altNameListString))
                return false;

            foreach (string item in altNameListString.Split(' '))
            {
                altNames.Add(item.Trim());
            }
            return altNames.Count > 0;
        }
    }
}
