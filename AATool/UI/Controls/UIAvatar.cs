using System;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Speedrunning;
using AATool.Net;
using AATool.Net.Requests;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIAvatar : UIPicture
    {
        public Uuid Player { get; private set; }
        public bool ShowName { get; set; }
        public int Scale { get; set; }

        private Leaderboard owner;
        private UIPicture face;
        private Badge badge;
        private UIGlowEffect glow;
        private UITextBlock name;
        private float nameOpacity;
        private bool cacheChecked;
        private bool emptyLeaderboardSlot;
        private bool liveChecked;
        private string offlineName;

        public void Glow() => this.glow.SkipToBrightness(this.Scale / 4f);

        private bool RegisteredOnLeaderboard => this.owner is not null;

        public UIAvatar()
        {
            this.BuildFromTemplate();
        }

        public UIAvatar(Uuid player, UIScreen screen) : this ()
        {
            this.InitializeRecursive(screen);
            this.SetPlayer(player);
        }

        public void SetEmptyLeaderboardSlot()
        {
            this.emptyLeaderboardSlot = true;
            this.SetLayer(Layer.Fore);
            this.SetTexture("unclaimed_slot");
        }

        public void SetPlayer(Uuid player)
        {
            if (this.Player.String != player.String)
            {
                this.Player = player;
                this.RefreshBadge();
                if (this.Player != Uuid.Empty)
                    Net.Player.FetchIdentityAsync(this.Player);
            }
            this.face.SetTexture($"avatar-{this.Player.String}");
        }

        public void SetPlayer(string name)
        {
            this.offlineName = name;
            this.face.SetTexture($"avatar-{Leaderboard.GetRealName(name ?? string.Empty).ToLower()}");
        }

        public void RegisterOnLeaderboard(Leaderboard board) => this.owner = board;

        public override void InitializeRecursive(UIScreen screen)
        { 
            this.face = this.First<UIPicture>();
            this.glow = this.First<UIGlowEffect>();
            this.name = this.First<UITextBlock>();
            this.nameOpacity = 0;
            if (!string.IsNullOrEmpty(this.offlineName))
            {
                if (Uuid.TryParse(this.offlineName, out Uuid uuid))
                {
                    this.offlineName = string.Empty;
                    this.SetPlayer(uuid);
                    new AvatarRequest(uuid.String).EnqueueOnce();
                }
                else
                {
                    this.SetPlayer(this.offlineName);
                }
            }
            base.InitializeRecursive(screen);
        } 

        protected override void UpdateThis(Time time)
        {
            this.UpdateGlowEffect();
            this.UpdateNameText(time);

            string category = this.RegisteredOnLeaderboard ? this.owner.Category : Leaderboard.Current.category;
            string version = this.RegisteredOnLeaderboard ? this.owner.Version : Leaderboard.Current.version;

            bool invalidated = !this.cacheChecked && Leaderboard.TryGet(category, version, out _);
            invalidated |= !this.liveChecked && Leaderboard.IsLiveAvailable(category, version);
            if (invalidated)
            {
                this.RefreshBadge(category, version);
                this.cacheChecked = true;
                if (Leaderboard.IsLiveAvailable(category, version))
                    this.liveChecked = true;
            }

            if (Config.Main.ShowMyBadge.Changed)
                this.badge?.SetVisibility(this.RegisteredOnLeaderboard || Config.Main.ShowMyBadge || this.Player != Tracker.GetMainPlayer());
        }

        private void UpdateGlowEffect()
        {
            if (this.emptyLeaderboardSlot)
            {
                this.glow.LerpToBrightness(1);
                this.glow.SetTint(this.Tint);
                return;
            }

            if (Net.Player.TryGetColor(this.Player, out Color accent) 
                || Net.Player.TryGetColor(this.offlineName?.ToLower(), out accent))
            { 
                this.glow.SetTint(accent);
                this.glow.LerpToBrightness(this.Scale / 4f);
            }
            else
            {
                //this.glow.LerpToBrightness(0);
            }   
        }

        private void UpdateNameText(Time time)
        {
            if (!this.ShowName || this.Scale < 4)
            {
                //don't show name
                this.name.SetText(string.Empty);
            }
            else if (Peer.TryGetLobby(out Lobby lobby) && lobby.TryGetUser(this.Player, out User user))
            {
                //show preferred name
                this.name.SetText(user.Name);
            }
            else if (Net.Player.TryGetName(this.Player, out string mcName))
            {
                //show minecraft name
                this.name.SetText(mcName);
            }
            else
            {
                //no name available
                this.name.SetText(string.Empty);
            }

            float targetNameOpacity = string.IsNullOrEmpty(this.name.WrappedText) ? 0 : 1;
            this.nameOpacity = MathHelper.Lerp(this.nameOpacity, targetNameOpacity, (float)(20 * time.Delta));
            this.nameOpacity = MathHelper.Clamp(this.nameOpacity, 0, 1);
            this.name.SetTextColor(Config.Main.TextColor.Value * this.nameOpacity);
        }

        public void SetBadge(Badge badge)
        {
            this.RemoveControl(this.badge);
            if (this.Width >= 32 && badge is not null)
            {
                this.badge = badge;
                this.AddControl(this.badge);
                this.badge.ResizeRecursive(this.Inner);
            }
            this.cacheChecked = true;
            this.liveChecked = true;
        }

        public void RefreshBadge()
        {
            string category = this.RegisteredOnLeaderboard ? this.owner.Category : Leaderboard.Current.category;
            string version = this.RegisteredOnLeaderboard ? this.owner.Version : Leaderboard.Current.version;
            this.RefreshBadge(category, version);
        }

        public void RefreshBadge(string category, string version)
        {
            if (this.emptyLeaderboardSlot && this.badge is not null)
                return;

            if (this.emptyLeaderboardSlot && this.Tag is int rank)
            {
                this.badge = Badge.GetEmptyRank(rank, category, version);
                this.AddControl(this.badge);
                this.badge.ResizeRecursive(this.Inner);
                return;
            }

            string playerName = Leaderboard.GetNickName(this.offlineName ?? string.Empty);
            if (string.IsNullOrEmpty(playerName))
                Net.Player.TryGetName(this.Player, out playerName);

            this.RemoveControl(this.badge);
            if (this.Width >= 32 && Badge.TryGet(this.Player, playerName, this.RegisteredOnLeaderboard, category, version, out this.badge))
            {
                this.AddControl(this.badge);
                if (this.Parent?.Parent?.Parent is UIPersonalBest pb)
                    this.First<RankBadge>()?.SetSubHour((int)Math.Ceiling(pb.Run.InGameTime.TotalHours));
                this.badge.ResizeRecursive(this.Inner);
            }
            this.badge?.SetVisibility(this.RegisteredOnLeaderboard || Config.Main.ShowMyBadge || this.Player != Tracker.GetMainPlayer());
        }

        public override void ResizeThis(Rectangle parent)
        {
            this.FlexWidth  = new Size(8 * this.Scale);
            this.FlexHeight = new Size(8 * this.Scale);
            base.ResizeThis(parent);

            if (this.Width < 32)
                this.badge?.Collapse();
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.Scale = Attribute(node, "scale", 4);
            this.offlineName = Attribute(node, "player", string.Empty);
            if (Attribute(node, "empty", false))
                this.SetEmptyLeaderboardSlot();
        }
    }
}
