using System;
using System.Collections.Generic;
using AATool.Data;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Saves;
using AATool.Settings;
using AATool.Utilities;

namespace AATool
{
    public static class Tracker
    {
        private const double REFRESH_INTERVAL = 1.0;

        public static ProgressState Progress { get; private set; }

        private static readonly AdvancementManifest Advancements = new ();
        private static readonly AchievementManifest Achievements = new ();
        private static readonly StatisticsManifest Statistics    = new ();

        public static bool Invalidated        => World.FolderChangedFlag || World.FilesChangedFlag|| NetworkContentChangedFlag;
        public static bool WorldFolderChanged => World.FolderChangedFlag || NetworkContentChangedFlag;
        public static bool WorldFilesChanged  => World.FilesChangedFlag  || NetworkContentChangedFlag;
        public static TimeSpan InGameTime     => World.CurrentState is SaveFolderState.Valid ? Progress.InGameTime : default;

        private static bool NetworkContentChangedFlag;

        public static int Percent => (int)(CompletedAdvancements / (double)AllAdvancements.Count * 100);

        public static int CompletedAdvancements => Config.IsPostExplorationUpdate
            ? Advancements.Completed
            : Achievements.Completed;

        public static Dictionary<string, Advancement> AllAdvancements => 
            Config.IsPostExplorationUpdate ? Advancements.AllAdvancements : Achievements.AllAdvancements;

        public static Dictionary<(string adv, string crit), Criterion> AllCriteria =>
            Config.IsPostExplorationUpdate ? Advancements.AllCriteria : Achievements.AllCriteria;

        public static Dictionary<string, Statistic> AllItems => Statistics.Items;

        public static bool TryGetAdvancement(string id, out Advancement advancement) =>
            AllAdvancements.TryGetValue(id, out advancement);

        public static bool TryGetCriterion(string adv, string crit, out Criterion criterion) =>
            AllCriteria.TryGetValue((adv, crit), out criterion);

        public static bool TryGetAdvancementGroup(string id, out AdvancementGroup group) =>
            Advancements.TryGetGroup(id, out group);

        public static bool TryGetItem(string id, out Statistic item) =>
            Statistics.TryGetItem(id, out item);

        public static bool IsComplete => Config.IsPostExplorationUpdate
            ? Advancements.Completed >= Advancements.AdvancementCount
            : Achievements.Completed >= Achievements.Count;

        public static int AdvancementCount => Config.IsPostExplorationUpdate 
            ? Advancements.AdvancementCount
            : Achievements.Count;

        private static World World;
        private static Timer RefreshTimer;
        private static string LastServerMessage;

        public static string WorldName          => World.Name;
        public static SaveFolderState SaveState => World.CurrentState;

        public static void Initialize()
        {
            Progress     = new ();
            World        = new ();
            RefreshTimer = new ();
            NetworkContentChangedFlag = true;
            UpdateManifestReferences();
        }

        public static void ClearFlags()
        {
            NetworkContentChangedFlag = false;
            World.ClearFlags();
        }

        private static void Clear()
        {
            Progress = new ();
            UpdateManifestProgress();
        }

        public static void Invalidate() => World.Invalidate();

        public static void Update(Time time)
        {
            //determine if it's time to update yet or not
            bool forceRefresh = Config.Tracker.GameVersionChanged();
            forceRefresh |= Config.Tracker.UseDefaultPathChanged();
            forceRefresh |= Config.Tracker.UseRemoteWorldChanged();
            forceRefresh |= !Config.Tracker.UseDefaultPath && Config.Tracker.CustomPathChanged();
            forceRefresh |= Peer.StateChangedFlag;

            RefreshTimer.Update(time);
            if (RefreshTimer.IsExpired || forceRefresh)
            {
                Sync();
                RefreshTimer.SetAndStart(REFRESH_INTERVAL);
            }      
        }

        public static HashSet<Uuid> GetAllPlayers()
        {
            var ids = new HashSet<Uuid>();
            foreach (Uuid id in Progress.Players.Keys)
                ids.Add(id);
            if (Peer.IsConnected && Peer.TryGetLobby(out Lobby lobby))
            {
                foreach (Uuid key in lobby.Users.Keys)
                    ids.Add(key);
            }
            return ids;
        }

        private static void Sync()
        {
            if (Client.TryGet(out Client client))
            {
                //update world from co-op server
                if (client.TryGetData(Protocol.PROGRESS, out string jsonString) && LastServerMessage != jsonString)
                {
                    NetworkContentChangedFlag = true;
                    LastServerMessage = jsonString;
                    Progress = ProgressState.FromJsonString(jsonString);

                    //re-load tracking manifests if game version has changed
                    Config.Tracker.TrySetGameVersion(Progress.GameVersion);
                    if (Config.Tracker.GameVersionChanged())
                        UpdateManifestReferences();
                    UpdateManifestProgress();
                }
            }
            else
            {
                //re-load tracking manifests if game version has changed
                if (Config.Tracker.GameVersionChanged())
                    UpdateManifestReferences();

                //prevent tracker from refreshing every time a file is downloaded
                if (SftpSave.IsDownloading)
                    return;

                //update from local files if they've changed since last time
                if (World.TryUpdate() || Peer.StateChangedFlag || Config.Tracker.UseRemoteWorldChanged())
                {
                    LastServerMessage = null;
                    Progress.Sync(World);
                    UpdateManifestProgress();

                    //broadcast changes to connected clients if server is running
                    if (Server.TryGet(out Server server) && server.Connected())
                        server.SendProgress();
                }
            }
        }

        private static void UpdateManifestReferences()
        {
            LastServerMessage = string.Empty;
            if (Config.IsPostExplorationUpdate)
                Advancements.UpdateReference();
            else
                Achievements.UpdateReference();
            Statistics.UpdateReference();
        }

        private static void UpdateManifestProgress()
        {
            if (Config.IsPostExplorationUpdate)
            {
                Advancements.Update(Progress);
                Achievements.ClearProgress();
            }
            else
            {
                Advancements.ClearProgress();
                Achievements.Update(Progress);
            }
            Statistics.Update(Progress);
        }
    }
}
