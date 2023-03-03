using System.Collections.Generic;
using Newtonsoft.Json;

namespace AATool.Data.Progress
{
    [JsonObject]
    public class NetworkCriteriaSet
    {
        [JsonProperty] public readonly string Advancement;
        [JsonProperty] public readonly HashSet<string> List;

        public NetworkCriteriaSet(string advancement)
        {
            this.Advancement = advancement;
            this.List = new HashSet<string>();
        }
    }
}
