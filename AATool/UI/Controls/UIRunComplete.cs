using System;
using System.Collections.Generic;
using AATool.Configuration;
using AATool.Data;
using AATool.Data.Progress;
using AATool.Net;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIRunComplete : UIControl
    {
        private const float PageHoldDuration = 20;
        private const float PageFadeDuration = 1;
        private const float PageFullDuration = PageHoldDuration + (PageFadeDuration * 2);

        private const float DonorsFadeHeight = 168;
        private const float DonorsFadeSpeed = 4;
        private const float DonorsFadeDuration = 0.75f;
        private const float DonorDuration = PageHoldDuration - (DonorsFadeDuration * 3);
        private const float LargeDonorsDuration = DonorDuration * 0.6f;
        private const float SmallDonorsDuration = DonorDuration * 0.4f;

        private const float SlideSpeed = 4f;
        private const float FadeSpeed = 0.75f;
        private const int StatsPages = 3;

        private UITextBlock body;
        private UITextBlock version;
        private UITextBlock creator;
        private UITextBlock url;
        private UIPicture ctm;
        private UIPicture bar;
        private Dictionary<string, UIControl> supporters;
        private UIControl netheriteSupporters;
        private UIControl diamondSupporters;
        private UIControl goldSupporters;
        private UIControl donorCover;

        private readonly Timer pageTimer = new ();
        private readonly SequenceTimer donorsTimer = new (
            LargeDonorsDuration,  //show netherite/diamond supporters
            DonorsFadeDuration,   //fade out netherite/diamond supporters
            DonorsFadeDuration,   //hold fade cover
            DonorsFadeDuration,   //fade in gold supporters
            SmallDonorsDuration); //show gold supporters

        private bool showCredits;
        private bool fadeOut;
        private float fade;
        private float donorCoverHeight;
        private float glowRotation;
        private float currentX;

        private int statsPageIndex;

        public UIRunComplete()
        {
            this.BuildFromTemplate();
        }

        public override void InitializeThis(UIScreen screen)
        {
            //aatool version
            this.version = this.First<UITextBlock>("version");

            //developer
            this.creator = this.First<UITextBlock>("creator");
            this.creator.SetText("Developed by Darwin Baker");
            this.url = this.First<UITextBlock>("patreon");
            this.url.SetText(Paths.Web.PatreonShort);
            this.ctm = this.First<UIPicture>("ctm");
            this.ctm.SetTexture("ctm");

            //main message
            this.body = this.First<UITextBlock>("body");

            //page flip indicator
            this.bar = this.First<UIPicture>("bar");

            //credits
            this.donorCover = this.First("donor_fade");
            this.netheriteSupporters = this.First("supporters_netherite");
            this.diamondSupporters = this.First("supporters_diamond");
            this.goldSupporters = this.First("supporters_gold");
            this.supporters = new () 
            {
                { "supporter_netherite", this.netheriteSupporters },
                { "supporter_diamond", this.diamondSupporters },
                { "supporter_gold", this.goldSupporters },
                { "beta testers", this.First("beta_testers")},
                { "special dedication", this.First("special_dedication") }
            };
        }

        protected override void UpdateThis(Time time)
        {
            int targetX = Config.Overlay.RightToLeft ? this.Root().Width - this.Width : 5;
            this.currentX = MathHelper.Lerp(this.currentX, targetX, SlideSpeed * (float)time.Delta);
            this.MoveTo(new Point((int)this.currentX, this.Y));

            this.glowRotation += (float)time.Delta * 0.25f;
            this.pageTimer.Update(time);
            if (this.pageTimer.IsExpired)
            {
                if (!this.fadeOut)
                {
                    this.fadeOut = true;
                    this.pageTimer.SetAndStart(PageHoldDuration);
                    this.fade = 0;

                    //swap main window content
                    this.showCredits ^= true;

                    //swap stats page content
                    this.statsPageIndex++;
                    if (this.statsPageIndex >= StatsPages)
                        this.statsPageIndex = 0;
                }
                else
                {
                    this.fadeOut = false;
                    this.pageTimer.SetAndStart(PageFadeDuration);
                }
            }

            this.UpdatePageSwapBar();

            this.fade = this.fadeOut
                ? Math.Min(this.fade + (FadeSpeed * (float)time.Delta), 1)
                : Math.Max(this.fade - (FadeSpeed * (float)time.Delta), 0);

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

            this.UpdateDonorFade(time);
        }

        private void UpdateDonorFade(Time time)
        {
            if (!this.showCredits)
            {
                if (this.donorsTimer.IsExpired)
                    this.donorsTimer.StartFromBeginning();
                return;
            }

            this.donorsTimer.Update(time);
            if (this.donorsTimer.IsExpired && this.donorsTimer.Index < 4)
                this.donorsTimer.Continue();
            
            switch (this.donorsTimer.Index)
            {
                case 0:
                    this.netheriteSupporters.Expand();
                    this.diamondSupporters.Expand();
                    this.goldSupporters.Collapse();
                    this.donorCover.Expand();
                    this.donorCoverHeight = 0;
                    break;
                case 2:
                    this.donorCoverHeight = MathHelper.Lerp(this.donorCoverHeight, DonorsFadeHeight, (float)(time.Delta * DonorsFadeSpeed));
                    break;
                case 3:
                    this.netheriteSupporters.Collapse();
                    this.diamondSupporters.Collapse();
                    this.goldSupporters.Expand();
                    this.donorCoverHeight = MathHelper.Lerp(this.donorCoverHeight, 0, (float)(time.Delta * DonorsFadeSpeed));
                    break;
                case 4:
                    this.donorCover.Collapse();
                    break;
            }
            this.donorCover.FlexHeight = new(this.donorCoverHeight);
            this.donorCover.ResizeThis(this.donorCover.Parent.Inner);
        }

        private void UpdatePageSwapBar()
        {
            double remaining = this.pageTimer.TimeLeft;
            if (this.pageTimer.Duration is PageHoldDuration)
                remaining += PageFadeDuration;

            double scale = remaining / PageFullDuration;
            this.bar.FlexWidth = new Size((int)(this.bar.MaxWidth.Absolute * scale));
            this.bar.ResizeThis(this.bar.Parent.Inner);
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
            this.pageTimer.SetAndStart(PageHoldDuration);
            this.showCredits = false;
            this.fadeOut = true;
            this.fade = 1;
            this.statsPageIndex = 0;
            this.MoveTo(new Point(Config.Overlay.RightToLeft ? this.Root().Right : -this.Parent.Width, this.Y));
            this.currentX = this.X;
            this.version.SetText(Main.ShortTitle);

            string title = Tracker.Category.GetCompletionMessage();
            string body = "";
            if (!Peer.IsRunning && Player.TryGetName(Tracker.GetMainPlayer(), out string name))
                body = $"{name} Has Completed\n";
            body += $"Minecraft: Java Edition ({Tracker.Category.CurrentVersion})\n" +
                $"{Tracker.Category.Name.Replace(" ", "\0")}\n \n" +
                $"{Tracker.GetFullIgt()}\nApproximate IGT\n";

            this.First<UITextBlock>("head").SetText(title);
            this.First<UITextBlock>("head_shadow").SetText(title);
            this.body.SetText(body);

            WorldState state = Tracker.State;
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

            this.First<UITextBlock>("flown").SetText(space      + state.KilometersFlown);
            this.First<UITextBlock>("bread").SetText(space      + state.TimesUsed("minecraft:bread"));
            this.First<UITextBlock>("enchants").SetText(space   + state.ItemsEnchanted);
            this.First<UITextBlock>("pearls").SetText(space     + state.TimesUsed("minecraft:ender_pearl"));
            this.First<UITextBlock>("temples").SetText(space    + state.TimesMined("minecraft:tnt") / 9);

            this.First<UITextBlock>("creepers").SetText(space   + state.TimesKilled("minecraft:creeper"));
            this.First<UITextBlock>("drowned").SetText(space    + state.TimesKilled("minecraft:drowned"));
            this.First<UITextBlock>("withers").SetText(space    + state.TimesKilled("minecraft:wither_skeleton"));
            this.First<UITextBlock>("fish").SetText(space       + (state.TimesKilled("minecraft:cod") + state.TimesKilled("minecraft:salmon")));
            this.First<UITextBlock>("phantoms").SetText(space   + state.TimesKilled("minecraft:phantom"));

            this.First<UITextBlock>("lecterns").SetText(space   + state.TimesMined("minecraft:lectern"));
            this.First<UITextBlock>("sugarcane").SetText(space  + state.TimesPickedUp("minecraft:sugarcane"));
            this.First<UITextBlock>("netherrack").SetText(space + state.TimesMined("minecraft:netherrack"));
            this.First<UITextBlock>("gold_blocks").SetText(space    + state.TimesMined("minecraft:gold_blocks"));
            this.First<UITextBlock>("ender_chests").SetText(space   + state.TimesMined("minecraft:ender_chest"));

            this.PopulateSupporterLists();
            this.Expand();
        }

        private void PopulateSupporterLists()
        {
            foreach (KeyValuePair<string, UIControl> panel in this.supporters)
            {
                panel.Value.ClearControls();
                if (!panel.Key.Contains("supporter"))
                {
                    var header = new UITextBlock() {
                        FlexWidth  = new Size(400),
                        FlexHeight = new Size(48),
                        HorizontalTextAlign = panel.Value.HorizontalAlign,
                        VerticalTextAlign = VerticalAlign.Top
                    };
                    header.SetFont("minecraft", 24);
                    panel.Value.AddControl(header);

                    if (panel.Key is "beta testers")
                    {
                        header.SetText("Tested by Elysaku & his cat Churro");
                        var elysaku = new UIPicture() {
                            FlexWidth = new (40),
                            FlexHeight = new (40),
                            Margin = new (40, 0, 0, -4),
                            HorizontalAlign = HorizontalAlign.Left,
                            VerticalAlign = VerticalAlign.Bottom,
                        };
                        elysaku.SetTexture("elysaku");
                        panel.Value.AddControl(elysaku);
                        var churro = new UIPicture() {
                            FlexWidth = new (40),
                            FlexHeight = new (40),
                            Margin = new (0, 40, 0, -4),
                            Padding = new Margin(4, 4, 8, 0),
                            HorizontalAlign = HorizontalAlign.Right,
                            VerticalAlign = VerticalAlign.Bottom,
                        };
                        churro.SetTexture("tabby");
                        panel.Value.AddControl(churro);
                    }
                    else if (panel.Key is "special dedication")
                    {
                        header.SetText("Dedicated to my lovely gf Wroxy");
                        var heart = new UIPicture() {
                            FlexWidth = new (36),
                            FlexHeight = new (36),
                            Margin = new (0, 0, 0, -4),
                            VerticalAlign = VerticalAlign.Bottom,
                        };
                        heart.SetTexture("shiny_heart");
                        panel.Value.AddControl(heart);
                    }
                }
            }

            foreach (Credit person in Credits.All)
            {
                bool donor = person.Role.ToLower() is not ("developer" or "beta testers" or "special dedication");
                if (!donor)
                    continue;

                UIControl panel = this.supporters[person.Role];
                var supporter = new UITextBlock() {
                    FlexWidth  = new Size(donor ? 180 : 220),
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

                tier.SetTexture(person.Role);
                if (supporter.HorizontalTextAlign is HorizontalAlign.Left)
                    supporter.SetText($"     {person.Name}");
                else
                    supporter.SetText($"{person.Name}     ");
                
                supporter.AddControl(tier);
                panel.AddControl(supporter);
            }
        }
    }
}