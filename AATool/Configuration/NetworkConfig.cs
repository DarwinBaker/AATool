using System.Collections.Generic;
using AATool.Net;
using Newtonsoft.Json;

namespace AATool.Configuration
{
    public abstract partial class Config
    {
        [JsonObject]
        public class NetworkConfig : Config
        {
            [JsonProperty] public readonly Setting<string> MinecraftName = new (string.Empty);
            [JsonProperty] public readonly Setting<string> PreferredName = new (string.Empty);
            [JsonProperty] public readonly Setting<string> Pronouns = new (string.Empty);

            [JsonProperty] public readonly Setting<string> Password = new (string.Empty);
            [JsonProperty] public readonly Setting<string> IP = new (string.Empty);
            [JsonProperty] public readonly Setting<int> Port = new (Protocol.Peers.DefaultPort);

            [JsonProperty] public readonly Setting<bool> AutoServerIP = new (true);
            [JsonProperty] public readonly Setting<bool> IsServer = new (false);

            protected override string GetId() => "network";
            protected override string GetLegacyId() => "network";

            public NetworkConfig()
            {
                this.RegisterSetting(this.MinecraftName);
                this.RegisterSetting(this.PreferredName);
                this.RegisterSetting(this.Pronouns);
                this.RegisterSetting(this.Password);
                this.RegisterSetting(this.IP);
                this.RegisterSetting(this.Port);
                this.RegisterSetting(this.AutoServerIP);
                this.RegisterSetting(this.IsServer);
            }

            protected override void ApplyLegacySetting(string key, object value)
            {
                ISetting setting = key switch {
                    "mojang_name"    => this.MinecraftName,
                    "display_name"   => this.PreferredName,
                    "pronouns"       => this.Pronouns,
                    "is_server"      => this.IsServer,
                    "ip"             => this.IP,
                    "port"           => this.Port,
                    "auto_server_ip" => this.AutoServerIP,
                    "saved_password" => this.Password,
                    _ => null
                };
                setting?.Set(value);
            }
        }
    }
}
