using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AATool
{
    public class Time
    {
        public double Delta         { get; private set; }
        public long TotalFrames     { get; private set; }
        public double TotalSeconds  { get; private set; }
        public double AverageFPS    { get; private set; }
        public double CurrentFPS    { get; private set; }

        private Queue<double> sampleBuffer = new Queue<double>();

        public void Update(GameTime gameTime)
        {
            //get time since last frame
            Delta = gameTime.ElapsedGameTime.TotalSeconds;
            CurrentFPS = 1 / Delta;

            //calculate fps
            sampleBuffer.Enqueue(CurrentFPS);
            if (sampleBuffer.Count > 60)
            {
                sampleBuffer.Dequeue();
                AverageFPS = sampleBuffer.Average(i => i);
            }
            else
                AverageFPS = CurrentFPS;

            TotalFrames++;
            TotalSeconds += Delta;
        }
    }
}