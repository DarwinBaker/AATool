using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AATool.Configuration;
using AATool.Net;
using Microsoft.Xna.Framework;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace AATool.Saves
{
    public static class MinecraftServer
    {
        private const string SftpPrefix     = "sftp://";
        private const double SaveInterval   = (5 * 60) + 5;
        private const double RetryInterval  = 10;
        private const int MaximumRetries    = 3;
        private const int AttemptIntervalMs = 5000;

        public static Exception LastError       { get; private set; }
        public static DateTime LastWorldSave    { get; private set; }
        public static SyncState State           { get; private set; }
        public static string MessageOfTheDay    { get; private set; }
        public static string WorldName          { get; private set; }

        private static readonly Utilities.Timer RefreshTimer = new ();

        private static ConnectionInfo Credentials;
        private static int CurrentDownloadPercent;
        private static float SmoothDownloadPercent;

        public static bool CredentialsValidated => Credentials is not null;
        public static bool LastSyncFailed => LastError is not null;
        public static bool IsDownloading => State is SyncState.Advancements or SyncState.Statistics;
        public static bool IsEnabled => Config.Tracking.UseSftp;

        public static int GetNextRefresh() => (int)Math.Max(Math.Ceiling(RefreshTimer.TimeLeft), 0);
        public static void InvalidateWorld() => WorldName = string.Empty;
        private static void SetState(SyncState state) => State = state;

        public static DateTime GetRefreshEstimate() => IsEnabled
            ? LastWorldSave.Add(TimeSpan.FromSeconds(SaveInterval))
            : default;

        public static void Update(Time time)
        {
            if (LastError is ArgumentException)
            {
                //invalid login credentials, don't try reconnecting
                return;
            }
            
            RefreshTimer.Update(time);
            if (IsEnabled && RefreshTimer.IsExpired)
                Sync();

            //smoothly interpolate progress percentage for status label
            float speed = (float)(8 * time.Delta);
            SmoothDownloadPercent = MathHelper.Lerp(SmoothDownloadPercent, CurrentDownloadPercent, speed);
        }

        public static void Sync()
        {
            if (State is not SyncState.Ready)
                return;

            SetState(SyncState.Connecting);

            //attempt to sync in the background
            Task.Run(() => 
            {
                SftpClient sftp = null;
                double remaining = 0;
                try
                {
                    if (!TryConnect(out sftp))
                        return;
                    if (!TryDownloadServerProperties(sftp))
                        return;
                    if (!TryGetWorldSaveTime(sftp, out DateTime latest))
                        return;
                    DateTime next = latest.Add(TimeSpan.FromSeconds(SaveInterval));
                    remaining = (next - DateTime.UtcNow).TotalSeconds;
                    if (remaining > 0)
                        RefreshTimer.SetAndStart(Math.Min(remaining, SaveInterval));

                    if (latest != LastWorldSave)
                    {
                        if (!TryDownloadProgress(sftp))
                            return;

                        LastWorldSave = latest;

                        //update client refresh estimates if hosting
                        if (Server.TryGet(out Server server))
                            server.SendNextRefresh();

                        Tracker.Invalidate();
                    }
                }
                finally
                {
                    try
                    {
                        sftp?.Disconnect();
                        sftp?.Dispose();
                    }
                    catch (ObjectDisposedException)
                    {
                        //sftp client already disposed. don't need to do anything
                    }
                    if (RefreshTimer.IsExpired || (LastSyncFailed && LastError is not ArgumentException))
                        RefreshTimer.SetAndStart(RetryInterval);
                    SetState(SyncState.Ready);
                }
            });
        }

        public static string GetLongStatusText()
        {
            if (State is SyncState.Connecting)
                return "Connecting to Minecraft server...";

            if (CredentialsValidated)
            {
                if (State is SyncState.Ready)
                {
                    string timeLeft = Tracker.GetEstimateString(GetNextRefresh());

                    //waiting
                    if (LastError is IOException)
                        return $"SFTP couldn't write to local files! Retrying in {timeLeft}";

                    if (LastError is SocketException)
                        return $"Couldn't reach dedicated Minecraft server. Retrying in {timeLeft}";

                    if (LastError is SftpPathNotFoundException)
                        return $"{LastError.Message} Retrying in {timeLeft}";

                    return LastSyncFailed
                        ? $"SFTP Error: {LastError.Message} Retrying in {timeLeft}" 
                        : $"Synced! Refreshing in {timeLeft}";
                }
                else
                {
                    //busy
                    return State switch {
                        SyncState.Connecting        => "Connecting to Minecraft server...",
                        SyncState.ServerProperties  => "Parsing server properties...",
                        SyncState.LastAutoSave      => "Comparing world time-stamps...",
                        SyncState.Advancements      => $"Syncing advancements... {Math.Ceiling(SmoothDownloadPercent)}%",
                        SyncState.Statistics        => $"Syncing statistics... {Math.Ceiling(SmoothDownloadPercent)}%",
                        _ => "Syncing..."
                    };
                }
            }

            if (LastError is SshAuthenticationException)
                return "SFTP login refused by Minecraft server.";

            if (LastError is ArgumentException)
                return "Invalid SFTP Login.";

            return $"SFTP not running: Retrying in {Tracker.GetEstimateString(GetNextRefresh())}";
        }

        public static string GetShortStatusText()
        {
            if (State is SyncState.Ready)
            {
                return CredentialsValidated
                    ? $"Refreshing in {Tracker.GetEstimateString(GetNextRefresh()).Replace(" ", "\0")}"
                    : "SFTP Offline";
            }
            else
            {
                return State is SyncState.Connecting
                    ? "Connecting..."
                    : "Syncing...";
            }
        }

        private static void ClearCredentials() => Credentials = null;

        private static void ApplyCredentials()
        {
            try
            {
                //fix host formatting if needed
                string host = ((string)Config.Sftp.Host).StartsWith(SftpPrefix)
                    ? ((string)Config.Sftp.Host).Substring(SftpPrefix.Length)
                    : Config.Sftp.Host;

                Credentials = new ConnectionInfo(host, Config.Sftp.Port, Config.Sftp.Username, new[] {
                    new PasswordAuthenticationMethod(Config.Sftp.Username, Config.Sftp.Password)
                }) { Timeout = TimeSpan.FromSeconds(5) };
            }
            catch (ArgumentException exception) 
            {
                LastError = exception;
            }
        }

        private static bool TryConnect(out SftpClient sftp)
        {
            LastError = null;
            sftp = null;
            ApplyCredentials();
            if (!CredentialsValidated)
                return false;

            try
            {
                //start sftp client and attempt connection
                sftp = new SftpClient(Credentials);
                sftp.Connect();
                return true;
            }
            catch (Exception exception)
            {
                //couldn't connect to sftp server
                sftp?.Dispose();
                RefreshTimer.SetAndStart(RetryInterval);

                LastError = exception;
                if (exception is SshAuthenticationException)
                    ClearCredentials();
                return false;
            }
        }


        private static bool TryDownloadProgress(SftpClient sftp)
        {
            //download advancement jsons
            SetState(SyncState.Advancements);
            if (!TryDownloadFolder(sftp, "advancements"))
                return false;

            //download statistic jsons
            SetState(SyncState.Statistics);
            return TryDownloadFolder(sftp, "stats");
        }

        private static bool TryGetProperty(string[] properties, string key, out string value)
        {
            //parse value from server properties ini
            value = string.Empty;
            foreach (string property in properties)
            {
                if (property.StartsWith(key))
                {
                    //value is everything right of the '=' symbol
                    value = property.Split('=')[1];
                    return true;
                }
            }
            return false;
        }

        private static bool TryDownloadServerProperties(SftpClient sftp, int failures = 0)
        {
            if (!string.IsNullOrEmpty(WorldName))
            {
                //early exit if properties already downloaded (assume they haven't changed)
                return true;
            }

            SetState(SyncState.ServerProperties);
            try
            {
                //download server properties
                string path = Path.Combine(Config.Sftp.ServerRoot, "server.properties");
                string[] properties = sftp.ReadAllText(path).Split('\n');
                if (TryGetProperty(properties, "level-name", out string world))
                    WorldName = world.TrimEnd();
                if (TryGetProperty(properties, "motd", out string message))
                    MessageOfTheDay = message.TrimEnd();
                return true;
            }
            catch (Exception exception)
            {
                if (exception is SshException or IOException && failures < MaximumRetries)
                {
                    //network error. try downloading properties again
                    Thread.Sleep(AttemptIntervalMs);
                    return TryDownloadServerProperties(sftp, failures + 1);
                }
                else
                {
                    //fatal error or out of retries. give up
                    LastError = exception;
                    return false;
                }
            }
        }

        private static bool TryGetWorldSaveTime(SftpClient sftp, out DateTime lastWorldSave, int failures = 0)
        {
            SetState(SyncState.LastAutoSave);
            string remote = Path.Combine(Config.Sftp.ServerRoot, WorldName, "level.dat");
            lastWorldSave = default;
            
            try
            {
                lastWorldSave = sftp.GetLastWriteTimeUtc(remote);
                return true;
            }
            catch (Exception exception)
            {
                if (exception is SshException or IOException && failures < MaximumRetries)
                {
                    if (exception is SftpPathNotFoundException)
                    {
                        //folder not found, world name might be wrong. refresh it next time
                        LastError = new SftpPathNotFoundException($"File not found: \"{remote}\".");
                        InvalidateWorld();
                        return false;
                    }
                    else
                    {
                        //try downloading properties again
                        Thread.Sleep(AttemptIntervalMs);
                        return TryGetWorldSaveTime(sftp, out lastWorldSave, failures + 1);
                    }
                }
                else
                {
                    //fatal error or out of retries. give up
                    LastError = exception;
                    return false;
                }
            }
        }

        public static bool TryDownloadFolder(SftpClient sftp, string name, int failures = 0)
        {
            //reset progress counter
            CurrentDownloadPercent = 0;
            SmoothDownloadPercent = 0;

            string localPath = Path.Combine(Paths.System.SftpWorldsFolder, WorldName, name);
            string remotePath = Path.Combine(Config.Sftp.ServerRoot, WorldName, name);
            try
            {
                //make sure directory exists
                Directory.CreateDirectory(localPath);

                //get new and old file lists
                IList<FileInfo> localFiles = GetFiles(localPath);
                IList<SftpFile> remoteFiles = sftp.ListDirectory(remotePath).ToList();

                //sync folder
                DeleteDepricatedFiles(remoteFiles, localFiles);
                DownloadAll(sftp, remoteFiles, localPath);
                return true;
            }
            catch (Exception exception)
            {
                if (exception is SshException or IOException && failures < MaximumRetries)
                {
                    if (exception is SftpPathNotFoundException)
                    {
                        //folder not found, so world name might be wrong. refresh it next time 
                        LastError = new SftpPathNotFoundException($"Path not found: \"{remotePath}\".");
                        InvalidateWorld();
                        return false;
                    }
                    else
                    {
                        //try downloading the folder again
                        Thread.Sleep(AttemptIntervalMs);
                        return TryDownloadFolder(sftp, name, failures + 1);
                    }
                }
                else
                {
                    //fatal error or out of retries. give up
                    LastError = exception;
                    return false;
                }
            }
        }

        private static bool TryDownloadFile(SftpClient sftp, string remote, string local, int failures = 0)
        {
            try
            {
                using (FileStream localFile = File.Create(local))
                    sftp.DownloadFile(remote, localFile);
                return true;
            }
            catch (Exception exception)
            {
                if (exception is SshException or IOException && failures < MaximumRetries)
                {
                    //try downloading the file again
                    Thread.Sleep(AttemptIntervalMs);
                    return TryDownloadFile(sftp, remote, local, failures + 1);
                }
                else
                {
                    //fatal error or out of retries. give up
                    LastError = exception;
                    return false;
                }
            }
        }

        private static void DeleteDepricatedFiles(IList<SftpFile> source, IList<FileInfo> destination)
        {
            //remove depricated files
            foreach (FileInfo localFile in destination)
            {
                bool depricated = true;
                foreach (SftpFile remoteFile in source)
                {
                    if (remoteFile.Name == localFile.Name)
                    {
                        //file is still supposed to exist
                        depricated = false;
                        break;
                    }
                }

                if (!depricated)
                    continue;

                try
                {
                    //file no longer on server (probably a run reset). clean it up
                    localFile.Delete();
                }
                catch
                {
                    //couldn't delete old file (probably open externally for some reason)
                    //just leave it be and move on
                }
            }
        }

        private static void DownloadAll(SftpClient sftp, IEnumerable<SftpFile> files, string downloadFolder)
        {
            if (!files.Any())
                return;

            //download remote files from server over sftp
            int counter = 1;
            foreach (SftpFile remoteFile in files)
            {
                //update percentage
                CurrentDownloadPercent = (int)(100 * ((double)counter / files.ToList().Count));
                counter++;
                if (!remoteFile.IsRegularFile)
                    continue;

                //download to local file
                string localFile = Path.Combine(downloadFolder, remoteFile.Name);
                TryDownloadFile(sftp, remoteFile.FullName, localFile);
            }
        }

        private static IList<FileInfo> GetFiles(string directory)
        {
            //iterate top level files
            var files = new List<FileInfo>();
            foreach (string file in Directory.GetFiles(directory))
                files.Add(new FileInfo(file));
            return files;
        }
    }
}