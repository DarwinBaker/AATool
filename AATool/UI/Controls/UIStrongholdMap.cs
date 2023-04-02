
using System;
using AATool.Data.Categories;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using FontStashSharp;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    internal class UIStrongholdMap : UICoordinateGrid
    {
        public override void DrawThis(Canvas canvas)
        {
            base.DrawThis(canvas);
            if (Tracker.Category is not AllPortals category)
                return;

            foreach (StrongholdRing ring in AllPortals.Rings)
                this.DrawRing(canvas, ring);
        }

        private void DrawRing(Canvas canvas, StrongholdRing ring)
        {
            float cellSize = Math.Abs(this.BlocksPerSquare / this.ViewScale / this.CoordScale);
            int wrappedX = (int)Math.Round(this.Offset.X % cellSize);
            int wrappedY = (int)Math.Round(this.Offset.Y % cellSize);
            var wrappedOffset = new Point(wrappedX, wrappedY);

            int size = ring.Diameter;
            var bounds = new Rectangle(-ring.EndDistance, -ring.EndDistance, size, size);
            canvas.Draw("stronghold_ring", bounds, Color.White * 0.25f, Layer.Fore);
        }
    }
}
