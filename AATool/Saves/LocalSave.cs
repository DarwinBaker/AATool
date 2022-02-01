using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace AATool.Saves
{
    public class LocalSave
    {
        public AdvancementsFolder Advancements { get; private set; }
        public AchievementsFolder Achievements { get; private set; }
        public StatisticsFolder Statistics { get; private set; }
        public DirectoryInfo CurrentFolder { get; private set; }
        public SaveState CurrentState { get; private set; }
        public bool ProgressChanged { get; private set; }
        public bool WorldChanged  { get; private set; }
        public bool Invalidated   { get; private set; }

        public string Name => this.CurrentFolder?.Name;
        public bool IsEmpty => this.CurrentFolder is null;

        public LocalSave()
        {
            this.Advancements = new AdvancementsFolder();
            this.Achievements = new AchievementsFolder();
            this.Statistics   = new StatisticsFolder();
            this.WorldChanged = true;
            this.ProgressChanged = true;
        }

        public HashSet<Uuid> GetAllUuids()
        {
            JsonFolder folder = Tracker.Category is AllAchievements
                ? this.Achievements
                : this.Advancements;
            return new HashSet<Uuid>(folder.Files.Keys);
        }

        public WorldState GetState()
        {
            var state = new WorldState();
            state.Sync(this);
            return state;
        }

        public void Invalidate() =>  this.Invalidated = true;

        public void ClearFlags()
        {
            this.WorldChanged = false;
            this.ProgressChanged = false;
        }

        public bool TryReadFiles()
        {
            //attempt to get most recently accessed world folder
            SaveState newState = this.TryGetLatestWorld(out DirectoryInfo latest);
            this.WorldChanged |= newState != this.CurrentState;

            if (newState is SaveState.Valid)
            {
                bool ready = !Tracker.WorldLocked;
                ready |= Config.Tracking.CustomSavePath.Changed && !Config.Tracking.UseDefaultPath;
                ready |= Config.Tracking.UseDefaultPath.Changed;
                ready |= Config.Tracking.UseSftp.Changed;
                ready &= latest.FullName != this.CurrentFolder?.FullName;
                if (this.CurrentFolder is null || ready)
                {
                    //world changed
                    this.CurrentFolder = latest;
                    this.Advancements.SetFolder(new DirectoryInfo(Path.Combine(this.CurrentFolder.FullName, "advancements")));
                    this.Achievements.SetFolder(new DirectoryInfo(Path.Combine(this.CurrentFolder.FullName, "stats")));
                    this.Statistics.SetFolder(new DirectoryInfo(Path.Combine(this.CurrentFolder.FullName, "stats")));
                    this.WorldChanged = true;

                    //unlock world
                    if (Tracker.WorldLocked)
                        Tracker.ToggleWorldLock();
                }

                //update progress if different
                this.ProgressChanged = this.Statistics.TryUpdate();
                this.ProgressChanged |= Tracker.Category is AllAchievements
                    ? this.Achievements.TryUpdate()
                    : this.Advancements.TryUpdate();
            }
            else
            {
                //world not set
                this.CurrentFolder = null;
            }

            //handle states
            this.CurrentState = newState;
            this.ProgressChanged |= this.Invalidated;
            this.Invalidated = false;

            return this.ProgressChanged || this.WorldChanged;
        }

        private static bool MightBeWorldFolder(DirectoryInfo folder)
        {
            return File.Exists(Path.Combine(folder.FullName, "level.dat"))
                || Directory.Exists(Path.Combine(folder.FullName, "advancements"))
                || Directory.Exists(Path.Combine(folder.FullName, "stats"));
        }

        private static DirectoryInfo MostRecentlyWritten(DirectoryInfo a, DirectoryInfo b)
        {
            if (a is null) return b;
            if (b is null) return a;
            return a.LastWriteTimeUtc > b.LastWriteTimeUtc ? a : b;
        }

        private SaveState TryGetLatestWorld(out DirectoryInfo directory)
        {
            directory = null;
            bool refresh = this.CurrentState 
                is SaveState.Valid 
                or SaveState.NoWorlds 
                or SaveState.PathNonExistent;

            refresh |= Config.Tracking.CustomSavePath.Changed;
            refresh |= Config.Tracking.UseDefaultPath.Changed;
            refresh |= Config.Tracking.UseSftp.Changed;
            if (!refresh) 
                return this.CurrentState;

            try
            {
                string currentSavesFolder = Paths.Saves.CurrentFolder();

                //return if path empty
                if (string.IsNullOrEmpty(currentSavesFolder))
                    return SaveState.PathEmpty;

                //return if folder doesn't exist
                var savesFolder = new DirectoryInfo(currentSavesFolder);
                if (!savesFolder.Exists)
                    return SaveState.PathNonExistent;

                DirectoryInfo[] subFolders = savesFolder.GetDirectories();
                DirectoryInfo latest = null;
                foreach (DirectoryInfo subFolder in subFolders)
                {
                    //sort by access time, skipping folders that aren't worlds
                    if (MightBeWorldFolder(subFolder) && subFolder == MostRecentlyWritten(subFolder, latest))
                        latest = subFolder;
                }

                //determine final state
                directory = latest;
                return latest is not null
                    ? SaveState.Valid
                    : SaveState.NoWorlds;
            }
            catch (Exception e) 
            {
                if (e is ArgumentException or NotSupportedException)
                    return SaveState.PathInvalid;
                if (e is PathTooLongException)
                    return SaveState.PathTooLong;
                return SaveState.PermissionError;
            }
        }
    }
}