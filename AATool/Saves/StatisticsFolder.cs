using AATool.Net;
using AATool.Settings;
using System;
using System.Collections.Generic;

namespace AATool.Saves
{
    public class StatisticsFolder : JSONFolder
    {
        private const int TICKS_PER_SECOND = 20;

        public TimeSpan GetInGameTime()
        {
            int longestPlayTimeInWorld = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                int ticks = (int)(json?["stats"]
                    ?["minecraft:custom"]
                    ?["minecraft:play_time"]
                    ?.Value ?? 0);

                if (ticks is 0)
                {
                    ticks = (int)(json?["stats"]
                    ?["minecraft:custom"]
                    ?["minecraft:play_one_minute"]
                    ?.Value ?? 0);
                }

                if (ticks > longestPlayTimeInWorld)
                    longestPlayTimeInWorld = ticks;
            }
            return TimeSpan.FromSeconds((int)((double)longestPlayTimeInWorld / TICKS_PER_SECOND));
        }

        public int GetTotalJumps()
        {
            int total = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                total += (int)(json?["stats"]?["minecraft:custom"]
                    ?["minecraft:jump"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetTotalSleeps()
        {
            int total = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                total += (int)(json?["stats"]?["minecraft:custom"]
                    ?["minecraft:sleep_in_bed"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetTotalDamageTaken()
        {
            int total = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                total += (int)(json?["stats"]?["minecraft:custom"]
                    ?["minecraft:damage_taken"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetTotalDamageDealt()
        {
            int total = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                total += (int)(json?["stats"]?["minecraft:custom"]
                    ?["minecraft:damage_dealt"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetTotalKilometersFlown()
        {
            double cm = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                cm += (int)(json?["stats"]
                    ?["minecraft:custom"]
                    ?["minecraft:aviate_one_cm"]
                    ?.Value ?? 0);
            }
            return (int)Math.Round(cm / 1000 / 1000);
        }

        public int GetTotalDeaths()
        {
            int total = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                total += (int)(json?["stats"]
                    ?["minecraft:custom"]
                    ?["	minecraft:deaths"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetTotalSaveAndQuits()
        {
            int total = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                total += (int)(json?["stats"]
                    ?["minecraft:custom"]
                    ?["	minecraft:leave_game"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetKillCount(string mob)
        {
            int total = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                total += (int)(json?["stats"]
                    ?["minecraft:killed"]
                    ?["minecraft:" + mob]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetUsedCount(string mob)
        {
            int total = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                total += (int)(json?["stats"]
                    ?["minecraft:used"]
                    ?["minecraft:" + mob]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetMinedCount(string block)
        {
            int total = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                total += (int)(json?["stats"]
                    ?["minecraft:mined"]
                    ?["minecraft:" + block]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetItemCountsFor(string item, out Dictionary<Uuid, int> playerItemCounts)
        {
            //count how many of this item everybody has picked up
            int total = 0;
            playerItemCounts = new Dictionary<Uuid, int>();
            foreach (KeyValuePair<Uuid, JSONStream> json in this.Files)
            {
                int count = Config.IsPostExplorationUpdate
                    ? (int)(json.Value?["stats"]?["minecraft:picked_up"]?[item]?.Value ?? 0)
                    : (int)(json.Value?["stat.pickup." + item]?.Value ?? 0);
                playerItemCounts[json.Key] = count;
                total += count;
            }
            return total;
        }

        public int GetItemsEnchanted()
        {
            int total = 0;
            foreach (JSONStream json in this.Files.Values)
            {
                total += (int)(json?["stats"]?["minecraft:custom"]
                    ?["minecraft:enchant_item"]
                    ?.Value ?? 0);
            }
            return total;
        }
    }
}