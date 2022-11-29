using System.Collections.Generic;
using AATool.Data.Objectives;
using AATool.Net;
using AATool.Utilities;

namespace AATool.Data.Progress
{
    public class WorldState : ProgressState
    {
        public static readonly WorldState Empty = new ();

        public Dictionary<Uuid, Contribution> Players { get; set; }

        public WorldState() : base()
        {
            this.Players = new ();
        }

        public WorldState(NetworkState state) : this()
        {
            //copy co-op state
            foreach (NetworkContribution player in state.Players)
            {
                //individual progress
                var contribution = new Contribution(player);
                this.Players[new Uuid(player.UUID.String)] = contribution;

                //combined advancements
                foreach (KeyValuePair<string, Completion> advancement in contribution.Advancements)
                {
                    if (!this.Advancements.TryGetValue(advancement.Key, out Completion first) || advancement.Value.Before(first.Timestamp))
                        this.Advancements[advancement.Key] = advancement.Value;
                }

                //combined criteria
                foreach (KeyValuePair<string, Completion> criterion in contribution.Criteria)
                {
                    if (!this.Criteria.TryGetValue(criterion.Key, out Completion first) || criterion.Value.Before(first.Timestamp))
                        this.Criteria[criterion.Key] = criterion.Value;
                }

                //combined stats
                this.CopyStats(player.PickupCounts, this.PickupCounts);
                this.CopyStats(player.DropCounts,   this.DropCounts);
                this.CopyStats(player.MineCounts,   this.MineCounts);
                this.CopyStats(player.CraftCounts,  this.CraftCounts);
                this.CopyStats(player.UseCounts,    this.UseCounts);
                this.CopyStats(player.KillCounts,   this.KillCounts);

                //enchanted golden apple
                this.ObtainedGodApple |= contribution.ObtainedGodApple;

                //ingame time
                if (contribution.InGameTime > this.InGameTime)
                    this.InGameTime = contribution.InGameTime;
            }
        }

        private void CopyStats(Dictionary<string, int> source, Dictionary<string, int> destination)
        {
            foreach (KeyValuePair<string, int> statistic in source)
            {
                _= destination.TryGetValue(statistic.Key, out int existing);
                destination[statistic.Key] = existing + statistic.Value;
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
                    if (player.Value.Criteria.TryGetValue(Criterion.Key(criterion.Owner.Id, criterion.Id), out Completion completion))
                        completionists.Add(completion);
                }
            }
            else if (objective is Block block)
            {
                foreach (KeyValuePair<Uuid, Contribution> player in this.Players)
                {
                    if (player.Value.UseCounts.ContainsKey(block.Id) is true)
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
            if (!ActiveInstance.TryGetLog(out string log) || !Player.TryGetName(Tracker.GetMainPlayer(), out string name))
                return;

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
