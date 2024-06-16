using System;
using System.Collections.Generic;
using AATool.Data.Objectives;
using AATool.Data.Objectives.Complex;
using AATool.Data.Progress;
using AATool.Net;
using Newtonsoft.Json.Linq;

namespace AATool.Saves
{
    public class AdvancementsFolder : JsonFolder
    {
        private class AdvancementCompletion
        {
            public readonly Dictionary<string, DateTime> CriteriaTimestamps;
            public readonly Uuid Player;
            public readonly string Id;
            public readonly bool AdvancementDone;

            public DateTime Timestamp { get; private set; }

            public AdvancementCompletion(JsonStream json, dynamic token, string advancement)
            {
                this.Id = advancement;
                this.Player = json.Player;
                this.Timestamp = DateTime.MinValue;
                this.CriteriaTimestamps = new Dictionary<string, DateTime>();
                this.AdvancementDone = token?["done"] == true;
            }

            public void AddCriterion(string adv, string crit, DateTime completed)
            {
                this.CriteriaTimestamps[Criterion.Key(adv, crit)] = completed;
                if (completed > this.Timestamp)
                    this.Timestamp = completed;
            }
        }

        private bool TryGetCompletionOf(string advancement, JsonStream json, out AdvancementCompletion completion)
        {
            dynamic token = json[advancement];
            completion = new AdvancementCompletion(json, token, advancement);
            if (token is null)
                return false;

            foreach (IEnumerable<JToken> criterion in token["criteria"]?.Children())
            {
                string[] tokens = criterion?.ToString().Split('\"');
                if (tokens.Length > 3 && DateTime.TryParse(tokens[3], out DateTime timestamp))
                    completion.AddCriterion(advancement, tokens[1].ToString(), timestamp);
            }
            return true;
        }

        private Dictionary<(string adv, string crit), DateTime> TryGetCompletionOf(
            Advancement advancement, JsonStream json)
        {
            var completed = new Dictionary<(string adv, string crit), DateTime>();
            dynamic criteriaList = json?[advancement.Id]?["criteria"];
            if (criteriaList is null)
                return completed;

            //advancement has criteria. add them
            foreach (string line in criteriaList.ToString().Split('\n'))
            {
                string[] tokens = line.Trim().Split('"');
                if (tokens.Length > 1)
                {
                    string criterion = tokens[1];
                    if (advancement.Criteria.Contains(criterion))
                        completed.Add((advancement.Id, criterion), default);
                }
            }
            return completed;
        }

        protected override void Update(JsonStream json, WorldState state, Contribution contribution)
        {
            foreach (Advancement advancement in Tracker.Advancements.AllAdvancements.Values)
            {
                if (this.TryGetCompletionOf(advancement.Id, json, out AdvancementCompletion progress))
                {
                    if (progress.AdvancementDone)
                        this.UpdateAdvancements(progress, state, contribution);
                    if (advancement.HasCriteria)
                        this.UpdateCriteria(progress, state, contribution);
                }
            }

            //detect god apple from chest using mojang banner recipe
            if (this.TryGetCompletionOf(EGap.BannerRecipe, json, out _))
            {
                state.ObtainedGodApple = true;
                contribution.ObtainedGodApple = true;
            }

            foreach (var recipe in ArmorTrims.Recipes)
            {
                //detect collection of armor trims
                if (this.TryGetCompletionOf(recipe, json, out AdvancementCompletion trim))
                    state.Recipes[recipe] = new Completion(trim.Player, trim.Timestamp);
            }

            //detect lapis from chest using lapis block recipe
            if (this.TryGetCompletionOf("minecraft:recipes/building_blocks/lapis_block", json, out _))
            {
                state.ObtainedLapis = true;
                state.ObtainedLapis = true;
            }
        }

        private void UpdateAdvancements(AdvancementCompletion advancement, WorldState state, Contribution contribution)
        {
            //update individual player progress
            var completion = new Completion(advancement.Player, advancement.Timestamp);
            contribution.Advancements.Add(advancement.Id, completion);

            //update combined progress
            if (!state.Advancements.TryGetValue(advancement.Id, out Completion globalFirst)
                || globalFirst.After(completion.Timestamp))
            { 
                state.Advancements[advancement.Id] = completion;         
            }
        }

        private void UpdateCriteria(AdvancementCompletion advancement, WorldState state, Contribution contribution)
        {
            foreach (KeyValuePair<string, DateTime> criterion in advancement.CriteriaTimestamps)
            {
                //update individual player progress
                var completion = new Completion(advancement.Player, criterion.Value);
                contribution.Criteria.Add(criterion.Key, completion);

                //update combined progress
                if (!state.Criteria.TryGetValue(criterion.Key, out Completion globalFirst)
                    || globalFirst.After(completion.Timestamp))
                {
                    state.Criteria[criterion.Key] = completion;
                }
            }
        }
    }
}
