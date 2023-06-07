using System;
using System.Collections.Generic;
using AATool.Net;
using AATool.Winforms.Controls;

namespace AATool.Data
{
    public static class Credits
    {
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

        private static Dictionary<string, Credit> ByName = new Dictionary<string, Credit>();
        private static Dictionary<Uuid, Credit> ByUuid = new Dictionary<Uuid, Credit>();

        public static readonly HashSet<Credit> All = new ()
        {
            new (Developer, "CTM", new Uuid("60bddec7-939c-4753-a898-cffa33134a4d"), "https://www.patreon.com/_ctm"),
            new (Dedication, "Wroxy"),

            new (BetaTester, "Elysaku", new Uuid("b2fcb273-9886-4a9b-bd7f-e005816fb7b7"), "https://www.twitch.tv/elysaku"),
            new (BetaTester, "Churro :3", "https://www.instagram.com/theelysaku/"),

            new (NetheriteTier, "greasyw00t",   new Uuid("1da9854f-f89a-4607-9d52-d0ef7a18f7bf")),
            new (NetheriteTier, "MathoX",       new Uuid("4c8b46f2-df0c-4417-9bc3-12a025fcca85")),
            new (NetheriteTier, "yuukster",     new Uuid("5b7fbb82-62df-4f0f-993a-e2747c2c5530")),
            new (NetheriteTier, "Toshio",       new Uuid("71c35984-2091-4b64-9601-6e7145c9dffe")),
            new (NetheriteTier, "Feinberg",     new Uuid("9a8e24df-4c85-49d6-96a6-951da84fa5c4")),
            new (NetheriteTier, "PeteZahHutt",  new Uuid("7ac3c39f-23d5-472a-a7c9-24798265fa15")),
            new (NetheriteTier, "Deadpool",     new Uuid("899c63ac-6590-46c0-b77c-4dae1543f707")),
            new (NetheriteTier, "merpmerp",     new Uuid("fc357f37-ebbb-4687-971f-df8016b41a6f")),
            new (NetheriteTier, "Oliver",       new Uuid("6174765b-7158-4d18-af89-4692b2704ae8")),
            new (NetheriteTier, "DCMii",        new Uuid("3d71114e-4d3d-469b-8a2c-2aeea4df1e86")),

            new (DiamondTier, "NiceTwice",  new Uuid("e43dad54-4b24-4da9-b690-a12fdc8626dc")),
            new (DiamondTier, "Cube1337x",  new Uuid("1ae14cb9-6a2f-4357-a71e-fac6f7012b59")),

            new (DiamondTier, "Nex", new string[] { "Ravager", "Magneton" },
                new Uuid[] { "cd74c07d-eb62-4874-a827-df5f30fbb3d1", "738afed3-40da-443c-ac43-50b5f16409ef" }),

            new (DiamondTier, "HAPTlCx",    new Uuid("b7860d3b-e24e-4f75-a0c7-5697ef592a80")),
            new (DiamondTier, "Soren"),
            new (DiamondTier, "macus",      new Uuid("d0193deb-f13c-449d-9a48-5a0ed38528d8")),

            new (GoldTier, "Antoine",       new Uuid("6350caba-e20b-4097-a384-931b76dca501")),
            new (GoldTier, "TheSwordElf"),
            new (GoldTier, "MeisterMaki",   new Uuid("fd21bbc7-c59c-41f7-ba86-5fa68dcd41bf")),
            new (GoldTier, "Infernalord",   new Uuid("fbea5a5f-6fbb-478b-af1d-f433df4903ed")),
            new (GoldTier, "MoleyG",        new Uuid("fa1bec35-0585-46c9-8f92-79f8be7cf9bc")),
            new (GoldTier, "AutomattPL",    new Uuid("fa61606e-8131-484c-8dee-506d1ff9a8dc")),
            new (GoldTier, "T_Wagz",        new Uuid("1351bf1c-84df-408d-9499-f67c45abb3e2")),
            new (GoldTier, "Colin_Henry",   new Uuid("c32aefa5-ea67-479e-bee6-13174ceb2769")),
            new (GoldTier, "NotValik"),
            new (GoldTier, "Switch",        new Uuid("b24da04f-8a72-4f28-a7cc-b8cbdfa57991")),
            new (GoldTier, "Melissa",       new Uuid("2482ebed-44c7-4a95-81f0-853ef3a55928")),
            new (GoldTier, "pneguin"),
            new (GoldTier, "Tompas"),
            new (GoldTier, "Xerionix",      new Uuid("2e0a64c0-bf62-4e41-91b4-be1d39d10324")),
            new (GoldTier, "Toomm"),
        };

        private static void InitializeLookups()
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
            InitializeLookups();
            return ByUuid.TryGetValue(player, out credit);
        }

        public static bool TryGet(string name, out Credit credit)
        {
            credit = default;
            if (string.IsNullOrEmpty(name))
                return false;

            InitializeLookups();
            return ByName.TryGetValue(name.ToLower(), out credit);
        }
    }

    public struct Credit
    {
        public string Name;
        public string Role;
        public string Link;
        public string[] AltNames;
        public Uuid[] Uuids;

        public Credit(string role, string name, Uuid? uuid = null, string link = "")
        {
            this.Name = name;
            this.Role = role;
            this.Uuids = new[] { uuid ?? Uuid.Empty };
            this.AltNames = new string[0];
            this.Link = link;
        }

        public Credit(string role, string name, string link)
        {
            this.Name = name;
            this.Role = role;
            this.Uuids = new[] { Uuid.Empty };
            this.AltNames = new string[0];
            this.Link = link;
        }

        public Credit(string role, string name, string[] names, Uuid[] uuids)
        {
            this.Name = name;
            this.Role = role;
            this.Uuids = uuids;
            this.AltNames = names;
            this.Link = string.Empty;
        }
    }
}
