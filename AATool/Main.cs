using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Reflection;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Trackers;
using AATool.Settings;
using System.Diagnostics;
using AATool.Utilities;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using AATool.Winforms.Forms;

namespace AATool
{
    public class Main : Game
    {
        public GraphicsDeviceManager GraphicsManager { get; private set; }
        public AdvancementTracker AdvancementTracker { get; private set; }
        public StatisticsTracker StatisticsTracker   { get; private set; }
        public AchievementTracker AchievementTracker { get; private set; }

        private Time time;
        private Display display;
        private GameVersionDetector gameVersionDetector;
        private Screen mainScreen;
        private Dictionary<Type, Screen> altScreens;

        private void AddScreen(Screen screen) => altScreens[screen.GetType()] = screen;

        public Main()
        {
            //set window title based on version
            var version  = Assembly.GetExecutingAssembly().GetName().Version;
            string name  = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            Window.Title = name + " " + version.Major + "." + version.Minor + "." + version.Build;

            GraphicsManager   = new GraphicsDeviceManager(this);
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60);
            InactiveSleepTime = TimeSpan.Zero;
            IsFixedTimeStep   = true;
            IsMouseVisible    = true;
        }

        protected override void Initialize()
        {
            //instantiate important objects
            time                = new Time();
            display             = new Display(GraphicsManager);
            gameVersionDetector = new GameVersionDetector();
            AdvancementTracker  = new AdvancementTracker();
            StatisticsTracker   = new StatisticsTracker();
            AchievementTracker  = new AchievementTracker();

            //load assets
            SpriteSheet.Initialize(GraphicsDevice);
            FontSet.Initialize(GraphicsDevice);

            //instantiate screens
            altScreens = new Dictionary<Type, Screen>();
            mainScreen = new MainScreen(this);
            AddScreen(new OverlayScreen(this));
            mainScreen.Form.BringToFront();

            base.Initialize();
            CheckForUpdatesAsync();
        }

        protected override void Update(GameTime gameTime)
        {
            time.Update(gameTime);
            display.Update(time);
            gameVersionDetector.Update();
            AdvancementTracker.Update(time);
            StatisticsTracker.Update(time);
            AchievementTracker.Update(time);

            //update each screen
            mainScreen.UpdateRecursive(time);
            foreach (var screen in altScreens.Values)
                screen.UpdateRecursive(time);

            TrackerSettings.Instance.Update();
            MainSettings.Instance.Update();
            OverlaySettings.Instance.Update();
            SpriteSheet.Update(time);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //render each secondary screen to its respective viewport
            foreach (var screen in altScreens.Values)
            {
                screen.Prepare(display);
                screen.DrawRecursive(display);
                if (MainSettings.Instance.LayoutDebug)
                    screen.DrawDebugRecursive(display);
                screen.Present(display);
            }

            //render main screen to default backbuffer
            mainScreen.Prepare(display);
            mainScreen.DrawRecursive(display);
            if (MainSettings.Instance.LayoutDebug)
                mainScreen.DrawDebugRecursive(display);
            mainScreen.Present(display);

            base.Draw(gameTime);
        }

        public static void ForceQuit()
        {
            //show user a message and quit if for some reason the program fails to load properly
            string caption = "Missing Assets";
            if (File.Exists("AAUpdate.exe"))
            {
                string message = "Error: One or more required assets failed to load. Would you like to repair your installation?";
                if (System.Windows.Forms.MessageBox.Show(message, caption, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                    RunUpdateAssistant(true);
            }
            else
            {
                string message = "Error: One or more required assets failed to load and the update executable could not be found. Would you like to go to the AATool GitHub page to download and re-install manually?";
                if (System.Windows.Forms.MessageBox.Show(message, caption, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                    Process.Start("https://github.com/DarwinBaker/AATool/releases/latest");
            }
            Environment.Exit(1);
        }

        public static async void CheckForUpdatesAsync(bool failSilently = true)
        {
            string html;
            using (var client = new WebClient())
                html = await client.DownloadStringTaskAsync("https://github.com/DarwinBaker/AATool/releases/latest/");

            //get latest github release page
            int startIndex       = html.IndexOf("DarwinBaker/AATool/releases/download/");
            int endIndex         = html.IndexOf(".zip");
            string latestLink    = html.Substring(startIndex, endIndex - startIndex);                  
            string patchNotes    = html.Substring(html.IndexOf("<div class=\"markdown-body\">"));
            patchNotes           = Regex.Replace(patchNotes.Substring(0, patchNotes.IndexOf("<details")), @"<[^>]*>", string.Empty);

            //get latest version number from download link
            string latestVersion = latestLink.Substring(latestLink.LastIndexOf('_') + 1);
            string[] latestSplit = latestVersion.Split('.');
            string latestDecimal = string.Empty;
            for (int i = 0; i < latestSplit.Length; i++)
                latestDecimal += (i == 1 ? "." : string.Empty) + latestSplit[i];

            //get current version number from this exe
            string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string[] currentSplit = currentVersion.Split('.');
            string currentDecimal = string.Empty;
            for (int i = 0; i < currentSplit.Length; i++)
                currentDecimal += (i == 1 ? "." : string.Empty) + currentSplit[i];

            //compare version numbers to determine if updates are available
            if (!double.TryParse(latestDecimal, out var latestNumber))
                return;
            if (latestNumber <= double.Parse(currentDecimal))
            {
                //aready have the latest version
                if (!failSilently)
                    System.Windows.Forms.MessageBox.Show("You already have the lastest version (" + currentVersion + ") of CTM's AATool.", "No Updates Available");
                return;
            }

            using (var dialog = new FUpdate(latestVersion, patchNotes))
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                    RunUpdateAssistant(false);
        }

        public static void RunUpdateAssistant(bool isError = false)
        {
            //start update executable with "return to AATool" flag
            using (var process = new Process())
            {
                process.StartInfo.FileName = "AAUpdate.exe";
                process.StartInfo.Arguments = "-r";
                process.Start();
                Environment.Exit(isError ? 1 : 0);
            }
        }
    }
}
