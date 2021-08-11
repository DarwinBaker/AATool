using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Reflection;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Settings;
using System.Diagnostics;
using AATool.Utilities;
using System.IO;
using AATool.Winforms.Forms;
using System.Windows.Forms;
using AATool.Net;
using System.Threading.Tasks;
using System.Linq;

namespace AATool
{
    public class Main : Game
    {
        /*==========================================================
        ||                                                        ||
        ||      --------------------------------------------      ||
        ||          { Welcome to the AATool source code! }        ||
        ||      --------------------------------------------      ||
        ||             Developed by Darwin 'CTM' Baker            ||
        ||                                                        ||
        ||                                                        ||
        ||       //To anyone building modified versions of        ||
        ||       //this program, please put your name here        ||
        ||       //to help differentiate unofficial builds.       ||
        ||                                                        ||
        ||       */const string MODDER_NAME = "";/*               ||
        ||                                                        ||
        ||       //Thanks!                                        ||
        ||                                                        ||
        ====================================================HDWGH?*/


        public static readonly bool IsBeta = true;

        public static bool IsClosing          { get; set; }
        public static string FullTitle        { get; private set; }
        public static string ShortTitle       { get; private set; }
        public static string Version          { get; private set; }
        public static Random RNG              { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }

        public readonly Time Time;

        private Display display;
        private UIScreen mainScreen;
        private Dictionary<Type, UIScreen> altScreens;
        private FNotes notesWindow;

        private void AddScreen(UIScreen screen) => this.altScreens[screen.GetType()] = screen;

        public Main()
        {
            this.InitializeMetaData();

            Graphics                = new GraphicsDeviceManager(this);
            this.TargetElapsedTime  = TimeSpan.FromSeconds(1.0 / 60);
            this.InactiveSleepTime  = TimeSpan.Zero;
            this.IsFixedTimeStep    = true;
            this.IsMouseVisible     = true;
            this.Time               = new Time();

            RNG = new Random();
        }

        protected override void Initialize()
        {
            //instantiate important objects
            this.display = new Display(Graphics);

            //load assets
            Tracker.Initialize();
            SpriteSheet.Initialize(this.GraphicsDevice);
            FontSet.Initialize(this.GraphicsDevice);
            //SpriteSheet.Atlas.SaveAsPng(File.Create("C:/files/pictures/atlas_test.png"), 2048, 2048);

            //instantiate screens
            this.altScreens = new ();
            this.mainScreen = new UIMainScreen(this);
            this.AddScreen(new UIOverlayScreen(this));
            this.mainScreen.Form.BringToFront();

            Task.Factory.StartNew(() => UpdateHelper.CheckAsync());
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            this.Time.Update(gameTime);
            this.display.Update(this.Time);

            //check minecraft version
            GameVersionDetector.Update();
            Tracker.TryUpdate(this.Time);

            //update each screen
            this.mainScreen.UpdateRecursive(this.Time);
            foreach (UIScreen screen in this.altScreens.Values)
                screen.UpdateRecursive(this.Time);

            //update notes screen
            if (NotesSettings.Instance.Enabled)
            {
                if (this.notesWindow is null || this.notesWindow.IsDisposed)
                {
                    this.notesWindow = new FNotes();
                    this.notesWindow.Show();
                }
                else
                {
                    this.notesWindow.UpdateCurrentSave(Tracker.WorldName);
                }
            }
            else if (this.notesWindow is not null && !this.notesWindow.IsDisposed)
            {
                this.notesWindow.Close();
            }

            NetRequest.Update(this.Time);
            SpriteSheet.Update(this.Time);
            base.Update(gameTime);
            Config.ClearFlags();
            Tracker.ClearFlags();
            Peer.ClearFlags();
        }

        protected override void Draw(GameTime gameTime)
        {
            lock (SpriteSheet.AtlasLock)
            {
                //render each secondary screen to its respective viewport
                foreach (UIScreen screen in this.altScreens.Values)
                {
                    screen.Prepare(this.display);
                    screen.DrawRecursive(this.display);
                    if (Config.Main.LayoutDebug)
                        screen.DrawDebugRecursive(this.display);
                    screen.Present(this.display);
                }

                //render main screen to default backbuffer
                this.mainScreen.Prepare(this.display);
                this.mainScreen.DrawRecursive(this.display);
                if (Config.Main.LayoutDebug)
                    this.mainScreen.DrawDebugRecursive(this.display);
                this.mainScreen.Present(this.display);
                base.Draw(gameTime);
            }
        }

        private void InitializeMetaData()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string name     = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            string extra    = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                .OfType<AssemblyDescriptionAttribute>()
                .FirstOrDefault()?.Description ?? string.Empty;

            ShortTitle = $"{name} {version.Major}.{version.Minor}.{version.Build}";
            FullTitle = string.IsNullOrWhiteSpace(extra)
                ? ShortTitle
                : $"{ShortTitle} {extra}";

            if (!string.IsNullOrWhiteSpace(MODDER_NAME))
                FullTitle += " - UNOFFICIALLY MODIFIED BY: " + MODDER_NAME;

            Version = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        public static void QuitBecause(string reason, Exception exception = null)
        {
            //show user a message and quit if for some reason the program fails to load properly
            string caption = "Missing Assets";
            if (File.Exists("AAUpdate.exe"))
            {
                string message = $"One or more required assets failed to load!\n{reason}\n\nWould you like to repair your installation?";
                if (exception is not null)
                    message += $"\n\n{exception.GetType()}:{exception.StackTrace}";
                DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result is DialogResult.Yes)
                    UpdateHelper.RunUpdateAssistant(true);
            }
            else
            {
                string message = $"One or more required assets failed to load and the update executable could not be found!\n{reason}\n\nWould you like to go to the AATool GitHub page to download and re-install manually?";
                if (exception is not null)
                    message += $"\n\n{exception.GetType()}:{exception.StackTrace}";
                DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result is DialogResult.Yes)
                    _ = Process.Start(Paths.URL_GITHUB_LATEST);
            }
            Environment.Exit(1);
        }
    }
}
