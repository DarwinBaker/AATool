using System.Collections.Generic;
using System.Linq;
using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIPinnedRow : UIControl
    {
        public const int FrameWidth = 136;

        public UIPinnedObjectiveFrame DraggedFrame { get; private set; }

        private List<UIPinnedObjectiveFrame> frames = new List<UIPinnedObjectiveFrame>();

        public bool Dragging => this.DraggedFrame is not null;
        public bool IsDragging(UIPinnedObjectiveFrame frame) => this.DraggedFrame == frame;

        public int OffScreenStart => this.HorizontalAlign == HorizontalAlign.Left
            ? -FrameWidth : this.Root().Width;

        private int PinnedEndOffset => this.HorizontalAlign == HorizontalAlign.Left
            ? this.frames.Count * FrameWidth : this.Root().Width - (this.frames.Count * FrameWidth);

        private UITextBlock status;
        private UIButton pinButton;
        private Timer initializeTimer = new (1f, true);
        private float dragOffset;
        private float dragStart;
        private float pinButtonOffset = 0;
        private float statusOffset = 1;
        private bool pinButtonLocked;

        private readonly Timer mouseMoveTimer = new (0.1, false);

        private readonly List<UIPinnedObjectiveFrame> unpinned = new ();
         
        private float LerpSpeed => this.initializeTimer.IsExpired ? 15f : 15f;

        public void SetStatusLabel(UITextBlock label) => this.status = label;

        public UIPinnedRow()
        {
            this.FlexWidth = new Size(1, SizeMode.Relative);
            this.FlexHeight = new Size(152, SizeMode.Absolute);
        }

        public override void InitializeRecursive(UIScreen screen) 
        {
            base.InitializeRecursive(screen);
            this.RefreshList();
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.pinButton = new UIButton() {
                UseCustomColors = true,
                BorderThickness = 4,
                Layer = Layer.Fore,
                FlexWidth = new (54),
                FlexHeight = new (54),
            };

            base.InitializeThis(screen);
            
            var pinIcon = new UIPicture() {
                FlexWidth = new (48),
                FlexHeight = new (48),
                Layer = Layer.Fore,
            };
            pinIcon.SetTexture("pin");         
            this.pinButton.AddControl(pinIcon);
            

            this.AddControl(this.pinButton);
            this.pinButton.OnClick += this.OnClick;

            //this.pinButton.InitializeRecursive(screen);
            //this.pinButton.ResizeRecursive(screen.Bounds);
        }

        private void OnClick(UIControl sender)
        {
            if (sender == this.pinButton && !this.pinButtonLocked)
            {
                (this.Root() as UIOverlayScreen)?.ShowObjectiveTray();
            }
        }

        public override void ResizeThis(Rectangle parent)
        {
            base.ResizeThis(parent);
        } 

        private void UpdatePinButton(Time time)
        {
            const int PinButtonSize = 50;
            float targetX = this.HorizontalAlign == HorizontalAlign.Left
                ? this.PinnedEndOffset + 30 
                : this.PinnedEndOffset - PinButtonSize - 30;

            this.pinButtonOffset = (int)MathHelper.Lerp(this.pinButtonOffset, targetX, 
                (float)(this.LerpSpeed * time.Delta));

            var bounds = new Rectangle(
                (int)this.pinButtonOffset, 
                this.Top + 25,
                54,
                54);
            this.pinButton.ResizeRecursive(bounds);

            if (Config.Overlay.FrameStyle != "Minecraft")
            {
                this.pinButton.BackColor = (this.Root() as UIOverlayScreen).FrameBackColor();
                this.pinButton.BorderColor = (this.Root() as UIOverlayScreen).FrameBorderColor();
            }
            else
            {
                if (this.pinButton.State == ControlState.Hovered)
                {
                    this.pinButton.BackColor = Config.Overlay.GreenScreen;
                    this.pinButton.BorderColor = Color.White;
                }
                else if (this.pinButton.State == ControlState.Pressed)
                {
                    this.pinButton.BackColor = Color.White;
                    this.pinButton.BorderColor = Color.White;
                }
                else
                {
                    this.pinButton.BackColor = Color.Transparent;
                    this.pinButton.BorderColor = Color.Transparent;
                }
            } 
        }

        protected override void UpdateThis(Time time)
        {
            if (Config.Overlay.PinnedObjectiveList.Changed)
                this.RefreshList();

            this.mouseMoveTimer.Update(time);
            this.initializeTimer.Update(time);

            for (int i = 0; i < this.frames.Count; i++)
                this.frames[i].LerpToIndex(i, (float)(this.LerpSpeed * time.Delta));

            if (this.status is not null)
                this.PositionStatusLabel(time);

            bool showPinButton = false;
            Point cursor = Input.Cursor(this.Root());
            if (this.Root().HasFocus && this.Bounds.Contains(cursor))
                showPinButton = !this.Dragging;
            this.pinButton.SetVisibility(showPinButton);

            if (this.Dragging || this.unpinned.Any())
                this.pinButtonLocked = true;
            else if (Input.LeftClickStarted)
                this.pinButtonLocked = false;

            if (this.unpinned.Any())
            {
                foreach (UIPinnedObjectiveFrame unpinned in this.unpinned)
                {
                    this.frames.Remove(unpinned);
                    this.RemoveControl(unpinned);
                }
                this.unpinned.Clear();
                this.SortFrames();
                this.SaveChanges();
            }
            this.UpdatePinButton(time);
        }

        public void Unpin(UIPinnedObjectiveFrame frame)
        {
            this.DraggedFrame = null;
            if (!this.unpinned.Contains(frame))
                this.unpinned.Add(frame);
        }

        public void RefreshList()
        {
            this.ClearControls();
            this.AddControl(this.pinButton);
            this.frames.Clear();
            if (Config.Overlay.PinnedObjectiveList.Value.TryGetCurrentList(out List<string> list))
            {
                int index = 0;
                foreach (string typeName in list)
                {
                    if (Tracker.TryGetComplexObjective(typeName, out ComplexObjective objective))
                    {
                        var frame = new UIPinnedObjectiveFrame(this, objective);
                        this.frames.Add(frame);
                        this.AddControl(frame);
                        frame.InitializeRecursive(this.Root());
                        frame.ResizeRecursive(this.Bounds);
                        frame.MoveTo(new Point(this.OffScreenStart, this.Top));
                        frame.PreciseCenterX = frame.X;
                    }
                    index++;
                }
            }
        }

        public bool TryStartDragging(UIPinnedObjectiveFrame sender, Point cursor)
        {
            if (!this.Dragging)
            {
                this.DraggedFrame = sender;
                this.dragStart = sender.PreciseCenterX;
                this.dragOffset = sender.Center.X - cursor.X;
                return true;
            }
            return false;
        }

        public void StopDragging(UIPinnedObjectiveFrame sender)
        {
            if (this.DraggedFrame == sender)
            {
                if (this.mouseMoveTimer.IsRunning)
                {
                    //swap frames if swiped
                    int oldIndex = this.frames.IndexOf(sender);
                    float distance = sender.PreciseCenterX - this.dragStart;
                    if (distance is > 30 and < 140)
                    {
                        if (oldIndex + 1 < this.frames.Count)
                        {
                            UIPinnedObjectiveFrame temp = this.frames[oldIndex + 1];
                            this.frames[oldIndex + 1] = sender;
                            this.frames[oldIndex] = temp;
                        }
                    }
                    else if (distance is < -30 and > -140)
                    {
                        if (oldIndex - 1 >= 0)
                        {
                            UIPinnedObjectiveFrame temp = this.frames[oldIndex - 1];
                            this.frames[oldIndex - 1] = sender;
                            this.frames[oldIndex] = temp;
                        }
                        
                    }
                }
                this.DraggedFrame = null;
                this.SaveChanges();
            }
        }

        public void ContinueDrag(UIPinnedObjectiveFrame sender, Point cursor, Time time)
        {
            const float DragLerpSpeed = 25f;
            sender.PreciseCenterX = (int)MathHelper.Lerp(sender.PreciseCenterX, 
                cursor.X + this.dragOffset - (sender.Width / 2), (float)(DragLerpSpeed * time.Delta));

            if (sender.X != (int)sender.PreciseCenterX)
            {
                sender.MoveTo(new Point((int)sender.PreciseCenterX, this.Top));
                this.mouseMoveTimer.Reset();
                this.SortFrames();
            }
        }

        public override void DrawRecursive(Canvas canvas)
        {
            base.DrawRecursive(canvas);

            if (this.IsCollapsed || this.pinButton.IsCollapsed)
                this.status?.DrawRecursive(canvas);
            else if (!this.IsCollapsed)
                this.pinButton?.DrawRecursive(canvas);

            this.DraggedFrame?.DrawRecursive(canvas);
        }

        private void SortFrames()
        {
            for (int i = 0; i < this.frames.Count - 1; i++)
            {
                int smallestVal = i;
                for (int j = i + 1; j < this.frames.Count; j++)
                {
                    if (this.frames[j].Center.X < this.frames[smallestVal].Center.X)
                        smallestVal = j;
                }
                //swap frames
                (this.frames[i], this.frames[smallestVal]) = (this.frames[smallestVal], this.frames[i]);
            }

            if (this.HorizontalAlign == HorizontalAlign.Right)
                this.frames.Reverse();
        }

        public void PositionStatusLabel(Time time)
        {
            if (Config.Overlay.ShowPickups)
            {       
                this.statusOffset = MathHelper.Lerp(this.statusOffset, this.PinnedEndOffset, (float)(this.LerpSpeed * time.Delta));
            }
            else
            {
                this.statusOffset = this.HorizontalAlign is HorizontalAlign.Left
                    ? MathHelper.Lerp(this.statusOffset, 20, (float)(this.LerpSpeed * time.Delta))
                    : MathHelper.Lerp(this.statusOffset, this.Width - 20, (float)(this.LerpSpeed * time.Delta));
            }

            if (this.status.X != (int)this.statusOffset)
            {
                if (this.HorizontalAlign == HorizontalAlign.Left)
                    this.status.ResizeRecursive(new Rectangle((int)this.statusOffset, this.Top, 180, 72));
                else
                    this.status.ResizeRecursive(new Rectangle((int)this.statusOffset - 180, this.Top, 180, 72));
            }
            //this.status.SetTextColor(this.);
        }

        public void SaveChanges()
        {
            if (Config.Overlay.PinnedObjectiveList.Value.TrySetCurrentList(this.frames)
                && Config.Overlay.TrySave())
            {
                (this.Root() as UIOverlayScreen)?.PinnedObjectivesSaved();
            }
        }
    }
}
