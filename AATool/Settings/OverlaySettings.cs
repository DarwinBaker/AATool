using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class OverlaySettings : SettingsGroup
    {
        public static OverlaySettings Instance = new OverlaySettings();

        private const string ENABLED        = "enabled";
        private const string SHOW_LABELS      = "show_labels";
        private const string SHOW_OVERVIEW  = "show_overview";
        private const string SHOW_CRITERIA  = "show_criteria";
        private const string SHOW_COUNTS    = "show_counts";
        private const string ONLY_FAVORITES = "only_chosen";
        private const string HIDE_COMPLETED = "hide_completed";
        private const string SPEED          = "speed";
        private const string WIDTH          = "width";
        private const string BACK_COLOR     = "back_color";
        private const string TEXT_COLOR     = "text_color";
        private const string FAVORITES      = "favorites";

        public bool Enabled                 { get => (bool)Entries[ENABLED];                set => Entries[ENABLED]         = value; }
        public bool OnlyShowFavorites       { get => (bool)Entries[ONLY_FAVORITES];         set => Entries[ONLY_FAVORITES]  = value; }
        public bool HideCompleted           { get => (bool)Entries[HIDE_COMPLETED];         set => Entries[HIDE_COMPLETED]  = value; }
        public bool ShowLabels              { get => (bool)Entries[SHOW_LABELS];            set => Entries[SHOW_LABELS]       = value; }
        public bool ShowCriteria            { get => (bool)Entries[SHOW_CRITERIA];          set => Entries[SHOW_CRITERIA]   = value; }
        public bool ShowCounts              { get => (bool)Entries[SHOW_COUNTS];            set => Entries[SHOW_COUNTS]     = value; }
        public bool ShowOverview            { get => (bool)Entries[SHOW_OVERVIEW];          set => Entries[SHOW_OVERVIEW]   = value; }
        public int Speed                    { get => (int)Entries[SPEED];                   set => Entries[SPEED]           = value; }
        public int Width                    { get => (int)Entries[WIDTH];                   set => Entries[WIDTH]           = value; }
        public Color BackColor              { get => (Color)Entries[BACK_COLOR];            set => Entries[BACK_COLOR]      = value; }
        public Color TextColor              { get => (Color)Entries[TEXT_COLOR];            set => Entries[TEXT_COLOR]      = value; }
        public HashSet<string> Favorites    { get => (HashSet<string>)Entries[FAVORITES];   set => Entries[FAVORITES]       = value; }

        public readonly HashSet<string> DefaultFavorites = new HashSet<string>()
        {
            "minecraft:story/cure_zombie_villager",
            "minecraft:nether/netherite_armor",
            "minecraft:nether/use_lodestone",
            "minecraft:nether/uneasy_alliance",
            "minecraft:nether/fast_travel",
            "minecraft:nether/summon_wither",
            "minecraft:nether/create_full_beacon",
            "minecraft:nether/all_effects",
            "minecraft:end/levitate",
            "minecraft:husbandry/silk_touch_nest",
            "minecraft:husbandry/obtain_netherite_hoe",
            "minecraft:husbandry/balanced_diet",
            "minecraft:husbandry/complete_catalogue",
            "minecraft:adventure/two_birds_one_arrow",
            "minecraft:adventure/arbalistic",
            "minecraft:adventure/bullseye",
            "minecraft:adventure/very_very_frightening",
            "minecraft:adventure/hero_of_the_village",
            "minecraft:adventure/adventuring_time",
            "minecraft:adventure/kill_all_mobs"
        };

        private OverlaySettings()
        {
            FileName = "overlay";
            Load();
            Favorites = new HashSet<string>((ICollection<string>)Entries[FAVORITES]);
        }

        public override void ResetToDefaults()
        {
            Entries = new Dictionary<string, object>()
            {
                { ENABLED,         false },
                { ONLY_FAVORITES,  false },
                { HIDE_COMPLETED,  false },
                { SHOW_LABELS,     true },
                { SHOW_OVERVIEW,   true},
                { SHOW_CRITERIA,   true },
                { SHOW_COUNTS,     true },
                { SPEED,           5 },
                { WIDTH,           1920 },
                { BACK_COLOR,      Color.Green },
                { TEXT_COLOR,      Color.White }
            };
            Favorites = DefaultFavorites;
        }
    }
}
