using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIBlockGroup : UIFlowPanel
    {
        public string GroupId { get; private set; }
        public string StartId { get; private set; }
        public string EndId { get; private set; }

        private readonly HashSet<string> excluded = new ();
        private List<UIBlockTile> tiles = new ();
        private List<Block> blocks;
        private UIBlockPopup popup;

        public UIBlockGroup()
        {
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.popup = screen.First<UIBlockPopup>();

            this.Populate();
        }

        private void Populate()
        {
            //start adding from beginning if no start id specified 
            if (!Tracker.Category.Blocks.TryGetGroup(this.GroupId, out this.blocks))
                return;

            foreach (Block block in this.blocks)
            {
                string id = block?.Id;

                //check if block should be added
                if (this.excluded.Contains(id))
                    continue;

                if (block is not null)
                {
                    //add block
                    var tile = new UIBlockTile {
                        BlockId = block.Id
                    };
                    this.tiles.Add(tile);
                    this.AddControl(tile);
                }
                else
                {
                    //add empty spacer
                    this.AddControl(new UIPanel() {
                        FlexWidth = new Size(38),
                        FlexHeight = new Size(38),
                        DrawMode = DrawMode.None,
                    });
                }
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;

            canvas.DrawRectangle(this.Bounds,
                Config.Main.BorderColor,
                Config.Main.BorderColor,
                this.BorderThickness,
                this.Layer);
        }

        protected override void UpdateThis(Time time)
        {
            //optimization to skip unnecessary hover checks
            Point cursor = Input.Cursor(this.Root());
            if (!this.Bounds.Contains(cursor))
                return;

            foreach (UIBlockTile tile in this.tiles)
            {
                if (tile.Bounds.Contains(cursor))
                {
                    if (Input.LeftClicked)
                        tile.Block.ToggleManualOverride();
                    if (Input.RightClicked)
                        this.popup.SetSource(tile);  

                    //found hovered block. don't need to check any more
                    break;
                }
            }
        }



        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.GroupId = Attribute(node, "group", string.Empty);
        }
    }
}
