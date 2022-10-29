using System;
using System.Diagnostics;
using AATool.Configuration;
using AATool.Data;
using AATool.Data.Categories;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Badges;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIBlockMessage : UIControl
    {
        private const int DaysBetweenReminders = 3;

        public bool Shown { get; private set; } = false;

        private readonly Timer delayTimer = new (1.5, true);

        private UIButton patreonButton;
        private UIButton closeButton;
        private UIButton thanksButton;

        private UITextBlock message;
        private UITextBlock priceGold;
        private UITextBlock priceDiamond;
        private UITextBlock priceNetherite;

        private UIControl panelButtonsPromo;
        private UIControl panelButtonsThanks;

        private UIAvatar playerGold;
        private UIAvatar playerDiamond;
        private UIAvatar playerNetherite;

        private float backgroundOpacity = 0;
        private float fadeSpeed = 8f;
        private bool badgesInitialized;
        private bool ignore;

        private const string IntroText = "This update was an INSANE amount of work! It took\nhundreds of hours to make this tracker a reality.\n \n";

        private const string PromoText = IntroText +
            "If you enjoy using AATool and are able,\nplease consider joining the Patreon!\n \n" +
            "It helps make awesome updates like this possible,\nyou get exclusive player badges and frames,\nand your name in the tracker's credits!";

        private const string ThanksDeadpool = IntroText +
            "Thank you so much for everything, Deadpool. You have always been\nan incredible supporter of the tracker and an incredible friend.\n \n" +
            "AATool would not be what it is today without you ♥";

        private string ThanksNetherite(string playerName) => IntroText +
            $"Thank you so much, {playerName}\nfor your incredible generosity!\n \n" +
            $"It helps more than you could possibly know.\nPlease enjoy your badges, player frames,\nand your name in the tracker's credits ♥";

        private string ThanksDiamond(string playerName) => IntroText +
            $"Thank you so much, {playerName}\nfor supporting AATool!\n \n" +
            $"You helped make this awesome update possible!\nPlease enjoy your diamond/gold badges, player frames,\nand your name in the tracker's credits ♥";

        private string ThanksGold(string playerName) => IntroText +
            $"Thank you so much, {playerName}\nfor supporting AATool!\n \n" +
            $"You helped make this awesome update possible!\nPlease enjoy your gold badge, player frame,\nand your name in the tracker's credits ♥";

        public bool IsPopupVisible => this.Shown 
            && this.delayTimer.IsExpired 
            && AllBlocks.MainSpritesLoaded 
            && AllBlocks.HelpSpritesLoaded;

        public UIBlockMessage()
        {
            this.BuildFromTemplate();
            this.Layer = Layer.Fore;
        }

        public override void InitializeThis(UIScreen screen)
        {
            base.InitializeThis(screen);

            this.patreonButton = this.First<UIButton>("button_patreon");
            if (this.patreonButton is not null)
                this.patreonButton.OnClick += this.OnClick;

            this.closeButton = this.First<UIButton>("button_close");
            if (this.closeButton is not null)
                this.closeButton.OnClick += this.OnClick;

            this.thanksButton = this.First<UIButton>("button_thanks");
            if (this.thanksButton is not null)
                this.thanksButton.OnClick += this.OnClick;

            this.message = this.First<UITextBlock>("message");

            this.priceGold = this.First<UITextBlock>("price_gold");
            this.priceDiamond = this.First<UITextBlock>("price_diamond");
            this.priceNetherite = this.First<UITextBlock>("price_netherite");

            this.panelButtonsPromo = this.First("panel_buttons_promote");
            this.panelButtonsThanks = this.First("panel_buttons_thanks");

            this.playerGold = this.First<UIAvatar>("main_player_gold");
            this.playerDiamond = this.First<UIAvatar>("main_player_diamond");
            this.playerNetherite = this.First<UIAvatar>("main_player_netherite");

            this.Shown = DateTime.Now > Config.Tracking.LastOpenedAllBlocks.Value.AddDays(DaysBetweenReminders);
            if (Credits.TryGet(Tracker.GetMainPlayer(), out Credit supporter))
            {
                if (supporter.Role is Credits.NetheriteTier)
                    this.Shown = Config.Tracking.LastOpenedAllBlocks == DateTime.MinValue;
            }
            this.First<UIGrid>()?.Collapse();
        }

        private void OnClick(UIControl sender)
        {
            Config.Tracking.LastOpenedAllBlocks.Set(DateTime.Now);
            Config.Tracking.TrySave();
            if (sender == this.patreonButton)
            {
                Process.Start(Paths.Web.PatreonFull);
                this.Shown = false;
                this.Collapse();
            }
            else if (sender == this.closeButton || sender == this.thanksButton)
            {
                this.Shown = false;
                this.Collapse();
            }
        }

        public void Show()
        {
            if (!this.Shown)
                this.backgroundOpacity = 0;
            this.Expand();
            this.Shown = true;
            this.delayTimer.Expire();
        }

        protected override void UpdateThis(Time time)
        {
            this.delayTimer.Update(time);
            if (this.IsPopupVisible)
            {
                this.First<UIGrid>()?.Expand();
                base.UpdateThis(time);

                if (!this.badgesInitialized || Tracker.MainPlayerChanged)
                {
                    Uuid currentPlayer = Tracker.GetMainPlayer();
                    this.playerGold.SetPlayer(currentPlayer);
                    this.playerDiamond.SetPlayer(currentPlayer);
                    this.playerNetherite.SetPlayer(currentPlayer);
                    this.InitializeBadges(currentPlayer);

                    if (!Player.TryGetName(currentPlayer, out string name))
                        name = Config.Tracking.LastPlayer;
                    if (!Credits.TryGet(currentPlayer, out Credit supporter))
                        Credits.TryGet(name, out supporter);

                    string role = supporter.Role;
                    this.panelButtonsPromo.SetVisibility(role is not Credits.NetheriteTier);
                    this.panelButtonsThanks.SetVisibility(role is Credits.NetheriteTier);

                    this.priceGold.SetText("$5");
                    this.priceDiamond.SetText("$15");
                    this.priceNetherite.SetText("$50");

                    if (currentPlayer == Credits.Deadpool || name.ToLower() == Credits.DeadpoolName)
                    {
                        this.message.SetText(ThanksDeadpool);
                        this.priceNetherite.SetText("Unlocked!");
                        this.priceDiamond.SetText("Unlocked!");
                        this.priceGold.SetText("Unlocked!");
                    }
                    else if (role is Credits.NetheriteTier)
                    {
                        this.message.SetText(this.ThanksNetherite(Leaderboard.GetNickName(name)));
                        this.priceNetherite.SetText("Unlocked!");
                        this.priceDiamond.SetText("Unlocked!");
                        this.priceGold.SetText("Unlocked!");
                    }
                    else if (role is Credits.DiamondTier)
                    {
                        this.message.SetText(this.ThanksDiamond(Leaderboard.GetNickName(name)));
                        this.priceDiamond.SetText("Unlocked!");
                        this.priceGold.SetText("Unlocked!");
                    }
                    else if (role is Credits.GoldTier)
                    {
                        this.message.SetText(this.ThanksGold(Leaderboard.GetNickName(name)));
                        this.priceGold.SetText("Unlocked!");
                    }
                    else
                    {
                        this.message.SetText(PromoText);
                    }

                    if (string.IsNullOrEmpty(role))
                        this.patreonButton.SetText("   Join The Patreon!");
                    else
                        this.patreonButton.SetText("   Visit The Patreon!");

                    if (string.IsNullOrEmpty(role))
                        this.closeButton.SetText("   Not Now, Thanks!");
                    else
                        this.closeButton.SetText("   Dismiss");
                }

                float targetOpacity = this.Shown ? 0.75f : 0;
                this.backgroundOpacity = MathHelper.Lerp(this.backgroundOpacity, targetOpacity, (float)(this.fadeSpeed * time.Delta));
            }
            else
            {
                this.First<UIGrid>()?.Collapse();
            }
        }

        private void InitializeBadges(Uuid currentPlayer)
        {
            this.playerGold.RefreshBadge();
            this.playerDiamond.RefreshBadge();
            this.playerNetherite.RefreshBadge();

            Player.TryGetName(currentPlayer, out string name);
            if (Leaderboard.TryGetRank(name, "All Advancements", "1.16", out int rank) && rank > 3)
            {
                this.playerGold.SetBadge(new RankBadge(rank, "All Advancements", "1.16", true, "gold"));
                this.playerDiamond.SetBadge(new RankBadge(rank, "All Advancements", "1.16", true, "diamond"));
                this.playerNetherite.SetBadge(new RankBadge(rank, "All Advancements", "1.16", true, "netherite"));
            }
            else
            {
                this.playerGold.SetBadge(new SupporterBadge(Credits.GoldTier));
                this.playerDiamond.SetBadge(new SupporterBadge(Credits.DiamondTier));
                this.playerNetherite.SetBadge(new SupporterBadge(Credits.NetheriteTier));
            }
            this.badgesInitialized = true;
        }

        public override void DrawRecursive(Canvas canvas)
        {
            if (this.Shown)
                base.DrawRecursive(canvas);
        }

        public override void DrawThis(Canvas canvas)
        {
            //dim the screen
            canvas.DrawRectangle(Main.Device.Viewport.Bounds, Color.Black * this.backgroundOpacity, null, 0, Layer.Fore);
        }
    }
}
