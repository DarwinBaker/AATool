using System;
using System.Collections.Generic;
using AATool.Data.Objectives;
using AATool.Data.Speedrunning;
using Microsoft.Xna.Framework;

namespace AATool.Data.Categories
{
    internal class AllPortals : Category
    {
        const int TotalStrongholds = 128;

        public static readonly List<StrongholdRing> Rings = new()
        {
            new StrongholdRing(3,  1280),
            new StrongholdRing(6,  4352),
            new StrongholdRing(10, 7424),
            new StrongholdRing(15, 10496),
            new StrongholdRing(21, 13568),
            new StrongholdRing(28, 16640),
            new StrongholdRing(36, 19712),
            new StrongholdRing(9,  22783),
        };

        static readonly List<Point> TestValues = new()
        {
            new Point(1700, 1200),
            new Point(11770, 6700),
        };

        public static readonly List<string> SupportedVersions = new () {
            "1.16",
        };

        private int filledPortals = 0;

        public override int GetTargetCount() => TotalStrongholds;
        public override int GetCompletedCount() => this.filledPortals;
        public override IEnumerable<Objective> GetOverlayObjectives() => null;
        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;

        public AllPortals()
        {
            this.Name = "All Portals";
            this.Objective = "End Portals";
            this.Action = "Filled";
            this.Acronym = "AP";
        }

        public override void LoadObjectives()
        {
            foreach (Point testValue in TestValues)
                this.FillPortal(testValue);
        }

        public void FillPortal(Point coordinates)
        {
            StrongholdRing ring = this.ClosestRing(coordinates);
            ring.FillPortal(coordinates);
            this.UpdateTotals();
        }

        public void ClearProgress()
        {
            this.filledPortals = 0;
            foreach (StrongholdRing ring in Rings)
                ring.ClearProgress();
        }

        private void UpdateTotals()
        {
            this.filledPortals = 0;
            foreach (StrongholdRing ring in Rings)
                this.filledPortals += ring.FilledPortalCount;
        }

        private StrongholdRing ClosestRing(Point coordinates)
        {
            double distanceFromOrigin = DistanceBetween(Point.Zero, coordinates);
            double smallestDifference = double.MaxValue;
            StrongholdRing closest = Rings[0];
            for (int i = 0; i < Rings.Count; i++)
            {
                double ringDifference = Math.Abs(distanceFromOrigin - Rings[i].OptimalBlindDistance);
                if (ringDifference < smallestDifference)
                {
                    smallestDifference = ringDifference;
                    closest = Rings[i];
                }
            }
            return closest;
        }

        private static double DistanceBetween(Point start, Point end)
        {
            double deltaX = Math.Pow(end.X - start.X, 2);
            double deltaY = Math.Pow(end.Y - start.Y, 2);
            return (float)Math.Sqrt(deltaY + deltaX);
        }
    }
}
