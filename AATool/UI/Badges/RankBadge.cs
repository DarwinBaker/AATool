using AATool.Configuration;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Controls;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class RankBadge : Badge
    {
        private readonly int rank;
        private readonly string category; 
        private readonly string version;
        private readonly bool claimed;

        public static readonly Color PopupGoldBack = new (123, 88, 34);
        public static readonly Color PopupGoldBorder = new (254, 224, 72);
        public static readonly Color PopupGoldText = new (255, 255, 230);

        public static readonly Color PopupSilverBack = new (107, 113, 128);
        public static readonly Color PopupSilverBorder = new (181, 191, 212);
        public static readonly Color PopupSilverText = new (255, 255, 255);

        public static readonly Color PopupCopperBack = new (117, 65, 59);
        public static readonly Color PopupCopperBorder = new (228, 134, 94);
        public static readonly Color PopupCopperText = new (255, 225, 194);

        public RankBadge(int rank, string category, string version, bool claimed = true, string variant = null)
        {
            this.rank = rank;
            this.category = category;
            this.version = version;
            this.claimed = claimed;

            this.FlexHeight = new (15);
            this.Margin = new Margin(-9, -0, -7, 0);

            string place = Leaderboard.GetPlace(rank);
            if (claimed)
                this.Description.SetText($"{place} Place\n{this.category} ({this.version})");
            else
                this.Description.SetText($"Unclaimed {place} Place\nLeaderboard Slot");

            if (rank <= 3)
            {
                this.PopupBackColor = this.rank switch {
                    1 => PopupGoldBack,
                    2 => PopupSilverBack,
                    3 => PopupCopperBack,
                    _ => default
                };
                this.PopupBorderColor = this.rank switch {
                    1 => PopupGoldBorder,
                    2 => PopupSilverBorder,
                    3 => PopupCopperBorder,
                    _ => default
                };
                this.PopupTextColor = this.rank switch {
                    1 => PopupGoldText,
                    2 => PopupSilverText,
                    3 => PopupCopperText,
                    _ => default
                };

                if (rank is 1)
                    this.Description.SetTextColor(PopupGoldText);
                else if (rank is 2)
                    this.Description.SetTextColor(PopupSilverText);
                else
                    this.Description.SetTextColor(PopupCopperText);

                this.FlexWidth = new (25);
                this.BackTexture = $"badge_rank_{this.rank}";
                this.Glow.SetTexture($"badge_rank_{this.rank}_glow");
                this.Glow.SkipToBrightness(1f);
            }
            else
            {
                var label = new UITextBlock() {
                    Layer = Layer.Fore,
                    HorizontalTextAlign = HorizontalAlign.Left,
                };
                label.SetText(place);
                label.SetTextColor(Color.White);
                this.AddControl(label);

                this.FlexWidth = new(35);
                if (rank >= 100)
                {
                    this.BackTexture = "badge_rank_large";
                    label.Padding = new Margin(place.EndsWith("d") ? 3 : 4, 0, 0, 0);
                    this.Glow.Margin = new Margin(0, 0, 0, 0);
                }
                else if (rank >= 10)
                {
                    this.BackTexture = "badge_rank_medium";
                    label.Padding = new Margin(place.EndsWith("d") ? 5 : 6, 0, 0, 0);
                    this.Glow.Margin = new Margin(-2, 0, 0, 0);
                }
                else
                {
                    this.BackTexture = "badge_rank_small";
                    label.Padding = new Margin(5, 0, 0, 0);
                    this.Glow.Margin = new Margin(-4, 0, 0, 0);
                }
                this.Glow.SetTexture($"badge_pb_{2}_glow");
                this.Glow.HorizontalAlign = HorizontalAlign.Left;
                this.Glow.SkipToBrightness(0.75f);

                if (!string.IsNullOrEmpty(variant))
                    this.BackTexture += $"_{variant}";
            }
        }

        public void SetSubHour(int hours)
        {
            if (this.claimed && hours <= 10)
            {
                string place = Leaderboard.GetPlace(this.rank);
                this.Description.SetText($"{place} Place - Sub {hours}\n{this.category} ({this.version})");
            }
        }

        protected override void UpdateThis(Time time)
        {
            base.UpdateThis(time);
        }

        public override void DrawThis(Canvas canvas)
        {
            this.DrawPopup(canvas);
            canvas.Draw(this.BackTexture, this.Inner, Color.White, Layer.Fore);
        }
    }
}
