
namespace AATool.Utilities
{
    public class SequenceTimer : Timer
    {
        public int Index { get; private set; }

        private readonly double[] sequence;

        public double Ratio => this.TimeLeft / this.sequence[this.Index];

        public SequenceTimer(params double[] sequence)
        {
            this.sequence = new double[sequence.Length];
            sequence.CopyTo(this.sequence, 0);
            this.Index = -1;
            this.Continue();
        }

        public void Continue() => this.SetAndStart(this.NextDuration());

        private double NextDuration()
        {
            this.Index++;
            if (this.Index == this.sequence.Length)
                this.Index = 0;

            double duration = 0;
            if (this.Index >= 0 && this.Index < this.sequence.Length)
                duration = this.sequence[this.Index];
            return duration;
        }
    }
}
