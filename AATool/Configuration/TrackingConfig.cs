using System;
using System.Collections.Generic;
using AATool.Data.Categories;
using Newtonsoft.Json;

namespace AATool.Configuration
{
    [JsonObject]
    public abstract partial class Config
    {
        [JsonObject]
        public class TrackingConfig : Config
        {
            [JsonProperty] public readonly Setting<string> LastSession = new (string.Empty);

            [JsonProperty] public readonly Setting<string> GameCategory = new ("All Advancements");
            [JsonProperty] public readonly Setting<string> GameVersion = new ("1.18");

            [JsonProperty] public readonly Setting<bool> AutoDetectVersion = new (true);
            [JsonProperty] public readonly Setting<bool> UseSftp = new (false);

            [JsonProperty] public readonly Setting<TrackerSource> Source = new (TrackerSource.ActiveInstance);
            [JsonProperty] public readonly Setting<string> CustomSavesPath  = new (string.Empty);
            [JsonProperty] public readonly Setting<string> CustomWorldPath = new (string.Empty);

            [JsonProperty] public readonly Setting<bool> BroadcastProgress = new (false);
            [JsonProperty] public readonly Setting<string> OpenTrackerKey = new (string.Empty);
            [JsonProperty] public readonly Setting<string> OpenTrackerUrl = new (string.Empty);

            protected override string GetId() => "tracking";
            protected override string GetLegacyId() => "tracker";

            [JsonIgnore]
            public bool WatchActiveInstance => !this.UseSftp 
                && (this.Source == TrackerSource.ActiveInstance || this.AutoDetectVersion || Tracker.Category is AllDeaths);
            
            [JsonIgnore]
            public bool SourceChanged => this.UseSftp.Changed || this.Source.Changed
                || (this.Source == TrackerSource.CustomSavesPath && this.CustomSavesPath.Changed)
                || (this.Source == TrackerSource.SpecificWorld && this.CustomWorldPath.Changed);

            public TrackingConfig()
            {
                this.RegisterSetting(this.LastSession);

                this.RegisterSetting(this.GameCategory);
                this.RegisterSetting(this.GameVersion);

                this.RegisterSetting(this.AutoDetectVersion);
                this.RegisterSetting(this.UseSftp);

                this.RegisterSetting(this.Source);
                this.RegisterSetting(this.CustomSavesPath);
                this.RegisterSetting(this.CustomWorldPath);

                this.RegisterSetting(this.BroadcastProgress);
                this.RegisterSetting(this.OpenTrackerKey);
                this.RegisterSetting(this.OpenTrackerUrl);
            }

            protected override void ApplyLegacySetting(string key, object value)
            {
                ISetting setting = key switch {
                    "last_aatool_run"     => this.LastSession,
                    "auto_game_version"   => this.AutoDetectVersion,
                    "custom_saves_folder" => this.CustomSavesPath,
                    "use_remote_world"    => this.UseSftp,
                    _ => null
                };
                setting?.Set(value);
            }
        }
    }
}
