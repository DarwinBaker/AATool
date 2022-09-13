using AATool.Configuration;
using AATool.Graphics;
using AATool.UI.Controls;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Badges
{
    class HundredThousandBadge : ThousandSeedsBadge
    {
        private const string ThousandSeedsBack = "badge_1k_couriway";
        private const string ThousandSeedsText = "badge_1k_text";
        private const string HundredHardcoreBack = "badge_100hc";
        private const string HundredHardcoreText = "badge_100hc_text";

        private static readonly SequenceTimer SwapTimer = new (10, 1, 0.75, 10, 1, 0.75);
        private static readonly bool ShowBoth = true;

        private static long LastFrameUpdated = -1;
        private static bool VariantsInitialized = false;

        public HundredThousandBadge(string player) : base (player)
        {
            if (!VariantsInitialized)
            {
                if (ShowBoth)
                    SwapTimer.Skip(2);
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
            switch (SwapTimer.Index)
            {
                case 0:
                    canvas.Draw(ThousandSeedsBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(ThousandSeedsText, this.Inner, Color.White, Layer.Fore);
                    break;
                case 1:
                    canvas.Draw(HundredHardcoreBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(ThousandSeedsBack, this.Inner, ColorHelper.Fade(Color.White, (ratio / 2) + 0.5f), Layer.Fore);
                    canvas.Draw(ThousandSeedsText, this.Inner, Color.White * ratio, Layer.Fore);
                    break;
                case 2:
                    this.Description.SetText("Complete 100 deathless\nHC no-reset Any% in a row");
                    canvas.Draw(HundredHardcoreBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(ThousandSeedsBack, this.Inner, ColorHelper.Fade(Color.White, ratio / 2), Layer.Fore);
                    canvas.Draw(HundredHardcoreText, this.Inner, ColorHelper.Fade(Color.White, 1 - ratio), Layer.Fore);
                    break;
                case 3:
                    canvas.Draw(HundredHardcoreBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(HundredHardcoreText, this.Inner, Color.White, Layer.Fore);
                    break;
                case 4:
                    canvas.Draw(ThousandSeedsBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(HundredHardcoreBack, this.Inner, ColorHelper.Fade(Color.White, (ratio / 2) + 0.5f), Layer.Fore);
                    canvas.Draw(HundredHardcoreText, this.Inner, Color.White * ratio, Layer.Fore);
                    break;
                case 5:
                    this.Description.SetText("Completed Any% on 1,000\nseeds without resetting");
                    canvas.Draw(ThousandSeedsBack, this.Inner, Color.White, Layer.Fore);
                    canvas.Draw(HundredHardcoreBack, this.Inner, ColorHelper.Fade(Color.White, ratio / 2), Layer.Fore);
                    canvas.Draw(ThousandSeedsText, this.Inner, ColorHelper.Fade(Color.White, 1 - ratio), Layer.Fore);
                    break;
            }
        }
    }
}
