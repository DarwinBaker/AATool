using System;
using AATool.Configuration;
using AATool.Data;
using AATool.Data.Players;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Screens;

namespace AATool.UI.Controls
{
    public class UIPersonalBest : UIPanel
    {
        public PersonalBest Run { get; private set; }

        private UIAvatar face;
        private bool nickNameChecked;
        private bool uuidRequested;
        private bool isClaimed;

        public void SetRun(PersonalBest run, bool isClaimed = true)
        {
            this.Run = run;
            this.isClaimed = isClaimed;
        }

        public UIPersonalBest(Leaderboard owner)
        {
            this.BuildFromTemplate();
            this.face = this.First<UIAvatar>();
            this.face?.RegisterOnLeaderboard(owner);
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.isClaimed || this.Run is null)
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
                    Player.FetchIdentity(ign);
                    this.uuidRequested = true;
                }
            }
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.face = this.First<UIAvatar>();
            this.face.Scale = 4;
            this.face.InitializeRecursive(screen);

            string time = "";
            if (this.Run is not null)
            {
                time = this.Run.InGameTime.Days is 0
                ? this.Run.InGameTime.ToString("hh':'mm':'ss")
                : $"{Math.Round(this.Run.InGameTime.TotalDays, 2)} Days";
            }

            if (this.isClaimed && this.Run is not null)
            {
                //populate with run data
                this.face.SetPlayer(Leaderboard.GetRealName(this.Run.Runner));
                this.face.RefreshBadge();

                this.First<UITextBlock>("name").SetText(this.Run.Runner);
                this.First<UITextBlock>("igt").SetText(time);
                this.First<UITextBlock>("date").SetText(this.Run.Date.ToShortDateString());

                DateTime estNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Leaderboard.TimeZone);
                int days = (int)(estNow - this.Run.Date).TotalDays;
                int years = (int)Math.Round(days / 365f);
                string age = "";
                if (years < 1)
                {
                    age = days switch {
                        0 => "Set Today",
                        1 => "Set Yesterday",
                        _ => $"{days} days ago",
                    };
                }
                else
                {
                    age = $"{years} year{(years > 1 ? "s" : "")} ago";            
                }
                this.First<UITextBlock>("age").SetText(age);
            }
            else
            {
                this.face.SetEmptyLeaderboardSlot();
                if (!Config.Main.FullScreenLeaderboards)
                    this.First<UITextBlock>("igt").SetText($"__:__:__");
            }
            this.face.Glow();
            base.InitializeRecursive(screen);
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;
            canvas.DrawRectangle(this.Bounds, this.BackColor, this.BorderColor, 2);
        }
    }
}
