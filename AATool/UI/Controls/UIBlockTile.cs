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

        private static readonly HashSet<string> DoubleTallBlocks = new () {
            "minecraft:cactus",
            "minecraft:sugar_cane",
            "minecraft:bamboo",
            "minecraft:rose_bush",
            "minecraft:peony",
            "minecraft:lilac",
            "minecraft:sunflower",
            "minecraft:small_dripleaf",
            "minecraft:armor_stand",
        };

        public string BlockId { get; set; }
        public Block Block;

        private UIPicture icon;
        //private UIPopup popup;
        private UIGlowEffect glowMain;
        private Color glowColor;
        private int scale;

        private float brightness;
        private bool manulOverride;

        public bool IsCompleted => this.Block.CompletedByAnyone();

        private string BackgroundTexture => this.Block?.ManuallyCompleted is true
            ? "block_tile_manual"
            : "block_tile_complete";

        public UIBlockTile()
        {
            this.scale = Config.Main.RelaxedMode ? 3 : 2;
            this.BuildFromTemplate();
        }

        public UIBlockTile(int scale = 2) : this()
        {
            this.scale = scale;
        }

        public override void InitializeThis(UIScreen screen)
        {
            //this.isMainWindow = screen is UIMainScreen;
            Tracker.Blocks.TryGet(this.BlockId, out this.Block);

            this.Name = this.Block.Id;
            //this.popup = this.First<UIPopup>();
            this.icon = this.First<UIPicture>();
            int size = 16 * this.scale;
            if (SpriteSheet.IsAnimated(this.Block.Icon + SpriteSheet.ResolutionPrefix + size))
                this.icon.SetLayer(Layer.Fore);
            this.icon?.SetTexture(this.Block.Icon);
            this.brightness = this.Block.CompletedByAnyone() ? 1 : 0;
            this.glowColor = Color.White;

            this.Style();
        }

        /*
        public bool TryShowTooltip()
        {
            if (!this.Block.CompletedByAnyone()
                && Config.Main.CompactMode
                && this.Bounds.Contains(Input.Current.Position))
            {
                this.GetRootScreen().First<UIBlockPopup>()?.SetSource(this);
                return true;
            }
            return false;
        }
        */

        protected override void UpdateThis(Time time)
        {
            if (this.Block is null)
                return;

            this.targetBrightness = this.Block.CompletedByAnyone() ? 1 : 0;
            this.brightness = MathHelper.Lerp(this.brightness, this.targetBrightness, (float)(10 * time.Delta));

            this.targetColor = this.Block.ManuallyCompleted ? Color.Cyan : Color.White;
            if (Math.Abs(this.brightness - this.targetBrightness) > 0.01f || this.glowColor != this.targetColor)
                UIMainScreen.Invalidate();
            this.glowColor = this.targetColor;
        }

        public override void DrawThis(Canvas canvas) 
        { 
            if (this.SkipDraw)
                return;

            Color borderColor;
            if (this.IsCompleted)
            {
                borderColor = this.Block.ManuallyCompleted
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

            if (Config.Main.CompactMode && this.Parent?.Parent is not UIBlockPopup) 
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
            else if (this.Block.Id is "minecraft:kelp")
            {
                this.FlexHeight *= 2;
                this.icon.FlexHeight *= 2;
                this.icon.FlexWidth *= 2;
                this.icon.SetLayer(Layer.Fore);
            }

            if (this.Block.Glows)
            {
                this.glowMain = new UIGlowEffect() { Scale = Config.Main.CompactMode ? 1 : 1.25f };              
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
