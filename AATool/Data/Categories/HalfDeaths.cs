using System;
using AATool.Graphics;

namespace AATool.Data.Categories
{
    public class HalfDeaths : AllDeaths
    {
        public override int GetTargetCount() =>
            (int)Math.Ceiling(base.GetTargetCount() / 2.0);

        public override string GetCompletionMessage() =>
            $"Half ({this.GetTargetCount()}) of All {this.Objective} {this.Action}!";

        public override string ViewName => "all_deaths";

        public HalfDeaths() : base()
        {
            this.Name      = "Half Deaths";
            this.Acronym   = "HD";
            this.Objective = "Deaths";
            this.Action    = "Experienced";
        }

        public override void LoadObjectives()
        {
            Tracker.Deaths.RefreshObjectives();
            Tracker.ComplexObjectives.RefreshObjectives();
        }
    }
}
