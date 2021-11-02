using AATool.Net;
using AATool.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace AATool.Saves
{
    public class World
    {
        public AdvancementsFolder Advancements { get; private set; }
        public AchievementsFolder Achievements { get; private set; }
        public StatisticsFolder Statistics     { get; private set; }
        public DirectoryInfo CurrentFolder     { get; private set; }
        public SaveFolderState CurrentState    { get; private set; }
        public string CurrentPath              { get; private set; }
        public bool FolderChangedFlag          { get; private set; }
        public bool FilesChangedFlag           { get; private set; }

        public string Name => this.CurrentFolder?.Name;
        public bool IsEmpty => this.CurrentFolder is null;

        private bool externallyInvalidated;

        public World()
        {
            this.Advancements = new();
            this.Achievements = new();
            this.Statistics   = new();
            this.FolderChangedFlag = true;
            this.FilesChangedFlag  = true;
        }

        public HashSet<Uuid> GetAllGuids()
        {
            var ids = new HashSet<Uuid>();
            if (Config.PostExplorationUpdate)
            {
                foreach (Uuid id in this.Advancements.Files.Keys)
                    ids.Add(id);
            }
            else
            {
                foreach (Uuid id in this.Achievements.Files.Keys)
                    ids.Add(id);
            }
            return ids;
        }

        public void Invalidate() => this.externallyInvalidated = true;

        public void ClearFlags()
        {
            this.FolderChangedFlag = false;
            this.FilesChangedFlag = false;
        }

        public bool TryUpdate()
        {
            //attempt to get most recently accessed world folder
            SaveFolderState newState = this.TryGetLatestWorld(out DirectoryInfo latest);
            this.FolderChangedFlag |= newState != this.CurrentState;

            if (newState is SaveFolderState.Valid)
            {
                if (this.CurrentFolder is null || latest.FullName != this.CurrentFolder.FullName)
                {
                    //world changed
                    this.CurrentFolder = latest;
                    this.Advancements.SetFolder(new DirectoryInfo(Path.Combine(this.CurrentFolder.FullName, "advancements")));
                    this.Achievements.SetFolder(new DirectoryInfo(Path.Combine(this.CurrentFolder.FullName, "stats")));
                    this.Statistics.SetFolder(new DirectoryInfo(Path.Combine(this.CurrentFolder.FullName, "stats")));

                    if (this.CurrentPath != this.CurrentFolder.FullName)
                    {
                        this.CurrentPath = this.CurrentFolder.FullName;
                        this.FolderChangedFlag = true;
                    }
                }

                //update progress if different
                this.FilesChangedFlag = this.Statistics.TryUpdate();
                this.FilesChangedFlag |= Config.PostExplorationUpdate
                    ? this.Advancements.TryUpdate()
                    : this.Achievements.TryUpdate();
            }
            else
            {
                //world not set
                this.CurrentFolder = null;
            }

            //handle states
            this.CurrentState = newState;
            this.FilesChangedFlag |= this.externallyInvalidated;
            this.externallyInvalidated = false;

            return this.FilesChangedFlag || this.FolderChangedFlag;
        }

        private static bool MightBeWorldFolder(DirectoryInfo folder)
        {
            return Directory.Exists(Path.Combine(folder.FullName, "stats"))
                || Directory.Exists(Path.Combine(folder.FullName, "advancements"));
        }

        private static DirectoryInfo MostRecentlyAccessed(DirectoryInfo a, DirectoryInfo b)
        {
            if (a is null)
                return b;
            if (b is null)
                return a;
            return a.LastAccessTime > b.LastAccessTime 
                ? a 
                : b;
        }

        private SaveFolderState TryGetLatestWorld(out DirectoryInfo directory)
        {
            directory = null;
            bool needsRefresh = this.CurrentState 
                is SaveFolderState.Valid 
                or SaveFolderState.NoWorlds 
                or SaveFolderState.NonExistentPath;

            needsRefresh |= Config.Tracker.CustomPathChanged();
            needsRefresh |= Config.Tracker.UseDefaultPathChanged();
            needsRefresh |= Config.Tracker.UseRemoteWorldChanged();
            if (!needsRefresh)
                return this.CurrentState;

            try
            {
                if (string.IsNullOrEmpty(Config.Tracker.SavesFolder))
                    return SaveFolderState.EmptyPath;

                var savesFolder = new DirectoryInfo(Config.Tracker.SavesFolder);
                if (!savesFolder.Exists)
                    return SaveFolderState.NonExistentPath;

                DirectoryInfo[] subFolders = savesFolder.GetDirectories();
                DirectoryInfo latest = null;
                foreach (DirectoryInfo folder in subFolders)
                {
                    //sort by access time
                    if (MightBeWorldFolder(folder) && folder == MostRecentlyAccessed(folder, latest))
                        latest = folder;
                }

                //determine final state
                directory = latest;
                return latest is null ? 
                    SaveFolderState.NoWorlds 
                    : SaveFolderState.Valid;
            }
            catch (ArgumentException)     { return SaveFolderState.InvalidPath; }
            catch (NotSupportedException) { return SaveFolderState.InvalidPath; }
            catch (PathTooLongException)  { return SaveFolderState.PathTooLong; }
            catch (SecurityException)     { return SaveFolderState.PermissionError; }
            catch (UnauthorizedAccessException) { return SaveFolderState.PermissionError; }
        }
    }
}