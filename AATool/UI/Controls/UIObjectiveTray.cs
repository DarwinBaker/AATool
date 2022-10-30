using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AATool.UI.Controls
{
    public class UIObjectiveTray : UIControl
    {
        private UIFlowPanel flowPanel;
        private UIPinnedRow pinnedRow;
        private UIButton cancel;
        private UITextBlock header;

        private List<UIButton> frameButtons = new ();

        private bool locked;

        public UIObjectiveTray(UIPinnedRow pinnedRow)
        {
            this.pinnedRow = pinnedRow;
            this.BuildFromTemplate();
            this.Collapse();
        }

        public override void InitializeThis(UIScreen screen)
        {
            base.InitializeThis(screen);
            this.flowPanel = this.First<UIFlowPanel>();
            this.cancel = this.First<UIButton>("cancel");
            this.header = this.First<UITextBlock>("header");
            if (this.cancel is not null)
            {
                this.cancel.UseCustomColor = true;
                this.cancel.OnClick += this.OnClick;
            }
            this.Populate();
        }

        public void Clear()
        {
            this.frameButtons.Clear();
            this.flowPanel.ClearControls();
        }

        public void Populate()
        {
            this.locked = true;

            //don't show duplicates
            var included = new List<string>(PinnedObjectiveSet.GetAllAvailable());
            if (Config.Overlay.PinnedObjectiveList.Value.TryGetCurrentList(out List<string> current))
            {
                foreach (string pinnedObjective in current)
                    included.Remove(pinnedObjective);
            }

            if (included.Any())
                this.header?.SetText($"Pick an objective to pin to your {Tracker.Category.CurrentMajorVersion} {Tracker.Category.Name} overlay");
            else
                this.header?.SetText($"All available objectives already pinned");

            this.Clear();
            foreach (string includedObjective in included)
            {
                if (this.TryCreateButton(includedObjective, out UIButton button))
                {
                    this.frameButtons.Add(button);
                    this.flowPanel.AddControl(button);
                    button.OnClick += this.OnClick;
                }
            }
        }

        private void OnClick(UIControl sender)
        {
            if (sender == this.cancel)
            {
                (this.Root() as UIOverlayScreen).HideObjectiveTray();
            }
            else if (sender.Tag is string objectiveName)
            {
                if (this.locked)
                    return;

                if (Config.Overlay.PinnedObjectiveList.Value.TryGetCurrentList(out List<string> list))
                {
                    list.Add(objectiveName);
                    this.pinnedRow.RefreshList();
                    Config.Overlay.TrySave();
                    (this.Root() as UIOverlayScreen)?.PinnedObjectivesSaved();
                }
                (this.Root() as UIOverlayScreen).HideObjectiveTray();
            }
        }

        private bool TryCreateButton(string objectiveName, out UIButton button)
        {
            if (Tracker.TryGetComplexObjective(objectiveName, out ComplexObjective objective))
            {
                var frame = new UIObjectiveFrame(objective, 3) { 
                    VerticalAlign = VerticalAlign.Top,
                };
                frame.InitializeRecursive(this.Root());
                frame.SetForeground();
                frame.Label.Margin = new Margin(0, 0, 80, 0);
                button = new UIButton() {
                    FlexWidth = new (86),
                    FlexHeight = new (86),
                    Padding = new Margin(0, 0, -8, 0),
                    Layer = Layer.Fore,
                    BorderThickness = 4,
                    Tag = objectiveName,
                    UseCustomColor = true,
                };
                button.AddControl(frame);
                return true;
            }
            button = null;
            return false;
        }

        protected override void UpdateThis(Time time)
        { 
            base.UpdateThis(time);
            if (Config.Overlay.FrameStyle != "Minecraft")
            {
                this.cancel.BackColor = (this.Root() as UIOverlayScreen).FrameBackColor();
                this.cancel.BorderColor = (this.Root() as UIOverlayScreen).FrameBorderColor();
            }
            else
            {
                this.cancel.BackColor = Color.Transparent;
                this.cancel.BorderColor = Color.Transparent;
            }
            
            if (!Input.LeftClicking && !Input.LeftClicked)
                this.locked = false;

            foreach (UIButton button in this.frameButtons)
            {
                if (button.State == ControlState.Hovered)
                {
                    button.BackColor = Config.Overlay.GreenScreen;
                    button.BorderColor = Color.White;
                }
                else if (button.State == ControlState.Pressed)
                {
                    button.BackColor = Color.White;
                    button.BorderColor = Color.White;
                }
                else
                {
                    button.BackColor = Color.Transparent;
                    button.BorderColor = Color.Transparent;
                }   
            }

            if (Input.Started(Keys.Escape) && this.Root().HasFocus)
                (this.Root() as UIOverlayScreen).HideObjectiveTray();
        }

        public override void DrawThis(Canvas canvas)
        {
            if (!this.IsCollapsed)
                canvas.DrawRectangle(this.Root().Bounds, Config.Overlay.GreenScreen, null, 0, Layer.Fore);

            if (Config.Overlay.FrameStyle == "Minecraft")
                canvas.Draw("overlay_cancel", this.cancel.Bounds, Color.White, Layer.Fore);
        }
    }
}
