using System;
using System.Collections.Generic;
using System.IO;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Utilities;

namespace AATool.Saves
{
    public class WorldFolder
    {
        public AdvancementsFolder Advancements { get; private set; }
        public AchievementsFolder Achievements { get; private set; }
        public StatisticsFolder Statistics { get; private set; }
        public DirectoryInfo CurrentFolder { get; private set; }
        public bool ProgressChanged { get; private set; }
        public bool Invalidated { get; private set; }
        public bool PathChanged { get; private set; }

        public bool IsEmpty => this.CurrentFolder is null;

        public string FullName => this.CurrentFolder?.FullName ?? string.Empty;
        public string Name => this.CurrentFolder?.Name ?? string.Empty;

        public WorldFolder()
        {
            this.Advancements = new AdvancementsFolder();
            this.Achievements = new AchievementsFolder();
            this.Statistics   = new StatisticsFolder();
            this.PathChanged = true;
            this.ProgressChanged = true;
        }

        public HashSet<Uuid> GetAllUuids()
        {
            JsonFolder folder = Tracker.Category is AllAchievements
                ? this.Achievements
                : this.Advancements;
            return new HashSet<Uuid>(folder.Players.Keys);
        }

        public void Invalidate() =>  this.Invalidated = true;

        public void ClearFlags()
        {
            this.PathChanged = false;
            this.ProgressChanged = false;
        }

        public void Unset() => this.SetPath(null);

        public void SetPath(DirectoryInfo worldFolder)
        {
            if (worldFolder is null)
            {
                if (this.CurrentFolder is not null)
                {
                    //no world folder to track
                    this.CurrentFolder = null;
                    this.Advancements.SetPath(null);
                    this.Achievements.SetPath(null);
                    this.Statistics.SetPath(null);
                }
                return;
            }

            //make sure folder actually changed
            if (worldFolder.FullName != this.CurrentFolder?.FullName)
            {
                ActiveInstance.SetLogStart();
                foreach (Death death in Tracker.Deaths.All.Values)
                    death.Clear();

                this.CurrentFolder = worldFolder;
                this.PathChanged = true;
                string advancementsFolder = Path.Combine(this.CurrentFolder.FullName, "advancements");
                string statisticsFolder = Path.Combine(this.CurrentFolder.FullName, "stats");

                //world changed
                this.Advancements.SetPath(advancementsFolder);
                this.Achievements.SetPath(statisticsFolder);
                this.Statistics.SetPath(statisticsFolder);

                //unlock world
                if (Tracker.WorldLocked)
                    Tracker.ToggleWorldLock();
            }
        }

        public bool TryRefresh()
        {
            //update progress
            this.ProgressChanged = this.Statistics.TryRefresh();
            this.ProgressChanged |= Tracker.Category is AllAchievements
                ? this.Achievements.TryRefresh()
                : this.Advancements.TryRefresh();

            this.ProgressChanged |= this.Invalidated;
            this.Invalidated = false;

            return this.ProgressChanged || this.PathChanged;
        }

        public WorldState GetState()
        {
            var state = new WorldState();

            //sync progress from local world
            if (Tracker.Category is AllAchievements)
                this.Achievements.Update(state);
            else
                this.Advancements.Update(state);

            //sync statistics from local world
            this.Statistics.Update(state);
            return state;
        }
    }
}