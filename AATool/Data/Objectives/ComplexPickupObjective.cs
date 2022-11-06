using System;
using AATool.Data.Progress;

namespace AATool.Data.Objectives
{
    public abstract class ComplexPickupObjective : ComplexObjective
    {
        public int Obtained { get; protected set; }
        public abstract int Required { get; }

        public ComplexPickupObjective(string id)
        {
            this.Id = id;
        }

        protected virtual int GetCount(ProgressState progress) =>
            progress.TimesPickedUp(this.Id) - progress.TimesDropped(this.Id);

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.Obtained = Math.Max(0, this.GetCount(progress));
            this.CompletionOverride = this.Obtained >= this.Required;
        }

        protected override void ClearAdvancedState()
        {
            this.Obtained = 0;
        }

        protected override string GetShortStatus() => 
            $"{this.Obtained} / {this.Required}";
    }
}
