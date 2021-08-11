using System;
using System.Collections.Generic;
using AATool.Data;
using AATool.Net;

namespace AATool.Saves
{
    public class AchievementsFolder : JSONFolder
    {
        private static bool IsCompleted(string achievement, JSONStream json)
        {
            if (int.TryParse(json?[achievement]?.ToString(), out int value))
                return value > 0;
            else if (int.TryParse(json?[achievement]?["value"]?.ToString(), out value))
                return value > 0;
            return false;
        }

        public bool GetAchievementCompletionFor(string achievement, out List<Uuid> players)
        {
            bool completed = false;
            players = new ();
            foreach (KeyValuePair<Uuid, JSONStream> json in this.Files)
            {
                if (IsCompleted(achievement, json.Value))
                {
                    players.Add(json.Key);
                    completed = true;
                }
            }
            return completed;
        }

        public Dictionary<Uuid, HashSet<(string, string)>> GetCompletedCriteriaByPlayer(Achievement achievement)
        {
            Dictionary<Uuid, HashSet<(string, string)>> playerCompletions = new ();
            foreach (KeyValuePair<Uuid, JSONStream> json in this.Files)
                playerCompletions[json.Key] = this.GetCompletedCriteria(achievement, json.Value);
            return playerCompletions;
        }

        private HashSet<(string, string)> GetCompletedCriteria(Advancement advancement, JSONStream json)
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
