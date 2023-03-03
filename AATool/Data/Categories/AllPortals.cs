using System;
using System.Collections.Generic;
using AATool.Data.Objectives;
using Microsoft.Xna.Framework;

namespace AATool.Data.Categories
{
    class StrongholdRing
    {
        const int Thickness = 1536;

        public int StrongholdCount { get; }
        public int FilledPortals { get; }
        public int StartDistance { get; }
        public int EndDistance { get; }

        public int OptimalBlindDistance => this.StartDistance + (Thickness / 2);

        public Point ReferenceStronghold { get; private set; }
        public Point[] BlindEstimates { get; private set; }
        public float AngleBetween { get; private set; }
        public float AngleOffset { get; private set; }

        public StrongholdRing(int strongholdCount, int startDistance)
        {
            this.BlindEstimates = new Point[strongholdCount - 1];
            this.StrongholdCount = strongholdCount;
            this.StartDistance = startDistance;
            this.EndDistance = startDistance + Thickness;
            this.AngleBetween = MathHelper.TwoPi / this.StrongholdCount;
        }

        public void SetReferenceStronghold(Point coords)
        {
            this.AngleOffset = Angle(Point.Zero, coords);
            this.CalculateOptimalBlindCoordinates();
        }

        private void CalculateOptimalBlindCoordinates()
        {
            double nextAngle = this.AngleOffset + this.AngleBetween;
            for (int i = 0; i < this.BlindEstimates.Length; i++)
            {
                nextAngle += this.AngleBetween;
                this.BlindEstimates[i] = this.EstimatedBlindCoordinates(nextAngle);
            }
        }

        public void ClearProgress()
        { 
            
        }

        private static float Angle(Point start, Point end)
        {
            return (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
        }

        private static double Distance(Point start, Point end)
        {
            double deltaX = Math.Pow(end.X - start.X, 2);
            double deltaY = Math.Pow(end.Y - start.Y, 2);
            return (float)Math.Sqrt(deltaY + deltaX);
        }

        private Point EstimatedBlindCoordinates(double angle)
        {
            int x = (int)Math.Round(this.OptimalBlindDistance * Math.Cos(angle));
            int y = (int)Math.Round(this.OptimalBlindDistance * Math.Sin(angle));
            return new Point(x, y);
        }
    }

    internal class AllPortals : Category
    {
        const int TotalStrongholds = 128;

        static readonly List<StrongholdRing> Rings = new()
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
                this.filledPortals += ring.FilledPortals;
        }
    }
}
