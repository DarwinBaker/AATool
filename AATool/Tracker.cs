using System;
using System.Collections.Generic;
using System.Linq;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Saves;
using AATool.Utilities;

namespace AATool
{
    public static class Tracker
    {
        private const double RefreshInterval = 1.0;

        public static readonly AdvancementManifest Advancements = new ();
        public static readonly AchievementManifest Achievements = new ();
        public static readonly PickupManifest Pickups = new ();
        public static readonly BlockManifest Blocks = new ();
        
        public static Category Category { get; private set; }
        public static WorldState State  { get; private set; }
        public static bool CoOpStateChanged { get; private set; }

        public static bool ObjectivesChanged => Config.Tracking.GameCategory.Changed || Config.Tracking.GameVersion.Changed;
        public static bool ProgressChanged => World.ProgressChanged || World.WorldChanged || World.Invalidated || CoOpStateChanged;
        public static bool Invalidated => ProgressChanged || ObjectivesChanged;

        public static string CurrentCategory => Category.Name;
        public static string CurrentVersion => Category.CurrentVersion;

        public static bool WorldLocked { get; private set; }
        public static void ToggleWorldLock() => WorldLocked ^= true;

        public static AdvancementManifest CurrentAdvancementManifest => Category is AllAchievements
            ? Achievements
            : Advancements;

        public static Dictionary<(string adv, string crit), Criterion> Criteria => Category is AllAchievements
            ? Achievements.Criteria
            : Advancements.Criteria;

        public static SaveState SaveState => World.CurrentState;
        public static string WorldName => World.Name;
        public static TimeSpan InGameTime => State.InGameTime;
        public static bool InGameTimeChanged => State.InGameTime != LastInGameTime;

        private static LocalSave World;
        private static Timer RefreshTimer;
        private static string LastServerMessage;
        private static TimeSpan LastInGameTime;

        public static bool TryGetAdvancement(string id, out Advancement advancement) =>
            CurrentAdvancementManifest.TryGet(id, out advancement);

        public static bool TryGetCriterion(string adv, string crit, out Criterion criterion) =>
            Criteria.TryGetValue((adv, crit), out criterion);

        public static bool TryGetAdvancementGroup(string id, out HashSet<Advancement> group) =>
            Advancements.TryGet(id, out group);

        public static bool TryGetPickup(string id, out Pickup item) =>
            Pickups.TryGet(id, out item);

        public static HashSet<Uuid> GetAllPlayers()
        {
            var ids = new HashSet<Uuid>();
            foreach (Uuid id in State.Players.Keys)
                ids.Add(id);
            if (Peer.IsConnected && Peer.TryGetLobby(out Lobby lobby))
            {
                foreach (Uuid key in lobby.Users.Keys)
                    ids.Add(key);
            }
            ids.Remove(Uuid.Empty);
            return ids;
        }

        public static void Initialize()
        {
            World = new LocalSave();
            State = new WorldState();
            RefreshTimer = new Timer();
            string lastVersion = Config.Tracking.GameVersion;
            TrySetCategory(Config.Tracking.GameCategory);
            TrySetVersion(lastVersion);
        }

        public static bool TrySetCategory(string category)
        {
            //check if category is the same 
            if (Category is not null && category == Category.Name)
                return false;

            try
            {
                Category = category.ToLower().Replace(" ", "").Replace("_", "") switch {
                    "alladvancements" => new AllAdvancements(),
                    "allachievements" => new AllAchievements(),
                    "halfpercent"     => new HalfPercent(),
                    "balanceddiet"    => new BalancedDiet(),
                    "adventuringtime" => new AdventuringTime(),
                    "monstershunted"  => new MonstersHunted(),
                    _ => throw new ArgumentException($"Category not supported: \"{category}\"."),
                };

                //save change to config
                Config.Tracking.GameCategory.Set(Category.Name);
                Config.Tracking.GameVersion.Set(Category.CurrentVersion);
                Config.Tracking.Save();

                RefreshObjectives();
                return true;
            }
            catch (ArgumentException)
            {
                if (Category is null)
                {
                    //fallback to all advancements
                    Category = new AllAdvancements();
                    Config.Tracking.GameCategory.Set(Category.Name);
                    Config.Tracking.Save();
                    RefreshObjectives();
                }
                return false;
            }
        }

        public static void TrySetVersion(string versionNumber) => 
            Category.TrySetVersion(versionNumber);

        public static void Invalidate(bool invalidateWorld = false)
        {
            if (invalidateWorld)
                World.Invalidate();
            RefreshTimer.Expire();
        }

        public static void ClearFlags()
        {
            LastInGameTime = InGameTime;
            CoOpStateChanged = false;
            World.ClearFlags();
        }

        public static void TryUpdate(Time time)
        {
            //check if we need to force an update right now
            bool invalidated = ObjectivesChanged;
            invalidated |= Config.Tracking.UseSftp.Changed;
            invalidated |= Config.Tracking.UseDefaultPath.Changed;
            invalidated |= !Config.Tracking.UseDefaultPath && Config.Tracking.CustomSavePath.Changed;
            invalidated |= Peer.StateChanged;

            if (RefreshTimer.IsExpired || invalidated)
            {
                Update();
                RefreshTimer.SetAndStart(RefreshInterval);
            }
            else
            {
                RefreshTimer.Update(time);
            }
        }

        private static void Update()
        {
            if (Client.TryGet(out Client client))
            {
                //update world from co-op server
                if (!client.TryGetData(Protocol.Headers.Progress, out string jsonString))
                    return;

                if (LastServerMessage != jsonString)
                {
                    CoOpStateChanged = true;
                    LastServerMessage = jsonString;
                    State = WorldState.FromJsonString(jsonString);

                    //sync category and version with host
                    TrySetCategory(State.GameCategory);
                    TrySetVersion(State.GameVersion);

                    //re-load tracking manifests if game version has changed
                    if (ObjectivesChanged)
                        RefreshObjectives();

                    UpdateManifestProgress();
                }
            }
            else
            {
                //reload objective manifests if game version has changed
                if (ObjectivesChanged)
                    RefreshObjectives();

                //wait to refresh until sftp transer is complete
                if (Config.Tracking.UseSftp && SftpSave.IsDownloading)
                    return;

                //update progress if source has been invalidated
                if (World.TryReadFiles() || Peer.StateChanged)
                {
                    LastServerMessage = null;
                    State = World.GetState();
                    UpdateManifestProgress();

                    //broadcast changes to connected clients if server is running
                    if (Server.TryGet(out Server server) && server.Connected())
                        server.SendProgress();
                }
            }
        }

        private static void RefreshObjectives()
        {
            Advancements.RefreshObjectives();
            Achievements.RefreshObjectives();
            Pickups.RefreshObjectives();
            Blocks.RefreshObjectives();
        }

        private static void UpdateManifestProgress()
        {
            Advancements.UpdateStates(State);
            Achievements.UpdateStates(State);
            Pickups.UpdateStates(State);
            Blocks.UpdateStates(State);
        }
    }
}
