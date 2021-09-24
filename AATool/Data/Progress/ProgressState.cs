using AATool.Net;
using AATool.Saves;
using AATool.Settings;
using AATool.UI.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace AATool.Data.Progress
{
    [JsonObject]
    public class ProgressState
    {
        [JsonProperty] public Dictionary<Uuid, Contribution> Players                      { get; private set; }
        [JsonProperty] public HashSet<string> AllCompletedAdvancements                  { get; private set; }
        [JsonProperty] public HashSet<(string adv, string crit)> AllCompletedCriteria   { get; private set; }
        [JsonProperty] public Dictionary<string, int> ItemTotals                        { get; private set; }
        [JsonProperty] public Dictionary<string, int> KillTotals                        { get; private set; }

        [JsonProperty] public TimeSpan InGameTime           { get; private set; }

        [JsonProperty] public string GameVersion            { get; private set; }

        [JsonProperty] public int Deaths                    { get; private set; }
        [JsonProperty] public int DamageTaken               { get; private set; }
        [JsonProperty] public int DamageDealt               { get; private set; }
        [JsonProperty] public int Jumps                     { get; private set; }
        [JsonProperty] public int Sleeps                    { get; private set; }
        [JsonProperty] public int SaveAndQuits              { get; private set; }

        [JsonProperty] public double TotalKilometersFlown   { get; private set; }
        [JsonProperty] public int BreadEaten                { get; private set; }
        [JsonProperty] public int ItemsEnchanted            { get; private set; } 
        [JsonProperty] public int EnderPearlsThrown         { get; private set; }
        [JsonProperty] public double TemplesRaided          { get; private set; }

        [JsonProperty] public int CreepersKilled            { get; private set; }
        [JsonProperty] public int DrownedKilled             { get; private set; }
        [JsonProperty] public int WitherSkeletonsKilled     { get; private set; }
        [JsonProperty] public int FishCollected             { get; private set; }
        [JsonProperty] public int PhantomsKilled            { get; private set; }

        [JsonProperty] public int SugarcaneCollected        { get; private set; }
        [JsonProperty] public int LecternsMined             { get; private set; }
        [JsonProperty] public int NetherrackMined           { get; private set; }
        [JsonProperty] public int GoldMined                 { get; private set; }
        [JsonProperty] public int EnderChestsMined          { get; private set; }


        public ProgressState()
        {
            this.Players                  = new();
            this.AllCompletedAdvancements = new();
            this.AllCompletedCriteria     = new();
            this.ItemTotals               = new();
            this.KillTotals               = new();
        }

        public static ProgressState FromJsonString(string jsonString)
        {
            dynamic json = JsonConvert.DeserializeObject<ProgressState>(jsonString);
            ProgressState progress = JsonConvert.DeserializeObject<ProgressState>(jsonString);
            progress.Players                  = json.Players;
            progress.AllCompletedAdvancements = json.AllCompletedAdvancements;
            progress.AllCompletedCriteria     = json.AllCompletedCriteria;
            progress.ItemTotals               = json.ItemTotals;
            progress.KillTotals               = json.KillTotals;
            foreach (KeyValuePair<Uuid, Contribution> contribution in progress.Players)
                Player.FetchIdentity(contribution.Key);
            return progress;
        }

        public string ToJsonString()
        {
            this.GameVersion = Config.Tracker.GameVersion;
            return JsonConvert.SerializeObject(this);
        }

        public HashSet<Uuid> CompletionistsOf(IAchievable achievable)
        {
            HashSet<Uuid> completionists = new ();
            if (achievable is Advancement advancement)
            {
                foreach (var player in this.Players)
                {
                    if (player.Value.IncludesAdvancement(advancement.Id))
                        completionists.Add(player.Key);
                }
            }
            else if (achievable is Criterion criterion)
            {
                foreach (var player in this.Players)
                {
                    if (player.Value.Criteria.Contains((criterion.ParentAdvancement.Id, criterion.ID)))
                        completionists.Add(player.Key);
                }
            }
            return completionists;
        }

        public bool IsAdvancementCompleted(string id) =>
            this.AllCompletedAdvancements.Contains(id);

        public bool IsCriterionCompleted((string, string) criterion) =>
            this.AllCompletedCriteria.Contains(criterion);

        public int ItemCount(string item) =>
            this.ItemTotals.TryGetValue(item, out int count) ? count : 0;

        public int KillCount(string mob) =>
            this.ItemTotals.TryGetValue(mob, out int count) ? count : 0;

        public void Sync(World world)
        {
            this.Clear();
            if (!world.IsEmpty)
            {
                //sync progress to local world
                this.CopyFrom(world);

                //sync statistics to local world
                this.SyncStatistics(world);
            }
        }

        private void Clear()
        {
            this.Players.Clear();
            this.AllCompletedAdvancements.Clear();
            this.AllCompletedCriteria.Clear();
            this.ItemTotals.Clear();
            this.KillTotals.Clear();
        }

        private void CopyFrom(World world)
        {
            //add each player uuid in current world
            foreach (Uuid id in world.GetAllGuids())
                this.Players[id] = new Contribution(id);

            if (Config.IsPostExplorationUpdate)
            {
                foreach (KeyValuePair<string, Advancement> advancement in Tracker.AllAdvancements)
                {
                    //add advancement if completed
                    if (world.Advancements.TryGetAdvancementCompletionFor(advancement.Key, out List<Uuid> ids))
                    {
                        this.AllCompletedAdvancements.Add(advancement.Key);
                        foreach (Uuid id in ids)
                            this.Players[id].AddAdvancement(advancement.Key);
                    }

                    if (advancement.Value.TryGetCriteria(out CriteriaSet criteria))
                    {
                        //add completed criteria if this advancement has any
                        var completions = world.Advancements.GetAllCriteriaCompletions(advancement.Value);
                        foreach (KeyValuePair<Uuid, HashSet<(string adv, string crit)>> player in completions)
                        {
                            foreach ((string adv, string crit) criterion in player.Value)
                            {
                                this.Players[player.Key].AddCriterion(criterion.adv, criterion.crit);
                                this.AllCompletedCriteria.Add(criterion);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, Advancement> advancement in Tracker.AllAdvancements)
                {
                    //add advancement if completed
                    if (world.Achievements.TryGetAchievementCompletionFor(advancement.Key, out List<Uuid> ids))
                    {
                        this.AllCompletedAdvancements.Add(advancement.Key);
                        foreach (Uuid id in ids)
                            this.Players[id].AddAdvancement(advancement.Key);
                    }

                    if (advancement.Value.TryGetCriteria(out CriteriaSet criteria))
                    {
                        //add completed criteria if this advancement has any
                        var completions = world.Achievements.GetAllCriteriaCompletions(advancement.Value);
                        foreach (KeyValuePair<Uuid, HashSet<(string adv, string crit)>> player in completions)
                        {
                            foreach ((string adv, string crit) criterion in player.Value)
                            {
                                this.Players[player.Key].AddCriterion(criterion.adv, criterion.crit);
                                this.AllCompletedCriteria.Add(criterion);
                            }
                        }
                    }
                }
            }
        }

        private void SyncStatistics(World world)
        {
            this.InGameTime   = world.Statistics.GetInGameTime();
            this.Deaths       = world.Statistics.GetTotalDeaths();
            this.Jumps        = world.Statistics.GetTotalJumps();
            this.Sleeps       = world.Statistics.GetTotalSleeps();
            this.DamageTaken  = world.Statistics.GetTotalDamageTaken();
            this.DamageDealt  = world.Statistics.GetTotalDamageDealt();
            this.SaveAndQuits = world.Statistics.GetTotalSaveAndQuits();

            this.TotalKilometersFlown   = world.Statistics.GetTotalKilometersFlown();
            this.BreadEaten             = world.Statistics.GetUsedCount("bread");
            this.ItemsEnchanted         = world.Statistics.GetItemsEnchanted();
            this.EnderPearlsThrown      = world.Statistics.GetUsedCount("ender_pearl");
            this.TemplesRaided          = world.Statistics.GetMinedCount("tnt") / 9;

            this.CreepersKilled         = world.Statistics.GetKillCount("creeper");
            this.DrownedKilled          = world.Statistics.GetKillCount("drowned");
            this.WitherSkeletonsKilled  = world.Statistics.GetKillCount("wither_skeleton");
            this.FishCollected          = world.Statistics.GetKillCount("cod") + world.Statistics.GetKillCount("salmon");
            this.PhantomsKilled         = world.Statistics.GetKillCount("phantom");

            this.LecternsMined          = world.Statistics.GetMinedCount("lectern");
            this.SugarcaneCollected     = world.Statistics.GetItemCountsFor("minecraft:sugar_cane", out _);
            this.NetherrackMined        = world.Statistics.GetMinedCount("netherrack");
            this.GoldMined              = world.Statistics.GetMinedCount("gold_block");
            this.EnderChestsMined       = world.Statistics.GetMinedCount("ender_chest");

            //add each item count
            foreach (string item in Tracker.AllItems.Keys)
            {
                int total = world.Statistics.GetItemCountsFor(item, out Dictionary<Uuid, int> countsByPlayer);
                if (total > 0)
                {
                    if (!this.ItemTotals.ContainsKey(item))
                        this.ItemTotals[item] = total;
                    else
                        this.ItemTotals[item] += total;

                    foreach (KeyValuePair<Uuid, int> count in countsByPlayer)
                    {
                        if (this.Players.TryGetValue(count.Key, out Contribution player))
                            player.AddItemCount(item, count.Value);
                    }
                }
            }
        }
    }
}
