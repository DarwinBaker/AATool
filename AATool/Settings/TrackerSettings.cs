using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AATool.Utilities;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class TrackerSettings : SettingsGroup
    {
        public static readonly TrackerSettings Instance = new ();

        public static HashSet<string> SupportedVersions { get; private set; }

        private const string TRACKER_VERSION    = "last_aatool_run";
        private const string GAME_VERSION       = "game_version";
        private const string REMOTE_WORLD       = "use_remote_world";
        private const string USE_DEFAULT_PATH   = "use_default_saves_folder";
        private const string AUTO_GAME_VERSION  = "auto_game_version";
        private const string CUSTOM_FOLDER      = "custom_saves_folder";

        private const string SFTP_HOST          = "sftp_host";
        private const string SFTP_PORT          = "sftp_port";
        private const string SFTP_USER          = "sftp_user";
        private const string SFTP_PASS          = "sftp_pass";

        public string GameVersion       { get => this.Get<string>(GAME_VERSION);    private set => this.Set(GAME_VERSION, value); }
        public string LastAAToolRun     { get => this.Get<string>(TRACKER_VERSION); set => this.Set(TRACKER_VERSION, value); }
        public bool UseRemoteWorld      { get => this.Get<bool>(REMOTE_WORLD);      set => this.Set(REMOTE_WORLD, value); }
        public bool UseDefaultPath      { get => this.Get<bool>(USE_DEFAULT_PATH);  set => this.Set(USE_DEFAULT_PATH, value); }
        public bool AutoDetectVersion   { get => this.Get<bool>(AUTO_GAME_VERSION); set => this.Set(AUTO_GAME_VERSION, value); }
        public string CustomPath        { get => this.Get<string>(CUSTOM_FOLDER);   set => this.Set(CUSTOM_FOLDER, value); }

        public string SftpHost          { get => this.Get<string>(SFTP_HOST);    set => this.Set(SFTP_HOST, value); }
        public int SftpPort             { get => this.Get<int>(SFTP_PORT);       set => this.Set(SFTP_PORT, value); }
        public string SftpUser          { get => this.Get<string>(SFTP_USER);    set => this.Set(SFTP_USER, value); }
        public string SftpPass          { get => this.Get<string>(SFTP_PASS);    set => this.Set(SFTP_PASS, value); }

        public bool GameVersionChanged()    => Instance.ValueChanged(GAME_VERSION);
        public bool UseRemoteWorldChanged() => Instance.ValueChanged(REMOTE_WORLD);
        public bool UseDefaultPathChanged() => Instance.ValueChanged(USE_DEFAULT_PATH);
        public bool CustomPathChanged()     => Instance.ValueChanged(CUSTOM_FOLDER);

        public static bool IsPostExplorationUpdate { get; private set; }

        public static string DefaultSavesFolder => 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "saves");
        
        public string SavesFolder => this.UseRemoteWorld 
            ? Paths.DIR_REMOTE_WORLDS 
            : this.UseDefaultPath 
                ? DefaultSavesFolder
                : this.CustomPath;

        public void TrySetGameVersion(string version)
        {
            try
            {
                if (SupportedVersions.Contains(version))
                    this.Set(GAME_VERSION, version);
                IsPostExplorationUpdate = new Version(version) >= new Version("1.12");
            }
            catch { }
        }

        private TrackerSettings()
        {
            this.ParseSupportedGameVersions();
            this.Load("tracker");
            this.TrySetGameVersion(this.GameVersion);
            this.Save();
        }

        private void ParseSupportedGameVersions()
        {
            //get list of supported versions from assets folder
            SupportedVersions = new HashSet<string>();
            try
            {
                var directory = new DirectoryInfo(Paths.DIR_GAME_VERSIONS);
                if (directory.Exists)
                {
                    foreach (DirectoryInfo versionFolder in directory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                        SupportedVersions.Add(versionFolder.Name);
                }
            }
            catch (Exception e)
            { 
                Main.QuitBecause("Error parsing game version manifest", e); 
            }
        }

        public override void ResetToDefaults()
        {
            this.Set(TRACKER_VERSION, "0.0");
            this.TrySetGameVersion("1.17");
            this.Set(USE_DEFAULT_PATH, true);
            this.Set(AUTO_GAME_VERSION, true);
            this.Set(CUSTOM_FOLDER, string.Empty);

            this.Set(REMOTE_WORLD, false);

            this.Set(SFTP_HOST, string.Empty);
            this.Set(SFTP_PORT, 22);
            this.Set(SFTP_USER, string.Empty);
            this.Set(SFTP_PASS, string.Empty);
        }
    }
}
