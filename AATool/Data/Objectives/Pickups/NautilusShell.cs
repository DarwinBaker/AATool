using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AATool.Data.Categories;

namespace AATool.Data.Objectives.Pickups
{
    class NautilusShell : Pickup
    {
        public const string ItemId = "minecraft:nautilus_shell";
        private const string HDWGH = "minecraft:nether/all_effects";
        private const string Conduit = "minecraft:conduit";

        public NautilusShell(XmlNode node) : base(node) 
        {
            if (Tracker.Category is AllBlocks)
                this.Icon = "shell_and_conduit";
        }

        protected override void HandleCompletionOverrides()
        {
            if (Tracker.Category is AllBlocks)
            {
                Tracker.TryGetBlock(Conduit, out Block conduit);
                this.CompletionOverride = conduit?.CompletedByAnyone() is true;
            }
            else
            {
                //check if hdwgh complete
                Tracker.TryGetAdvancement(HDWGH, out Advancement hdwgh);
                this.CompletionOverride = hdwgh?.CompletedByAnyone() is true;
            }
        }

        protected override void UpdateLongStatus()
        {
            //show if hdwgh is complete 
            if (this.CompletionOverride)
                this.FullStatus = Tracker.Category is AllBlocks ? "Conduit Placed" : "HDWGH Complete";
            else
                base.UpdateLongStatus();
        }
    }
}
