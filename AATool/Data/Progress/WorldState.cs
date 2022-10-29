using System;
using System.Collections.Generic;
using System.ComponentModel;
using AATool.Data.Objectives;
using AATool.Net;
using AATool.Utilities;
using Newtonsoft.Json;

namespace AATool.Data.Progress
{
    [TypeConverter(typeof(WorldState))]
    [JsonObject]
    public class WorldState : ProgressState
    {
        public static readonly WorldState Empty = new ();

        [JsonProperty] public Dictionary<Uuid, Contribution> Players { get; set; }

        [JsonProperty] public string GameCategory { get; set; }
        [JsonProperty] public string GameVersion { get; set; }

        public WorldState()
        {
            this.Advancements = new ();
            this.Criteria = new ();
            this.DeathMessages = new ();
            this.Players = new ();
        }

        public string ToJsonString()
        {
            //store current category and version data then serialize
            this.GameCategory = Tracker.CurrentCategory;
            this.GameVersion = Tracker.CurrentVersion;
            return JsonConvert.SerializeObject(this);
        }

        public static WorldState FromJsonString(string jsonString)
        {
            try
            {
                //deserialize progress
                dynamic json = JsonConvert.DeserializeObject<WorldState>(jsonString);
                WorldState state = JsonConvert.DeserializeObject<WorldState>(jsonString);

                //clone complex data structures
                state.Advancements = json.CompletedAdvancements;
                state.Criteria = json.CompletedCriteria;
                state.Players = json.Players;
                state.PickupCounts = json.PickedUp;
                state.DropCounts = json.Dropped;
                state.MineCounts = json.Mined;
                state.CraftCounts = json.Crafted;
                state.UseCounts = json.Used;
                state.KillCounts = json.Killed;

                //make sure everyone's names and avatars are loaded
                foreach (KeyValuePair<Uuid, Contribution> contribution in state.Players)
                    Player.FetchIdentityAsync(contribution.Key);
                return state;
            }
            catch
            {
                //error deserializing world state
                return new WorldState();
            }
        }

        public override HashSet<Completion> CompletionsOf(IObjective objective)
        {
            //compile a list of all players who have completed this objective
            var completionists = new HashSet<Completion>();
            if (objective is Advancement advancement)
            {
                foreach (KeyValuePair<Uuid, Contribution> player in this.Players)
                {
                    if (player.Value.Advancements.TryGetValue(advancement.Id, out Completion completion))
                        completionists.Add(completion);
                }
            }
            else if (objective is Criterion criterion)
            {
                foreach (KeyValuePair<Uuid, Contribution> player in this.Players)
                {
                    if (player.Value.Criteria.TryGetValue((criterion.Owner.Id, criterion.Id), out Completion completion))
                        completionists.Add(completion);
                }
            }
            else if (objective is Block block)
            {
                foreach (KeyValuePair<Uuid, Contribution> player in this.Players)
                {
                    if (player.Value.UseCounts.ContainsKey(block.Id))
                        completionists.Add(new Completion(player.Key, default));
                }
            }
            else if (objective is Death death)
            {
                if (this.DeathMessages.Contains(death.Id))
                    completionists.Add(new Completion(Tracker.GetMainPlayer(), default));
            }
            return completionists;
        }

        public void SyncDeathMessages()
        {
            if (ActiveInstance.TryGetLog(out string log) && Player.TryGetName(Tracker.GetMainPlayer(), out string name))
            {
                log = log.ToLower();
                foreach (Death death in Tracker.Deaths.All.Values)
                {
                    foreach (string message in death.Messages)
                    {
                        if (log.Contains($"[server thread/info]: {name.ToLower()} {message}"))
                        {
                            this.DeathMessages.Add(death.Id);
                            break;
                        }
                    }
                }
            }
        }
    }
}
