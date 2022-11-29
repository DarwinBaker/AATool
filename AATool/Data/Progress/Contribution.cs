using System;
using System.Collections.Generic;
using AATool.Data.Objectives;
using AATool.Net;

namespace AATool.Data.Progress
{
    public class Contribution : ProgressState
    {
        public readonly Uuid Player;

        public Contribution(Uuid Player) : base()
        {
            this.Player = Player;
        }

        public Contribution(NetworkContribution network) : this(network.UUID)
        {
            this.Player = network.UUID;
            this.InGameTime = network.InGameTime;
            this.ObtainedGodApple = network.ObtainedGodApple;

            //add advancements
            foreach (KeyValuePair<string, DateTime> advancement in network.Advancements)
                this.Advancements[advancement.Key] = new Completion(this.Player, advancement.Value);
            //add criteria
            foreach (string criterion in network.Criteria)
                this.Criteria[criterion] = default;
            //add stats
            foreach (KeyValuePair<string, int> stat in network.PickupCounts)
                this.PickupCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in network.DropCounts)
                this.DropCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in network.MineCounts)
                this.MineCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in network.CraftCounts)
                this.CraftCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in network.UseCounts)
                this.UseCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in network.KillCounts)
                this.KillCounts[stat.Key] = stat.Value;
        }

        public override HashSet<Completion> CompletionsOf(IObjective objective)
        {
            //compile a list of all players who have completed this objective
            var completionists = new HashSet<Completion>();
            if (objective is Advancement advancement)
            {
                if (this.Advancements.TryGetValue(advancement.Id, out Completion completion))
                    completionists.Add(completion);
            }
            else if (objective is Criterion criterion)
            {
                if (this.Criteria.TryGetValue(Criterion.Key(criterion.OwnerId, criterion.Id), out Completion completion))
                    completionists.Add(completion);
            }
            else if (objective is Block block)
            {
                if (this.WasUsed(block.Id))
                    completionists.Add(new Completion(this.Player, default));
            }
            else if (objective is Death death)
            {
                if (this.DeathMessages.Contains(death.Id))
                    completionists.Add(new Completion(this.Player, default));
            }
            return completionists;
        }
    }
}
