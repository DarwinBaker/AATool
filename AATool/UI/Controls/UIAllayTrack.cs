using System.Collections.Generic;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.Utilities.Easings;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIAllayTrack : UIPanel
    {
        private class Allay
        {
            public const int HorizontalOffset = 75;
            public const int AllaySize = 48;
            public const int BlockSize = 32;
            public const int VerticalFlyRange = 16;
            public const float GlowRotationSpeed = 0.25f;

            public static float GlowRotation { get; set; } = 0f;

            private static int StartingIndex;

            private Easing easing = new Easing(Ease.Sinusoidal, 1.25, true, false);
            private UIAllayTrack track;
            private Block block;
            private float xOffset;
            private Rectangle bounds;
            private Rectangle plateBounds;
            private Rectangle blockBounds;
            private bool movingUp = true;

            public static void UpdateGlowRotation(Time time) => 
                GlowRotation += (float)(GlowRotationSpeed * time.Delta);

            public Allay(UIAllayTrack track, int offset)
            {
                this.PickBlock();
                this.track = track;
                this.xOffset = track.Inner.Left + offset - HorizontalOffset;
                this.bounds = new Rectangle(
                    (int)this.xOffset,
                    this.track.Inner.Center.Y,
                    AllaySize, AllaySize);

                double timingOffset = this.easing.Duration / (StartingIndex % 2 * 4);
                this.easing.TimeLeft -= timingOffset;
                this.easing.TimeElapsed += timingOffset;
                StartingIndex++;
            }

            private void PickBlock()
            {
                do
                {
                    int randomBlockIndex = Main.RNG.Next(0, Tracker.Blocks.AllBlocksList.Count);
                    this.block = Tracker.Blocks.AllBlocksList[randomBlockIndex];
                }
                while (this.block.DoubleHeight);
            }

            public void Update(Time time)
            {
                this.easing.Update(time);

                int offsetY = this.movingUp 
                    ? (int)(this.easing.InOut() * VerticalFlyRange) 
                    : VerticalFlyRange - (int)(this.easing.InOut() * VerticalFlyRange);

                if (this.easing.IsExpired)
                {
                    this.movingUp ^= true;
                    this.easing.Reset();
                }

                this.xOffset += 1f;
                this.bounds = new Rectangle(
                    (int)this.xOffset,
                    this.track.Top + offsetY, //* direction,
                    AllaySize, AllaySize);

                this.plateBounds = new Rectangle(
                    this.bounds.Center.X - 4,
                    this.bounds.Center.Y + 4,
                    48, 24);

                if (this.block.DoubleHeight)
                {
                    this.blockBounds = new Rectangle(
                    this.plateBounds.Center.X - 16,
                    this.plateBounds.Top - 46,
                        32,
                        64);
                }
                else
                {
                    this.blockBounds = new Rectangle(
                    this.plateBounds.Center.X - 16,
                    this.plateBounds.Top - 14,
                    BlockSize,
                    BlockSize);
                }
                

                if (this.bounds.Left > this.track.Right)
                    this.Reset();
            }

            private void Reset()
            {
                this.PickBlock();
                this.xOffset = this.track.Inner.Left- HorizontalOffset;
            }

            public void Draw(Canvas canvas)
            {
                canvas.Draw(
                    "player_frame_diamond_glow",
                    this.bounds.Center.ToVector2() + new Vector2(6, 0),
                    GlowRotation,
                    new Vector2(0.65f),
                    Color.White * 0.75f,
                    Layer.Fore);

                canvas.Draw("allay_fly", this.bounds, Color.White, Layer.Fore);
                canvas.Draw("allay_plate", this.plateBounds, Color.White, Layer.Fore);
                canvas.Draw(this.block.Icon, this.blockBounds, Color.White, Layer.Fore);
            }
        }

        private List<Allay> allays = new List<Allay>();
        private int allayCount = 6;

        private void Populate()
        {
            this.allays.Clear();
            int spacing = (this.Inner.Width + Allay.HorizontalOffset) / this.allayCount;
            for (int i = 0; i < this.allayCount; i++)
            {
                this.allays.Add(new Allay(this, i * spacing));
            }
        }

        public override void ResizeThis(Rectangle parent)
        {
            base.ResizeThis(parent);
            this.Populate();
        }

        protected override void UpdateThis(Time time)
        {
            Allay.UpdateGlowRotation(time);
            foreach (Allay allay in this.allays)
                allay.Update(time);
        }

        public override void DrawThis(Canvas canvas)
        {
            foreach (Allay allay in this.allays)
                allay.Draw(canvas);
        }
    }
}
