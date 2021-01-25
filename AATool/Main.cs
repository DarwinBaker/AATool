using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Reflection;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Trackers;

namespace AATool
{
    public class Main : Game
    {
        public GraphicsDeviceManager GraphicsManager    { get; private set; }
        public AdvancementTracker AdvancementTracker    { get; private set; }
        public StatisticsTracker StatisticsTracker      { get; private set; }

        private Time time;
        private Display display;
        private Screen mainScreen;
        private Dictionary<Type, Screen> altScreens;

        private void AddScreen(Screen screen) => altScreens[screen.GetType()] = screen;

        public Main()
        {
            var version  = Assembly.GetExecutingAssembly().GetName().Version;
            string name  = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            Window.Title = name + " " + version.Major + "." + version.Minor;

            GraphicsManager   = new GraphicsDeviceManager(this);
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60);
            IsFixedTimeStep   = true;
            IsMouseVisible    = true;
        }

        protected override void Initialize()
        {
            //instantiate important objects
            display            = new Display(GraphicsManager);
            time               = new Time();
            AdvancementTracker = new AdvancementTracker();
            StatisticsTracker  = new StatisticsTracker();
            
            //load assets
            TextureSet.Initialize(GraphicsDevice);
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
            AdvancementTracker.Update(time);
            StatisticsTracker.Update(time);

            //update each screen
            mainScreen.UpdateRecursive(time);
            foreach (var screen in altScreens.Values)
                screen.UpdateRecursive(time);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //render each secondary screen to its respective viewport
            foreach (var screen in altScreens.Values)
            {
                screen.Prepare(display);
                screen.DrawRecursive(display);
                screen.Present(display);
            }

            //render main screen to default backbuffer
            mainScreen.Prepare(display);
            mainScreen.DrawRecursive(display);
            mainScreen.Present(display);

            base.Draw(gameTime);
        }
    }
}
