using AATool.Data.Speedrunning;
using AATool.UI.Controls;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class MostConsecutiveRecordsBadge : Badge
    {
        public override string GetListName => "Consecutive WRs";

        public MostConsecutiveRecordsBadge()
        {
            this.FlexWidth = new (38);
            this.FlexHeight = new (15);
            this.Margin = new Margin(-14, -0, -7, 0);

            this.PopupBackColor = RankBadge.PopupGoldBack;
            this.PopupBorderColor = RankBadge.PopupGoldBorder;
            this.PopupTextColor = RankBadge.PopupGoldText;

            this.Description.SetTextColor(RankBadge.PopupGoldText);
            this.Description.SetText($"Most consecutive AA WRs\n ({Leaderboard.MostConsecutiveRecordsCount} back-to-back)");

            this.BackTexture = $"badge_most_records_wide";
            this.Glow.SetTexture("badge_large_gold_glow");
            this.Glow.SkipToBrightness(0.75f);

            var label = new UITextBlock() {
                Layer = Layer.Fore,
            };
            label.SetText($"{Leaderboard.MostConsecutiveRecordsCount}xWR");
            label.SetTextColor(Color.White);
            this.AddControl(label);
        }
    }
}
