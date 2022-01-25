using System.Collections.Generic;
using AATool.Net;
using Newtonsoft.Json;

namespace AATool.Configuration
{
    public abstract partial class Config
    {
        [JsonObject]
        public class SftpConfig : Config
        {
            [JsonProperty] public readonly Setting<string> Host = new (string.Empty);
            [JsonProperty] public readonly Setting<string> Username = new (string.Empty);
            [JsonProperty] public readonly Setting<string> Password = new (string.Empty);
            [JsonProperty] public readonly Setting<int> Port = new (22);

            protected override string GetId() => "sftp";
            protected override string GetLegacyId() => "tracker";

            public SftpConfig()
            {
                this.RegisterSetting(this.Host);
                this.RegisterSetting(this.Username);
                this.RegisterSetting(this.Password);
                this.RegisterSetting(this.Port);
            }

            protected override void ApplyLegacySetting(string key, object value)
            {
                ISetting setting = key switch {
                    "sftp_host" => this.Host,
                    "sftp_user" => this.Username,
                    "sftp_pass" => this.Password,
                    "sftp_port" => this.Port,
                    _ => null
                };
                setting?.Set(value);
            }
        }
    }
}
