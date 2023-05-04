using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    internal class NetheriteUpgrade : ComplexObjective
    {
        public const string Recipe = "minecraft:recipes/misc/netherite_upgrade_smithing_template";

        public bool Obtained { get; private set; }

        public NetheriteUpgrade() : base()
        {
            this.Icon = "upgrade_netherite";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.CompletionOverride = progress.Recipes.ContainsKey(Recipe);
        }

        protected override void ClearAdvancedState()
        {
        }

        protected override string GetLongStatus() => "Netherite Up";
        protected override string GetShortStatus() => "Netherite Up";
    }
}
