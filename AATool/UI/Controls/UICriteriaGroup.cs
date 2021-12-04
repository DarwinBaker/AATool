using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AATool.Data;
using AATool.Data.Progress;
using AATool.Graphics;
using AATool.Net;
using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UICriteriaGroup : UIPanel
    {
        private Advancement advancement;
        private CriteriaSet criteriaGroup;

        private UITextBlock label;
        private UIProgressBar bar;
        private UIButton playerButton;
        private UIButton autoButton;
        private UIPicture mode;
        private UIFlowPanel criteriaPanel;
        private UIFlowPanel playerPanel;
        private UITextBlock singlePlayerMessage;
        private UITextBlock noPlayerMessage;

        private string advancementName;
        private bool hidePlayersPanel;
        private int playersLoggedIn;
        private bool largePlayers;

        public UICriteriaGroup()
        {
            this.BuildFromSourceDocument();
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            UIAdvancement adv  = this.First<UIAdvancement>();
            this.criteriaPanel = this.First<UIFlowPanel>("criteria");
            this.label         = this.First<UITextBlock>("progress");
            this.autoButton    = this.First<UIButton>("auto");
            this.playerButton  = this.First<UIButton>("player_button");
            this.mode          = this.First<UIPicture>("mode");
            this.playerPanel   = this.First<UIFlowPanel>("players");
            this.bar           = this.First<UIProgressBar>("bar");
            this.singlePlayerMessage = this.First<UITextBlock>("single");
            this.noPlayerMessage     = this.First<UITextBlock>("nobody");
            Tracker.TryGetAdvancement(this.advancementName, out this.advancement);

            if (this.advancement is not null)
            {
                this.criteriaGroup = this.advancement.Criteria;
                adv.AdvancementName = this.advancementName;
                if (Config.Main.CompactMode)
                {
                    this.criteriaPanel.Padding = new Margin(8, 8, 8, 16);
                    this.First("advancement_space")?.Collapse();
                    this.RemoveControl(adv);
                }
                else
                {
                    this.criteriaPanel.Padding = new Margin(8, 8, 8, 40);
                }
                this.bar.SetMax(this.advancement.Criteria.Count);
                this.PopulateCriteria();
            }

            if (this.autoButton is not null)
                this.autoButton.OnClick += this.OnClicked;
            this.playerButton.OnClick += this.OnClicked;

            //set up in compact mode
            if (Config.Main.CompactMode)
            {
                //this.First("advancement_space").Collapse();
                this.label.Margin = new Margin(8, 0, 0, 6);
                this.criteriaPanel.CellWidth = Config.PostExplorationUpdate ? 68 : 100;
                this.criteriaPanel.RemoveControl(adv);
                this.bar.Collapse();
            }
            else if (!Config.PostExplorationUpdate)
            {
                this.criteriaPanel.CellWidth = 100;
            }

            this.bar.SkipToValue(this.criteriaGroup.NumberCompletedBy(this.advancement.GetDesignatedPlayer()));
            base.InitializeRecursive(screen);
        }

        private void PopulateCriteria()
        {
            //populate criteria flow panel
            UIControl spacer = this.First("advancement_space");
            this.criteriaPanel.ClearControls();
            if (!Config.Main.CompactMode)
                this.criteriaPanel.AddControl(spacer);

            foreach (KeyValuePair<string, Criterion> criterion in this.advancement.Criteria.All)
            {
                var crit = new UICriterion {
                    AdvancementID = this.advancementName,
                    CriterionID   = criterion.Key
                };
                this.criteriaPanel.AddControl(crit);
            }
        }

        public override void ResizeThis(Rectangle parent)
        {
            base.ResizeThis(parent);

            if (this.bar is not null)
                this.bar.FlexWidth = new Size(this.Content.Width - 48);

            int top = (this.Height - this.singlePlayerMessage.TextBounds.Height) / 2;
            this.singlePlayerMessage.Padding = new Margin(10, 10, top, 0);
            this.noPlayerMessage.Padding = new Margin(10, 10, top, 0);
        }

        private void OnClicked(UIControl sender)
        {
            if (sender == this.playerButton)
            {
                if (this.playerPanel.IsCollapsed)
                    this.SwitchToPlayers();
                else
                    this.hidePlayersPanel = true;
            }
            else if (sender == this.autoButton)
            {
                this.advancement.LinkDesignation();
                this.hidePlayersPanel = true;
            }
            else if (sender is UIButton button && button.Tag is Uuid id)
            {
                this.advancement.UnlinkDesignation();
                this.advancement.Designate(id);
                this.hidePlayersPanel = true;
            }
        }

        protected override void UpdateThis(Time time)
        {
            this.UpdateVisibility();
            this.UpdateProgress();
        }

        private void UpdateProgress()
        {
            Uuid designated = this.advancement.GetDesignatedPlayer();
            string text = $"{this.criteriaGroup.NumberCompletedBy(designated)} / {this.criteriaGroup.Count}";
            if (this.Width >= 120)
                text += $" {this.criteriaGroup.Goal}";

            if (this.Width > 180)
                text += $" ({this.criteriaGroup.PercentCompletedBy(designated)}%)";
            this.label.SetText(text);
            this.bar?.LerpToValue(this.criteriaGroup.NumberCompletedBy(designated));
        }

        private void SwitchToProgress()
        {
            this.playerPanel.ClearControls();
            this.playerPanel.Collapse();
            this.singlePlayerMessage.Collapse();
            this.noPlayerMessage.Collapse();
            this.First("criteria").Expand();
            this.First<UIAdvancement>()?.Expand();
        }

        private void SwitchToPlayers()
        {
            this.PopulatePlayerList();
            this.playerPanel.Expand();
            this.First("criteria").Collapse();
            this.First<UIAdvancement>()?.Collapse();
            this.mode.Collapse();
        }

        private void UpdateVisibility()
        {
            //close players panel
            if (this.hidePlayersPanel)
            {
                this.SwitchToProgress();
                this.hidePlayersPanel = false;
            }

            //toggle link mode icon
            if (this.playerPanel.IsCollapsed && Peer.IsConnected && Peer.IsClient)
                this.mode.Expand();
            else
                this.mode.Collapse();

            //toggle player select button
            if (Peer.IsConnected || (Tracker.SaveState is SaveFolderState.Valid && Tracker.Progress.Players.Any()))
            {
                this.playerButton.Expand();
            }
            else
            {
                this.playerButton.Collapse();
                this.SwitchToProgress();
            }
            this.playerButton.First<UIPlayerFace>().SetPlayer(this.advancement.GetDesignatedPlayer());

            if (!this.playerPanel.IsCollapsed)
            {
                if (!this.hidePlayersPanel)
                {
                    int loggedIn = 0;
                    if (Peer.TryGetLobby(out Lobby lobby))
                        loggedIn = lobby.UserCount;

                    if (Tracker.WorldFolderChanged || loggedIn != this.playersLoggedIn)
                        this.PopulatePlayerList();
                    this.playersLoggedIn = loggedIn;
                }
            }

            if (Peer.IsClient)
            {
                if (this.advancement.DesignationLinked)
                    this.mode.SetTexture("linked");
                else
                    this.mode.SetTexture("unlinked");
            }
        }

        private int GetMaxPlayerScale(int required)
        {
            for (int i = 4; i > 2; i--)
            {
                int maxColumns = this.playerPanel.Width / (i * 18);
                int maxRows    = this.playerPanel.Height / (i * 16);
                if (maxColumns * maxRows >= required)
                    return i;
            }
            return 2;
        }

        private void UpdateAutoButton(int scale)
        {
            //only show autosync button to clients
            if (Peer.IsClient)
                this.autoButton.Expand();
            else
                this.autoButton.Collapse();

            if (this.largePlayers)
                this.autoButton.First("auto_text").Expand();
            else
                this.autoButton.First("auto_text").Collapse();

            this.autoButton.FlexWidth  = new Size((8 * scale) + 6);
            this.autoButton.FlexHeight = new Size((8 * scale) + 6);
            this.autoButton.ResizeRecursive(this.playerPanel.Content);
        }

        private void PopulatePlayerList()
        {
            this.playerPanel.ClearControls();
            this.autoButton?.First<UIGlowEffect>().SkipToBrightness(0);
            this.autoButton?.First<UIGlowEffect>().LerpToBrightness(1);

            //get uuids for all network peers and save files
            HashSet<Uuid> ids = Tracker.GetAllPlayers();
            int required = ids.Count + (Peer.IsClient ? 1 : 0);
            int scale = this.GetMaxPlayerScale(required);
            this.largePlayers = scale > 3;

            //calculate scale
            this.playerPanel.CellWidth = this.largePlayers
                ? 18 * scale
                : 14 * scale;

            this.playerPanel.CellHeight = this.largePlayers
                ? 16 * scale
                : 14 * scale;


            if (Tracker.Progress.Players.Count is 0)
                this.noPlayerMessage.Expand();
            else
                this.noPlayerMessage.Collapse();

            if (ids.Count is 1)
                this.singlePlayerMessage.Expand();
            else
                this.singlePlayerMessage.Collapse();


            int remainder = required is 1
                ? this.playerPanel.Width - this.playerPanel.CellWidth
                : this.playerPanel.Width % this.playerPanel.CellWidth;

            this.playerPanel.Padding = new Margin(remainder / 2, remainder / 2, Math.Min(remainder / 2, 16), 0); 
            this.UpdateAutoButton(scale);
            if (Peer.IsClient)
                this.playerPanel.AddControl(this.autoButton);
            this.playerPanel.ResizeThis(this.Content);

            //create buttons for each player
            foreach (Uuid id in ids)
            {
                if (id == Uuid.Empty)
                    continue;

                var button = new UIButton() {
                    FlexWidth = new Size((8 * scale) + 6),
                    FlexHeight = new Size((8 * scale) + 6),
                    Tag = id,
                };
                var face = new UIPlayerFace(id, this.Root()) { 
                    Scale    = scale,
                    ShowName = true 
                };
                button.AddControl(face);
                button.OnClick += this.OnClicked;
                this.playerPanel.AddControl(button);
            }
            this.playerPanel.ReflowChildren();
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.advancementName = ParseAttribute(node, "advancement", string.Empty);
        }
    }
}
