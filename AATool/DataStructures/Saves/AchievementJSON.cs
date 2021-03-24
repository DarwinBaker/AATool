using System.Collections.Generic;

namespace AATool.DataStructures
{
    public class AchievementJSON : SaveJSON
    {
        public bool IsCompleted(string achievement)
        {
            int value = 0;
            if (int.TryParse(json?[achievement]?.ToString(), out value))
                return value > 0;
            else if (int.TryParse(json?[achievement]?["value"]?.ToString(), out value))
                return value > 0;
            return false;
        }

        public AchievementJSON()
        {
            folderName = "stats";
        }

        public HashSet<string> GetCompletedCriteriaFor(Achievement achievement)
        {
            var completed = new HashSet<string>();
            dynamic criteria = json?[achievement.ID]?["progress"];
            if (criteria != null)
            {
                //achievement has criteria; add them
                foreach (string line in criteria.ToString().Split('\n'))
                {
                    string[] tokens = line.Trim().Split('"');
                    if (tokens.Length > 1)
                    {
                        string criterion = tokens[1];
                        if (achievement.Criteria.ContainsKey(criterion))
                            completed.Add(criterion);
                    }
                }
            }
            return completed;
        }
    }
}
