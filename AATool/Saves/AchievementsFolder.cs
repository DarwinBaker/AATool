using System;
using System.Collections.Generic;
using AATool.Data;
using AATool.Data.Objectives;
using AATool.Net;

namespace AATool.Saves
{
    public class AchievementsFolder : JsonFolder
    {
        private static bool IsCompleted(string achievement, JsonStream json)
        {
            if (int.TryParse(json?[achievement]?.ToString(), out int value))
                return value > 0;
            else if (int.TryParse(json?[achievement]?["value"]?.ToString(), out value))
                return value > 0;
            return false;
        }

        public bool TryGetAchievementCompletionFor(string achievement, out List<Uuid> players)
        {
            bool completed = false;
            players = new ();
            foreach (KeyValuePair<Uuid, JsonStream> json in this.Players)
            {
                if (IsCompleted(achievement, json.Value))
                {
                    players.Add(json.Key);
                    completed = true;
                }
            }
            return completed;
        }

        public Dictionary<Uuid, HashSet<(string adv, string crit)>> GetAllCriteriaCompletions(Advancement advancement)
        {
            //return
            var completions = new Dictionary<Uuid, HashSet<(string, string)>>();
            foreach (KeyValuePair<Uuid, JsonStream> json in this.Players)
                completions[json.Key] = this.GetCompletedCriteria(advancement, json.Value);
            return completions;
        }

        private HashSet<(string, string)> GetCompletedCriteria(Advancement advancement, JsonStream json)
        {
            HashSet<(string, string)> completed = new ();
            dynamic criteriaList = json?[advancement.Id]?["progress"];
            if (criteriaList is not null)
            {
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
            }
            return completed;
        }
    }
}
