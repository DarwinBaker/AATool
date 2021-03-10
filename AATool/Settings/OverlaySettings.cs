using AATool.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class OverlaySettings : SettingsGroup
    {
        public static OverlaySettings Instance = new OverlaySettings();

        public const string ENABLED        = "enabled";
        public const string SHOW_LABELS    = "show_labels";
        public const string SHOW_OVERVIEW  = "show_overview";
        public const string SHOW_CRITERIA  = "show_criteria";
        public const string SHOW_COUNTS    = "show_counts";
        public const string ONLY_FAVORITES = "only_chosen";
        public const string HIDE_COMPLETED = "hide_completed";
        public const string RIGHT_TO_LEFT  = "right_to_left";
        public const string SPEED          = "speed";
        public const string WIDTH          = "width";
        public const string BACK_COLOR     = "back_color";
        public const string TEXT_COLOR     = "text_color";

        public FavoritesList Favorites;

        public bool Enabled                 { get => Get<bool>(ENABLED);                set => Set(ENABLED, value); }
        public bool OnlyShowFavorites       { get => Get<bool>(ONLY_FAVORITES);         set => Set(ONLY_FAVORITES, value); }
        public bool HideCompleted           { get => Get<bool>(HIDE_COMPLETED);         set => Set(HIDE_COMPLETED, value); }
        public bool ShowLabels              { get => Get<bool>(SHOW_LABELS);            set => Set(SHOW_LABELS, value); }
        public bool ShowCriteria            { get => Get<bool>(SHOW_CRITERIA);          set => Set(SHOW_CRITERIA, value); }
        public bool ShowCounts              { get => Get<bool>(SHOW_COUNTS);            set => Set(SHOW_COUNTS, value); }
        public bool ShowOverview            { get => Get<bool>(SHOW_OVERVIEW);          set => Set(SHOW_OVERVIEW, value); }
        public bool RightToLeft             { get => Get<bool>(RIGHT_TO_LEFT);          set => Set(RIGHT_TO_LEFT, value); }
        public int Speed                    { get => Get<int>(SPEED);                   set => Set(SPEED, value); }
        public int Width                    { get => Get<int>(WIDTH);                   set => Set(WIDTH, value); }
        public Color BackColor              { get => Get<Color>(BACK_COLOR);            set => Set(BACK_COLOR, value); }
        public Color TextColor              { get => Get<Color>(TEXT_COLOR);            set => Set(TEXT_COLOR, value); }

        private OverlaySettings()
        {
            FileName = "overlay";
            Load();
        }

        public void LoadFavorites()
        {
            if (!Favorites.LoadXml(Path.Combine(Paths.DIR_FAVORITES, TrackerSettings.Instance.GameVersion + ".xml")))
                Favorites.Clear();
        }

        public void SaveFavorites()
        {
            Directory.CreateDirectory(Paths.DIR_FAVORITES);
            Favorites.SaveXml(Path.Combine(Paths.DIR_FAVORITES, TrackerSettings.Instance.GameVersion + ".xml"));
        }

        public override void ReadDocument(XmlDocument document)
        {
            base.ReadDocument(document);
            LoadFavorites();
        }

        public override void WriteDocument(XmlWriter writer)
        {
            base.WriteDocument(writer);
            SaveFavorites();
        }

        public override void ResetToDefaults()
        {
            Entries = new Dictionary<string, object>();
            Set(ENABLED, false);
            Set(HIDE_COMPLETED, false);
            Set(SHOW_LABELS, true);
            Set(SHOW_OVERVIEW, true);
            Set(SHOW_CRITERIA, true);
            Set(SHOW_COUNTS, true);
            Set(RIGHT_TO_LEFT, true);
            Set(SPEED, 5);
            Set(WIDTH, 1920);
            Set(BACK_COLOR, Color.FromNonPremultiplied(0, 170, 0, 255));
            Set(TEXT_COLOR, Color.White);
            Favorites = new FavoritesList();
            base.ResetToDefaults();
        }
    }
}
