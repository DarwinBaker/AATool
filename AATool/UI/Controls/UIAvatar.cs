using System;
using System.Xml;
using AATool.Configuration;
using AATool.Data;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net;
using AATool.Net.Requests;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using SharpDX.Win32;

namespace AATool.UI.Controls
{
    class UIAvatar : UIPicture
    {
        private static readonly Color GlintColor = new (200, 200, 200, 200);

        private const string GoldFrameTexture = "player_frame_gold";
        private const string DiamondFrameTexture = "player_frame_diamond";
        private const string NetheriteFrameTexture = "player_frame_netherite";

        public Uuid Player { get; private set; }
        public bool ShowName { get; set; }
        public int Scale { get; set; }
        public bool LockBadgeAndFrame { get; set; }

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
        private Rectangle frameRectangle;
        private string frameTexture;
        private bool showFrame;
        private bool frameGlint;

        public Badge Badge => this.badge;

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
                this.Player = new Uuid(player.String);
                this.RefreshBadge();
                if (this.Player != Uuid.Empty)
                    Net.Player.FetchIdentityAsync(this.Player);
                if (Credits.TryGet(player, out Credit supporter))
                    this.SetFrame(supporter.HighestRole);
            }
            this.face.SetTexture($"avatar-{this.Player.String}");
        }

        public void SetPlayer(string name)
        {
            this.offlineName = name;
            this.face.SetTexture($"avatar-{Leaderboard.GetRealName(name ?? string.Empty).ToLower()}");

            string realName = Leaderboard.GetRealName(name ?? string.Empty).ToLower();
            string nickName = Leaderboard.GetNickName(name ?? string.Empty).ToLower();
            if (Credits.TryGet(realName, out Credit supporter) || Credits.TryGet(nickName, out supporter))
                this.SetFrame(supporter.HighestRole);
        }

        public void SetFrame(string role, bool skipChecks = false)
        {
            if (skipChecks)
            {
                if (role is Credits.GoldTier)
                {
                    this.SetGoldFrame();
                    this.showFrame = true;
                }
                else if (role is Credits.DiamondTier)
                {
                    this.SetDiamondFrame();
                    this.showFrame = true;
                }
                else if (role is Credits.NetheriteTier)
                {
                    this.SetNetheriteFrame();
                    this.showFrame = true;
                }
                return;
            }

            bool isMainPlayer = this.Player == Tracker.GetMainPlayer();
            if (role is Credits.NetheriteTier or Credits.Developer or Credits.BetaTester)
            {
                if (isMainPlayer && Config.Main.PreferredPlayerFrame == "Gold")
                    this.SetGoldFrame();
                else if (isMainPlayer && Config.Main.PreferredPlayerFrame == "Diamond")
                    this.SetDiamondFrame();
                else if (isMainPlayer && (Config.Main.PreferredPlayerFrame == "Netherite" || (role is Credits.NetheriteTier && Config.Main.PreferredPlayerFrame == "Default")))
                    this.SetNetheriteFrame();
                else
                    this.SetNoFrame();
            }
            else if (role is Credits.DiamondTier)
            {
                if (isMainPlayer && Config.Main.PreferredPlayerFrame == "Gold")
                    this.SetGoldFrame();
                else if (isMainPlayer && Config.Main.PreferredPlayerFrame == "None")
                    this.SetNoFrame();
                else
                    this.SetDiamondFrame();
            }
            else if(role is Credits.GoldTier)
            {
                if (isMainPlayer && Config.Main.PreferredPlayerFrame == "None")
                    this.SetNoFrame();
                else
                    this.SetGoldFrame();
            }
        }

        private void SetNoFrame()
        {
            this.frameTexture = string.Empty;
            this.frameGlint = false;
        }

        private void SetGoldFrame()
        {
            this.frameTexture = GoldFrameTexture;
            this.frameGlint = false;
        }

        private void SetDiamondFrame()
        {
            this.frameTexture = DiamondFrameTexture;
            this.frameGlint = true;
        }

        private void SetNetheriteFrame()
        {
            this.frameTexture = NetheriteFrameTexture;
            this.frameGlint = true;
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
            this.showFrame &= UIMainScreen.ActiveTab is UIMainScreen.TrackerTab;
            base.InitializeRecursive(screen);
        } 

        protected override void UpdateThis(Time time)
        {
            this.UpdateGlowEffect();
            this.UpdateNameText(time);

            string category = this.RegisteredOnLeaderboard ? this.owner.Category : Leaderboard.Current.category;
            string version = this.RegisteredOnLeaderboard ? this.owner.Version : Leaderboard.Current.version;

            bool invalidated = (!this.cacheChecked && Leaderboard.TryGet(category, version, out _))
                || (!this.liveChecked && Leaderboard.IsLiveAvailable(category, version))
                || Config.Main.PreferredPlayerBadge.Changed
                || Config.Main.PreferredPlayerFrame.Changed
                || (Net.Player.IdentityCacheInvalidated && !this.RegisteredOnLeaderboard);
            if (invalidated)
            {
                this.RefreshBadge(category, version);
                this.RefreshFrame();
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

            if (!this.RegisteredOnLeaderboard && this.showFrame)
            {
                if (this.frameTexture is GoldFrameTexture)
                {
                    this.glow.LerpToBrightness(this.Scale / 4f);
                    this.glow.SetTint(Color.Gold);
                    return;
                }
                else if (this.frameTexture is DiamondFrameTexture)
                {
                    this.glow.LerpToBrightness(this.Scale / 4f);
                    this.glow.SetTint(Color.CornflowerBlue);
                    return;
                }
                else if (this.frameTexture is NetheriteFrameTexture)
                {
                    this.glow.LerpToBrightness(this.Scale / 4f);
                    this.glow.SetTint(Color.Violet);
                    return;
                }
            }

            if (Net.Player.TryGetColor(this.Player, out Color accent) 
                || Net.Player.TryGetColor(this.offlineName?.ToLower(), out accent))
            { 
                this.glow.LerpToBrightness(this.Scale / 4f);
                this.glow.SetTint(accent);
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

        public void SetBadge(Badge badge, bool ignoreSizeCheck = false)
        {
            this.RemoveControl(this.badge);
            if ((this.Width >= 32 || ignoreSizeCheck) && badge is not null)
            {
                this.badge = badge;
                this.AddControl(this.badge);
                if (this.Width >= 32)
                {
                    this.badge.ResizeRecursive(this.Inner);
                }
            }
            this.cacheChecked = true;
            this.liveChecked = true;
        }

        public void RefreshFrame()
        {
            if (this.LockBadgeAndFrame)
                return;

            if (Credits.TryGet(this.Player, out Credit supporter) || Credits.TryGet(this.offlineName, out supporter))
                this.SetFrame(supporter.HighestRole);
        }

        public void RefreshBadge()
        {
            string category = this.RegisteredOnLeaderboard ? this.owner.Category : Leaderboard.Current.category;
            string version = this.RegisteredOnLeaderboard ? this.owner.Version : Leaderboard.Current.version;
            this.RefreshBadge(category, version);
        }

        public void RefreshBadge(string category, string version)
        {
            if (this.LockBadgeAndFrame)
                return;

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

            if (this.owner?.Category is "HHHAA")
                this.RefreshBadge();

            if (this.Width < 32)
                this.badge?.Collapse();
            else
                this.badge?.Expand();

            const int FrameThickness = 3;
            this.frameRectangle = new Rectangle(
                this.Left - FrameThickness,
                this.Top - FrameThickness,
                this.Width + (FrameThickness * 2),
                this.Height + (FrameThickness * 2));
        }

        public override void DrawThis(Canvas canvas)
        {
            base.DrawThis(canvas);

            if (this.showFrame && !string.IsNullOrEmpty(this.frameTexture) && !this.RegisteredOnLeaderboard)
            {
                canvas.Draw(this.frameTexture, this.frameRectangle, Color.White, Layer.Fore);
                if (this.frameGlint)
                    canvas.Draw("glint", this.frameRectangle, GlintColor, Layer.Fore);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.Scale = Attribute(node, "scale", 4);
            this.offlineName = Attribute(node, "player", string.Empty);
            if (Attribute(node, "empty", false))
                this.SetEmptyLeaderboardSlot();
            this.showFrame = Attribute(node, "show_frame", true);
        }
    }
}
