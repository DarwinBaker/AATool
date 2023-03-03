using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using AATool.Data.Categories;
using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data.Objectives.Complex
{
    class Bees : ComplexObjective
    {
        public const string BlockId = "minecraft:bee_nest";

        private const string TotalBeelocation = "minecraft:husbandry/silk_touch_nest";
        private const string BeeOurGuest = "minecraft:husbandry/safely_harvest_honey";
        private const string BalancedDiet = "minecraft:husbandry/balanced_diet";
        private const string TwoByTwo = "minecraft:husbandry/bred_all_animals";
        private const string StickySituation = "minecraft:adventure/honey_block_slide";
        private const string HoneyBottle = "honey_bottle";
        private const string Bee = "minecraft:bee";

        private const string HoneyBlock = "minecraft:honey_block";
        private const string HoneyCombBlock = "minecraft:honeycomb_block";

        private const string EmptyTexture = "bee_nest";
        private const string FullTexture = "bee_nest_full";

        private static readonly Version CavesAndCliffsPartOne = new ("1.17");

        private int estimatedCount;
        private int estimatedPlaced;

        //all advancements
        private bool totalBeelocation;
        private bool beeOurGuest;
        private bool stickySituation;
        private bool drinkHoney;
        private bool breedBees;

        private bool balancedDiet;
        private bool twoByTwo;

        //all blocks
        private bool honeyBlockPlaced;
        private bool honeycombBlockPlaced;
        private bool allCandlesPlaced;
        private bool allWaxedCopperPlaced;

        private readonly List<string> remainingObjectives = new List<string>();
        private bool doneWithBees;

        public static readonly string[] AllWaxedCopper = new [] {
            "minecraft:waxed_copper_block",
            "minecraft:waxed_cut_copper",
            "minecraft:waxed_cut_copper_stairs",
            "minecraft:waxed_cut_copper_slab",
            "minecraft:waxed_exposed_copper",
            "minecraft:waxed_exposed_cut_copper",
            "minecraft:waxed_exposed_cut_copper_stairs",
            "minecraft:waxed_exposed_cut_copper_slab",
            "minecraft:waxed_weathered_copper",
            "minecraft:waxed_weathered_cut_copper",
            "minecraft:waxed_weathered_cut_copper_stairs",
            "minecraft:waxed_weathered_cut_copper_slab",
            "minecraft:waxed_oxidized_copper",
            "minecraft:waxed_oxidized_cut_copper",
            "minecraft:waxed_oxidized_cut_copper_stairs",
            "minecraft:waxed_oxidized_cut_copper_slab",
        };

        public static readonly string[] AllCandles = new [] {
            "minecraft:white_candle",
            "minecraft:red_candle",
            "minecraft:orange_candle",
            "minecraft:yellow_candle",
            "minecraft:lime_candle",
            "minecraft:green_candle",
            "minecraft:cyan_candle",
            "minecraft:light_blue_candle",
            "minecraft:blue_candle",
            "minecraft:purple_candle",
            "minecraft:magenta_candle",
            "minecraft:pink_candle",
            "minecraft:brown_candle",
            "minecraft:light_gray_candle",
            "minecraft:gray_candle",
            "minecraft:black_candle",
            "minecraft:candle",
        };

        public Bees() : base() 
        {
            this.Name = "Bees";
        }

        private bool CopperAndCandlesAdded => Version.TryParse(Tracker.Category.CurrentVersion,
            out Version current) && current >= CavesAndCliffsPartOne;

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.estimatedCount = progress.TimesPickedUp(BlockId)
                - progress.TimesDropped(BlockId)
                - progress.TimesUsed(BlockId);

            this.estimatedCount = Math.Max(this.estimatedCount, 0);
            this.estimatedPlaced = progress.TimesUsed(BlockId);

            if (Tracker.Category is AllBlocks)
            {
                //check blocks
                this.honeyBlockPlaced = progress.WasUsed(HoneyBlock);
                this.honeycombBlockPlaced = progress.WasUsed(HoneyCombBlock);

                this.allWaxedCopperPlaced = this.AllWaxedCopperPlaced(progress);
                this.allCandlesPlaced = this.AllCandlesPlaced(progress);
            }
            else
            {
                //check advancements
                this.totalBeelocation = progress.AdvancementCompleted(TotalBeelocation);
                this.beeOurGuest = progress.AdvancementCompleted(BeeOurGuest);
                this.stickySituation = progress.AdvancementCompleted(StickySituation);

                this.balancedDiet = progress.AdvancementCompleted(BalancedDiet);
                this.twoByTwo = progress.AdvancementCompleted(TwoByTwo);

                this.drinkHoney = progress.CriterionCompleted(BalancedDiet, HoneyBottle);
                this.breedBees = progress.CriterionCompleted(TwoByTwo, Bee);
            }

            this.BuildRemainingObjectiveList();

            this.CompletionOverride = this.doneWithBees = !this.remainingObjectives.Any();
        }

        private void BuildRemainingObjectiveList()
        {
            this.remainingObjectives.Clear();
            if (Tracker.Category is AllBlocks)
            {
                //check blocks
                if (!this.honeyBlockPlaced)
                    this.remainingObjectives.Add("Still\0Needs\nHoney\0Block");
                if (!this.honeycombBlockPlaced)
                    this.remainingObjectives.Add("Still\0Needs\nHoneycomb");
                if (this.CopperAndCandlesAdded && !this.allCandlesPlaced)
                    this.remainingObjectives.Add("Still\0Needs\nCandles");
                if (this.CopperAndCandlesAdded && !this.allWaxedCopperPlaced)
                    this.remainingObjectives.Add("Still\0Needs\nWaxed\0Copper");
            }
            else
            {
                //check advancements
                if (!this.totalBeelocation)
                    this.remainingObjectives.Add("Still\0Needs\nBeelocation");
                if (!this.beeOurGuest)
                    this.remainingObjectives.Add("Must\0Harvest\nHoney");
                if (!this.stickySituation)
                    this.remainingObjectives.Add("Still\0Needs\nHoney\0Block");
                if (!this.drinkHoney && !this.balancedDiet)
                    this.remainingObjectives.Add("Needs\0To\nDrink\0Honey");
                if (!this.breedBees && !this.twoByTwo)
                    this.remainingObjectives.Add("Needs\0To\nBreed\0Bees");
            }
        }

        private bool AllWaxedCopperPlaced(ProgressState progress)
        {
            foreach (string copperVariant in AllWaxedCopper)
            {
                if (!progress.WasUsed(copperVariant))
                    return false;
            }
            return true;
        }

        private bool AllCandlesPlaced(ProgressState progress)
        {
            foreach (string candleVariant in AllCandles)
            {
                if (!progress.WasUsed(candleVariant))
                    return false;
            }
            return true;
        }

        protected override void ClearAdvancedState()
        {
            this.remainingObjectives.Clear();

            this.estimatedCount = 0;
            this.estimatedPlaced = 0;

            this.totalBeelocation = false;
            this.beeOurGuest = false;
            this.stickySituation = false;
            this.drinkHoney = false;

            this.balancedDiet = false;
            this.twoByTwo = false;

            this.honeyBlockPlaced = false;
            this.honeycombBlockPlaced = false;
            this.allCandlesPlaced = false;
            this.allWaxedCopperPlaced = false;

            this.doneWithBees = false;
        }

        protected override string GetShortStatus()
            => this.doneWithBees ? "Done" : $"Hives:\0{this.estimatedCount}";

        protected override string GetLongStatus()
        {
            if (this.doneWithBees)
                return "Done\0With\nBees";

            if (this.remainingObjectives.Count is 1)
                return $"{this.remainingObjectives[0]}";

            int count = this.estimatedPlaced > 0 
                ? this.estimatedPlaced 
                : this.estimatedCount;
            string name = count is 1 ? "Hive" : "Hives";

            return this.estimatedPlaced > 0 
                ? $"{this.estimatedPlaced}\0{name}\nPlaced" 
                : $"{this.estimatedCount}\0{name}\nCollected";
        }

        protected override string GetCurrentIcon()
        {
            return this.CompletionOverride 
                ? "bee_nest_full_pickup" 
                : "bee_nest_pickup";
        }
    }
}
