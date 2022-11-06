using System;
using System.Collections.Generic;
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
        }

        public void UpdateState(ProgressState progress)
        {
            foreach (ComplexObjective objective in this.AllByName.Values)
                objective.UpdateState(progress);
        }
    }
}
