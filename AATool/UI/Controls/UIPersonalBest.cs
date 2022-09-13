using System;
using AATool.Configuration;
using AATool.Data;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Badges;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIPersonalBest : UIPanel
    {
        public Run Run { get; private set; }
        public int Rank { get; set; }
        public bool IsSmall { get; set; }
        public bool IsMainPlayer { get; set; }

        private UIAvatar face;
        private UITextBlock name;
        private UITextBlock igt;
        private UITextBlock date;
        private UITextBlock age;
        private bool nickNameChecked;
        private bool uuidRequested;
        private bool isClaimed;

        public void SetRun(Run run, bool isClaimed = true)
        {
            this.Run = run;
            this.isClaimed = isClaimed;
            if (this.isClaimed)
            {
                this.IsMainPlayer = Player.TryGetName(Tracker.GetMainPlayer(), out string mainPlayerName)
                    ? Leaderboard.GetRealName(this.Run.Runner).ToLower() == mainPlayerName.ToLower()
                    : Leaderboard.GetRealName(this.Run.Runner).ToLower() == Config.Tracking.LastPlayer.Value.ToLower();
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

            string time = string.Empty;
            if (this.Run is not null)
            {
                time = this.Run.InGameTime.TotalHours >= 1
                    ? (int)this.Run.InGameTime.TotalHours + this.Run.InGameTime.ToString("':'mm':'ss")
                    : this.Run.InGameTime.ToString("mm':'ss");
            }

            if (this.isClaimed && this.Run is not null)
            {
                //populate with run data
                this.face.SetPlayer(Leaderboard.GetRealName(this.Run.Runner));
                this.face.RefreshBadge();

                this.name.SetText(this.Run.Runner);
                this.igt.SetText(time);

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
            }
            this.face.Glow();
            base.InitializeRecursive(screen);
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
