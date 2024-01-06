using AATool.Data.Speedrunning;
using AATool.UI.Controls;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class MostConcurrentRecordsBadge : Badge
    {
        public override string GetListName => "Concurrent WRs";

        public MostConcurrentRecordsBadge()
        {
            this.FlexWidth = new (32);
            this.FlexHeight = new (15);
            this.Margin = new Margin(-8, -0, -7, 0);

            this.PopupBackColor = RankBadge.PopupGoldBack;
            this.PopupBorderColor = RankBadge.PopupGoldBorder;
            this.PopupTextColor = RankBadge.PopupGoldText;

            string versionList = string.Empty;
            for (int i = 0; i < Leaderboard.ListOfMostConcurrentRecords.Count; i++)
            {
                versionList += Leaderboard.ListOfMostConcurrentRecords[i].GameVersion;
                if (i < Leaderboard.ListOfMostConcurrentRecords.Count - 1)
                    versionList += ", ";
            }

            this.Description.SetTextColor(RankBadge.PopupGoldText);
            this.Description.SetText($"Most concurrent AA WRs\n ({versionList})");

            this.BackTexture = $"badge_most_records";
            this.Glow.SetTexture("badge_large_gold_glow");
            this.Glow.SkipToBrightness(0.75f);

            var label = new UITextBlock() {
                Layer = Layer.Fore,
            };
            label.SetText($"{Leaderboard.ListOfMostConcurrentRecords.Count}xWR");
            label.SetTextColor(Color.White);
            this.AddControl(label);
        }
    }
}
