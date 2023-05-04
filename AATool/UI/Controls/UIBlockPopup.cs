using System;
using System.Collections.Generic;
using System.Linq;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIBlockPopup : UIPanel
    {
        private const float LerpSpeed = 30;

        private UIBlockGrid blockGrid;
        private List<UIBlockTile> selection;
        private UITextBlock blockName;
        private UITextBlock algebraicNotation;
        private UIBlockTile source;
        private UIBlockTile preview;
        private UIPanel window;
        private UIControl topArrow;
        private UIControl bottomArrow;

        private UIButton highlightButton;
        private UIButton confirmButton;
        private UIButton clearHighlightedButton;
        private UIButton clearConfirmedButton;

        private Vector2 targetLocation;
        private Vector2 currentLocation;
        private Point previousLocation;

        private string back;
        private string border;
        private string glow;

        private readonly Timer showTimer = new (5);

        public UIBlockPopup()
        {
            this.BuildFromTemplate();
            this.Layer = Layer.Fore;
        }

        public void Finalize(Time time)
        {
            this.showTimer.Update(time);
            bool justExpanded = false;
            if (this.showTimer.IsExpired && !Input.RightClicking)
            {
                this.Collapse();
                this.source = null;
                this.selection = null;
                if (this.blockGrid?.SelectionMade is true)
                    this.blockGrid?.ClearSelection();
            }
            else if (this.IsCollapsed && (this.source is not null || this.selection is not null))
            {
                this.Expand();
                justExpanded = true;
            }

            this.currentLocation = Vector2.Lerp(this.currentLocation, this.targetLocation, (float)(LerpSpeed * time.Delta));
            if (Vector2.Distance(this.currentLocation, this.targetLocation) < 0.1f)
                this.currentLocation = this.targetLocation;

            var nextLocation = this.currentLocation.ToPoint();
            if (nextLocation != this.previousLocation || justExpanded)
            {
                int x = nextLocation.X - (this.Width / 2);
                x = MathHelper.Clamp(x, -32, this.Root().Width - 32);
                this.MoveTo(new Point(x, nextLocation.Y + 8));
                this.previousLocation = nextLocation;

            }
            this.preview?.SetBlockOpacity(1f);
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.blockGrid = this.Root().First<UIBlockGrid>();
            this.window = this.First<UIPanel>("window");
            this.blockName = this.window.First<UITextBlock>("block_name");
            this.algebraicNotation = this.window.First<UITextBlock>("algebraic_notation");
            this.topArrow = this.First("arrow_top");
            this.bottomArrow = this.First("arrow_bottom");
            this.window.DrawMode = DrawMode.ChildrenOnly;



            this.highlightButton = this.First<UIButton>("selection_highlight");
            this.highlightButton.OnClick += this.Click;
            this.highlightButton.First<UITextBlock>().HorizontalTextAlign = HorizontalAlign.Left;
            this.highlightButton.First<UITextBlock>().Padding = new Margin(24, 0, 0, 0);

            this.clearHighlightedButton = this.First<UIButton>("clear_highlighted");
            this.clearHighlightedButton.OnClick += this.Click;
            this.clearHighlightedButton.First<UITextBlock>().HorizontalTextAlign = HorizontalAlign.Left;
            this.clearHighlightedButton.First<UITextBlock>().Padding = new Margin(24, 0, 0, 0);



            this.confirmButton = this.First<UIButton>("selection_confirm");
            this.confirmButton.OnClick += this.Click;
            this.confirmButton.First<UITextBlock>().HorizontalTextAlign = HorizontalAlign.Left;
            this.confirmButton.First<UITextBlock>().Padding = new Margin(24, 0, 0, 0);

            this.clearConfirmedButton = this.First<UIButton>("clear_confirmed");
            this.clearConfirmedButton.OnClick += this.Click;
            this.clearConfirmedButton.First<UITextBlock>().HorizontalTextAlign = HorizontalAlign.Left;
            this.clearConfirmedButton.First<UITextBlock>().Padding = new Margin(24, 0, 0, 0);
        }

        private void Click(UIControl sender)
        {
            if (sender == this.highlightButton)
            {
                foreach (UIBlockTile tile in this.selection)
                {
                    if (!tile.Block.Highlighted && !tile.Block.IsComplete())
                        tile.Block.Highlighted = true;
                }
                (Tracker.Category as AllBlocks)?.SaveChecklist();
            }
            else if (sender == this.clearHighlightedButton)
            {
                foreach (UIBlockTile tile in this.selection)
                {
                    if (tile.Block.Highlighted && !tile.Block.IsComplete())
                        tile.Block.Highlighted = false;
                }
                (Tracker.Category as AllBlocks)?.SaveChecklist();
            }
            else if (sender == this.confirmButton)
            {
                foreach (UIBlockTile tile in this.selection)
                {
                    if (!tile.Block.Highlighted && tile.Block.IsComplete())
                        tile.Block.Highlighted = true;
                }
                (Tracker.Category as AllBlocks)?.SaveChecklist();
            }
            else if (sender == this.clearConfirmedButton)
            {
                this.ClearConfirmed();
                (Tracker.Category as AllBlocks)?.SaveChecklist();
            }
            this.blockGrid.ClearSelection();
            this.Hide();
        }

        private void ClearConfirmed()
        {
            int count = 0;
            foreach (UIBlockTile tile in this.selection)
            {
                if (tile.Block.Highlighted && tile.Block.IsComplete())
                    count++;
            }

            var result = System.Windows.Forms.DialogResult.OK;
            if (count > 1)
            {
                result = System.Windows.Forms.MessageBox.Show(
                    $"You are about to clear {count} currently confirmed (green) blocks. You'll have to manually re-confirm them all after making sure they've been placed at your central location. Are you sure you want to perform this action?",
                    $"Clear {count} confirmed blocks",
                    System.Windows.Forms.MessageBoxButtons.OKCancel,
                    System.Windows.Forms.MessageBoxIcon.Warning);
            }

            if (result is System.Windows.Forms.DialogResult.OK)
            {
                foreach (UIBlockTile tile in this.selection)
                {
                    if (tile.Block.Highlighted && tile.Block.IsComplete())
                        tile.Block.Highlighted = false;
                }
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            canvas.Draw(this.glow, this.window.Bounds, Config.Main.TextColor, Layer.Fore);
            canvas.Draw(this.back, this.window.Bounds, this.Root().FrameBackColor(), Layer.Fore);
            canvas.Draw(this.border, this.window.Bounds, this.Root().FrameBorderColor(), Layer.Fore);

            if (this.VerticalAlign is VerticalAlign.Top)
            {
                canvas.Draw($"popup_block_arrow_bottom_glow",
                    this.bottomArrow.Bounds,
                    Config.Main.TextColor,
                    Layer.Fore);
                canvas.Draw($"popup_block_arrow_bottom_back",
                    this.bottomArrow.Bounds,
                    this.Root().FrameBackColor(),
                    Layer.Fore);
                canvas.Draw($"popup_block_arrow_bottom_border",
                    this.bottomArrow.Bounds,
                    this.Root().FrameBorderColor(),
                    Layer.Fore);
            }
            else
            {
                canvas.Draw($"popup_block_arrow_top_glow",
                    this.topArrow.Bounds,
                    Config.Main.TextColor,
                    Layer.Fore);
                canvas.Draw($"popup_block_arrow_top_back",
                    this.topArrow.Bounds,
                    this.Root().FrameBackColor(),
                    Layer.Fore);
                canvas.Draw($"popup_block_arrow_top_border",
                    this.topArrow.Bounds,
                    this.Root().FrameBorderColor(),
                    Layer.Fore);
            }
        }

        public override void MoveTo(Point point)
        {
            if (this.VerticalAlign is VerticalAlign.Top)
            {
                base.MoveTo(new Point(point.X, point.Y - this.Height - 8));
            }
            else
            {
                base.MoveTo(new Point(point.X, point.Y - 8));
            }
        }

        public void Hide()
        {
            this.showTimer.Expire();
        }

        public void SetSource(UIBlockTile block)
        {
            if (block is null)
            {
                this.Hide();
                return;
            }

            if (block.Block == this.source?.Block)
            {
                if (Input.RightClicked)
                    this.Hide();
                return;
            }

            if (this.source is null)
            {
                this.Arrange(block.Center);
                this.targetLocation = block.Center.ToVector2();
                this.currentLocation = block.Center.ToVector2();
                this.MoveTo(block.Center);
            }
            else
            {
                this.Arrange(block.Center);
            }
            //this.showTimer.SetAndStart(5.0);

            if (this.source == block)
                return;

            this.source = block;
            this.blockName.SetText(block.Block.Name);
            this.algebraicNotation.SetText(block.AlgebraicNotation);
            this.targetLocation = block.Center.ToVector2();

            this.back = "popup_block_back";
            this.border = "popup_block_border";
            this.glow = "popup_block_glow";
            this.FlexWidth = new(128);

            this.blockName.Expand();
            this.algebraicNotation.Expand();
            this.RemoveControl(this.preview);
            int top = block.Block.DoubleHeight || block.BlockId is "minecraft:kelp" ? 32 : 68;
            this.preview = new UIBlockTile(block.Block.DoubleHeight ? 2 : 3) {
                VerticalAlign = VerticalAlign.Top,
                DrawMode = DrawMode.ChildrenOnly,
                BlockId = block.Block.Id,
                Margin = new Margin(0, 0, top, 0),
                Layer = Layer.Fore,
            };
            this.window.ClearControls();
            this.window.AddControl(this.blockName);
            this.window.AddControl(this.algebraicNotation);
            this.window.AddControl(this.preview);
            this.preview.InitializeRecursive(this.Root());

            this.ResizeRecursive(this.Parent.Inner);
            UIMainScreen.Invalidate();
        }

        public void SetSelection(IEnumerable<UIBlockTile> blocks, Rectangle selection, Point endPoint)
        {
            if (blocks is null || !blocks.Any())
                return;

            this.selection = new (blocks);

            this.blockName?.Collapse();
            this.algebraicNotation?.Collapse();
            this.preview?.Collapse();

            //int left = selectionBounds.Left / 38 * 38;
            //int width = selectionBounds.Width / 38 * 38;

            //var offset = new Point(left + width / 2, selectionBounds.Top / 38 * 38);
            int selectionLeftBound = ((selection.Left - 20) / 38 * 38);
            int selectionRightBound = (int)(Math.Ceiling((selection.Right - 20) / 38.0) * 38) + 38;
            int selectionTopBound = ((selection.Top - 20) / 38 * 38);
            int selectionBottomBound = (int)(Math.Ceiling((selection.Bottom - 20) / 38.0) * 38);

            int selectionWidth = selectionRightBound - selectionLeftBound;
            int selectionHeight  = selectionBottomBound - selectionTopBound;

            //position center top/bottom
            int spaceAbove = selectionTopBound;
            int spaceBelow = this.Root().Height - selectionBottomBound;
            int centerX = selectionLeftBound + (selectionWidth / 2);

            Point offset;
            if (spaceAbove < 4 * 38 || selectionHeight > 4 * 38)
            {
                offset = new Point(centerX, selectionBottomBound);
            }
            else
            {
                offset = new Point(centerX, selectionTopBound + 28);
            }
            //offset = new Point(centerX, selectionBottomBound);

            if (this.source is null)
            {
                this.Arrange(offset);
                this.targetLocation = offset.ToVector2();
                this.currentLocation = offset.ToVector2();
                this.MoveTo(offset);
            }
            else
            {
                this.Arrange(offset);
            }
            this.showTimer.SetAndStart(10);

            this.source = null;
            this.targetLocation = offset.ToVector2();

            this.back = "popup_block_back";
            this.border = "popup_block_border";
            this.glow = "popup_block_glow";
            this.FlexWidth = new(128);

            this.RemoveControl(this.preview);
            this.window.ClearControls();

            this.window.AddControl(this.highlightButton);
            this.window.AddControl(this.clearHighlightedButton);
            this.window.AddControl(this.confirmButton);
            this.window.AddControl(this.clearConfirmedButton);

            int highlighted = 0;
            int confirmed = 0;
            int complete = 0;
            int incomplete = 0;
            foreach (UIBlockTile tile in this.selection)
            {
                if (tile.Block.Highlighted)
                {
                    if (tile.Block.IsComplete())
                        confirmed++;
                    else
                        highlighted++;
                }
                else
                {
                    if (tile.Block.IsComplete())
                        complete++;
                    else
                        incomplete++;
                }
            }


            this.highlightButton.Enabled = incomplete > 0;
            this.highlightButton.First<UIPicture>()?
                .SetTint(this.highlightButton.Enabled ? Color.White : ColorHelper.Fade(Color.White, 0.1f));

            this.clearHighlightedButton.Enabled = highlighted > 0;
            this.clearHighlightedButton.SetText(this.clearHighlightedButton.Enabled ? $"Clear {highlighted}" : "Clear");
            this.clearHighlightedButton.First<UIPicture>()?
                .SetTint(this.clearHighlightedButton.Enabled ? Color.White : ColorHelper.Fade(Color.White, 0.1f));

            this.confirmButton.Enabled = complete > 0;
            this.confirmButton.First<UIPicture>()?
                .SetTint(this.confirmButton.Enabled ? Color.White : ColorHelper.Fade(Color.White, 0.1f));

            this.clearConfirmedButton.Enabled = confirmed > 0;
            this.clearConfirmedButton.SetText(this.clearConfirmedButton.Enabled ? $"Clear {confirmed}" : "Clear");
            this.clearConfirmedButton.First<UIPicture>()?
                .SetTint(this.clearConfirmedButton.Enabled ? Color.White : ColorHelper.Fade(Color.White, 0.1f));

            this.ResizeRecursive(this.Parent.Inner);
            UIMainScreen.Invalidate();
        }

        public void Arrange(Point targetCenter)
        {
            this.HorizontalAlign = HorizontalAlign.Center;
            this.VerticalAlign = targetCenter.Y > 4 * 38
                ? VerticalAlign.Top
                : VerticalAlign.Bottom; 
        }
    }
}
