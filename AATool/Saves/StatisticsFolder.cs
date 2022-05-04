using System;
using System.Collections.Generic;
using AATool.Net;

namespace AATool.Saves
{
    public class StatisticsFolder : JsonFolder
    {
        private const double TicksPerSecond = 20.0;

        public TimeSpan GetInGameTime()
        {
            long mostTicks = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                if (json is null)
                    continue;

                //1.17+
                long ticks = json["stats"]?["minecraft:custom"]?["minecraft:play_time"]?.Value ?? 0;

                //1.12 - 1.16
                if (ticks is 0)
                    ticks = json["stats"]?["minecraft:custom"]?["minecraft:play_one_minute"]?.Value ?? 0;

                //pre-1.12
                if (ticks is 0)
                    ticks = json["stat.playOneMinute"]?.Value ?? 0;

                if (ticks > mostTicks)
                    mostTicks = ticks;
            }
            return TimeSpan.FromSeconds(mostTicks / TicksPerSecond);
        }

        public bool TryGetUseCount(string id, out List<Uuid> players)
        {
            players = new ();
            foreach (KeyValuePair<Uuid, JsonStream> json in this.Players)
            {
                int count = (int)(json.Value
                    ?["stats"]
                    ?["minecraft:used"]
                    ?[id]
                    ?.Value ?? 0);

                if (count > 0)
                    players.Add(json.Key);
            }
            return players.Count > 0;
        }

        public int GetTotalJumps()
        {
            int total = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                total += (int)(json
                    ?["stats"]
                    ?["minecraft:custom"]
                    ?["minecraft:jump"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetTotalSleeps()
        {
            int total = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                total += (int)(json
                    ?["stats"]
                    ?["minecraft:custom"]
                    ?["minecraft:sleep_in_bed"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetTotalDamageTaken()
        {
            int total = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                total += (int)(json
                    ?["stats"]
                    ?["minecraft:custom"]
                    ?["minecraft:damage_taken"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetTotalDamageDealt()
        {
            int total = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                total += (int)(json
                    ?["stats"]
                    ?["minecraft:custom"]
                    ?["minecraft:damage_dealt"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetTotalKilometersFlown()
        {
            double cm = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                cm += (int)(json
                    ?["stats"]
                    ?["minecraft:custom"]
                    ?["minecraft:aviate_one_cm"]
                    ?.Value ?? 0);
            }
            return (int)Math.Round(cm / 1000 / 1000);
        }

        public int GetTotalDeaths()
        {
            int total = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                total += (int)(json
                    ?["stats"]
                    ?["minecraft:custom"]
                    ?["minecraft:deaths"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int GetTotalSaveAndQuits()
        {
            int total = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                total += (int)(json?["stats"]
                    ?["minecraft:custom"]
                    ?["	minecraft:leave_game"]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int KillsOf(string mob)
        {
            int total = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                total += (int)(json?["stats"]
                    ?["minecraft:killed"]
                    ?["minecraft:" + mob]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int TimesUsed(string id)
        {
            int total = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                total += (int)(json?["stats"]
                    ?["minecraft:used"]
                    ?["minecraft:" + id]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int TimesMined(string block)
        {
            int total = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                total += (int)(json?["stats"]
                    ?["minecraft:mined"]
                    ?["minecraft:" + block]
                    ?.Value ?? 0);
            }
            return total;
        }

        public int TimesPickedUp(string item, out Dictionary<Uuid, int> playerItemCounts)
        {
            //count how many of this item everybody has picked up
            int total = 0;
            playerItemCounts = new Dictionary<Uuid, int>();
            foreach (KeyValuePair<Uuid, JsonStream> json in this.Players)
            {
                int count = 0;
                count = (int)(json.Value?["stats"]?["minecraft:picked_up"]?[item]?.Value ?? 0);

                //handle pre-1.12 formatting
                if (count is 0)
                    count = (int)(json.Value?["stat.pickup." + item]?.Value ?? 0);

                playerItemCounts[json.Key] = count;
                total += count;
            }
            return total;
        }

        public int TimesDropped(string item, out Dictionary<Uuid, int> playerItemCounts)
        {
            //count how many of this item everybody has picked up
            int total = 0;
            playerItemCounts = new Dictionary<Uuid, int>();
            foreach (KeyValuePair<Uuid, JsonStream> json in this.Players)
            {
                int count = 0;
                count = (int)(json.Value?["stats"]?["minecraft:dropped"]?[item]?.Value ?? 0);

                //handle pre-1.12 formatting
                if (count is 0)
                    count = (int)(json.Value?["stat.drop." + item]?.Value ?? 0);

                playerItemCounts[json.Key] = count;
                total += count;
            }
            return total;
        }

        public int GetItemsEnchanted()
        {
            int total = 0;
            foreach (JsonStream json in this.Players.Values)
            {
                total += (int)(json?["stats"]?["minecraft:custom"]
                    ?["minecraft:enchant_item"]
                    ?.Value ?? 0);
            }
            return total;
        }
    }
}