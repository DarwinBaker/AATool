
namespace AATool.Utilities
{
    public class Timer
    {
        public double Duration { get; private set; }
        public double TimeLeft { get; private set; }

        public bool IsExpired => TimeLeft <= 0;
        
        public void Reset()           => TimeLeft = Duration;
        public void Expire()          => TimeLeft = 0;
        public void Update(Time time) => TimeLeft -= time.Delta;

        public Timer(double duration)
        { 
            Duration = duration; 
            TimeLeft = 0; 
        }
    }
}
