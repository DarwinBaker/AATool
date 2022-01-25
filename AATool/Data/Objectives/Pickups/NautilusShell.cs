using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AATool.Data.Objectives.Pickups
{
    class NautilusShell : Pickup
    {
        public const string ItemId = "minecraft:nautilus_shell";
        private const string HDWGH = "minecraft:nether/all_effects";

        public NautilusShell(XmlNode node) : base(node) { }

        protected override void HandleCompletionOverrides()
        {
            //check if egap has been eaten
            Tracker.TryGetAdvancement(HDWGH, out Advancement hdwgh);
            this.CompletionOverride = hdwgh?.CompletedByAnyone() is true;
        }

        protected override void UpdateLongStatus()
        {
            //show if hdwgh is complete 
            if (this.CompletionOverride)
                this.FullStatus = "HDWGH Complete";
            else
                base.UpdateLongStatus();
        }

        protected override void UpdateShortStatus()
        {
            if (this.CompletionOverride)
                this.ShortStatus = "Eaten";
            else
                base.UpdateShortStatus();
        }
    }
}
