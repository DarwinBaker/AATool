using System;
using System.Collections.Generic;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using System.Linq;
using AATool.Data.Progress;
using AATool.Net;
using Microsoft.Xna.Framework;

namespace AATool.Saves
{
    public class StatisticsFolder : JsonFolder
    {
        private const double TicksPerSecond = 20.0;

        public TimeSpan GetInGameTime(JsonStream json)
        {
            if (json is null)
                return TimeSpan.Zero;

            //1.17+
            long ticks = json["stats"]?["minecraft:custom"]?["minecraft:play_time"]?.Value ?? 0;

            //1.12 - 1.16
            if (ticks is 0)
                ticks = json["stats"]?["minecraft:custom"]?["minecraft:play_one_minute"]?.Value ?? 0;

            //pre-1.12
            if (ticks is 0)
                ticks = json["stat.playOneMinute"]?.Value ?? 0;

            return TimeSpan.FromSeconds(ticks / TicksPerSecond);
        }

        public int GetKilometersFlown(JsonStream json)
        {
            double cm = (int)(json?["stats"]?["minecraft:custom"]?["minecraft:aviate_one_cm"]?.Value ?? 0);
            return (int)Math.Round(cm / 100 / 1000);
        }

        public int GetCustomStat(JsonStream json, string name) =>
            (int)(json?["stats"]?["minecraft:custom"]?[name]?.Value ?? 0);

        protected override void Update(JsonStream json, WorldState state, Contribution contribution)
        {
            this.UpdateGlobalStats(json, state);
            this.UpdateCounts("minecraft:picked_up", "pickup", json, state.PickupCounts, contribution.PickupCounts);
            this.UpdateCounts("minecraft:dropped", "drop", json, state.DropCounts, contribution.DropCounts);
            this.UpdateCounts("minecraft:mined", "mineBlock", json, state.MineCounts, contribution.MineCounts);
            this.UpdateCounts("minecraft:crafted", string.Empty, json, state.CraftCounts, contribution.CraftCounts);
            this.UpdateCounts("minecraft:used", "useItem", json, state.UseCounts, contribution.UseCounts);
            this.UpdateCounts("minecraft:killed", "killEntity", json, state.KillCounts, contribution.KillCounts);
        }

        private void UpdateGlobalStats(JsonStream json, WorldState state)
        {
            //use longest igt of all applicable players
            TimeSpan igt = this.GetInGameTime(json);
            if (igt > state.InGameTime)
                state.InGameTime = igt;

            state.KilometersFlown += this.GetKilometersFlown(json);
            state.ItemsEnchanted += this.GetCustomStat(json, "minecraft:enchant_item");
            state.SaveAndQuits += this.GetCustomStat(json, "minecraft:leave_game");
            state.DamageDealt += this.GetCustomStat(json, "minecraft:damage_dealt");
            state.DamageTaken += this.GetCustomStat(json, "minecraft:damage_taken");
            state.Sleeps += this.GetCustomStat(json, "minecraft:sleep_in_bed");
            state.Deaths += this.GetCustomStat(json, "minecraft:deaths");
            state.Jumps += this.GetCustomStat(json, "minecraft:jump");
        }

        private void UpdateCounts(string modernKey, string oldKey, JsonStream json,
            Dictionary<string, int> globalCounts, Dictionary<string, int> playerCounts)
        {
            dynamic modernCounts = json?["stats"]?[modernKey];
            if (modernCounts is not null)
            {
                //count how many of each item this player has picked up
                foreach (dynamic pickup in modernCounts)
                {
                    if (pickup.Name is not string name)
                        continue;
                    if (!int.TryParse(pickup.Value?.ToString(), out int count))
                        continue;

                    globalCounts.TryGetValue(name, out int total);
                    globalCounts[name] = total + count;
                    playerCounts.TryGetValue(name, out int current);
                    playerCounts[name] = current + count;
                }
            }
            else if (!string.IsNullOrEmpty(oldKey))
            {
                //handle pre-1.12 formatting
                Dictionary<string, int> oldVersionCounts = this.GetOldVersionCounts(oldKey, json.ToString());
                foreach (KeyValuePair<string, int> pickup in oldVersionCounts)
                {
                    globalCounts.TryGetValue(pickup.Key, out int total);
                    globalCounts[pickup.Key] = total + pickup.Value;
                    playerCounts.TryGetValue(pickup.Key, out int current);
                    playerCounts[pickup.Key] = current + pickup.Value;
                }
            }
        }

        private Dictionary<string, int> GetOldVersionCounts(string group, string json)
        {
            var list = new Dictionary<string, int>();
            string prefix = $"stat.{group}.";
            string jsonContent = json.ToString();
            int index = 0;
            int pickupNameStart;
            do
            {
                if (index < 0)
                    break;

                pickupNameStart = jsonContent.IndexOf(prefix, index);
                if (pickupNameStart > -1)
                {
                    int valueStart = jsonContent.IndexOf("\":", pickupNameStart);
                    int valueEnd = jsonContent.IndexOf(",", pickupNameStart);
                    int valueLength = valueEnd - valueStart - 2;

                    if (valueLength > 0)
                    {
                        string name = jsonContent.Substring(pickupNameStart + prefix.Length, valueStart - pickupNameStart - prefix.Length);
                        if (int.TryParse(jsonContent.Substring(valueStart + 2, valueLength), out int count))
                            list[name] = count;
                    }
                    index = valueEnd; 
                }
            }
            while (pickupNameStart > -1);
            return list;
        } 
    }
}