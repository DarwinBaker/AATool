using System.Collections.Generic;

namespace AATool.Data
{
    public static class Credits
    {
        public static readonly HashSet<Credit> All = new ()
        {
            new ("developer", "CTM", "https://www.patreon.com/_ctm"),
            new ("dedication", "Wroxy"),

            new ("beta_tester", "Elysaku", "https://www.twitch.tv/elysaku"),
            new ("beta_tester", "Churro :3", "https://www.instagram.com/theelysaku/"),

            new ("supporter_netherite", "Toshio"),
            new ("supporter_emerald", "NiceTwice"),
            new ("supporter_emerald", "Cube1337x"),
            new ("supporter_diamond", "Nex"),
            new ("supporter_diamond", "HAPTlCx"),
            new ("supporter_gold", "Antoine"),
            new ("supporter_gold", "TheSwordElf"),
            new ("supporter_gold", "MeisterMaki"),
            new ("supporter_gold", "Deadpool"),
            new ("supporter_gold", "Feinberg"),
            new ("supporter_gold", "Infernalord"),
            new ("supporter_gold", "MoleyG"),
            new ("supporter_gold", "AutomattPL"),
            new ("supporter_gold", "T_Wagz"),
            new ("supporter_gold", "Colin_Henry"),
            new ("supporter_gold", "NotValik"),
            new ("supporter_gold", "Switch"),
            new ("supporter_gold", "Melissa"),
            new ("supporter_gold", "Penguida"),
        };
    }

    public struct Credit
    {
        public string Name;
        public string Role;
        public string Link;

        public Credit(string role, string name, string link = "")
        {
            this.Name = name;
            this.Role = role;
            this.Link = link;
        }
    }
}
