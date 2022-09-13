using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIBlockGrid : UIControl
    {
        public static readonly Point EmptySelection = new (int.MinValue);

        public bool SelectionMade { get; private set; }
        public bool WasSelectionMade { get; private set; }
        public bool MakingSelection { get; private set; }
        public bool Searching { get; private set; }
        public int HighlightedInSelection { get; private set; }
        public int ConfirmedInSelection { get; private set; }
        public Point SelectionStart { get; private set; }
        public Point SelectionEnd { get; private set; }
        public Rectangle Selection { get; private set; }

        public bool DimScreen { get; private set; }

        public readonly HashSet<UIBlockTile> SelectedBlocks = new ();

        private readonly Dictionary<(int row, int col), UIBlockTile> blockTiles = new ();

        private UIBlockPopup popup;
        private UITextInput searchInput;
        private UITextBlock searchResults;
        private UIControl searchShortcut;

        private int blockTileRows;
        private int blockTileColumns;
        private int blocksFound;
        private bool clearedSelection;

        public bool IsSearching => this.searchInput.State is ControlState.Pressed;

        public void RegisterBlockTile(UIBlockTile block)
        {
            if (block is null)
                return;

            //update blocktile grid bounds
            Point coordinates = block.GetGridCoordinates();
            this.blockTileRows = Math.Max(coordinates.Y, this.blockTileRows);
            this.blockTileColumns = Math.Max(coordinates.X, this.blockTileColumns);

            //update blocktile dictionary
            this.blockTiles[(coordinates.Y, coordinates.X)] = block;
            if (block.Block?.DoubleHeight is true)
                this.blockTiles[(coordinates.Y + 1, coordinates.X)] = block;
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.popup = this.Root().First<UIBlockPopup>();
            this.searchInput = this.Root().First<UITextInput>("block_search");
            if (this.searchInput is not null)
                this.searchInput.OnTextChanged += this.BlockSearchTextChanged;
            this.searchResults = this.Root().First<UITextBlock>("block_search_results");
            this.searchShortcut = this.Root().First("block_search_shortcut");
        }

        protected override void UpdateThis(Time time)
        {
            this.UpdateShortcuts();
            this.UpdateSelection();

            bool wasDim = this.DimScreen;
            this.DimScreen = this.MakingSelection || this.SelectionMade || this.Searching;
            if (this.DimScreen != wasDim)
                UIMainScreen.Invalidate();
        }

        public override void UpdateRecursive(Time time)
        {
            base.UpdateRecursive(time);

            UIBlockTile.ClickConfirmationTimer.Update(time);
            if (UIBlockTile.ClickConfirmationTimer.IsExpired)
            {
                if (UIBlockTile.AwaitingConfirmation is not null)
                    UIBlockTile.AwaitingConfirmation = null;
            }
            else if (Input.LeftClicked && UIBlockTile.ClickConfirmationTimer.TimeElapsed > 0.1f)
            {
                UIBlockTile.ClickConfirmationTimer.Expire();
            }
            this.WasSelectionMade = this.SelectionMade;
        }

        private void UpdateShortcuts()
        {
            bool showShortcut = !this.Searching && !this.SelectionMade && (this.searchInput?.State != ControlState.Pressed);
            this.searchShortcut?.SetVisibility(showShortcut);

            //ctrl+f (all blocks quick find)
            bool ctrl = Input.IsDown(Microsoft.Xna.Framework.Input.Keys.LeftControl)
                || Input.IsDown(Microsoft.Xna.Framework.Input.Keys.RightControl);
            if (ctrl && Input.IsDown(Microsoft.Xna.Framework.Input.Keys.F) && !this.SelectionMade)
            {
                this.searchInput?.SetFocus(true);
            }
        }

        private void UpdateSelection()
        {
            //this function is huge and complicated for many important reasons
            //and now that it works we are going to hope we don't need to touch it again
            if (Tracker.Category is not AllBlocks || UIMainScreen.ActiveTab is not UIMainScreen.TrackerTab)
                return;

            if (this.SelectionMade)
            {
                if (!Tracker.IsWorking || Input.LeftClicked || Input.RightClicked)
                {
                    this.ClearSelection();
                    return;
                }
                this.Searching = false;
            }

            if (!Tracker.IsWorking)
                return;

            if (Input.LeftClicking && this.Root().Form.Focused)
            {
                Point cursorPosition = Input.Cursor(this.Root());

                //update selection points
                if (this.SelectionStart == EmptySelection && this.Bounds.Contains(cursorPosition))
                    this.SelectionStart = cursorPosition;
                if (this.SelectionStart != EmptySelection)
                    this.SelectionEnd = cursorPosition;

                if (this.SelectionStart != this.SelectionEnd)
                {
                    if (this.SelectedBlocks.Count > 1)
                    {
                        foreach (UIBlockTile block in this.blockTiles.Values)
                            block.SetActiveState(false);
                    }
                    this.UpdateSelectionBounds();
                }

                if (this.SelectionStart != EmptySelection && this.SelectionStart != this.SelectionEnd)
                    this.UpdateSelectedBlocks();
            }

            //validate contents of selection
            if (!Input.LeftClicking)
            {
                if (this.SelectedBlocks.Count > 1)
                {
                    this.SelectionMade = true;
                    this.popup.SetSelection(this.SelectedBlocks, this.Selection, this.SelectionEnd);
                    this.SelectedBlocks.Clear();
                    this.Selection = default;
                }
                else
                {
                    this.SelectedBlocks.Clear();
                    if (this.Selection != default)
                    {
                        foreach (UIBlockTile tile in this.blockTiles.Values)
                            tile.SetActiveState(true);
                        this.Selection = default;

                    }
                    this.SelectionStart = EmptySelection;
                    this.SelectionEnd = EmptySelection;
                    this.MakingSelection = false;
                }
            }
        }

        public void Finalize(Time time)
        {
            this.popup.Finalize(time);
            if (this.clearedSelection)
            {
                this.clearedSelection = false;
                this.SelectionMade = false;
            }
        }

        private void UpdateSelectionBounds()
        {
            //calculate rectangular selection bounds
            int left = Math.Min(this.SelectionStart.X, this.SelectionEnd.X);
            int right = Math.Max(this.SelectionStart.X,this. SelectionEnd.X);
            int top = Math.Min(this.SelectionStart.Y, this.SelectionEnd.Y);
            int bottom = Math.Max(this.SelectionStart.Y, this.SelectionEnd.Y);
            //clamp to bounds of screen
            left = Math.Min(left, (this.blockTileColumns + 1) * 38);
            right = Math.Min(right, (this.blockTileColumns + 1) * 38);
            top = Math.Min(top, (this.blockTileRows + 1) * 38);
            bottom = Math.Min(bottom, (this.blockTileRows + 1) * 38);
            this.Selection = new Rectangle(left, top, right - left, bottom - top);
        }

        public void ClearSelection()
        {
            this.SelectionStart = EmptySelection;
            this.SelectionEnd = EmptySelection;
            this.Selection = default;
            this.SelectedBlocks.Clear();
            this.popup?.SetSource(null);
            foreach (UIBlockTile tile in this.blockTiles.Values)
                tile.SetActiveState(true);
            this.clearedSelection = true;
            this.MakingSelection = false;
            this.SelectionMade = false;
            //this.DimScreen = false;
        }

        private void UpdateSelectedBlocks()
        {
            //convert mouse selection coordinates to block grid coordinates
            int leftCol = MathHelper.Clamp((int)Math.Truncate((this.Selection.Left - 20) / 38.0), 0, this.blockTileColumns);
            int rightCol = MathHelper.Clamp((int)Math.Ceiling((this.Selection.Right - 20) / 38.0), 0, this.blockTileColumns + 1);
            int topRow = MathHelper.Clamp((int)Math.Truncate((this.Selection.Top - 20) / 38.0), 0, this.blockTileRows);
            int bottomRow = MathHelper.Clamp((int)Math.Ceiling((this.Selection.Bottom - 20) / 38.0), 0, this.blockTileRows + 1);

            //clear previous selection
            this.HighlightedInSelection = 0;
            this.ConfirmedInSelection = 0;
            if (this.SelectedBlocks.Count > 1)
            {
                foreach (UIBlockTile block in this.SelectedBlocks)
                    block.SetActiveState(false);
                this.MakingSelection = true;
                //this.DimScreen = true;
            }

            //activate block tiles in selection region
            this.SelectedBlocks.Clear();
            for (int row = topRow; row < bottomRow; row++)
            {
                for (int col = leftCol; col < rightCol; col++)
                {
                    if (this.blockTiles.TryGetValue((row, col), out UIBlockTile tile))
                    {
                        if (tile.Block.Highlighted is true)
                        {
                            if (tile.Block.IsComplete())
                                this.ConfirmedInSelection++;
                            else
                                this.HighlightedInSelection++;
                        }
                        tile.SetActiveState(true);
                        this.SelectedBlocks.Add(tile);
                    }
                }
            }
        }

        private void BlockSearchTextChanged(UIControl sender)
        {
            if (sender == this.searchInput)
            {
                if (this.SelectionMade)
                {
                    this.searchResults?.SetText("");
                    return;
                }

                this.ClearSelection();
                string query = (sender as UITextInput)?.UserInput
                    .Replace(" ", "").Replace(" ", "").Replace("\t", "").Replace("\n", "").ToLower();
                this.blocksFound = 0;
                foreach (UIBlockTile block in this.blockTiles.Values)
                {
                    bool match = block.SatisfiesSearch(query);
                    block.SetActiveState(match);
                    if (match)
                        this.blocksFound++;
                }
                this.Searching = !string.IsNullOrEmpty(query);    

                if (!this.Searching)
                {
                    this.searchResults?.SetText("");
                    this.DimScreen = false;
                }
                else
                {
                    this.DimScreen = true;
                    if (this.blocksFound is 0)
                        this.searchResults?.SetText("No Results");
                    else if (this.blocksFound is 1)
                        this.searchResults?.SetText("1 Result");
                    else
                        this.searchResults?.SetText($"{this.blocksFound} Results");
                }
            }
        }

        public override void DrawRecursive(Canvas canvas)
        {
            base.DrawRecursive(canvas);
            if (UIBlockTile.AwaitingConfirmation is UIBlockTile block)
            {
                string texture = block.Block.DoubleHeight 
                    ? "block_clear_confirm_tall" 
                    : "block_clear_confirm";

                canvas.DrawRectangle(block.Bounds, Config.Main.BorderColor.Value * 0.75f, null, 0, Layer.Fore);
                canvas.Draw(texture, block.Bounds, Config.Main.TextColor, Layer.Fore);

                int remaining = (int)Math.Round(1 + ((block.Width - 6) * UIBlockTile.ClickConfirmationTimer.Normalized), 
                    0, MidpointRounding.AwayFromZero) / 2 * 2;
                int sides = block.Width - remaining;
                var bar = new Rectangle(block.Left + (sides / 2), block.Bottom - 4, remaining, 1);
                canvas.DrawRectangle(bar, Config.Main.TextColor, null, 0, Layer.Fore);
            }
        }
    }
}
