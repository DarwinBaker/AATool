using AATool.Data;
using AATool.Data.Progress;
using AATool.Graphics;
using AATool.Settings;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AATool.UI.Controls
{
    public class UIRunComplete : UIControl
    {
        private const int PAGE_HOLD_DURATION = 15;
        private const int PAGE_FADE_DURATION = 1;
        private const int PAGE_FULL_DURATION = PAGE_HOLD_DURATION + PAGE_FADE_DURATION * 2;

        private const int STATS_PAGES        = 3;
        private const float FADE_SPEED       = 0.75f;
        private const float SLIDE_SPEED      = 4f;

        private UITextBlock body;
        private UITextBlock version;
        private UITextBlock creator;
        private UITextBlock url;
        private UIPicture ctm;
        private UIPicture bar;
        private Dictionary<string, UIControl> supporters;
        private UIControl statsGeneral;
        private UIControl statsKills;
        private UIControl statsMined;
        private UITextBlock statsMisc;

        private Timer pageTimer;
        private bool showCredits;
        private bool fadeOut;
        private float fade;
        private float glowRotation;
        private float currentX;

        private int statsPageIndex;

        public UIRunComplete()
        {
            this.BuildFromSourceDocument();
            this.pageTimer = new Timer();
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.bar     = this.First<UIPicture>("bar");
            this.version = this.First<UITextBlock>("version");
            this.creator = this.First<UITextBlock>("creator");
            this.creator.SetText("Developed by Darwin Baker");
            this.body    = this.First<UITextBlock>("body");
            this.url     = this.First<UITextBlock>("patreon");
            this.url.SetText(Paths.URL_PATREON_FRIENDLY);
            this.ctm     = this.First<UIPicture>("ctm");
            this.ctm.SetTexture("ctm");

            this.statsMisc    = this.First<UITextBlock>("stats_misc");
            this.statsGeneral = this.First("stats_general");
            this.statsKills   = this.First("stats_kills");
            this.statsMined   = this.First("stats_mined");

            //credits
            this.supporters = new() {
                { "supporters", this.First("supporters")},
                { "beta_testers", this.First("beta_testers")},
                { "special_dedication", this.First("special_dedication") }
            };
            this.First<UITextBlock>("supporters_label")?.SetText("Supporters ----------------------------------------");
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            int targetX = Config.Overlay.RightToLeft ? this.GetRootScreen().Width - this.Width : 5;
            this.currentX = MathHelper.Lerp(this.currentX, targetX, SLIDE_SPEED * (float)time.Delta);
            this.MoveTo(new Point((int)this.currentX, this.Y));

            this.glowRotation += (float)time.Delta * 0.25f;
            this.pageTimer.Update(time);
            if (this.pageTimer.IsExpired)
            {
                if (!this.fadeOut)
                {
                    this.fadeOut = true;
                    this.pageTimer.SetAndStart(PAGE_HOLD_DURATION);
                    this.fade = 0;

                    //swap main window content
                    this.showCredits ^= true;

                    //swap stats page content
                    this.statsPageIndex++;
                    if (this.statsPageIndex >= STATS_PAGES)
                        this.statsPageIndex = 0;
                }
                else
                {
                    this.fadeOut = false;
                    this.pageTimer.SetAndStart(PAGE_FADE_DURATION);
                }
            }

            this.UpdatePageSwapBar();

            this.fade = this.fadeOut
                ? Math.Min(this.fade + (FADE_SPEED * (float)time.Delta), 1)
                : Math.Max(this.fade - (FADE_SPEED * (float)time.Delta), 0);

            Color fadeA = this.showCredits
                ? new (this.fade + 0.1f, this.fade + 0.1f, this.fade + 0.1f, this.fade)
                : new Color(1, 1, 1, 0);

            Color fadeB = this.showCredits
                ? new Color(1, 1, 1, 0)
                : new (this.fade + 0.1f, this.fade + 0.1f, this.fade + 0.1f, this.fade);

            Color fadeMagenta = this.showCredits
                ? new Color(1, 1, 1, 0)
                : new(this.fade + 0.1f, 0, this.fade + 0.1f, this.fade);

            Color fadeCyan = this.showCredits
                ? new(0, (this.fade * 0.85f) + 0.1f, this.fade + 0.1f, this.fade)
                : new Color(1, 1, 1, 0);

            this.FadeControlsRecursive(this.First("fadegroup_overview"), fadeB);
            this.FadeControlsRecursive(this.First("body"), fadeMagenta);
            this.FadeControlsRecursive(this.First("fadegroup_credits"), fadeA);
            this.FadeControlsRecursive(this.url, fadeCyan);

            if (this.showCredits)
                this.bar.SetTint(fadeA * 0.5f);
            else
                this.bar.SetTint(fadeB * 0.5f);

            if (this.showCredits)
            {
                Color temp = fadeA;
                fadeA = fadeB;
                fadeB = temp;
            }

            //cycle stats
            switch (this.statsPageIndex)
            {
                case 0:
                    this.FadeControlsRecursive(this.First("stats_general"), fadeB);
                    this.FadeControlsRecursive(this.First("stats_kills"), fadeA);
                    this.FadeControlsRecursive(this.First("stats_mined"), fadeA);
                    break;
                case 1:
                    this.FadeControlsRecursive(this.First("stats_general"), fadeA);
                    this.FadeControlsRecursive(this.First("stats_kills"), fadeB);
                    this.FadeControlsRecursive(this.First("stats_mined"), fadeA);
                    break;
                case 2:
                    this.FadeControlsRecursive(this.First("stats_general"), fadeA);
                    this.FadeControlsRecursive(this.First("stats_kills"), fadeA);
                    this.FadeControlsRecursive(this.First("stats_mined"), fadeB);
                    break;
            }
        }
        private void UpdatePageSwapBar()
        {
            double remaining = this.pageTimer.TimeLeft;
            if (this.pageTimer.Duration is PAGE_HOLD_DURATION)
                remaining += PAGE_FADE_DURATION;

            double scale = remaining / PAGE_FULL_DURATION;
            this.bar.FlexWidth = new Size((int)(this.bar.MaxWidth.Absolute * scale));
            this.bar.ResizeThis(this.bar.Parent.Content);
        }

        private void FadeControlsRecursive(UIControl panel, Color tint)
        {
            var children = new List<UIControl>();
            panel.GetTreeRecursive(children);
            foreach (UIControl control in children)
            {
                if (control is UITextBlock text)
                    text.SetTextColor(tint);
                else if (control is UIGlowEffect glow)
                    glow.SkipToBrightness(tint.A / 255f);
                else if (control is UIPicture picture)
                    picture.SetTint(tint);
            }
        }

        public void Show()
        {
            this.pageTimer.SetAndStart(PAGE_HOLD_DURATION);
            this.showCredits = false;
            this.fadeOut     = true;
            this.fade        = 1;
            this.statsPageIndex = 0;
            this.MoveTo(new Point(Config.Overlay.RightToLeft ? this.GetRootScreen().Right : -this.Parent.Width, this.Y));
            this.currentX    = this.X;

            string title = $"All {Tracker.AdvancementCount} Advancements Complete!";
            string body = Config.IsPostExplorationUpdate
                ? $" \nMinecraft: Java Edition ({Config.Tracker.GameVersion})\n" +
                    $"All\0Advancements\n\n" +
                    $"{Tracker.InGameTime}\nApproximate IGT\n"
                : $" \nMinecraft: Java Edition\n" +
                    $"All\0Achievements\n\n" +
                    $"{Tracker.InGameTime}\nApproximate IGT\n";

            this.First<UITextBlock>("head").SetText(title);
            this.First<UITextBlock>("head_shadow").SetText(title);
            this.body.SetText(body);
            this.version.SetText(Main.ShortTitle);

            ProgressState prog = Tracker.Progress;
            string space = new (' ', 6);

            /*
            this.statsMisc.Clear();
            this.statsMisc.Append($"Deaths: {prog.Deaths}\n");
            this.statsMisc.Append($"Damage Taken: {prog.DamageTaken}\n");
            this.statsMisc.Append($"Damage Dealt: {prog.DamageDealt}\n");
            this.statsMisc.Append($"Jumps: {string.Format("{0:n0}", prog.Jumps)}\n");
            this.statsMisc.Append($"Nights Slept: {prog.Sleeps}\n");
            this.statsMisc.Append($"Save & Quits: {prog.SaveAndQuits}\n");
            */

            this.First<UITextBlock>("flown").SetText(space      + prog.TotalKilometersFlown);
            this.First<UITextBlock>("bread").SetText(space      + prog.BreadEaten.ToString());
            this.First<UITextBlock>("enchants").SetText(space   + prog.ItemsEnchanted.ToString());
            this.First<UITextBlock>("pearls").SetText(space     + prog.EnderPearlsThrown.ToString());
            this.First<UITextBlock>("temples").SetText(space    + prog.TemplesRaided);

            this.First<UITextBlock>("creepers").SetText(space   + prog.CreepersKilled.ToString());
            this.First<UITextBlock>("drowned").SetText(space    + prog.DrownedKilled.ToString());
            this.First<UITextBlock>("withers").SetText(space    + prog.WitherSkeletonsKilled.ToString());
            this.First<UITextBlock>("fish").SetText(space       + prog.FishCollected.ToString());
            this.First<UITextBlock>("phantoms").SetText(space   + prog.PhantomsKilled.ToString());

            this.First<UITextBlock>("lecterns").SetText(space   + prog.LecternsMined.ToString());
            this.First<UITextBlock>("sugarcane").SetText(space  + prog.SugarcaneCollected.ToString());
            this.First<UITextBlock>("netherrack").SetText(space + prog.NetherrackMined.ToString());
            this.First<UITextBlock>("gold_blocks").SetText(space    + prog.GoldMined.ToString());
            this.First<UITextBlock>("ender_chests").SetText(space   + prog.EnderChestsMined.ToString());

            this.PopulateSupporterLists();
            this.Expand();
        }

        private void PopulateSupporterLists()
        {
            foreach (KeyValuePair<string, UIControl> panel in this.supporters)
            {
                panel.Value.ClearControls();
                if (panel.Key is "supporters")
                    continue;

                var header = new UITextBlock() {
                    FlexWidth  = new Size(400),
                    FlexHeight = new Size(48),
                    HorizontalTextAlign = panel.Value.HorizontalAlign,
                    VerticalTextAlign = VerticalAlign.Top
                };
                header.SetFont("minecraft", 24);
                header.SetText(System.Threading.Thread.CurrentThread.CurrentCulture
                    .TextInfo.ToTitleCase(panel.Key.Replace("_", " ")) 
                    + Environment.NewLine + new string('-', 18));
                panel.Value.AddControl(header);
            }

            foreach (Supporter person in Credits.Supporters)
            {
                bool donor = person.Role.ToLower() is not ("developer" or "beta_testers" or "special_dedication");

                UIControl panel;
                if (donor)
                    panel = this.supporters["supporters"];
                else if (!this.supporters.TryGetValue(person.Role, out panel))
                    continue;

                var supporter = new UITextBlock() {
                    FlexWidth  = new Size(220),
                    FlexHeight = new Size(32),
                    HorizontalTextAlign = panel.HorizontalAlign,
                    VerticalTextAlign = VerticalAlign.Top
                };
                supporter.SetFont("minecraft", 24);

                var tier = new UIPicture() {
                    FlexWidth       = new Size(32),
                    FlexHeight      = new Size(32),
                    Margin          = new Margin(0, 0, -4, 0),
                    HorizontalAlign = panel.HorizontalAlign,
                    VerticalAlign   = VerticalAlign.Top
                };

                if (supporter.HorizontalTextAlign is HorizontalAlign.Left)
                    supporter.SetText($"     {person.Name}");
                else
                    supporter.SetText($"{person.Name}     ");
                supporter.AddControl(tier);

                string icon = person.Role.ToLower() switch {
                    "beta_testers"       => "enchanted_golden_apple",
                    "special_dedication" => "poppy",
                    _                    => "supporter",
                };
                if (icon is "supporter")
                    tier.SetTexture("supporter_" + person.Role);
                else
                    tier.SetTexture(icon);
                panel.AddControl(supporter);
            }
        }
    }
}