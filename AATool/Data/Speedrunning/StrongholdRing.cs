using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace AATool.Data.Speedrunning
{
    class StrongholdRing
    {
        const int Thickness = 1536;

        public int TotalStrongholdCount { get; }
        public int FilledPortalCount { get; }
        public int StartDistance { get; }
        public int EndDistance { get; }

        public int OptimalBlindDistance => this.StartDistance + (Thickness / 2);

        public Point ReferenceStronghold { get; private set; }
        public Point[] BlindEstimates { get; private set; }
        public List<Point> FilledPortals { get; private set; }
        public float AngleBetweenStrongholds { get; private set; }
        public float AngleOffset { get; private set; }
        public int Diameter => this.EndDistance * 2;

        public StrongholdRing(int strongholdCount, int startDistance)
        {
            this.FilledPortals = new List<Point>();
            this.BlindEstimates = new Point[strongholdCount - 1];
            this.TotalStrongholdCount = strongholdCount;
            this.StartDistance = startDistance;
            this.EndDistance = startDistance + Thickness;
            this.AngleBetweenStrongholds = MathHelper.TwoPi / this.TotalStrongholdCount;
        }

        public void FillPortal(Point coords)
        {
            if (this.FilledPortals.Count >= this.TotalStrongholdCount)
                return;

            if (!this.FilledPortals.Any())
                this.SetReferenceStronghold(coords);

            this.FilledPortals.Add(coords);
        }

        public void SetReferenceStronghold(Point coords)
        {
            this.AngleOffset = Angle(Point.Zero, coords);
            this.CalculateOptimalBlindCoordinates();
        }

        private void CalculateOptimalBlindCoordinates()
        {
            double nextAngle = this.AngleOffset + this.AngleBetweenStrongholds;
            for (int i = 0; i < this.BlindEstimates.Length; i++)
            {
                nextAngle += this.AngleBetweenStrongholds;
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

        private Point EstimatedBlindCoordinates(double angle)
        {
            int x = (int)Math.Round(this.OptimalBlindDistance * Math.Cos(angle));
            int y = (int)Math.Round(this.OptimalBlindDistance * Math.Sin(angle));
            return new Point(x, y);
        }
    }
}
