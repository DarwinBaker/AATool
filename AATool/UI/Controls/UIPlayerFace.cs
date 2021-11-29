using System.Xml;
using AATool.Net;
using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIPlayerFace : UIPicture
    {
        public bool ShowName { get; set; }
        public int Scale     { get; set; }
        public Uuid PlayerId { get; set; }

        private UIPicture face;
        private UITextBlock name;
        private UIGlowEffect glow;
        private float nameOpacity;

        public UIPlayerFace(Uuid id) : this ()
        {
            this.PlayerId = id;
            this.UpdatePlayerFace();
        }

        public UIPlayerFace()
        {
            this.BuildFromSourceDocument();
            this.face = this.First<UIPicture>();
            this.glow = this.First<UIGlowEffect>();
            this.name = this.First<UITextBlock>();
            this.nameOpacity = 0;
        }

        protected override void UpdateThis(Time time)
        {
            this.UpdatePlayerFace();
            this.UpdateGlowEffect();
            this.UpdateNameText(time);
        }

        public void UpdatePlayerFace()
        {
            this.face.SetTexture(this.PlayerId.ToString());
        }

        private void UpdateGlowEffect()
        {
            if (Player.TryGetColor(this.PlayerId, out Color accent))
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
            else if (Peer.TryGetLobby(out Lobby lobby) && lobby.TryGetUser(this.PlayerId, out User user))
            {
                //show preferred name
                this.name.SetText(user.Name);
            }
            else if (Player.TryGetName(this.PlayerId, out string mcName))
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
            this.name.SetTextColor(Config.Main.TextColor * this.nameOpacity);
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
            this.Scale = ParseAttribute(node, "scale", 4);
        }
    }
}
