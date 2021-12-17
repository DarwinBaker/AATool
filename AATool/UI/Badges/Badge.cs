using AATool.Net;
using AATool.UI.Controls;

namespace AATool.UI.Badges
{
    public static class Badge
    {
        private static readonly Uuid Elysaku  = new ("b2fcb273-9886-4a9b-bd7f-e005816fb7b7");
        private static readonly Uuid Couriway = new ("994f9376-3f80-48bc-9e72-ee92f861911d");

        public static bool TryGet(Uuid player, int scale, out UIControl badge)
        {
            badge = null;
            if (player.String == Elysaku.String)
                badge = new ImmortalBadge(scale);
            else if (player.String == Couriway.String)
                badge = new UnstoppableBadge(scale);

            return badge is not null;
        }
    }
}
