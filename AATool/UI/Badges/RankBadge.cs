using AATool;
using AATool.Configuration;
using AATool.Data;
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
        public const string ListNameNetherite = "Rank (Netherite)";
        public const string ListNameDiamond = "Rank (Diamond)";
        public const string ListNameGold = "Rank (Gold)";

        private static readonly Color GlintColor = new (200, 200, 200, 200);
        private static readonly Color PopupGlintColor = new (140, 140, 140, 140);

        public static readonly Color PopupGoldBack = new (123, 88, 34);
        public static readonly Color PopupGoldBorder = new (254, 224, 72);
        public static readonly Color PopupGoldText = new (255, 255, 230);

        public static readonly Color PopupSilverBack = new (107, 113, 128);
        public static readonly Color PopupSilverBorder = new (181, 191, 212);
        public static readonly Color PopupSilverText = new (255, 255, 255);

        public static readonly Color PopupCopperBack = new (117, 65, 59);
        public static readonly Color PopupCopperBorder = new (228, 134, 94);
        public static readonly Color PopupCopperText = new (255, 225, 194);

        public static readonly Color PopupDiamondBack = new (50, 105, 115);
        public static readonly Color PopupDiamondBorder = new (61, 224, 229);
        public static readonly Color PopupDiamondText = new (255, 255, 230);

        public static readonly Color PopupNetheriteBack = new (123, 88, 34);
        public static readonly Color PopupNetheriteBorder = new (254, 224, 72);
        public static readonly Color PopupNetheriteText = new (255, 255, 230);

        private static readonly Color GoldGlowColor = new (140, 140, 140, 140);
        private static readonly Color DiamondGlowColor = new (80, 140, 190, 205);
        private static readonly Color NetheriteGlowColor = new (215, 150, 255, 110);

        public readonly int Rank;
        public readonly string Variant;
        public readonly string Category;
        public readonly string Version;
        public readonly bool Claimed;

        public override string GetListName => "Rank";

        public RankBadge(int rank, string category, string version, bool claimed = true, string variant = null)
        {
            this.Rank = rank;
            this.Category = category;
            this.Version = version;
            this.Claimed = claimed;
            this.Variant = variant;

            this.FlexHeight = new (15);
            this.Margin = new Margin(-9, -0, -7, 0);

            string place = Leaderboard.GetPlace(rank);
            if (claimed)
                this.Description.SetText($"{place} Place\n{this.Category} ({this.Version})");
            else
                this.Description.SetText($"Unclaimed {place} Place\nLeaderboard Slot");

            if (rank <= 3)
            {
                this.PopupBackColor = this.Rank switch {
                    1 => PopupGoldBack,
                    2 => PopupSilverBack,
                    3 => PopupCopperBack,
                    _ => default
                };
                this.PopupBorderColor = this.Rank switch {
                    1 => PopupGoldBorder,
                    2 => PopupSilverBorder,
                    3 => PopupCopperBorder,
                    _ => default
                };
                this.PopupTextColor = this.Rank switch {
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
                this.BackTexture = $"badge_rank_{this.Rank}";
                this.Glow.SetTexture($"badge_rank_{this.Rank}_glow");
                this.Glow.SkipToBrightness(1f);
            }
            else
            {
                /*
                if (!string.IsNullOrEmpty(variant))
                {
                    this.PopupBackColor = variant switch {
                        "gold" => PopupGoldBack,
                        "diamond" => PopupDiamondBack,
                        "netherite" => PopupNetheriteBack,
                        _ => default
                    };
                    this.PopupBorderColor = variant switch {
                        "gold" => PopupGoldBorder,
                        "diamond" => PopupDiamondBorder,
                        "netherite" => PopupNetheriteBorder,
                        _ => default
                    };
                    this.PopupTextColor = variant switch {
                        "gold" => PopupGoldText,
                        "diamond" => PopupDiamondText,
                        "netherite" => PopupNetheriteText,
                        _ => default
                    };
                }
                */

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
                    this.BackTexture = $"badge_rank_large";
                    label.Padding = new Margin(place.EndsWith("d") ? 3 : 4, 0, 0, 0);
                    this.Glow.Margin = new Margin(0, 0, 0, 0);
                }
                else if (rank >= 10)
                {
                    this.BackTexture = $"badge_rank_medium";
                    label.Padding = new Margin(place.EndsWith("d") ? 5 : 6, 0, 0, 0);
                    this.Glow.Margin = new Margin(-2, 0, 0, 0);
                }
                else
                {
                    this.BackTexture = $"badge_rank_small";
                    label.Padding = new Margin(5, 0, 0, 0);
                    this.Glow.Margin = new Margin(-4, 0, 0, 0);
                }
                //this.Glow.SetTexture($"badge_rank_2_glow");
                this.Glow.HorizontalAlign = HorizontalAlign.Left;
                this.Glow.SkipToBrightness(0.75f);

                if (this.Variant is "gold")
                    this.Glow.SetTexture($"badge_rank_1_glow");
                else if (this.Variant is "diamond")
                    this.Glow.SetTexture($"badge_rank_diamond_glow");
                else if (this.Variant is "netherite")
                    this.Glow.SetTexture($"badge_rank_netherite_glow");

                if (!string.IsNullOrEmpty(variant))
                    this.BackTexture += $"_{variant}";
            }
        }

        public void SetSubHour(int hours)
        {
            if (this.Claimed && hours <= 10)
            {
                string place = Leaderboard.GetPlace(this.Rank);
                this.Description.SetText($"{place} Place - Sub {hours}\n{this.Category} ({this.Version})");
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

            if (this.Rank <= 3)
                return;

            //draw glint effect
            if (this.Variant is "netherite" or "diamond")
            {
                if (this.Rank >= 100)
                    canvas.Draw("glint", new Rectangle(this.Inner.Left, this.Inner.Top, this.Inner.Width, this.Inner.Height), GlintColor, Layer.Fore);
                else if (this.Rank >= 10)
                    canvas.Draw("glint", new Rectangle(this.Inner.Left, this.Inner.Top, this.Inner.Width - 2, this.Inner.Height), GlintColor, Layer.Fore);
                else
                    canvas.Draw("glint", new Rectangle(this.Inner.Left, this.Inner.Top, this.Inner.Width - 10, this.Inner.Height), GlintColor, Layer.Fore);
            }

            //draw gold gilding
            /*
            if (this.Variant is "netherite")
            {
                if (this.Rank >= 100)
                    canvas.Draw("large_netherite_gild", this.Bounds, Color.White, Layer.Fore);
                else if (this.Rank >= 10)
                    canvas.Draw("medium_netherite_gild", this.Bounds, Color.White, Layer.Fore);
                else
                    canvas.Draw("small_netherite_gild", this.Bounds, Color.White, Layer.Fore);
            }
            */
        }

        protected override void DrawPopup(Canvas canvas) 
        {
            if (!this.Hovering || !HoverTimer.IsExpired || HoverTimer.TimeElapsed <= 0)
                return;

            if (this.Rank <= 3)
            {
                base.DrawPopup(canvas); 
            }
            else if (this.Variant is "netherite")
            {
                canvas.Draw(PopupGlow, this.Description.Bounds, NetheriteGlowColor, Layer.Fore);
                canvas.Draw("popup_badge_netherite", this.Description.Inner, Color.Lavender, Layer.Fore);
                canvas.Draw("glint", this.Description.Inner, PopupGlintColor, Layer.Fore);
                canvas.Draw("popup_badge_netherite_gild", this.Description.Inner, Color.White, Layer.Fore);
            }
            else if (this.Variant is "diamond")
            {
                canvas.Draw(PopupGlow, this.Description.Bounds, DiamondGlowColor, Layer.Fore);
                canvas.Draw("popup_badge_diamond", this.Description.Inner, Color.White, Layer.Fore);
                canvas.Draw("glint", this.Description.Inner, GlintColor, Layer.Fore);
            }
            else if (this.Variant is "gold")
            {
                canvas.Draw(PopupGlow, this.Description.Bounds, GoldGlowColor, Layer.Fore);
                canvas.Draw("popup_badge_gold", this.Description.Inner, Color.White, Layer.Fore);
            }
            else
            {
                base.DrawPopup(canvas);
            }
        }
    }
}