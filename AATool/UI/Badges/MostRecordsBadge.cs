using AATool.Data.Speedrunning;
using AATool.UI.Controls;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class MostRecordsBadge : Badge
    {
        public MostRecordsBadge()
        {
            this.FlexWidth = new (32);
            this.FlexHeight = new (15);
            this.Margin = new Margin(-9, -0, -7, 0);

            this.PopupBackColor = RankBadge.PopupGoldBack;
            this.PopupBorderColor = RankBadge.PopupGoldBorder;
            this.PopupTextColor = RankBadge.PopupGoldText;

            string versionList = string.Empty;
            for (int i = 0; i < Leaderboard.ListOfMostRecords.Count; i++)
            {
                versionList += Leaderboard.ListOfMostRecords[i].GameVersion;
                if (i < Leaderboard.ListOfMostRecords.Count - 1)
                    versionList += ", ";
            }

            this.Description.SetTextColor(RankBadge.PopupGoldText);
            this.Description.SetText($"Holds the most AA records\n ({versionList})");

            this.BackTexture = $"badge_most_records";
            this.Glow.SetTexture("badge_large_gold_glow");
            this.Glow.SkipToBrightness(0.75f);

            var label = new UITextBlock() {
                Layer = Layer.Fore,
            };
            label.SetText($"{Leaderboard.ListOfMostRecords.Count}xWR");
            label.SetTextColor(Color.White);
            this.AddControl(label);
        }
    }
}
