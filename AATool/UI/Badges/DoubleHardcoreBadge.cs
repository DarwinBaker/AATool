using AATool.Graphics;
using AATool.UI.Controls;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class DoubleHardcoreBadge : HundredHardcoreBadge
    {
        private const string Background = "badge_100hc";
        private const string HalfHeartText = "badge_hhhaa_text";
        private const string HundredHardcore = "badge_100hc_text";

        private static readonly SequenceTimer SwapTimer = new (5, 1, 0.75, 5, 1, 0.75);
        private static readonly bool ShowBoth = true;

        private static long LastFrameUpdated = -1;
        private static bool VariantsInitialized = false;

        public DoubleHardcoreBadge() : base (100)
        {
            if (!VariantsInitialized)
            {
                if (ShowBoth)
                    SwapTimer.Skip(3);
                VariantsInitialized = true;
            }
        }

        protected override void UpdateThis(Time time)
        {
            base.UpdateThis(time);
            if (ShowBoth && LastFrameUpdated != time.TotalFrames)
            {
                SwapTimer.Update(time);
                if (SwapTimer.IsExpired)
                    SwapTimer.Continue();
                LastFrameUpdated = time.TotalFrames;
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            this.DrawPopup(canvas);
            float ratio = (float)SwapTimer.Ratio;
            canvas.Draw(Background, this.Inner, Color.White, Layer.Fore);
            switch (SwapTimer.Index)
            {
                case 0:
                    canvas.Draw(HalfHeartText, this.Inner, Color.White, Layer.Fore);
                    break;
                case 1:
                    canvas.Draw(HalfHeartText, this.Inner, Color.White * ratio, Layer.Fore);
                    break;
                case 2:
                    this.Description.SetText("Complete 100 deathless\nHC no-reset Any% in a row");
                    canvas.Draw(HundredHardcore, this.Inner, ColorHelper.Fade(Color.White, 1 - ratio), Layer.Fore);
                    break;
                case 3:
                    canvas.Draw(HundredHardcore, this.Inner, Color.White, Layer.Fore);
                    break;
                case 4:
                    canvas.Draw(HundredHardcore, this.Inner, Color.White * ratio, Layer.Fore);
                    break;
                case 5:
                    this.Description.SetText("Completed AA on Hardcore\nwith half a heart (twice)");
                    canvas.Draw(HalfHeartText, this.Inner, ColorHelper.Fade(Color.White, 1 - ratio), Layer.Fore);
                    break;
            }
        }
    }
}
