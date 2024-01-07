using System;
using System.Diagnostics;
using AATool.Configuration;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIPersonalBest : UIPanel
    {
        public Run Run { get; private set; }
        public int Rank { get; set; }
        public bool IsSmall { get; set; }
        public bool IsMainPlayer { get; set; }
        public bool DisableButton { get; set; }
        public string NameOverride { get; set; }
        public string TimeOverride { get; set; }

        internal UIAvatar face;
        private protected UIButton profileButton;
        private protected UIButton linkButton;
        private protected UITextBlock name;
        private protected UITextBlock igt;
        private protected UITextBlock date;
        private protected UITextBlock age;
        private protected bool nickNameChecked;
        private protected bool uuidRequested;
        private protected bool isClaimed;

        public virtual void SetRun(Run run, bool isClaimed = true)
        {
            this.Run = run;
            this.isClaimed = isClaimed;
            if (this.isClaimed)
            {
                if (Player.TryGetName(Tracker.GetMainPlayer(), out string mainPlayerName))
                {
                    this.IsMainPlayer = Leaderboard.GetRealName(this.Run.Runner).ToLower() == mainPlayerName.ToLower()
                        || Leaderboard.GetNickName(this.Run.Runner).ToLower() == mainPlayerName.ToLower();
                }
                else if (true)
                {
                    this.IsMainPlayer = Leaderboard.GetRealName(this.Run.Runner).ToLower() == Config.Tracking.LastPlayer.Value.ToLower()
                        || Leaderboard.GetNickName(this.Run.Runner).ToLower() == Config.Tracking.LastPlayer.Value.ToLower();
                }
            }             
        }

        public void SetRank(int rank)
        {
            if (this.face is not null)
                this.face.Tag = rank;
            this.isClaimed = false;
        }

        public UIPersonalBest(Leaderboard owner)
        {
            this.BuildFromTemplate();
            this.face = this.First<UIAvatar>();
            this.face?.RegisterOnLeaderboard(owner);
        }

        protected override void UpdateThis(Time time)
        {
            if (this.isClaimed && this.Run is not null)
            {
                Uuid mainPlayer = Tracker.GetMainPlayer();
                if (mainPlayer == Uuid.Empty && Config.Tracking.Filter == ProgressFilter.Solo)
                    this.IsMainPlayer = Leaderboard.GetRealName(this.Run.Runner).ToLower() == Config.Tracking.SoloFilterName.Value.ToLower();
                else if (Player.TryGetName(mainPlayer, out string mainPlayerName))
                    this.IsMainPlayer = Leaderboard.GetRealName(this.Run.Runner).ToLower() == mainPlayerName.ToLower();
            }
            else
            {
                this.face.SetTint(Config.Main.TextColor.Value * 2);
                return;
            }

            if (!this.nickNameChecked && Leaderboard.NickNamesLoaded)
            {
                string ign = Leaderboard.GetRealName(this.Run.Runner);
                if (Player.TryGetUuid(ign, out Uuid id))
                {
                    this.face.SetPlayer(id);
                    this.nickNameChecked = true;
                }
                if (!this.uuidRequested)
                {
                    //Player.FetchIdentityAsync(ign);
                    //this.uuidRequested = true;
                }
            }
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.face = this.First<UIAvatar>();
            this.face.Scale = 4;
            this.face.InitializeRecursive(screen);
            this.name = this.First<UITextBlock>("name");
            this.igt = this.First<UITextBlock>("igt");
            this.date = this.First<UITextBlock>("date");
            this.age = this.First<UITextBlock>("age");
            this.profileButton = this.First<UIButton>("profile");
            this.profileButton.OnClick += this.ProfileButtonClick;
            this.linkButton = this.First<UIButton>("link");
            this.linkButton.OnClick += this.LinkButtonClick;

            if (this.DisableButton)
            {
                this.First<UIPanel>("profile_panel").DrawMode = DrawMode.ChildrenOnly;
                this.profileButton.DrawMode = DrawMode.ChildrenOnly;
            }

            if (UIMainScreen.ActiveTab is not UIMainScreen.TrackerTab)
            {
                this.name.Margin = new Margin(0, 0, 7, 0);
                this.igt.Margin = new Margin(0, 0, 22, 0);
                this.date.Margin = new Margin(0, 0, 7, 0);
                this.age.Margin = new Margin(0, 0, 22, 0);
                this.date.Padding = new Margin(0, 30, 0, 0);
                this.age.Padding = new Margin(0, 30, 0, 0);
                this.date.HorizontalTextAlign = HorizontalAlign.Right;
                this.age.HorizontalTextAlign = HorizontalAlign.Right;
            }

            string time = string.Empty;
            if (this.Run is not null)
            {
                TimeSpan timeSpan = this.Run.InGameTime == TimeSpan.Zero
                    ? this.Run.RealTime
                    : this.Run.InGameTime;

                if (timeSpan.TotalHours >= 1)
                {
                    time = (int)timeSpan.TotalHours + timeSpan.ToString("':'mm':'ss");
                }
                else if (UIMainScreen.ActiveTab is UIMainScreen.AnyPercentRankingsTab)
                {
                    time = timeSpan.ToString("m'm 'ss's 'fff'ms'");
                }
                else
                {
                    time = timeSpan.ToString("m':'ss");
                    ///string rta = this.Run.RealTime.ToString("m':'ss");
                    //time = $"{igt} IGT, {rta} RTA";
                }
            }

            if (this.isClaimed && this.Run is not null)
            {
                //populate with run data
                this.face.SetPlayer(Leaderboard.GetRealName(this.Run.Runner));

                if (this.Run is HardcoreStreak streak)
                {
                    this.face.LockBadgeAndFrame = true;
                    this.face.SetBadge(new HundredHardcoreBadge(streak.BestStreak), true);
                }
                else if (this.Run.Comment == "1K No Reset")
                {
                    this.face.LockBadgeAndFrame = true;
                    this.face.SetBadge(new NoResetsBadge(this.Run.Runner, this.Run.ExtraStat), true);
                    this.TimeOverride = $"{this.Run.ExtraStat:N0} Completed";
                }
                else if (this.Run.Comment == "AA SSG WR")
                {
                    this.face.LockBadgeAndFrame = true;
                    this.face.SetBadge(new RankBadge(1, "AA SSG", "1.16"), true);
                }
                else if (this.Run.Comment == "Most Concurrent Records")
                {
                    this.face.LockBadgeAndFrame = true;
                    this.face.SetBadge(new MostConcurrentRecordsBadge(), true);
                }
                else if (this.Run.Comment == "Most Consecutive Records")
                {
                    this.face.LockBadgeAndFrame = true;
                    this.face.SetBadge(new MostConsecutiveRecordsBadge(), true);
                }
                else
                {
                    this.face.RefreshBadge();
                }

                this.name.SetText(this.NameOverride ?? this.Run.Runner);
                this.igt.SetText(this.TimeOverride ?? time);

                if (!this.IsSmall)
                {
                    string format = this.Run.Runner.Length < 13 ? "MM/dd/yyyy" : "M/d/yy";
                    this.date.SetText(this.Run.Date.ToString(format));
                }

                DateTime estNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Leaderboard.TimeZone);
                int days = (int)(estNow - this.Run.Date).TotalDays;
                int months = days / 30;
                int years = (int)Math.Round(days / 365f, 1);
                string age;
                if (years < 1)
                {
                    if (days > 30)
                    {
                        age = $"{months} month{(months > 1 ? "s" : "")} ago";
                    }
                    else
                    {
                        age = days switch {
                            0 => "Set Today",
                            1 => "Set Yesterday",
                            _ => $"{days} days ago",
                        };
                    }
                }
                else
                {
                    age = $"{years} year{(years > 1 ? "s" : "")} ago";
                }
                if (!this.IsSmall)
                    this.age.SetText(age);
            }
            else
            {
                this.face.SetEmptyLeaderboardSlot();
                this.igt.SetText($"__:__:__");
                this.profileButton.DrawMode = DrawMode.ChildrenOnly;
            }

            if (!this.IsSmall && this.Run is AllVersionsRun avRun)
            {
                this.age.SetText($"({avRun.Range})");
            }

            this.face.Glow();
            base.InitializeRecursive(screen);
        }

        public override void ResizeRecursive(Rectangle rectangle) 
        { 
            base.ResizeRecursive(rectangle);
            if (string.IsNullOrEmpty(this.Run?.Link) || UIMainScreen.ActiveTab is UIMainScreen.TrackerTab)
                this.linkButton.Collapse();
        }

        private void ProfileButtonClick(UIControl sender)
        {
            if (this.Run is null || this.DisableButton)
                return;

            if (!string.IsNullOrEmpty(this.Run.RunnerSrcId))
                RunnerProfile.SetCurrentId(this.Run.RunnerSrcId);
            else
                RunnerProfile.SetCurrentName(this.Run.Runner);

            UIMainScreen.SetActiveTab(UIMainScreen.RunnerProfileTab);
        }

        private void LinkButtonClick(UIControl sender)
        {
            if (UIMainScreen.ActiveTab is not UIMainScreen.TrackerTab)
            {
                if (!string.IsNullOrEmpty(this.Run.Link))
                    Process.Start(this.Run.Link);
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            if (!this.SkipDraw)
            {
                canvas.DrawRectangle(this.Bounds, this.BackColor, this.BorderColor, 2);
                if (this.IsMainPlayer)
                    canvas.DrawRectangle(new Rectangle(this.Inner.Left + 1, this.Inner.Top - 8, this.Inner.Width - 2, this.Inner.Height + 2), Config.Main.BorderColor, null, 0);
            }    
        }
    }
}
