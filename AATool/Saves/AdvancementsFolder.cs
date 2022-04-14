using System;
using System.Collections.Generic;
using AATool.Data.Objectives;
using AATool.Net;
using Newtonsoft.Json.Linq;

namespace AATool.Saves
{
    public class AdvancementCompletion
    {
        public string Id;
        public DateTime First;
        public Dictionary<Uuid, DateTime> Players;
    }

    public class AdvancementsFolder : JsonFolder
    {
        public bool TryGetAdvancementCompletions(string advancementId, out Dictionary<Uuid, DateTime> completions)
        {
            bool completed = false;
            completions = new ();

            //check for completion by each player in the world
            foreach (KeyValuePair<Uuid, JsonStream> json in this.Players)
            {
                dynamic token = json.Value[advancementId];
                if (token?["done"] != true)
                    continue;

                var adv = new AdvancementCompletion();
                DateTime timestamp = default;
                foreach (IEnumerable<JToken> criterion in token?["criteria"]?.Children())
                {
                    string[] dateTokens = criterion.ToString()?.Split('\"');
                    string date = dateTokens.Length > 3 ? dateTokens[3] : null;
                    DateTime.TryParse(date, out timestamp);
                }
                completions.Add(json.Key, timestamp);
                completed = true;
            }
            return completed;
        }

        public Dictionary<Uuid, HashSet<(string adv, string crit)>> GetAllCriteriaCompletions(Advancement advancement)
        {
            var completions = new Dictionary<Uuid, HashSet<(string, string)>>();
            foreach (KeyValuePair<Uuid, JsonStream> json in this.Players)
                completions[json.Key] = this.GetCompletedCriteriaInFile(advancement, json.Value);
            return completions;
        }

        private HashSet<(string adv, string crit)> GetCompletedCriteriaInFile(Advancement advancement, JsonStream json)
        {
            var completed = new HashSet<(string, string)>();
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
                        completed.Add((advancement.Id, criterion));
                }
            }
            return completed;
        }
    }
}
