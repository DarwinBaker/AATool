using System.Collections.Generic;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class NetworkSettings : SettingsGroup
    {
        public static NetworkSettings Instance = new NetworkSettings();

        public const string MOJANG_NAME     = "mojang_name";
        public const string DISPLAY_NAME    = "display_name";
        public const string PRONOUNS        = "pronouns";
        public const string IS_SERVER       = "is_server";
        public const string IP_ADDRESS      = "ip";
        public const string PORT            = "port";
        public const string AUTO_SERVER_IP  = "auto_server_ip";
        public const string PASSWORD        = "saved_password";

        public string MinecraftName     { get => this.Get<string>(MOJANG_NAME);  set => this.Set(MOJANG_NAME, value); }
        public string PreferredName     { get => this.Get<string>(DISPLAY_NAME); set => this.Set(DISPLAY_NAME, value); }
        public string Pronouns          { get => this.Get<string>(PRONOUNS);     set => this.Set(PRONOUNS, value); }
        public bool IsServer            { get => this.Get<bool>(IS_SERVER);      set => this.Set(IS_SERVER, value); }
        public string IP                { get => this.Get<string>(IP_ADDRESS);   set => this.Set(IP_ADDRESS, value); }
        public string Port              { get => this.Get<string>(PORT);         set => this.Set(PORT, value); }
        public string Password          { get => this.Get<string>(PASSWORD);     set => this.Set(PASSWORD, value); }
        public bool AutoServerIP        { get => this.Get<bool>(AUTO_SERVER_IP); set => this.Set(AUTO_SERVER_IP, value); }

        private NetworkSettings()
        {
            this.Load("network");
        }

        public override void ResetToDefaults()
        {
            this.Set(MOJANG_NAME, null);
            this.Set(DISPLAY_NAME, null);
            this.Set(PRONOUNS, null);
            this.Set(IS_SERVER, false);
            this.Set(IP_ADDRESS, null);
            this.Set(PORT, "25562");
            this.Set(AUTO_SERVER_IP, true);
            this.Set(PASSWORD, string.Empty);
        }
    }
}
