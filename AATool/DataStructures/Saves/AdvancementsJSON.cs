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

        public HashSet<string> GetCompletedCriteriaFor(string advancement)
        {
            var completed = new HashSet<string>();
            dynamic criteria = json?[advancement]?["criteria"];
            if (criteria != null)
            {
                //advancement has criteria. add them
                foreach (string line in criteria.ToString().Split('\n'))
                {
                    string[] values = line.Trim().Split('"');
                    if (values.Length > 1)
                        completed.Add(values[1]);
                }
            }
            return completed;
        }
    }
}
