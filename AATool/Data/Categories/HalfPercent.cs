using System;
using System.Collections.Generic;

namespace AATool.Data.Categories
{
    public class HalfPercent : AllAdvancements
    {
        public static readonly new List<string> SupportedVersions = new () {
            "1.16"
        };

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;

        public override int GetTargetCount() =>
            (int)Math.Ceiling(base.GetTargetCount() / 2.0);

        public override string GetCompletionMessage() =>
            $"Half ({this.GetTargetCount()}) of All {this.Objective} {this.Action}!";

        public HalfPercent() : base()
        {
            this.Name = "Half Percent";
            this.Acronym = "HP";
        }
    }
}
