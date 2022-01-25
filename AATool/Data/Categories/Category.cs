using System;
using System.Collections.Generic;
using System.Linq;
using AATool.Configuration;

namespace AATool.Data.Categories
{
    public abstract class Category : ICategory
    {
        public string Name                    { get; protected set; }
        public string Acronym                 { get; protected set; }
        public string Action                  { get; protected set; }
        public string Objective               { get; protected set; }
        public string CurrentVersion          { get; protected set; }

        public string LayoutName => this.Name.ToLower().Replace(" ", "_");
        public string LatestSupportedVersion => this.GetSupportedVersions().First();

        public Category()
        {
            this.CurrentVersion = this.LatestSupportedVersion;
        }

        public bool IsComplete() => this.GetCompletedCount() >= this.GetTargetCount();
        public virtual string GetCompletionMessage() => $"All {this.GetTargetCount()} {this.Objective} {this.Action}!";

        public abstract IEnumerable<string> GetSupportedVersions();
        public abstract int GetTargetCount();
        public abstract int GetCompletedCount();

        public bool TrySetVersion(string version)
        {
            if (this.GetSupportedVersions().Contains(version))
            {
                this.CurrentVersion = version;
                Config.Tracking.GameVersion.Set(this.CurrentVersion);
                return true;
            }
            return false;
        }

        public int GetCompletionPercent()
        {
            int target = this.GetTargetCount();
            if (target < 1)
                return 0;

            int clamped = Math.Min(this.GetCompletedCount(), target);
            double normalized = (double)clamped / this.GetTargetCount();
            return (int)(normalized * 100);
        }   
    }
}
