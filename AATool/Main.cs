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

namespace AATool
{
    public class Main : Game
    {
        public GraphicsDeviceManager GraphicsManager    { get; private set; }
        public AdvancementTracker AdvancementTracker    { get; private set; }
        public StatisticsTracker StatisticsTracker      { get; private set; }

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
            Window.Title = name + " " + version.Major + "." + version.Minor + "." + version.Revision;

            GraphicsManager   = new GraphicsDeviceManager(this);
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60);
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

            //load assets
            SpriteSheet.Initialize(GraphicsDevice);
            FontSet.Initialize(GraphicsDevice);

            //instantiate screens
            altScreens = new Dictionary<Type, Screen>();
            mainScreen = new MainScreen(this);
            AddScreen(new OverlayScreen(this));
            mainScreen.Form.BringToFront();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            time.Update(gameTime);
            display.Update(time);
            gameVersionDetector.Update();
            AdvancementTracker.Update(time);
            StatisticsTracker.Update(time);

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
            string message = "Error: One or more required assets failed to load. Did you move \"AATool.exe\" away from the \"assets\" folder? Try re-installing.";
            string caption = "Missing Assets";
            System.Windows.Forms.MessageBox.Show(message, caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            Environment.Exit(1);
        }
    }
}
