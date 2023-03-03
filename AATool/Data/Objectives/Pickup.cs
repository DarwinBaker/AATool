using System;
using AATool.Data.Progress;

namespace AATool.Data.Objectives
{
    public class Pickup : ComplexObjective
    {
        public int Obtained { get; protected set; }
        public virtual int Required { get; }

        public override string FullStatus => this.GetLongStatus();
        public override string TinyStatus => this.GetShortStatus();

        public Pickup(string id, string name = null, int required = 1)
        {
            this.Id = id;
            this.Icon = id.Replace("minecraft:", string.Empty);
            this.Name = this.ShortName = name ?? string.Empty;
            this.Required = required;
        }
         
        protected virtual int GetCount(ProgressState progress)
        {
            int count = progress.TimesPickedUp(this.Id)
                + progress.TimesCrafted(this.Id)
                - progress.TimesDropped(this.Id)
                - progress.TimesUsed(this.Id);
            return Math.Max(count, 0);
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.Obtained = this.GetCount(progress);
            this.CompletionOverride = this.Obtained > 0 && this.Obtained >= this.Required;
        }

        protected override void ClearAdvancedState()
        {
            this.Obtained = 0;
        }

        protected override string GetShortStatus()
        {
            if (this.Name is "Estimated" && this.Obtained is 0)
                return "0";
            return this.Name.Contains(" ") ? this.Name : $"{this.Obtained}\0{this.Name}";
        }


        protected override string GetLongStatus()
        {
            if (this.Name is "Estimated" && this.Obtained is 0)
                return "0";
            return this.Name.Contains(" ") ? this.Name : $"{this.Obtained}\0{this.Name}";
        }
    }
}
