using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Reflection;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Configuration;
using System.Diagnostics;
using AATool.Utilities;
using System.IO;
using AATool.Winforms.Forms;
using System.Windows.Forms;
using AATool.Net;
using System.Linq;
using AATool.Saves;
using AATool.Net.Requests;

namespace AATool
{
    public class Main : Game
    {
        /*==========================================================
        ||                                                        ||
        ||      --------------------------------------------      ||
        ||         { Welcome to the AATool source code! }         ||
        ||      --------------------------------------------      ||
        ||             Developed by Darwin 'CTM' Baker            ||
        ||                                                        ||
        ||                                                        ||
        ||       //To anyone building modified versions of        ||
        ||       //this program, please put your name here        ||
        ||       //to help differentiate unofficial builds        ||
        ||                                                        ||
        ||       */const string ModderName = "";/*                ||
        ||                                                        ||
        ||       //Thanks!                                        ||
        ||                                                        ||
        ====================================================HDWGH?*/

        public static bool IsClosing        { get; set; }
        public static string FullTitle      { get; private set; }
        public static string ShortTitle     { get; private set; }
        public static Version Version       { get; private set; }
        public static Random RNG            { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }

        public static UIMainScreen PrimaryScreen { get; private set; }
        public static Dictionary<Type, UIScreen> SecondaryScreens { get; private set; }

        public readonly Time Time;

        private Canvas canvas;
        private FNotes notesWindow;

        private bool announceUpdate;

        public static bool IsBeta => FullTitle.ToLower().Contains("beta");
        public static bool IsModded => !string.IsNullOrEmpty(ModderName);

        public Main()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version;
            Graphics = new GraphicsDeviceManager(this);
            RNG = new Random();

            Config.Initialize();

            this.TargetElapsedTime = Config.Main.FpsCap == 0 
                ? TimeSpan.FromSeconds(1.0 / 60) 
                : TimeSpan.FromSeconds(1.0 / Config.Main.FpsCap);
            this.InactiveSleepTime = TimeSpan.Zero;
            this.IsFixedTimeStep = true;
            this.IsMouseVisible = true;
            this.Time = new Time();
        }

        protected override void Initialize()
        {
            this.canvas = new Canvas(Graphics);

            //load assets
            Tracker.Initialize();
            SpriteSheet.Initialize(this.GraphicsDevice);
            FontSet.Initialize(this.GraphicsDevice);
            NetRequest.Enqueue(new UpdateRequest());

            //check build number of last aatool session
            Version.TryParse(Config.Tracking.LastSession, out Version lastSession);
            if (lastSession is null || lastSession < Version.Parse("1.3.2"))
                this.announceUpdate = true;
            Config.Tracking.LastSession.Set(Version.ToString());
            Config.Tracking.Save();

            this.UpdateTitle();

            //instantiate screens
            SecondaryScreens = new ();
            PrimaryScreen = new UIMainScreen(this);
            this.AddScreen(new UIOverlayScreen(this));
            PrimaryScreen.Form.BringToFront();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            this.Time.Update(gameTime);
            this.canvas.Update(this.Time);

            //check minecraft version
            GameVersionDetector.Update();
            Tracker.TryUpdate(this.Time);
            SftpSave.Update(this.Time);

            //update visibilty of update popup
            if (UpdateRequest.IsDone && !UpdateRequest.Suppress)
            {
                if (this.announceUpdate || UpdateRequest.UserInitiated || UpdateRequest.UpdatesAreAvailable())
                    this.ShowUpdateScreen();
            }

            //update each screen
            PrimaryScreen.UpdateRecursive(this.Time);
            foreach (UIScreen screen in SecondaryScreens.Values)
                screen.UpdateRecursive(this.Time);

            //update notes screen
            if (Config.Notes.Enabled)
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

            if (Config.Main.FpsCap.Changed)
            {
                this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / Config.Main.FpsCap);
                this.UpdateTitle();
            }
            else if (Tracker.ObjectivesChanged || Tracker.InGameTimeChanged)
            {
                this.UpdateTitle();
            }
            
            NetRequest.Update(this.Time);
            SpriteSheet.Update(this.Time);
            base.Update(gameTime);
            Config.ClearAllFlags();
            Tracker.ClearFlags();
            Peer.ClearFlags();
        }

        protected override void Draw(GameTime gameTime)
        {
            lock (SpriteSheet.Atlas)
            {
                //render each secondary screen to its respective viewport
                foreach (UIScreen screen in SecondaryScreens.Values)
                {
                    screen.Prepare(this.canvas);
                    screen.DrawRecursive(this.canvas);
                    screen.Present(this.canvas);
                }

                //render main screen to default backbuffer
                PrimaryScreen.Prepare(this.canvas);
                PrimaryScreen.DrawRecursive(this.canvas);
                PrimaryScreen.Present(this.canvas);
                base.Draw(gameTime);
            }
        }

        private void AddScreen(UIScreen screen)
        {
            if (SecondaryScreens.TryGetValue(screen.GetType(), out UIScreen old))
                old.Dispose();
            SecondaryScreens[screen.GetType()] = screen;
        }

        private void ShowUpdateScreen()
        {
            this.AddScreen(new UIUpdateScreen(this, this.announceUpdate));
            UpdateRequest.Suppress = true;
            UpdateRequest.UserInitiated = false;
        }

        private void UpdateTitle()
        {
            string name  = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            string extra = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                .OfType<AssemblyDescriptionAttribute>()
                .FirstOrDefault()?.Description ?? string.Empty;

            ShortTitle = $"{name} {Version}";
            if (!string.IsNullOrWhiteSpace(extra))
                ShortTitle += $" {extra}";

            FullTitle = ShortTitle;
            if (!string.IsNullOrWhiteSpace(ModderName))
                FullTitle += $" - UNOFFICIALLY MODIFIED BY: {ModderName}";

            FullTitle += $"   ｜   {Tracker.Category.CurrentVersion} {Tracker.Category.Name}";
            if (Tracker.InGameTime > TimeSpan.Zero)
                FullTitle += $"   ｜   { Tracker.InGameTime:hh':'mm':'ss} IGT";
            if (Config.Main.FpsCap < 60)
                FullTitle += $"   ｜   {Config.Main.FpsCap.Value} FPS Cap";
            if (PrimaryScreen is not null)
                PrimaryScreen.Form.Text = "  " + FullTitle;

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
                    UpdateHelper.RunAAUpdate(1);
            }
            else
            {
                string message = $"One or more required assets failed to load and the update executable could not be found!\n{reason}\n\nWould you like to go to the AATool GitHub page to download and re-install manually?";
                if (exception is not null)
                    message += $"\n\n{exception.GetType()}:{exception.StackTrace}";
                DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result is DialogResult.Yes)
                    _ = Process.Start(Paths.Web.LatestRelease);
            }
        }
    }
}
