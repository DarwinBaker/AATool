using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Data.Progress;
using AATool.Exceptions;
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
        public static readonly DeathManifest Deaths = new ();

        public static Category Category { get; private set; }
        public static WorldState State  { get; private set; }
        public static Exception LastError { get; private set; }
        public static bool DesignationsChanged { get; private set; }
        public static bool MainPlayerChanged { get; private set; }
        public static bool CoOpStateChanged { get; private set; }
        public static bool WorldLocked { get; private set; }
        public static string Status { get; private set; }

        public static bool Invalidated =>
            SavesFolderChanged || ObjectivesChanged || ProgressChanged;

        public static bool SavesFolderChanged => 
            PreviousSavesPath != Paths.Saves.CurrentFolder();

        public static bool WorldChanged => World.PathChanged;

        public static bool ObjectivesChanged => 
            Config.Tracking.GameCategory.Changed || Config.Tracking.GameVersion.Changed;

        public static bool ProgressChanged => 
            World.ProgressChanged || World.PathChanged || World.Invalidated || CoOpStateChanged;

        public static bool IsWorking => LastError is null;
        public static string CurrentCategory => Category.Name;
        public static string CurrentVersion => Category.CurrentVersion;

        public static AdvancementManifest CurrentAdvancementSet => Category is not AllAchievements
            ? Advancements
            : Achievements;

        public static Dictionary<(string adv, string crit), Criterion> AllCriteria =>
            CurrentAdvancementSet.AllCriteria;

        public static Dictionary<(string adv, string crit), Criterion> RemainingCriteria =>
            CurrentAdvancementSet.RemainingCriteria;

        public static string WorldName => World.Name;
        public static TimeSpan InGameTime => State.InGameTime;
        public static bool InGameTimeChanged => State.InGameTime != LastInGameTime;
        public static TrackerSource Source => Config.Tracking.Source;

        public static void ToggleWorldLock() => WorldLocked ^= true;

        public static string GetFullIgt() => $"{(int)InGameTime.TotalHours}:{InGameTime:mm':'ss}";

        public static string GetShortIgt() => $"{InGameTime:hh':'mm':'ss}";

        public static string GetDays() => InGameTime.Days is 0 ? string.Empty : $"{Math.Round(InGameTime.TotalDays, 2)} IRL Days";

        public static string GetDaysAndHours()
        {
            if (InGameTime.Days is 0)
                return string.Empty;

            return InGameTime.Days is 1
                ? $"1 Day, {InGameTime.Hours}.{(int)(InGameTime.Minutes / 60.0f * 100)} Hrs Played"
                : $"{InGameTime.Days} Days, {InGameTime.Hours}.{(int)(InGameTime.Minutes / 60.0f * 100)} Hrs Played";
        }

        private static WorldFolder World;
        private static Timer RefreshTimer;
        private static TimeSpan LastInGameTime;
        private static Uuid PreviousMainPlayer;
        private static string PreviousSavesPath;
        private static string PreviousWorldPath;
        private static string LastServerMessage;

        public static void InvalidateDesignations()
        {
            DesignationsChanged = true;
            CurrentAdvancementSet.RefreshRemainingCriteria();
        }

        public static bool TryGetAdvancement(string id, out Advancement advancement) =>
            CurrentAdvancementSet.TryGet(id, out advancement);

        public static bool TryGetCriterion(string adv, string crit, out Criterion criterion) =>
            AllCriteria.TryGetValue((adv, crit), out criterion);

        public static bool TryGetAdvancementGroup(string id, out HashSet<Advancement> group) =>
            Advancements.TryGet(id, out group);

        public static bool TryGetPickup(string id, out Pickup item) =>
            Pickups.TryGet(id, out item);

        public static bool TryGetBlock(string id, out Block block) =>
            Blocks.TryGet(id, out block);

        public static bool TryGetDeath(string id, out Death death) =>
            Deaths.TryGet(id, out death);

        public static Uuid GetMainPlayer()
        {
            if (Config.Tracking.Filter == ProgressFilter.Solo)
            {
                if (PreviousMainPlayer == Uuid.Empty & Player.TryGetUuid(Config.Tracking.SoloFilterName, out Uuid player))
                    MainPlayerChanged = true;
                PreviousMainPlayer = player;
                return player;
            }
            else
            {
                return State.Players.Keys.FirstOrDefault();
            }
        }

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
            else if (Config.Tracking.Filter == ProgressFilter.Solo)
            {
                if (Player.TryGetUuid(Config.Tracking.SoloFilterName, out Uuid soloPlayer))
                    ids.Add(soloPlayer);
            }
            ids.Remove(Uuid.Empty);
            return ids;
        }

        public static void Initialize()
        {
            World = new WorldFolder();
            State = new WorldState();
            RefreshTimer = new Timer();
            string lastVersion = Config.Tracking.GameVersion;
            TrySetCategory(Config.Tracking.GameCategory);
            TrySetVersion(lastVersion);
        }

        public static string GetStatusText()
        {
            if (Peer.IsServer && Peer.IsConnected)
            {
                return $"Hosting: \"{WorldName}\"";
            }
            else if (IsWorking)
            {
                return Source is TrackerSource.ActiveInstance && ActiveInstance.HasNumber
                    ? $"Instance {ActiveInstance.Number}: \"{WorldName}\""
                    : $"Tracking: \"{WorldName}\"";
            }
            return LastError.Message;
        }
        
        public static bool TrySetCategory(string category)
        {
            //check if category is the same
            if (Category is not null && category == Category.Name)
                return false;

            try
            {
                Category = category.ToLower().Replace(" ", "").Replace("_", "") switch {       
                    //main categories
                    "alladvancements" => new AllAdvancements(),
                    "allachievements" => new AllAchievements(),
                    "halfpercent"     => new HalfPercent(),

                    //single advancement categories
                    "balanceddiet"    => new BalancedDiet(),
                    "adventuringtime" => new AdventuringTime(),
                    "monstershunted"  => new MonstersHunted(),

                    //random extensions
                    "allblocks" => new AllBlocks(),
                    "alldeaths" => new AllDeaths(),
                    "halfdeaths" => new HalfDeaths(),

                    _ => throw new ArgumentException($"Category not supported: \"{category}\"."),
                };
                //save change to config
                Config.Tracking.GameCategory.Set(Category.Name);
                Config.Tracking.GameVersion.Set(Category.CurrentVersion);
                Config.Tracking.Save();

                Category.LoadObjectives();
                return true;
            }
            catch
            {
                if (Category is null)
                {
                    //fallback to all advancements
                    Category = new AllAdvancements();
                    Config.Tracking.GameCategory.Set(Category.Name);
                    Config.Tracking.Save();
                    Category.LoadObjectives();
                }
                return false;
            }
        }

        public static bool TrySetVersion(string versionNumber) => 
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
            DesignationsChanged = false;
            MainPlayerChanged = false;
            CoOpStateChanged = false;
            World.ClearFlags();
        }

        public static void Update(Time time)
        {
            RefreshTimer.Update(time);
            if (RefreshTimer.IsExpired || ObjectivesChanged || Config.Tracking.SourceChanged)
            {
                UpdateCurrentWorld();

                if (Client.TryGet(out Client client))
                    ParseCoOpProgress(client);
                else
                    ReadLocalFiles();

                RefreshTimer.SetAndStart(RefreshInterval);
            }
            Category.Update();
        }

        private static void UpdateCurrentWorld()
        {
            if (Config.Tracking.Source.Changed)
                WorldLocked = false;

            string savesPath = string.Empty;
            string worldPath = string.Empty;
            DirectoryInfo latestWorld = null;
            try
            {
                if (Source is TrackerSource.SpecificWorld && !Config.Tracking.UseSftp)
                {
                    //set world to user-defined path
                    WorldLocked = true;
                    worldPath = Config.Tracking.CustomWorldPath;

                    //check if path is empty
                    if (string.IsNullOrEmpty(worldPath))
                    {
                        if (LastError is not ArgumentException || Config.Tracking.SourceChanged)
                            throw new ArgumentException("User-specified world path empty");
                        return;
                    }

                    //exit early if path invalid and unchanged
                    if (LastError is not InvalidPathException || Config.Tracking.SourceChanged)
                    {
                        //validate path (throws if invalid characters present)]
                        try
                        {
                            latestWorld = new DirectoryInfo(worldPath);
                        }
                        catch
                        {
                            throw new InvalidPathException();
                        }
                    }
                }
                else
                {
                    //get current saves folder
                    savesPath = Paths.Saves.CurrentFolder();

                    //exit early if path invalid
                    if (LastError is InvalidPathException && savesPath == PreviousSavesPath)
                        return;

                    //unlock world if saves folder changed
                    if (PreviousSavesPath != savesPath)
                        WorldLocked = false;

                    //make sure path isn't empty
                    if (string.IsNullOrEmpty(savesPath))
                    {
                        if (LastError is not ArgumentException || Config.Tracking.SourceChanged)
                        {
                            throw Source is TrackerSource.ActiveInstance
                                ? new ArgumentException("Tab into Minecraft to start tracking")
                                : new ArgumentException("Custom saves path is empty");
                        }
                        return;
                    }

                    //validate path (throws if invalid characters present)
                    var savesFolder = new DirectoryInfo(savesPath);

                    //make sure folder actually exists
                    if (!savesFolder.Exists)
                    {
                        //avoid re-throwing duplicate exception
                        if (LastError is not NoSavesFolderException)
                            throw new NoSavesFolderException(savesFolder.FullName);
                        return;
                    }

                    if (WorldLocked)
                    {
                        //keep same world
                        worldPath = PreviousWorldPath;
                        latestWorld = new DirectoryInfo(worldPath);
                    }
                    else
                    {
                        //find most recently modified world in folder
                        DirectoryInfo[] potentialWorlds = savesFolder.GetDirectories();
                        foreach (DirectoryInfo worldFolder in potentialWorlds)
                        {
                            //skip any folders that definitely aren't worlds
                            if (!Paths.Saves.MightBeWorldFolder(worldFolder))
                                continue;

                            //sort by write time
                            if (worldFolder == Paths.Saves.MostRecentlyWritten(worldFolder, latestWorld))
                                latestWorld = worldFolder;
                        }
                        worldPath = latestWorld?.FullName;
                    }
                }

                //make sure folder actually exists
                if (latestWorld is null || !latestWorld.Exists)
                {
                    if (LastError is not NoWorldException || latestWorld?.FullName != World?.FullName)
                        throw new NoWorldException();
                    return;
                }

                if (latestWorld.FullName != World.FullName)
                    World.SetPath(latestWorld);

                LastError = null;
            }
            catch (Exception e)
            {
                if (!World.IsEmpty)
                {
                    World.Unset();
                    WorldLocked = false;
                }
                LastError = e;
            }
            finally
            {
                PreviousSavesPath = savesPath;
                PreviousWorldPath = worldPath;
            }
        }

        private static void ParseCoOpProgress(Client client)
        {
            //update world from co-op server
            if (client is null || !client.TryGetData(Protocol.Headers.Progress, out string jsonString))
                return;

            if (LastServerMessage != jsonString)
            {
                CoOpStateChanged = true;
                LastServerMessage = jsonString;
                State = WorldState.FromJsonString(jsonString);

                //sync category and version with host
                TrySetCategory(State.GameCategory);
                TrySetVersion(State.GameVersion);

                //reload objectives if game version has changed
                if (ObjectivesChanged)
                    Category.LoadObjectives();

                SetState(State);

                if (Config.Tracking.BroadcastProgress)
                    OpenTracker.BroadcastProgress();
            }
        }

        private static void ReadLocalFiles()
        {
            //reload objective manifests if game version has changed
            if (ObjectivesChanged)
                Category.LoadObjectives();

            //wait to refresh until sftp transer is complete
            if (Config.Tracking.UseSftp && MinecraftServer.IsDownloading)
                return;

            ReadLatestLog();

            //update progress if source has been invalidated
            if (World.TryRefresh() || Peer.StateChanged || Config.Tracking.FilterChanged)
            {
                LastServerMessage = null;
                State = World.GetState();
                SetState(State);

                //broadcast changes to connected clients if server is running
                if (Server.TryGet(out Server server) && server.Connected())
                    server.SendProgress();

                //broadcast progress to opentracker
                if (Config.Tracking.BroadcastProgress)
                    OpenTracker.BroadcastProgress();
            }
        }

        private static void ReadLatestLog()
        {  
            if (Category is AllDeaths)
            {
                //attempt to sync death messages
                int before = State.DeathMessages.Count;
                State.SyncDeathMessages();
                if (State.DeathMessages.Count != before)
                    Deaths.SetState(State);
            }
        }

        private static void SetState(WorldState state)
        {
            Advancements.SetState(state);
            Achievements.SetState(state);
            Blocks.SetState(state);
            Pickups.SetState(state);
        }
    }
}
