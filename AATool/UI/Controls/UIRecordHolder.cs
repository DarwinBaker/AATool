using System.Linq;
using System.Xml;
using AATool.Data.Speedrunning;
using AATool.Net.Requests;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    internal class UIRecordHolder : UIPanel
    {
        protected UIAvatar Avatar;
        protected UITextBlock Title;
        protected UITextBlock Subtitle;
        protected UITextBlock Runner;
        protected UITextBlock Details;
        protected UIButton ProfileButton;

        protected Leaderboard Board;
        protected string SourceSheet;
        protected string SourcePage;

        protected string TitleString = string.Empty;
        protected string SubtitleString = string.Empty;
        protected string Category = string.Empty;
        protected string Version = string.Empty;

        protected bool UpToDate;

        public virtual bool LiveBoardAvailable => 
            SpreadsheetRequest.DownloadedPages.Contains((this.SourceSheet, this.SourcePage));

        public UIRecordHolder()
        {
            this.BuildFromTemplate();
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);

            this.TryGetFirst(out this.Avatar);
            this.TryGetFirst(out this.Title, "title");
            this.TryGetFirst(out this.Subtitle, "subtitle");
            this.TryGetFirst(out this.Runner, "runner");
            this.TryGetFirst(out this.Details, "details");

            if (this.TryGetFirst(out this.ProfileButton, "profile"))
                this.ProfileButton.OnClick += this.ProfileButtonClick;

            this.Avatar.LockBadgeAndFrame = true;

            if (this.Category is "All Advancements")
            {
                this.Title.SetText(string.IsNullOrEmpty(this.TitleString) ? $"{this.Version} AA" : this.TitleString);
                this.Subtitle.SetText(string.IsNullOrEmpty(this.SubtitleString) ? "RSG" : this.SubtitleString);
            }
            else
            {
                this.Title.SetText(string.IsNullOrEmpty(this.TitleString) ? this.Category : this.TitleString);
                this.Subtitle.SetText(string.IsNullOrEmpty(this.SubtitleString) ? this.Version : this.SubtitleString);
            }

            //attempt to populate with cached data
            if (Leaderboard.TryGet(this.Category, this.Version, out Leaderboard board))
            {
                this.Board = board;
                this.Populate();
            }

            this.RequestRefresh();
        }

        protected virtual void ProfileButtonClick(UIControl sender)
        {
            if (this.Board?.Runs?.FirstOrDefault() is not Run wr)
            {
                new SpreadsheetRequest($"{this.Version} {this.Category}", this.SourceSheet, this.SourcePage).EnqueueOnce();
                return;
            }

            if (UIMainScreen.ActiveTab is not UIMainScreen.TrackerTab)
            {
                if (!string.IsNullOrEmpty(wr.RunnerSrcId))
                    RunnerProfile.SetCurrentId(wr.RunnerSrcId);
                else
                    RunnerProfile.SetCurrentName(wr.Runner);

                UIMainScreen.SetActiveTab(UIMainScreen.RunnerProfileTab);
            }
        }

        protected virtual void RequestRefresh()
        {
            if (this.Category is "All Blocks")
            {
                this.SourceSheet = Paths.Web.ABSheet;
                this.SourcePage = this.Version switch {
                    "1.16" => Paths.Web.ABPage16,
                    "1.18" => Paths.Web.ABPage18,
                    "1.19" => Paths.Web.ABPage19,
                    "1.20" => Paths.Web.ABPage20,
                    _ => string.Empty
                };
            }
            else if (this.Category is "All Advancements")
            {
                this.SourceSheet = Paths.Web.AASheet;
                this.SourcePage = this.Version is "1.16" ? Paths.Web.AAPage16 : Paths.Web.AAPageOthers;
            }
            else
            {
                this.SourceSheet = Paths.Web.ABSheet;
                this.SourcePage = Paths.Web.ABPageChallenges;
            }

            if (!this.LiveBoardAvailable)
            {
                new SpreadsheetRequest($"{this.Version} {this.Category}", this.SourceSheet, this.SourcePage).EnqueueOnce();
                new SpreadsheetRequest("Player Nicknames", Paths.Web.NicknameSheet).EnqueueOnce();
            }
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.UpToDate && this.LiveBoardAvailable)
            {
                if (Leaderboard.TryGet(this.Category, this.Version, out Leaderboard live))
                {
                    this.Board = live;
                    this.Populate();
                }
                this.UpToDate = true;
            }
        }

        protected virtual void Populate()
        {
            if (this.Board?.Runs?.FirstOrDefault() is not Run wr)
                return;

            new AvatarRequest(wr.Runner).EnqueueOnce();
            this.Avatar.SetPlayer(wr.Runner);
            this.SetBadge();
            this.Runner.SetText(wr.Runner);

            if (this.Category is "All Advancements")
            {
                if (wr.InGameTime.TotalHours >= 1)
                    this.Details.SetText($"{(int)wr.InGameTime.TotalHours}:{wr.InGameTime:mm':'ss} IGT");
                else
                    this.Details.SetText($"{wr.InGameTime:m':'ss} IGT");
            }
            else
            {
                if (wr.RealTime != default && this.Category is not ("All Blocks" or "All Items"))
                {
                    this.Details.SetText($"{wr.InGameTime:m':'ss} IGT    {wr.RealTime:m':'ss} RTA");
                }
                else
                {
                    if (wr.InGameTime.TotalHours >= 1)
                        this.Details.SetText($"{(int)wr.InGameTime.TotalHours}:{wr.InGameTime:mm':'ss} IGT");
                    else
                        this.Details.SetText($"{wr.InGameTime:m':'ss} IGT");
                }
            }
        }

        protected virtual void SetBadge()
        {
            this.Avatar.SetBadge(new RankBadge(1, this.Category, this.Version));
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.SetBadge();
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.TitleString = Attribute(node, "title", this.TitleString);
            this.SubtitleString = Attribute(node, "subtitle", this.SubtitleString);
            this.Category = Attribute(node, "category", this.Category);
            this.Version = Attribute(node, "version", this.Version);
        }
    }
}
