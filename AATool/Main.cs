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


        public static readonly bool IsBeta = false;

        public static bool IsClosing        { get; set; }
        public static string FullTitle      { get; private set; }
        public static string ShortTitle     { get; private set; }
        public static Version Version       { get; private set; }
        public static Random RNG            { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }

        public static UIMainScreen PrimaryScreen { get; private set; }
        public static Dictionary<Type, UIScreen> SecondaryScreens { get; private set; }

        public readonly Time Time;

        private Display display;
        private FNotes notesWindow;

        private bool announceUpdate;

        public static bool IsModded => !string.IsNullOrEmpty(ModderName);

        private void AddScreen(UIScreen screen)
        {
            if (SecondaryScreens.TryGetValue(screen.GetType(), out UIScreen old))
                old.Dispose();
            SecondaryScreens[screen.GetType()] = screen;
        }

        public Main()
        {
            this.UpdateTitle();

            Graphics = new GraphicsDeviceManager(this);
            RNG = new Random();

            if (Config.Main.FpsCap is 0)
                this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60);
            else
                this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / Config.Main.FpsCap);

            this.InactiveSleepTime = TimeSpan.Zero;
            this.IsFixedTimeStep = true;
            this.IsMouseVisible = true;
            this.Time = new Time();
        }

        protected override void Initialize()
        {
            this.display = new Display(Graphics);

            //load assets
            Tracker.Initialize();
            SpriteSheet.Initialize(this.GraphicsDevice);
            FontSet.Initialize(this.GraphicsDevice);
            NetRequest.Enqueue(new UpdateRequest());

            SpriteSheet.DumpAtlas();

            Version.TryParse(Config.Tracker.LastAAToolRun, out Version lastVersion);
            if (lastVersion is null || lastVersion < Version.Parse("1.3.2"))
                this.announceUpdate = true;
            Config.Tracker.LastAAToolRun = Version.ToString();
            Config.Tracker.Save();

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
            this.display.Update(this.Time);

            //check minecraft version
            GameVersionDetector.Update();
            Tracker.Update(this.Time);
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

            if (Config.Main.FpsCapChanged())
            {
                this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / Config.Main.FpsCap);
                this.UpdateTitle();
            }
            else if (Config.Tracker.GameVersionChanged())
            {
                this.UpdateTitle();
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
            lock (SpriteSheet.Atlas)
            {
                //render each secondary screen to its respective viewport
                foreach (UIScreen screen in SecondaryScreens.Values)
                {
                    screen.Prepare(this.display);
                    screen.DrawRecursive(this.display);
                    if (Config.Main.LayoutDebug)
                        screen.DrawDebugRecursive(this.display);
                    screen.Present(this.display);
                }

                //render main screen to default backbuffer
                PrimaryScreen.Prepare(this.display);
                PrimaryScreen.DrawRecursive(this.display);
                if (Config.Main.LayoutDebug)
                    PrimaryScreen.DrawDebugRecursive(this.display);
                PrimaryScreen.Present(this.display);
                base.Draw(gameTime);
            }
        }

        private void ShowUpdateScreen()
        {
            this.AddScreen(new UIUpdateScreen(this, this.announceUpdate));
            UpdateRequest.Suppress = true;
            UpdateRequest.UserInitiated = false;
        }

        private void UpdateTitle()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version;
            string name  = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            string extra = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                .OfType<AssemblyDescriptionAttribute>()
                .FirstOrDefault()?.Description ?? string.Empty;

            ShortTitle = $"{name} {Version.Major}.{Version.Minor}.{Version.Build}";
            FullTitle  = $"{name} {Version}";
            if (!string.IsNullOrWhiteSpace(extra))
                FullTitle += $" {extra}";

            if (!string.IsNullOrWhiteSpace(ModderName))
                FullTitle += $" - UNOFFICIALLY MODIFIED BY: {ModderName}";

            FullTitle += $"   ｜   Minecraft {Config.Tracker.GameVersion}";
            if (Config.Main.FpsCap < 60)
                FullTitle += $"   ｜   {Config.Main.FpsCap} FPS Cap";
            if (PrimaryScreen is not null)
                PrimaryScreen.Form.Text = FullTitle;
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
                    _ = Process.Start(Paths.URL_GITHUB_LATEST);
            }
        }
    }
}
