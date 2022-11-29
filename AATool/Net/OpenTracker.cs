using System;
using System.Net.Http;
using AATool.Configuration;
using AATool.Utilities;

namespace AATool.Net
{
    static class OpenTracker
    {
        public static Exception LastError { get; private set; }
        public static string WatchUrl { get; private set; }

        private static readonly HttpClient Client = new ();
        private static readonly Timer Cooldown = new (5);

        private static bool Invalidated = false;

        public static void Update(Time time)
        {
            Cooldown.Update(time);
            if (Invalidated && !IsOnCooldown)
                BroadcastProgress();
        }

        public static bool IsOnCooldown => Cooldown.IsRunning;

        public static async void BroadcastProgress()
        {
            if (IsOnCooldown)
            {
                Invalidated = true;
                return;
            }

            if (string.IsNullOrEmpty(Config.Tracking.OpenTrackerKey))
                return;
            if (string.IsNullOrEmpty(Config.Tracking.OpenTrackerUrl))
                return;

            Cooldown.Reset();
            Invalidated = false;

            try
            {
                string aaKey = AAKey.Strip(Config.Tracking.OpenTrackerKey);
                var content = new StringContent(aaKey);// + Tracker.State.ToJsonString());
                HttpResponseMessage response = await Client.PostAsync(Config.Tracking.OpenTrackerUrl, content);
                string message = await response.Content.ReadAsStringAsync();
                WatchUrl = message;
                LastError = null;
            }
            catch (Exception e) { LastError = e; }
        }
    }
}
