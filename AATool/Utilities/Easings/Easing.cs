using AATool.Utilities.Easings.Functions;
using System;
using System.Collections.Generic;

namespace AATool.Utilities.Easings
{
    public class Easing : Timer
    {
        public static readonly Dictionary<Ease, IEasingFunction> Functions = new Dictionary<Ease, IEasingFunction>()
        {
            { Ease.Back,            new Back()          },
            { Ease.Bounce,          new Bounce()        },
            { Ease.Circular,        new Circular()      },
            { Ease.Cubic,           new Cubic()         },
            { Ease.Elastic,         new Elastic()       },
            { Ease.Exponential,     new Exponential()   },
            { Ease.Quadratic,       new Quadratic()     },
            { Ease.Quartic,         new Quartic()       },
            { Ease.Quintic,         new Quintic()       },
            { Ease.Sinusoidal,      new Sinusoidal()    }
        };

        public IEasingFunction Function { get; set; }
        public bool Repeats             { get; set; }

        private float scaledTime;

        public float In()    => this.Function.In(scaledTime);
        public float Out()   => this.Function.Out(scaledTime);
        public float InOut() => this.Function.InOut(scaledTime);

        public Easing(Ease function, double duration, bool startNow = false, bool repeats = false) : base(duration, startNow)
        {
            this.Function = Functions[function];
            this.Repeats = repeats;
        }

        public void Play(Ease function)
        {
            this.Function = Functions[function];
            this.Reset();
        }

        public override void Update(Time time)
        {
            base.Update(time);
            if (this.IsExpired &&this.Repeats)
                this.Reset();
            this.scaledTime = (float)Math.Min(this.TimeElapsed / this.Duration, 1);
        }
    }
}