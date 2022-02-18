using System;
using System.Collections.Generic;
using AATool.Data;
using AATool.Data.Objectives;
using AATool.Net;

namespace AATool.Saves
{
    public class AdvancementsFolder : JsonFolder
    {
        public bool TryGetAdvancementCompletionFor(string advancement, out List<Uuid> players)
        {
            bool completed = false;
            players = new List<Uuid>();
            foreach (KeyValuePair<Uuid, JsonStream> json in this.Players)
            {
                if (IsCompletedInFile(advancement, json.Value))
                {
                    players.Add(json.Key);
                    completed = true;
                }
            }
            return completed;
        }

        private static bool IsCompletedInFile(string advancement, JsonStream json) =>
            json[advancement]?["done"] == true;


        public Dictionary<Uuid, HashSet<(string adv, string crit)>> GetAllCriteriaCompletions(Advancement advancement)
        {
            //return
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
