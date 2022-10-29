using System;
using System.Collections.Generic;
using System.Linq;
using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.Data.Progress;

namespace AATool.Data.Categories
{
    public abstract class Category : ICategory
    {
        public string Name      { get; protected set; }
        public string Acronym   { get; protected set; }
        public string Action    { get; protected set; }
        public string Objective { get; protected set; }

        public string CurrentVersion { get; private set; }
        public string CurrentMajorVersion { get; private set; }

        public virtual string ViewName => this.Name.ToLower().Replace(" ", "_");
        public string LatestSupportedVersion => this.GetSupportedVersions().First();

        public virtual string GetDefaultVersion() => this.LatestSupportedVersion;

        public Category()
        {
            this.CurrentVersion = this.GetDefaultVersion();
            if (Version.TryParse(this.CurrentVersion, out Version current))
                this.CurrentMajorVersion = $"{current.Major}.{current.Minor}";
        }

        public virtual bool IsComplete() => 
            this.GetCompletedCount() >= this.GetTargetCount();

        public virtual string GetCompletionMessage() => 
            $"All {this.GetTargetCount()} {this.Objective} {this.Action}!";

        public virtual string GetStatus() => 
            $"{this.GetCompletedCount()} / {this.GetTargetCount()} {this.Objective} {this.Action}";

        public abstract IEnumerable<Objective> GetOverlayObjectives();
        public abstract IEnumerable<string> GetSupportedVersions();
        public abstract int GetTargetCount();
        public abstract int GetCompletedCount();

        public bool TrySetVersion(string version)
        {
            if (Version.TryParse(version, out Version number))
            {
                //handle sub-versioning of 1.16 due to piglin brutes
                version = number > Version.Parse("1.16.1") && number < Version.Parse("1.17")
                    ? "1.16.5"
                    : $"{number.Major}.{number.Minor}";
            }

            if (this.GetSupportedVersions().Contains(version))
            {
                this.CurrentVersion = version;
                this.CurrentMajorVersion = number is not null 
                    ? $"{number.Major}.{number.Minor}"
                    : this.CurrentVersion;
                Config.Tracking.GameVersion.Set(this.CurrentVersion);
                Config.Tracking.TrySave();
                return true;
            }
            return false;
        }

        public abstract void LoadObjectives();
        public virtual void Update() { }
        public int GetCompletionPercent() => (int)(this.GetCompletionRatio() * 100);

        public float GetCompletionRatio()
        {
            int target = this.GetTargetCount();
            if (target < 1)
                return 0;
            int clamped = Math.Min(this.GetCompletedCount(), target);
            return (float)clamped / this.GetTargetCount();
        }
    }
}
