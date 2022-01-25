using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AATool.Data.Objectives.Pickups
{
    class WitherSkull : Pickup
    {
        public const string ItemId = "minecraft:wither_skeleton_skull";
        private const string MonsterHunter = "minecraft:adventure/kill_all_mobs";
        private const string Wither = "minecraft:wither";

        public WitherSkull(XmlNode node) : base(node) { }

        protected override void HandleCompletionOverrides()
        {
            //check if wither has been killed
            Tracker.TryGetCriterion(MonsterHunter, Wither, out Criterion killWither);
            this.CompletionOverride = killWither?.CompletedByDesignated() is true;
        }

        protected override void UpdateLongStatus()
        {
            //show wither killed if applicable
            if (this.CompletionOverride)
                this.FullStatus = "Wither Has Been Killed";
            else
                base.UpdateLongStatus();
        }
    }
}