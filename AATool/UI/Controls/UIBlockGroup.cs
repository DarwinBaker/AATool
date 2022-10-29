using System.Collections.Generic;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIBlockGroup : UIFlowPanel
    {
        public string GroupId { get; private set; }
        public string StartId { get; private set; }
        public string EndId { get; private set; }

        private readonly HashSet<string> excluded = new ();
        private readonly List<UIBlockTile> tiles = new ();

        private List<Block> blocks;

        private UIBlockGrid blockGrid;
        private UIBlockPopup popup;

        public UIBlockGroup()
        {
            this.HorizontalAlign = HorizontalAlign.Left;
            this.VerticalAlign = VerticalAlign.Top;
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.blockGrid = this.Root().First<UIBlockGrid>();
            this.popup = screen.First<UIBlockPopup>();
            this.Populate();
        }

        private void Populate()
        {
            //start adding from beginning if no start id specified 
            if (!Tracker.Blocks.TryGetGroup(this.GroupId, out this.blocks))
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
                        FlexWidth = new Size(UIBlockTile.Dimension),
                        FlexHeight = new Size(UIBlockTile.Dimension),
                        DrawMode = DrawMode.None,
                    });
                }
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.blockGrid.IsActive)
                return;

            Point cursor = Input.Cursor(this.Root());

            //optimization, skip almost all unnecessary block hover checks
            if (!this.Bounds.Contains(cursor) || !this.Root().Form.Focused)
                return;

            foreach (UIBlockTile tile in this.tiles)
            {
                //check for hovered block
                if (!tile.Bounds.Contains(cursor))
                    continue;

                if (Input.LeftClicked && (Tracker.IsWorking || Peer.IsClient))
                {
                    //toggle highlight of single block
                    if (this.blockGrid.SelectionMade || this.blockGrid.WasSelectionMade || !tile.IsActive || UIMainScreen.SettingsJustClosed)
                        return;

                    if (tile.TryToggleHighlight())
                        (Tracker.Category as AllBlocks)?.SaveChecklist();
                }
                else if ((Input.RightClicking || Input.RightClicked) && this.blockGrid.Selection == default)
                {
                    //show the block details popup
                    this.popup.SetSource(tile);
                }

                //optimization, found hovered block. don't need to check any more
                return;
            }
        }



        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.GroupId = Attribute(node, "group", string.Empty);
        }
    }
}
