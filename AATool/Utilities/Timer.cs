
namespace AATool.Utilities
{
    public class Timer
    {
        public double Duration { get; private set; }
        public double TimeLeft { get; private set; }
        public double TimeElapsed { get; private set; }

        public bool IsExpired => this.TimeLeft <= 0;
        public bool IsRunning => this.TimeLeft > 0;

        public Timer() { }

        public Timer(double duration, bool startNow = false) : this()
        {
            this.Duration = duration;
            if (startNow)
                this.TimeLeft = duration;
        }

        public void SetAndStart(double duration)
        {
            this.Duration = duration;
            this.Reset();
        }

        public void SetAndStop(double duration)
        {
            this.Duration = duration;
            this.Expire();
        }

        public void Reset()
        {
            this.TimeLeft = this.Duration;
            this.TimeElapsed = 0;
        }

        public void Expire()
        {
            this.TimeLeft = 0;
        }

        public virtual void Update(Time time)
        {
            this.TimeElapsed += time.Delta;
            this.TimeLeft    -= time.Delta;
        }
    }
}
