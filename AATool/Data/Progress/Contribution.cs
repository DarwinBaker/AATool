using System;
using System.Collections.Generic;
using AATool.Data.Objectives;
using AATool.Net;
using AATool.UI.Controls;

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
            this.Player = new Uuid(network.UUID.String);
            this.ObtainedGodApple = network.ObtainedGodApple;

            //add advancements
            foreach (KeyValuePair<string, DateTime> advancement in network.Advancements)
                this.Advancements[advancement.Key] = new Completion(this.Player, advancement.Value);

            //add criteria
            foreach (NetworkCriteriaSet criteriaSet in network.Multiparts)
            {
                foreach (string crit in criteriaSet.List)
                {
                    string id = Criterion.Key(criteriaSet.Advancement, crit);
                    this.Criteria[id] = new Completion(this.Player, default);
                }
            }
                
            //add stats
            foreach (KeyValuePair<string, int> stat in network.Pickup)
                this.PickupCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in network.Drop)
                this.DropCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in network.Mine)
                this.MineCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in network.Craft)
                this.CraftCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in network.Use)
                this.UseCounts[stat.Key] = stat.Value;
            foreach (KeyValuePair<string, int> stat in network.Kill)
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
