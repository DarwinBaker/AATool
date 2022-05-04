using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIBlockTile : UIButton
    {
        private static readonly Color CompleteBorder = new (230, 218, 164);
        private static readonly Color ManualBorder = new (64, 255, 200);

        private Color targetColor;
        private float targetBrightness;
        private bool wasManuallyCompleted;

        public string BlockId { get; set; }
        public string GlowTexture { get; set; }

        public Block Block;

        private UIPicture icon;
        private UIGlowEffect glowMain;
        private Color glowColor;
        private int scale;

        private float brightness;

        public bool IsCompleted => this.Block.CompletedByAnyone();

        public UIBlockTile()
        {
            this.scale = 2;
            this.BuildFromTemplate();
        }

        public UIBlockTile(int scale = 2) : this()
        {
            this.scale = scale;
        }

        public override void InitializeThis(UIScreen screen)
        {
            //this.isMainWindow = screen is UIMainScreen;
            Tracker.TryGetBlock(this.BlockId, out this.Block);

            this.Name = this.Block.Id;
            //this.popup = this.First<UIPopup>();
            this.icon = this.First<UIPicture>();
            int size = 16 * this.scale;
            if (SpriteSheet.IsAnimated(this.Block.Icon + Sprite.ResolutionFlag + size))
                this.icon.SetLayer(Layer.Fore);
            this.icon?.SetTexture(this.Block.Icon);
            this.brightness = this.Block.CompletedByAnyone() ? 1 : 0;
            this.glowColor = Color.White;

            this.Style();
        }

        protected override void UpdateThis(Time time)
        {
            if (this.Block is null)
                return;
            this.targetBrightness = this.Block.CompletedByAnyone() ? 1 : 0;
            this.brightness = MathHelper.Lerp(this.brightness, this.targetBrightness, (float)(10 * time.Delta));

            this.wasManuallyCompleted |= this.Block.ManuallyCompleted;
            this.targetColor = this.Block.ManuallyCompleted || this.wasManuallyCompleted 
                ? Color.Cyan 
                : Color.White;

            bool brightnessChanging = Math.Abs(this.brightness - this.targetBrightness) > 0.01f;
            if (brightnessChanging || this.glowColor != this.targetColor)
                UIMainScreen.Invalidate();
            else if (this.brightness is 0)
                this.wasManuallyCompleted = false;
            else if (this.brightness is 1)
                this.wasManuallyCompleted = this.Block.ManuallyCompleted;

            this.glowColor = this.targetColor;
        }

        public override void DrawThis(Canvas canvas) 
        { 
            if (this.SkipDraw)
                return;

            Color borderColor;
            if (this.IsCompleted)
            {
                borderColor = this.Block.ManuallyCompleted || this.wasManuallyCompleted
                    ? ManualBorder * Math.Max(this.brightness, 0.5f)
                    : CompleteBorder * Math.Max(this.brightness, 0.5f);
            }
            else
            {
                borderColor = Config.Main.BorderColor;
            }

            canvas.DrawRectangle(this.Bounds,
                Config.Main.BackColor,
                borderColor,
                this.BorderThickness,
                this.Layer);

            canvas.Draw("block_tile_complete", this.Inner, this.glowColor * this.brightness);     
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.BlockId = Attribute(node, "id", string.Empty);
            this.scale = Attribute(node, "scale", this.scale);
        }

        private void Style()
        {
            this.icon.FlexWidth = new(16 * this.scale);
            this.icon.FlexHeight = new(16 * this.scale);

            if (this.Parent?.Parent is not UIBlockPopup) 
            {
                this.FlexWidth = new (38);
                this.FlexHeight = new (38);
            }
            else
            {
                this.icon.SetLayer(this.Layer);
            }
            
            if (this.Block.DoubleHeight)
            {
                this.FlexHeight *= 2;
                this.icon.FlexHeight *= 2;
            }
            else if (this.Block.Id is "kelp")
            {
                this.FlexHeight *= 2;
                this.icon.FlexHeight *= 2;
                this.icon.FlexWidth *= 2;
                this.icon.SetLayer(Layer.Fore);
            }

            if (this.Block.Id.Contains("torch"))
                this.icon.SetLayer(Layer.Fore);

            if (this.Block.Glows)
            {
                this.glowMain = new UIGlowEffect() { Scale = 1 };              
                this.glowMain.SkipToBrightness(this.Block.LightLevel);
                if (this.BlockId.Contains("redstone"))
                    this.glowMain.SetTexture("redstone_glow");
                else if (this.BlockId.Contains("amethyst"))
                    this.glowMain.SetTexture("amethyst_glow");
                else if (this.BlockId.Contains("soul"))
                    this.glowMain.SetTexture("sea_lantern_glow");
                else if (this.BlockId is "lantern" || this.BlockId.Contains("torch") || this.BlockId.Contains("campfire"))
                    this.glowMain.SetTexture("shroomlight_glow");
                else
                    this.glowMain.SetTexture(this.BlockId + "_glow");
                this.icon.AddControl(this.glowMain);
            }
        }

        public override void ResizeChildren()
        {
            base.ResizeChildren();
            if (this.Block.Id is "minecraft:kelp")
            {
                var virtualBounds = new Rectangle(
                    this.Center.X - (this.icon.FlexWidth.GetAbsoluteInt() / 2),
                    this.Center.Y - (this.icon.FlexHeight.GetAbsoluteInt() / 2),
                    this.icon.FlexWidth.GetAbsoluteInt(),
                    this.icon.FlexHeight.GetAbsoluteInt());
                this.icon.ResizeThis(virtualBounds);
            }
        }
    }
}
