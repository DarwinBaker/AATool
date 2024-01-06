using AATool.Configuration;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UICommunityTabs : UIPanel
    {
        UIButton overview;
        UIButton profile;
        UIButton graph;
        UIButton aa;
        UIButton any;
        UIButton challenges;
        UIButton extensions;
        UIButton tracker;

        public UICommunityTabs()
        {
            this.BuildFromTemplate();
            
        }

        public override void InitializeThis(UIScreen screen)
        {
            base.InitializeThis(screen);
            if (this.TryGetFirst(out this.overview, UIMainScreen.OverviewTab))
                this.overview.OnClick += this.ButtonClick;
            if (this.TryGetFirst(out this.profile, UIMainScreen.RunnerProfileTab))
                this.profile.OnClick += this.ButtonClick;
            if (this.TryGetFirst(out this.graph, UIMainScreen.RecordGraphTab))
                this.graph.OnClick += this.ButtonClick;
            if (this.TryGetFirst(out this.aa, UIMainScreen.AARankingsTab))
                this.aa.OnClick += this.ButtonClick;
            if (this.TryGetFirst(out this.any, UIMainScreen.AnyPercentRankingsTab))
                this.any.OnClick += this.ButtonClick;
            if (this.TryGetFirst(out this.challenges, UIMainScreen.ChallengesTab))
                this.challenges.OnClick += this.ButtonClick;
            if (this.TryGetFirst(out this.extensions, UIMainScreen.ExtensionsTab))
                this.extensions.OnClick += this.ButtonClick;
            if (this.TryGetFirst(out this.tracker, UIMainScreen.TrackerTab))
                this.tracker.OnClick += this.ButtonClick;

            UIButton active = UIMainScreen.ActiveTab switch {
                UIMainScreen.OverviewTab => this.overview,
                UIMainScreen.RunnerProfileTab => this.profile,
                UIMainScreen.RecordGraphTab => this.graph,
                UIMainScreen.AARankingsTab => this.aa,
                UIMainScreen.AnyPercentRankingsTab => this.any,
                UIMainScreen.ChallengesTab => this.challenges,
                UIMainScreen.ExtensionsTab => this.extensions,
                _ => null
            };
            this.SetActiveButton(active);
        }

        private void ButtonClick(UIControl sender)
        {
            UIMainScreen.SetActiveTab(sender.Name);
        }

        private void SetActiveButton(UIButton button)
        {
            if (button is null)
                return;
            button.Enabled = false;
            button.UseHighlightedColors = true;
        }
    }
}
