using System;
using System.Collections.Generic;
using System.ComponentModel;
using AATool.Data.Objectives;
using AATool.Net;
using Newtonsoft.Json;

namespace AATool.Data.Progress
{
    [TypeConverter(typeof(Contribution))]
    [JsonObject]
    public class Contribution : ProgressState
    {
        [JsonProperty] public readonly Uuid Player;

        [JsonConstructor]
        public Contribution(Uuid Player,
            Dictionary<string, Completion> Advancements,
            Dictionary<(string, string), Completion> Criteria,
            Dictionary<string, Completion> Recipes) 
            : base(Advancements, Criteria, Recipes)
        {
            this.Player = Player;
        }

        public Contribution(Uuid Player) : base()
        {
            this.Player = Player;
        }

        public static Contribution FromJsonString(string jsonString) =>
            JsonConvert.DeserializeObject<Contribution>(jsonString);

        public string ToJsonString() => 
            JsonConvert.SerializeObject(this);

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
                if (this.Criteria.TryGetValue((criterion.OwnerId, criterion.Id), out Completion completion))
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
