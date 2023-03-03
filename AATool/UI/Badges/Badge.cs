using System;
using AATool.Configuration;
using AATool.Data;
using AATool.Data.Categories;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Controls;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Renci.SshNet;
using SharpDX.Win32;

namespace AATool.UI.Badges
{
    public class Badge : UIControl
    {
        protected const string PopupGlow = "popup_badge_glow";
        protected const double HoverDelay = 0.1;

        public static readonly Timer HoverTimer = new (HoverDelay);

        public static Badge GetEmptyRank(int rank, string category, string version) => new RankBadge(rank, category, version, false);
  
        public static bool TryGet(Uuid uuid, string name, bool onLeaderboard, string category, string version, out Badge badge)
        {
            //yes this is spaghetti code, i do not care

            bool multiboard = UIMainScreen.ActiveTab == UIMainScreen.MultiboardTab;

            badge = null;
            name = name?.ToLower();
            if (uuid != Uuid.Empty)
            { 
                name = Leaderboard.GetNickName(uuid.String, name);
                uuid = new Uuid(uuid.String);
            }

            Uuid mainPlayer = Tracker.GetMainPlayer();
            _= Player.TryGetName(mainPlayer, out string mainName);
            bool isMainPlayer = uuid == mainPlayer || (!string.IsNullOrEmpty(name) && name.ToLower() == mainName?.ToLower());
            
            if (isMainPlayer && !onLeaderboard && Config.Main.PreferredPlayerBadge == "None")
                return false;

            Leaderboard.TryGetRank(name, category, version, out int rank);
            if (rank is 1)
                badge = new RankBadge(rank, category, version, true);

            bool supporterOverride = isMainPlayer && Config.Main.PreferredPlayerBadge.Value is "Gold" or "Diamond" or "Netherite";


            //legendary badges
            if (badge is null && !onLeaderboard && !supporterOverride)
                TryGiveLegendaryBadge(uuid, name, ref badge);

            //unique badges
            if (badge is null && !onLeaderboard && !supporterOverride)
                TryGiveUniqueBadge(uuid, ref badge);

            //vip badges
            if (badge is null && !onLeaderboard && !supporterOverride)
                TryGiveVipBadge(uuid, name, ref badge);

            //normal rank badges
            if (badge is null)
                TryGiveRankBadge(uuid, name, category, version, rank, ref badge);

            if (badge is null)
                TryGiveSupporterBadge(uuid, name, ref badge);

            return badge is not null;
        }

        private static void TryGiveLegendaryBadge(Uuid uuid, string name, ref Badge badge)
        {
            if (name == Leaderboard.RunnerWithMostWorldRecords.ToLower())
            {
                badge = new MostRecordsBadge();
            }
            else if (name == Leaderboard.RsgRunner?.ToLower())
            {
                badge = new RankBadge(1, $"Any% RSG", "1.16", true);
            }
            else if (name == Leaderboard.SsgRunner?.ToLower())
            {
                badge = new RankBadge(1, $"Any% SSG", "1.16", true);
            }
            else
            {
                string player = uuid != Uuid.Empty ? uuid.String : name;
                badge = player switch {
                    Credits.Elysaku or Credits.ElysakuName => new HalfHeartHardcoreBadge(),
                    Credits.Couriway or Credits.CouriwayName => new ThousandSeedsBadge(Credits.CouriwayName),
                    Credits.MoleyG or Credits.MoleyGName => new ThousandSeedsBadge(Credits.MoleyGName),
                    _ => null
                };
            }
        }

        private static void TryGiveUniqueBadge(Uuid uuid, ref Badge badge)
        {
            if (uuid.String is Credits.Ctm)
            {
                if (Config.Main.PreferredPlayerBadge == "Moderator")
                    badge = new ModBadge("Moderator Badge");
                else if (Config.Main.PreferredPlayerBadge == "VIP")
                    badge = new VipBadge("VIP Badge");
                else
                    badge = new DeveloperBadge();
            }
            else if (uuid.String is Credits.Deadpool)
            {
                if (Config.Main.PreferredPlayerBadge == "VIP")
                    badge = new VipBadge("VIP Badge");
                else
                    badge = new ModBadge("Leaderboard Manager\n& Community Legend");
            }
        }

        private static void TryGiveVipBadge(Uuid uuid, string name, ref Badge badge)
        {
            string player = uuid != Uuid.Empty ? uuid.String : name;
            badge = player switch {
                Credits.CaptainSparklez or Credits.CaptainSparklezName => new VipBadge("Your vids have brought us\nso much joy, thank you ♥"),
                Credits.Illumina or Credits.IlluminaName => new VipBadge("Your first AA run was the\n beginning of everything ♥"),
                _ => null
            };
        }

        private static void TryGiveRankBadge(Uuid uuid, string name, string category, string version, int rank, ref Badge badge)
        {
            Uuid mainPlayer = Tracker.GetMainPlayer();
            _= Player.TryGetName(mainPlayer, out string mainName);

            string variant = null;
            if (Credits.TryGet(uuid, out Credit supporter) || Credits.TryGet(name, out supporter))
            {
                name ??= string.Empty;
                if (supporter.Role is Credits.NetheriteTier)
                {
                    if (uuid != mainPlayer && name.ToLower() != mainName?.ToLower())
                        variant = "netherite";
                    else if (Config.Main.PreferredPlayerBadge == "Basic Rank")
                        variant = null;
                    else if (Config.Main.PreferredPlayerBadge == "Gold")
                        variant = "gold";
                    else if (Config.Main.PreferredPlayerBadge == "Diamond")
                        variant = "diamond";
                    else
                        variant = "netherite";
                }
                else if (supporter.Role is Credits.DiamondTier)
                {
                    if (uuid != mainPlayer && name.ToLower() != mainName?.ToLower())
                        variant = "diamond";
                    else if (Config.Main.PreferredPlayerBadge == "Basic Rank")
                        variant = null;
                    else if (Config.Main.PreferredPlayerBadge == "Gold")
                        variant = "gold";
                    else
                        variant = "diamond";
                }
                else if (supporter.Role is Credits.GoldTier)
                {
                    if (uuid != mainPlayer && name.ToLower() != mainName?.ToLower())
                        variant = "gold";
                    else if (Config.Main.PreferredPlayerBadge == "Basic Rank")
                        variant = null;
                    else
                        variant = "gold";
                }
            }
            if (badge is null && rank is > 0)
                badge = new RankBadge(rank, category, version, true, variant);
        }

        private static void TryGiveSupporterBadge(Uuid uuid, string name, ref Badge badge)
        {
            string role = null;
            if (Credits.TryGet(uuid, out Credit supporter) || Credits.TryGet(name, out supporter))
            {
                if (supporter.Role is Credits.NetheriteTier or Credits.Developer)
                {
                    if (Config.Main.PreferredPlayerBadge == "Basic Rank")
                        role = null;
                    else if (Config.Main.PreferredPlayerBadge == "Gold")
                        role = Credits.GoldTier;
                    else if (Config.Main.PreferredPlayerBadge == "Diamond")
                        role = Credits.DiamondTier;
                    else
                        role = Credits.NetheriteTier;
                }
                else if (supporter.Role is Credits.DiamondTier)
                {
                    if (Config.Main.PreferredPlayerBadge == "Basic Rank")
                        role = null;
                    else if (Config.Main.PreferredPlayerBadge == "Gold")
                        role = Credits.GoldTier;
                    else
                        role = Credits.DiamondTier;
                }
                else if (supporter.Role is Credits.GoldTier)
                {
                    if (Config.Main.PreferredPlayerBadge == "Basic Rank")
                        role = null;
                    else
                        role = Credits.GoldTier;
                }
            }
            if (badge is null && !string.IsNullOrEmpty(role))
                badge = new SupporterBadge(role);
        }

        public virtual string GetListName => "Default";

        protected readonly UITextBlock Description;
        protected readonly UIGlowEffect Glow;
        protected bool Hovering;
        protected string BackTexture;
        protected string TextTexture;

        protected Color PopupBackColor;
        protected Color PopupBorderColor;
        protected Color PopupTextColor;
        protected Color PopupGlowColor;

        public Badge()
        {
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;
            this.Layer = Layer.Fore;
            this.PopupGlowColor = ColorHelper.Fade(Color.White, 0.5f);
            this.Hovering = HoverTimer.TimeElapsed > 0;

            this.Description = new UITextBlock() {
                Padding = new Margin(10, 10, 10, 10),
                Layer = Layer.Fore,
            };
            this.Description.Collapse();
            this.AddControl(this.Description);

            this.Glow = new UIGlowEffect() {
                Scale = 1,
                Brightness = 0,
            };
            this.Glow.SetRotationSpeed(250f);
            this.AddControl(this.Glow);
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            int left = MathHelper.Clamp(this.Inner.Center.X - 90, -8, this.Root().Width - 180);
            int top = Math.Max(this.Inner.Top - 68, -4);
            this.Description.ResizeThis(new Rectangle(left, top, 180, 70));
        }

        public override void ResizeThis(Rectangle parent)
        {
            this.Margin.Resize(parent.Size);
            this.FlexWidth.Resize(int.MaxValue);
            this.FlexHeight.Resize(int.MaxValue);

            //clamp size to min and max
            this.Width = this.FlexWidth;
            this.Height = this.FlexHeight;
            this.X = parent.Left + this.Margin.Left;
            this.Y = parent.Top + this.Margin.Top;

            this.Padding.Resize(this.Size);

            //calculate internal rectangle
            this.Inner = new Rectangle(
                this.X + this.Padding.Left,
                this.Y + this.Padding.Top,
                this.Width - this.Padding.Horizontal,
                this.Height - this.Padding.Vertical);

            this.Hovering = this.Inner.Contains(Input.Cursor(this.Root()));
        }

        protected override void UpdateThis(Time time)
        {
            bool wasHovering = this.Hovering;
            this.Hovering = this.Inner.Contains(Input.Cursor(this.Root()));
            if (this.Hovering)
            {
                HoverTimer.Update(time);
                if (!wasHovering)
                    HoverTimer.Reset();

                if (HoverTimer.IsExpired)
                    this.Description.Expand();
            }
            else if (wasHovering)
            {
                this.Description.Collapse();
            }
        }

        public override void DrawThis(Canvas canvas)
        { 
            this.DrawPopup(canvas);
            canvas.Draw(this.BackTexture, this.Inner, Color.White, Layer.Fore);
            canvas.Draw(this.TextTexture, this.Inner, Color.White, Layer.Fore);
        }

        protected virtual void DrawPopup(Canvas canvas)
        {
            if (this.Hovering && HoverTimer.IsExpired && HoverTimer.TimeElapsed > 0)
            {
                canvas.Draw(PopupGlow, this.Description.Bounds, this.PopupGlowColor, Layer.Fore);
                if (this.PopupBackColor == default)
                    canvas.DrawRectangle(this.Description.Inner, Config.Main.BackColor, Config.Main.BorderColor, 4, Layer.Fore);
                else
                    canvas.DrawRectangle(this.Description.Inner, this.PopupBackColor, this.PopupBorderColor, 4, Layer.Fore);
            }
        }
    }
}
