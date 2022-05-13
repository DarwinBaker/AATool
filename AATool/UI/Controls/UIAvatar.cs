using System.Xml;
using AATool.Configuration;
using AATool.Data.Players;
using AATool.Net;
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
        private UIControl badge;
        private UIGlowEffect glow;
        private UITextBlock name;
        private float nameOpacity;
        private bool cacheChecked;
        private bool emptyLeaderboardSlot;
        private bool liveChecked;
        private string offlineName;

        public void Glow() => this.glow.SkipToBrightness(this.Scale / 4f);

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
            base.InitializeRecursive(screen);
        } 

        protected override void UpdateThis(Time time)
        {
            this.UpdateGlowEffect();
            this.UpdateNameText(time);

            bool invalidated = !this.cacheChecked && Leaderboard.TryGet(Leaderboard.Current, out _);
            invalidated |= !this.liveChecked && Leaderboard.IsLiveAvailable;

            if (invalidated)
            {
                this.RefreshBadge();
                this.cacheChecked = true;
                if (Leaderboard.IsLiveAvailable)
                    this.liveChecked = true;
            } 
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
                this.glow.LerpToBrightness(0);
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

        public void RefreshBadge()
        {
            string playerName = Leaderboard.GetNickName(this.offlineName ?? string.Empty);
            if (string.IsNullOrEmpty(playerName))
                Net.Player.TryGetName(this.Player, out playerName);

            this.RemoveControl(this.badge);
            string boardName = this.owner is null 
                ? Leaderboard.Current 
                : this.owner.BoardName ;

            if (Badge.TryGet(this.Player, playerName, 2, boardName, out this.badge))
            {
                this.AddControl(this.badge);
                this.badge.ResizeRecursive(this.Inner);
            }
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
        }
    }
}
