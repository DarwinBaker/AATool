using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using AATool.Data.Objectives.Complex;
using AATool.Data.Progress;

namespace AATool.Data.Objectives
{
    public class ComplexObjectiveManifest : IManifest
    {
        public Dictionary<string, ComplexObjective> AllByName { get; private set; }

        public bool TryGet(string typeName, out ComplexObjective objective) => 
            this.AllByName.TryGetValue(typeName?.ToLower() ?? string.Empty, out objective);

        public ComplexObjectiveManifest()
        {
            this.AllByName = new();
            this.RefreshObjectives();
        }

        public void ClearObjectives() => this.AllByName.Clear();

        public void RefreshObjectives()
        {
            this.ClearObjectives();
            foreach (string type in ComplexObjective.Types.Keys)
            {
                if (ComplexObjective.TryCreateInstance(type, out ComplexObjective objective))
                    this.AllByName.Add(type, objective);
            }

            //misc items
            this.AddPickup("minecraft:nether_wart", "Wart", 3);
            this.AddPickup("minecraft:ghast_tear", "/\04\0Tears", 4);
            this.AddPickup("minecraft:pufferfish", "/\02\0Puffers", 2);
            this.AddPickup("minecraft:azure_bluet", "Azure Bluet", 1);
            this.AddPickup("minecraft:rabbit_foot", "Rabbit's Foot", 1);
            this.AddPickup("minecraft:fermented_spider_eye", "Fermented Eye", 1);
            this.AddPickup("minecraft:pottery_sherd", "Pottery Shard", 4);
            this.AddPickup("minecraft:sniffer_egg", "Sniffer Egg", 1);
        }

        private void AddPickup(string id, string name, int required)
        {
            this.AllByName.Add(id, new Pickup(id, name, required));
        }

        public void UpdateState(ProgressState progress)
        {
            foreach (ComplexObjective objective in this.AllByName.Values)
                objective.UpdateState(progress);
        }

        public void UpdateDynamicIcons(Time time)
        {
            if (AllByName.TryGetValue(nameof(ArmorTrims).ToLower(), out ComplexObjective trims))
                (trims as ArmorTrims)?.UpdateDynamicIcon(time);
        }
    }
}
