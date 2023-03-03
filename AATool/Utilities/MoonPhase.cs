using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D9;

namespace AATool.Utilities
{
    public static class MoonPhase
    {
        public enum Phase 
        {
            Full, 
            WaningGibbous, 
            WaningQuarter, 
            WaningCrescent, 
            New, 
            WaxingCrescent, 
            WaxingQuarter, 
            WaxingGibbous 
        }

        public static void Test()
        {
            var phase = new Dictionary<TimeSpan, Phase>();
            var isNight = new Dictionary<TimeSpan, bool>();
            var slimeRates = new Dictionary<TimeSpan, int>();
            var spawnSlimes = new Dictionary<TimeSpan, bool>();
            var spawnBlackCats = new Dictionary<TimeSpan, bool>();
            var end = TimeSpan.FromHours(8);
            for (TimeSpan igt = TimeSpan.Zero; igt < end; igt = igt.Add(TimeSpan.FromMinutes(1)))
            {
                isNight[igt] = IsNightTime(igt);
                if (isNight[igt])
                {
                    phase[igt] = PhaseOf(igt);
                    slimeRates[igt] = SlimeSpawnPercentage(igt);
                    spawnSlimes[igt] = SpawnsSlimes(igt);
                    spawnBlackCats[igt] = SpawnsBlackCats(igt);
                }
            }
        }

        public static bool IsDayTime(TimeSpan igt)
            => Math.Floor(igt.TotalMinutes / 10) % 2 is 0;

        public static bool IsNightTime(TimeSpan igt)
            => !IsDayTime(igt);

        public static Phase PhaseOf(TimeSpan igt) 
            => (Phase)(int)(Math.Floor(igt.TotalMinutes / 20) % 8);

        public static bool SpawnsBlackCats(TimeSpan igt)
            => IsNightTime(igt) && PhaseOf(igt) is Phase.Full;

        public static bool SpawnsSlimes(TimeSpan igt)
            => IsNightTime(igt) && PhaseOf(igt) is not Phase.New;

        public static int SlimeSpawnPercentage(TimeSpan igt)
        {
            return PhaseOf(igt) switch {
                Phase.Full => 100,
                Phase.WaningGibbous => 75,
                Phase.WaxingGibbous => 75,
                Phase.WaningQuarter => 50,
                Phase.WaxingQuarter => 50,
                Phase.WaxingCrescent => 25,
                Phase.WaningCrescent => 25,
                _ => 0,
            };
        }
    }
}
