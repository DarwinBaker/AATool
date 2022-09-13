using System;
using System.Linq;
using AATool.Configuration;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net;
using AATool.Net.Requests;
using AATool.UI.Controls;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    public class Badge : UIControl
    {
        //creator of aatool
        protected const string Developer = "60bddec7-939c-4753-a898-cffa33134a4d";
        protected const string DeveloperName = "_ctm";

        //completed the first ever half-heart hardcore all advancements speedrun
        protected const string Elysaku = "b2fcb273-9886-4a9b-bd7f-e005816fb7b7";
        protected const string ElysakuName = "elysaku";

        //completed 1000 any% RSG speedruns in a row without resetting
        protected const string Couriway = "994f9376-3f80-48bc-9e72-ee92f861911d";
        protected const string CouriwayName = "couriway";

        //completed 999 any% RSG speedruns in a row without resetting FeelsStrongMan
        protected const string MoleyG = "fa1bec35-0585-46c9-8f92-79f8be7cf9bc";
        protected const string MoleyGName = "moleyg";

        //manages the aa community leaderboards
        protected const string Deadpool = "899c63ac-6590-46c0-b77c-4dae1543f707";

        //the best minecraft songs ever feelsstrongman
        protected const string CaptainSparklez = "5f820c39-5883-4392-b174-3125ac05e38c";
        protected const string CaptainSparklezName = "captainsparklez";

        //the founding father of all advancements
        protected const string Illumina = "46405168-e9ce-40a0-99a4-0b989a912c77";
        protected const string IlluminaName = "illumina";

        protected const string PopupGlow = "popup_badge_glow";
        protected const double HoverDelay = 0.1;

        public static readonly Timer HoverTimer = new (HoverDelay);

        public static Badge GetEmptyRank(int rank, string category, string version) => new RankBadge(rank, category, version, false);
  
        public static bool TryGet(Uuid uuid, string name, bool onLeaderboard, string category, string version, out Badge badge)
        {
            bool multiboard = UIMainScreen.ActiveTab == UIMainScreen.MultiboardTab;

            badge = null;
            name = name?.ToLower();
            if (uuid != Uuid.Empty)
            { 
                name = Leaderboard.GetNickName(uuid.String, name);
                uuid = new Uuid(uuid.String);
            }

            Leaderboard.TryGetRank(name, category, version, out int rank);
            if (rank is 1)
                badge = new RankBadge(rank, category, version, true);

            //legendary badges
            if (badge is null && !onLeaderboard)
                TryGiveLegendaryBadge(uuid, name, ref badge);

            //unique badges
            if (badge is null && !onLeaderboard)
                TryGiveUniqueBadge(uuid, ref badge);

            //vip badges
            if (badge is null && !onLeaderboard)
                TryGiveVipBadge(uuid, name, ref badge);

            //normal rank badges
            if (badge is null)
                TryGiveRankBadge(uuid, name, category, version, rank, ref badge);
            
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
                    Elysaku or ElysakuName => new HalfHeartHardcoreBadge(),
                    Couriway or CouriwayName => new ThousandSeedsBadge(CouriwayName),
                    MoleyG or MoleyGName => new ThousandSeedsBadge(MoleyGName),
                    _ => null
                };
            }
        }

        private static void TryGiveUniqueBadge(Uuid uuid, ref Badge badge)
        {
            badge = uuid.String switch {
                Developer => new DeveloperBadge(),
                Deadpool => new ModBadge("Leaderboard Manager\n& Community Legend"),
                _ => null
            };
        }

        private static void TryGiveVipBadge(Uuid uuid, string name, ref Badge badge)
        {
            string player = uuid != Uuid.Empty ? uuid.String : name;
            badge = player switch {
                CaptainSparklez or CaptainSparklezName => new VipBadge("Your vids have brought us\nso much joy, thank you ♥"),
                Illumina or IlluminaName => new VipBadge("Your first AA run was the\n beginning of everything ♥"),
                _ => null
            };
        }

        private static void TryGiveRankBadge(Uuid uuid, string name, string category, string version, int rank, ref Badge badge)
        {
            if (badge is null && rank is > 0)
                badge = new RankBadge(rank, category, version, true, null);
        }

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
