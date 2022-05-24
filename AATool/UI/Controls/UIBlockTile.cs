using System;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIBlockTile : UIButton
    {
        private const float FadeFast = 10f;
        private const float FadeSlow = 5f;

        public string BlockId { get; set; }

        public Block Block;

        private UIPicture icon;
        private UIGlowEffect glowMain;
        private bool spriteLoaded;
        private bool wasHighlighted;
        private bool wasPlaced;
        private string highlightTexture;
        private float opacityHighlight;
        private float opacityPlaced;
        private float opacityBlock;
        private int scale;

        private bool IsAnimated => SpriteSheet.IsAnimated(this.Block.Icon + Sprite.ResolutionFlag + (16 * this.scale));

        private string PlacedTexture => this.Block.DoubleHeight ? "block_tile_tall_gold" : "block_tile_gold";

        public UIBlockTile()
        {
            this.BuildFromTemplate();
            this.scale = 2;
            this.BorderThickness = 1;
            this.opacityBlock = AllBlocks.SpritesLoaded ? 1 : 0;
        }

        public UIBlockTile(int scale = 2) : this()
        {
            this.scale = scale;
            this.opacityBlock = AllBlocks.SpritesLoaded ? 1 : 0;
        }

        public override void InitializeThis(UIScreen screen)
        {
            //this.isMainWindow = screen is UIMainScreen;
            Tracker.TryGetBlock(this.BlockId, out this.Block);

            this.Name = this.Block.Id;
            //this.popup = this.First<UIPopup>();
            this.icon = this.First<UIPicture>();
            if (this.IsAnimated)
                this.icon.SetLayer(Layer.Fore);
            this.icon?.SetTexture(this.Block.Icon);
            this.opacityPlaced = this.Block.HasBeenPlaced ? 1 : 0;
            this.opacityHighlight = this.Block.Highlighted ? 1 : 0;

            this.Style();
        }

        protected override void UpdateThis(Time time)
        {
            if (this.Block is null)
                return;

            //fade blocks in once textures load
            this.UpdateIconOpacity(time);
            this.UpdateBackgroundOpacity(time);
            this.UpdateTextures();
        }

        private void UpdateIconOpacity(Time time)
        {
            float target = Tracker.IsWorking || Peer.IsClient ? 1 : 0.5f;
            if (this.opacityBlock != target)
            {
                this.opacityBlock = MathHelper.Lerp(this.opacityBlock, AllBlocks.SpritesLoaded ? target : 0, (float)(5 * time.Delta));
                if (this.opacityBlock > 0.99)
                    this.opacityBlock = 1;

                this.icon.SetTint(ColorHelper.Fade(Color.White, this.opacityBlock));
                if (!this.spriteLoaded && AllBlocks.SpritesLoaded && this.IsAnimated)
                {
                    this.spriteLoaded = true;
                    this.icon.SetLayer(Layer.Fore);
                }
            }
        }

        private void UpdateBackgroundOpacity(Time time)
        {
            float targetPlaced = this.Block.HasBeenPlaced ? 1 : 0;
            if (this.opacityPlaced != targetPlaced)
            {
                this.opacityPlaced = MathHelper.Lerp(this.opacityPlaced, targetPlaced, (float)(FadeSlow * time.Delta));
                if (this.opacityPlaced > 0.99)
                    this.opacityPlaced = 1;
                else if (this.opacityPlaced < 0.01)
                    this.opacityPlaced = 0;
                UIMainScreen.Invalidate();
            }

            float targetHighlight = this.Block.Highlighted && (Tracker.IsWorking || Peer.IsClient) ? 1 : 0;
            if (this.opacityHighlight != targetHighlight)
            {
                if (this.opacityHighlight > 0.99)
                    this.opacityHighlight = 1;
                else if (this.opacityHighlight < 0.01)
                    this.opacityHighlight = 0;
                this.opacityHighlight = MathHelper.Lerp(this.opacityHighlight, targetHighlight, (float)(FadeFast * time.Delta));
                UIMainScreen.Invalidate();
            }
        }

        private void UpdateTextures()
        {
            if (this.wasPlaced != this.Block.HasBeenPlaced || this.Block.Highlighted != this.wasHighlighted)
            {
                if (this.Block.DoubleHeight)
                {
                    this.highlightTexture = this.Block.HasBeenPlaced
                        ? "block_tile_tall_green"
                        : "block_tile_tall_red";
                }
                else
                {
                    this.highlightTexture = this.Block.HasBeenPlaced
                        ? "block_tile_green"
                        : "block_tile_red";
                }
                this.wasPlaced = this.Block.HasBeenPlaced;
                this.wasHighlighted = this.Block.Highlighted;
            }
        }

        public override void DrawThis(Canvas canvas) 
        { 
            if (this.SkipDraw)
                return;
            
            //background
            canvas.DrawRectangle(this.Bounds,
                Config.Main.BackColor,
                Config.Main.BorderColor,
                this.BorderThickness,
                this.Layer);

            //gold placed state
            if (this.opacityPlaced > 0.01)
                canvas.Draw(this.PlacedTexture, this.Bounds, ColorHelper.Fade(Color.White, this.opacityPlaced));

            //green confirmed state
            if (this.opacityHighlight > 0.01)
                canvas.Draw(this.highlightTexture, this.Bounds, ColorHelper.Fade(Color.White, this.opacityHighlight));
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
            
            if (this.Block.Id is "minecraft:kelp")
            {
                this.FlexHeight *= 2;
                this.icon.FlexHeight *= 2;
                this.icon.FlexWidth *= 2;
                this.icon.SetLayer(Layer.Fore);
            }
            else if (this.Block.DoubleHeight)
            {
                this.FlexHeight *= 2;
                this.icon.FlexHeight *= 2;
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
                else if (this.BlockId is "minecraft:lantern" || this.BlockId.Contains("torch") || this.BlockId.Contains("campfire"))
                    this.glowMain.SetTexture("shroomlight_glow");
                else
                    this.glowMain.SetTexture(this.BlockId.Replace("minecraft:", "") + "_glow");
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
