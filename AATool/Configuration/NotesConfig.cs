using System.Collections.Generic;
using Newtonsoft.Json;

namespace AATool.Configuration
{
    public abstract partial class Config
    {
        [JsonObject]
        public class NotesConfig : Config
        {
            [JsonProperty] public readonly Setting<bool> Enabled     = new (false);
            [JsonProperty] public readonly Setting<bool> AlwaysOnTop = new (true);

            [JsonProperty] public readonly Setting<int> Width = new (420);
            [JsonProperty] public readonly Setting<int> Height = new (420);

            protected override string GetId() => "notes";
            protected override string GetLegacyId() => "notes";

            public NotesConfig()
            {
                this.RegisterSetting(this.Enabled);
                this.RegisterSetting(this.AlwaysOnTop);
                this.RegisterSetting(this.Width);
                this.RegisterSetting(this.Height);
            }

            protected override void ApplyLegacySetting(string key, object value)
            {
                ISetting setting = key switch {
                    "always_on_top" => this.AlwaysOnTop,
                    "enabled" => this.Enabled,
                    _ => null
                };
                setting?.Set(value);
            }
        }
    }
}
