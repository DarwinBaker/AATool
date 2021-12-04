using AATool.Graphics;
using AATool.Net;
using AATool.UI.Controls;

namespace AATool.UI.Badges
{
    public static class Badge
    {
        private static readonly Uuid Elysaku = new ("b2fcb273-9886-4a9b-bd7f-e005816fb7b7");

        public static bool TryGet(Uuid player, int scale, out UIControl badge)
        {
            badge = null;
            if (player.String == Elysaku.String)
                badge = new HalfHeartHardcoreBadge(scale);

            return badge is not null;
        }
    }
}
