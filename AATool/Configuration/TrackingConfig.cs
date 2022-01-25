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

            [JsonProperty] public readonly Setting<string> GameCategory = new (new AllAdvancements().Name);
            [JsonProperty] public readonly Setting<string> GameVersion = new (new AllAdvancements().LatestSupportedVersion);

            [JsonProperty] public readonly Setting<bool> AutoDetectVersion = new (true);
            [JsonProperty] public readonly Setting<bool> UseDefaultPath    = new (true);
            [JsonProperty] public readonly Setting<bool> UseSftp           = new (false);
            [JsonProperty] public readonly Setting<string> CustomSavePath  = new (string.Empty);

            protected override string GetId() => "tracking";
            protected override string GetLegacyId() => "tracker";

            public TrackingConfig()
            {
                this.RegisterSetting(this.LastSession);
                this.RegisterSetting(this.GameCategory);
                this.RegisterSetting(this.GameVersion);
                this.RegisterSetting(this.CustomSavePath);
                this.RegisterSetting(this.AutoDetectVersion);
                this.RegisterSetting(this.UseDefaultPath);
                this.RegisterSetting(this.UseSftp);
            }

            protected override void ApplyLegacySetting(string key, object value)
            {
                ISetting setting = key switch {
                    "use_default_saves_folder" => this.UseDefaultPath,
                    "last_aatool_run"     => this.LastSession,
                    "auto_game_version"   => this.AutoDetectVersion,
                    "custom_saves_folder" => this.CustomSavePath,
                    "use_remote_world"    => this.UseSftp,
                    _ => null
                };
                setting?.Set(value);
            }
        }
    }
}
