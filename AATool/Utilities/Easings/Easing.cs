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

        public float In()    => Function.In(scaledTime);
        public float Out()   => Function.Out(scaledTime);
        public float InOut() => Function.InOut(scaledTime);

        public Easing(Ease function, double duration, bool startNow = false, bool repeats = false) : base(duration, startNow)
        {
            Function = Functions[function];
            Repeats = repeats;
        }

        public void Play(Ease function)
        {
            Function = Functions[function];
            Reset();
        }

        public override void Update(Time time)
        {
            base.Update(time);
            if (IsExpired && Repeats)
                Reset();
            scaledTime = (float)Math.Min(TimeElapsed / Duration, 1);
        }
    }
}