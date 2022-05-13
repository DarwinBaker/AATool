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
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

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

        public static string FullTitle      { get; private set; }
        public static string ShortTitle     { get; private set; }
        public static Version Version       { get; private set; }
        public static Random RNG            { get; private set; }

        public static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;

        public static GraphicsDeviceManager GraphicsManager { get; private set; }
        public static GraphicsDevice Device => GraphicsManager?.GraphicsDevice;

        public static UIMainScreen PrimaryScreen { get; private set; }
        public static UIOverlayScreen OverlayScreen { get; private set; }
        public static Dictionary<Type, UIScreen> SecondaryScreens { get; private set; }

        public static bool IsBeta => FullTitle.ToLower().Contains("beta");
        public static bool IsModded => !string.IsNullOrEmpty(ModderName);

        public readonly Time Time;

        private FNotes notesWindow;
        private bool announceUpdate;
        private Uuid playerOne;

        public Main()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version;
            GraphicsManager = new GraphicsDeviceManager(this);
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
            //load assets
            SpriteSheet.Initialize();
            Tracker.Initialize();
            Canvas.Initialize();
            FontSet.Initialize();

            //check for updates in the background
            new UpdateRequest().EnqueueOnce();

            if (Config.Tracking.Filter == ProgressFilter.Solo)
                Player.FetchIdentity(Config.Tracking.SoloFilterName);

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
            OverlayScreen = new UIOverlayScreen(this);
            this.AddScreen(OverlayScreen);
            PrimaryScreen.Form.BringToFront();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.BeginUpdate(this.IsActive);

            this.Time.Update(gameTime);

            Debug.BeginTiming("update_main");

            //check minecraft version
            ActiveInstance.Update(this.Time);
            MinecraftServer.Update(this.Time);
            Tracker.Update(this.Time);
            OpenTracker.Update(this.Time);
            SpriteSheet.Update(this.Time);
            Canvas.Update(this.Time);

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

            //update window title
            if (Config.Main.FpsCap.Changed)
            {
                this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / Config.Main.FpsCap);
                this.UpdateTitle();
            }
            else if (Tracker.ObjectivesChanged 
                || Tracker.InGameTimeChanged 
                || Config.Tracking.FilterChanged
                || Tracker.ProgressChanged)
            {
                this.UpdateTitle();
            }
            
            NetRequest.Update(this.Time);
            Config.ClearAllFlags();
            Tracker.ClearFlags();
            Peer.ClearFlags();
            Input.EndUpdate();

            Debug.EndTiming("update_main");

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Debug.BeginTiming("draw_main");
            lock (SpriteSheet.Atlas)
            {
                //render each secondary screen to its respective viewport
                foreach (UIScreen screen in SecondaryScreens.Values)
                {
                    screen.Prepare();
                    screen.Render();
                    screen.Present();
                }

                //render main screen to default backbuffer
                PrimaryScreen.Prepare();
                PrimaryScreen.Render();
                PrimaryScreen.Present();
                base.Draw(gameTime);
            }
            Debug.EndTiming("draw_main");
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

        private void AppendTitle(string text) => FullTitle += $"   ｜   {text}";

        private void UpdateTitle()
        {
            string name = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            string description = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                .OfType<AssemblyDescriptionAttribute>()
                .FirstOrDefault()?.Description ?? string.Empty;

            ShortTitle = string.IsNullOrWhiteSpace(description) 
                ? $"{name} {Version}"
                : $"{name} {Version} {description}";

            FullTitle = string.IsNullOrWhiteSpace(ModderName) 
                ? ShortTitle 
                : $"{ShortTitle} - UNOFFICIALLY MODIFIED BY: {ModderName}";

            //add category, version, and progress to title
            int completed = Tracker.Category.GetCompletedCount();
            int total = Tracker.Category.GetTargetCount();
            this.AppendTitle($"{Tracker.Category.CurrentVersion} {Tracker.Category.Name} ({completed} / {total})");

            if (Tracker.InGameTime > TimeSpan.Zero)
            {
                if (Tracker.InGameTime.Days is 0)
                {
                    //add igt to title
                    this.AppendTitle($"{Tracker.GetFullIgt()} IGT");
                }
                else if (string.IsNullOrEmpty(Tracker.WorldName))
                {
                    //add world name and days/hours played to title
                    this.AppendTitle($"{Tracker.GetDaysAndHours()}");
                }
                else
                {
                    //add world name and days/hours played to title
                    this.AppendTitle($"{Tracker.WorldName}: {Tracker.GetDaysAndHours()}");
                }
            }
            

            //add igt to title
            HashSet<Uuid> players = Tracker.GetAllPlayers();
            if (players.Count > 1 && Config.Tracking.Filter == ProgressFilter.Combined)
                this.AppendTitle($"Tracking {players.Count} Players");
            else if (Player.TryGetName(Tracker.GetMainPlayer(), out string playerOne))
                this.AppendTitle(playerOne);

            //add fps cap to title
            if (Config.Main.FpsCap < 60)
                this.AppendTitle($"{Config.Main.FpsCap.Value} FPS Cap");

            //assign title to window
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
