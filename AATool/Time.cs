using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AATool
{
    public sealed class Time
    {
        private const int SAMPLE_SIZE = 60;

        public double TotalSeconds  { get; private set; }
        public long TotalFrames     { get; private set; }
        public double Delta         { get; private set; }
        public double AverageFPS    { get; private set; }
        public double CurrentFPS    { get; private set; }

        private static readonly Queue<double> SampleBuffer = new ();

        public void Update(GameTime gameTime)
        {
            //get time since last frame
            this.Delta = gameTime.ElapsedGameTime.TotalSeconds;
            this.CurrentFPS = 1 / this.Delta;

            //calculate fps
            SampleBuffer.Enqueue(this.CurrentFPS);
            if (SampleBuffer.Count > SAMPLE_SIZE)
            {
                SampleBuffer.Dequeue();
                this.AverageFPS = SampleBuffer.Average(i => i);
            }
            else
            {
                this.AverageFPS = this.CurrentFPS;
            }

            TotalFrames++;
            TotalSeconds += this.Delta;
        }
    }
}