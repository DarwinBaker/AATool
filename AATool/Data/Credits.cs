using System;
using System.Collections.Generic;
using System.IO;
using AATool.Data.Speedrunning;
using AATool.Net;
using AATool.Net.Requests;
using AATool.Winforms.Controls;

namespace AATool.Data
{
    public static class Credits
    {
        static bool SupporterSheetLoaded;

        public const string Developer = "developer";
        public const string Dedication = "special dedication";
        public const string BetaTester = "beta testers";

        public const string GoldTier = "supporter_gold";
        public const string DiamondTier = "supporter_diamond";
        public const string NetheriteTier = "supporter_netherite";

        //creator of aatool
        public const string Ctm = "60bddec7-939c-4753-a898-cffa33134a4d";
        public const string CtmName = "_ctm";

        //completed the first ever half-heart hardcore all advancements speedrun
        public const string Elysaku = "b2fcb273-9886-4a9b-bd7f-e005816fb7b7";
        public const string ElysakuName = "elysaku";

        //completed 1000 any% RSG speedruns in a row without resetting
        public const string Couriway = "994f9376-3f80-48bc-9e72-ee92f861911d";
        public const string CouriwayName = "couriway";

        //completed 999 any% RSG speedruns in a row without resetting FeelsStrongMan
        public const string MoleyG = "fa1bec35-0585-46c9-8f92-79f8be7cf9bc";
        public const string MoleyGName = "moleyg";

        //manages the aa community leaderboards
        public const string Deadpool = "899c63ac-6590-46c0-b77c-4dae1543f707";
        public const string DeadpoolName = "marvelord";

        //the best minecraft songs ever feelsstrongman
        public const string CaptainSparklez = "5f820c39-5883-4392-b174-3125ac05e38c";
        public const string CaptainSparklezName = "captainsparklez";

        //the founding father of all advancements
        public const string Illumina = "46405168-e9ce-40a0-99a4-0b989a912c77";
        public const string IlluminaName = "illumina";

        //first to complete 100 hardcore runs in a row without dying
        public const string Feinberg = "9a8e24df-4c85-49d6-96a6-951da84fa5c4";
        public const string FeinbergName = "feinberg";

        private static bool Initialized = false;

        public static HashSet<Credit> All { get; private set; } = new();

        private static Dictionary<string, Credit> ByName = new();
        private static Dictionary<Uuid, Credit> ByUuid = new();

        public static readonly HashSet<Credit> Special = new ()
        {
            new (Developer, "", "CTM", new Uuid("60bddec7-939c-4753-a898-cffa33134a4d"), "https://www.patreon.com/_ctm"),
            new (Dedication, "", "Wroxy"),

            new (BetaTester, "", "Elysaku", new Uuid("b2fcb273-9886-4a9b-bd7f-e005816fb7b7"), "https://www.twitch.tv/elysaku"),
            new (BetaTester, "", "Churro :3", "https://www.instagram.com/theelysaku/"),
        };

        private static void EnsureLookupsInitialized()
        {
            if (Initialized)
                return;

            foreach (Credit credit in All)
            {
                ByName[credit.Name.ToLower()] = credit;
                foreach (string alt in credit.AltNames)
                    ByName[alt] = credit;
                foreach (Uuid uuid in credit.Uuids)
                    ByUuid[uuid] = credit;
            }
            Initialized = true;
        }

        public static bool TryGet(Uuid player, out Credit credit)
        {
            EnsureLookupsInitialized();
            return ByUuid.TryGetValue(player, out credit);
        }

        public static bool TryGet(string name, out Credit credit)
        {
            credit = default;
            if (string.IsNullOrEmpty(name))
                return false;

            EnsureLookupsInitialized();
            return ByName.TryGetValue(name.ToLower(), out credit);
        }

        internal static void Initialize()
        {
            TryLoadCached();
            new SpreadsheetRequest("supporters", Paths.Web.SupporterSheet).EnqueueOnce();
        }

        public static bool SyncSheet(string csv)
        {
            if (SupporterSheet.TryParse(csv, out SupporterSheet sheet))
            {
                sheet.GetCredits(out HashSet<Credit> all);
                All = all;
                SupporterSheetLoaded = true;
                sheet.SaveToCache();
            }
            return SupporterSheetLoaded;
        }

        private static bool TryLoadCached()
        {
            string path = Paths.System.SupportersFile;
            if (File.Exists(path))
            {
                try
                {
                    string csv = File.ReadAllText(path);
                    if (SupporterSheet.TryParse(csv, out SupporterSheet sheet))
                    {
                        sheet.GetCredits(out HashSet<Credit> all);
                        All = all;
                        return true;
                    }
                }
                catch
                {
                    //couldn't read cached supporters, move on
                }
            }
            return false;
        }
    }

    public struct Credit
    {
        public string Name;
        public string HighestRole;
        public string CurrentRole;
        public string Link;
        public List<string> AltNames;
        public List<Uuid> Uuids;

        public readonly bool Active => !string.IsNullOrEmpty(this.CurrentRole);

        public Credit(string highestRole, string currentRole, string name, Uuid? uuid = null, string link = "")
        {
            this.Name = name;
            this.HighestRole = RoleKey(highestRole);
            this.CurrentRole = RoleKey(currentRole);
            this.Uuids = new() { uuid ?? Uuid.Empty };
            this.AltNames = new();
            this.Link = link;
        }

        public Credit(string highestRole, string currentRole, string name, string link)
        {
            this.Name = name;
            this.HighestRole = RoleKey(highestRole);
            this.CurrentRole = RoleKey(currentRole);
            this.Uuids = new() { Uuid.Empty };
            this.AltNames = new();
            this.Link = link;
        }

        public Credit(string highestRole, string currentRole, string name, List<string> names, List<Uuid> uuids)
        {
            this.Name = name;
            this.HighestRole = RoleKey(highestRole);
            this.CurrentRole = RoleKey(currentRole);
            this.Uuids = uuids;
            this.AltNames = names;
            this.Link = string.Empty;
        }

        private static string RoleKey(string tier)
        {
            return tier switch {
                "Netherite" => Credits.NetheriteTier,
                "Diamond" => Credits.DiamondTier,
                "Gold" => Credits.GoldTier,
                _ => tier
            };
        }
    }
}
