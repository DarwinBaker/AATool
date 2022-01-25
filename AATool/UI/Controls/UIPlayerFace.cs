using System.Xml;
using AATool.Configuration;
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

        private UIPicture face;
        private UIControl badge;
        private UIGlowEffect glow;
        private UITextBlock name;
        private float nameOpacity;

        public UIAvatar()
        {
            this.BuildFromTemplate();
        }

        public UIAvatar(Uuid player, UIScreen screen) : this ()
        {
            this.InitializeRecursive(screen);
            this.SetPlayer(player);
        }

        public void SetPlayer(Uuid player)
        {
            if (this.Player.String != player.String)
            {
                this.Player = player;
                this.face.SetTexture(this.Player.ToString());
                this.InitializeBadge(player);
                this.glow.SkipToBrightness(0);
            }
        }

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
        }

        private void UpdateGlowEffect()
        {
            if (Net.Player.TryGetColor(this.Player, out Color accent))
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

        private void InitializeBadge(Uuid player)
        {
            this.RemoveControl(this.badge);
            if (Badge.TryGet(player, 2, out this.badge))
            {
                this.AddControl(this.badge);
                this.badge.Margin = new Margin(-13, -0, -10, 0);
                this.badge.ResizeRecursive(this.Inner);
            }
        }

        public override void ResizeThis(Rectangle parent)
        {
            this.FlexWidth  = new Size(8 * this.Scale);
            this.FlexHeight = new Size(8 * this.Scale);
            base.ResizeThis(parent);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.Scale = Attribute(node, "scale", 4);
        }
    }
}
