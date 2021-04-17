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
        private Screen mainScreen;
        private Dictionary<Type, Screen> altScreens;
        private FNotes notesWindow;

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
            UpdateHelper.TryCheckForUpdatesAsync();
        }

        protected override void Update(GameTime gameTime)
        {
            time.Update(gameTime);
            display.Update(time);

            //check minecraft version
            GameVersionDetector.Update();

            AdvancementTracker.Update(time);
            StatisticsTracker.Update(time);
            AchievementTracker.Update(time);

            //update each screen
            mainScreen.UpdateRecursive(time);
            foreach (var screen in altScreens.Values)
                screen.UpdateRecursive(time);

            //update notes screen
            if (NotesSettings.Instance.Enabled)
            {
                if (notesWindow == null || notesWindow.IsDisposed)
                {
                    notesWindow = new FNotes();
                    notesWindow.Show();
                }
                else if (TrackerSettings.IsPostExplorationUpdate)
                    notesWindow.UpdateCurrentSave(AdvancementTracker.CurrentSaveName);
                else
                    notesWindow.UpdateCurrentSave(AchievementTracker.CurrentSaveName);
            }
            else if (notesWindow != null && !notesWindow.IsDisposed)
                notesWindow.Close();

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
                    UpdateHelper.RunUpdateAssistant(true);
            }
            else
            {
                string message = "Error: One or more required assets failed to load and the update executable could not be found. Would you like to go to the AATool GitHub page to download and re-install manually?";
                if (System.Windows.Forms.MessageBox.Show(message, caption, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                    Process.Start("https://github.com/DarwinBaker/AATool/releases/latest");
            }
            Environment.Exit(1);
        }
    }
}
