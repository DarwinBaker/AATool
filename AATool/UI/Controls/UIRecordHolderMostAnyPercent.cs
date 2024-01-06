using System.Linq;
using System.Xml;
using AATool.Data.Speedrunning;
using AATool.Net.Requests;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIRecordHolderMostAnyPercent : UIRecordHolder
    {
        string runner;
        int runs;

        public UIRecordHolderMostAnyPercent() : base()
        {
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.Populate();
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);
        }

        protected override void Populate()
        {
            if (this.Board?.Runs?.FirstOrDefault() is not Run wr)
                return;

            new AvatarRequest(wr.Runner).EnqueueOnce();
            this.runner = wr.Runner;
            this.runs = wr.ExtraStat;

            this.Title.SetText("Most Runs");
            this.Subtitle.SetText("No-Reset Any% RSG");

            this.Avatar.SetPlayer(wr.Runner);
            this.SetBadge();

            this.Runner.SetText(wr.Runner);
            this.Details.SetText($"{this.runs:N0} Completions");
        }

        protected override void SetBadge()
        {
            this.Avatar.SetBadge(new NoResetsBadge(this.runner, this.runs));
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.Category = "1K No Reset";
            this.Version = "1.16";
        }
    }
}
