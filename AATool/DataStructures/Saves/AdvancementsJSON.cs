using System.Collections.Generic;

namespace AATool.DataStructures
{
    public class AdvancementsJSON : SaveJSON
    {
        public bool IsCompleted(string advancement) => json?[advancement]?["done"] == true;

        public AdvancementsJSON()
        {
            folderName = "advancements";
        }

        public HashSet<string> GetCompletedCriteriaFor(Advancement advancement)
        {
            var completed = new HashSet<string>();
            dynamic criteria = json?[advancement.ID]?["criteria"];
            if (criteria != null)
            {
                //advancement has criteria. add them
                foreach (string line in criteria.ToString().Split('\n'))
                {
                    string[] tokens = line.Trim().Split('"');
                    if (tokens.Length > 1)
                    {
                        string criterion = tokens[1];
                        if (advancement.Criteria.ContainsKey(criterion))
                            completed.Add(criterion);
                    }
                }
            }
            return completed;
        }
    }
}
