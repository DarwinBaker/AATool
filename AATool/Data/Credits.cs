using System.Collections.Generic;

namespace AATool.Data
{
    public static class Credits
    {
        public static readonly HashSet<Credit> All = new ()
        {
            new ("developer", "CTM", "https://www.patreon.com/_ctm"),
            new ("special dedication", "Wroxy"),

            new ("beta testers", "Elysaku", "https://www.twitch.tv/elysaku"),
            new ("beta testers", "Churro :3", "https://www.instagram.com/theelysaku/"),

            new ("supporter_netherite", "greasyw00t"),
            new ("supporter_netherite", "MathoX"),
            new ("supporter_netherite", "the_yuukster"),
            new ("supporter_netherite", "Toshio"),
            new ("supporter_netherite", "Feinberg"),
            new ("supporter_netherite", "PeteZahHutt"),
            new ("supporter_netherite", "Deadpool"),
            
            new ("supporter_diamond", "NiceTwice"),
            new ("supporter_diamond", "Cube1337x"),
            new ("supporter_diamond", "Nex"),
            new ("supporter_diamond", "HAPTlCx"),
            new ("supporter_diamond", "Soren"),
            new ("supporter_diamond", "macus"),

            new ("supporter_gold", "Antoine"),
            new ("supporter_gold", "TheSwordElf"),
            new ("supporter_gold", "MeisterMaki"),
            new ("supporter_gold", "Infernalord"),
            new ("supporter_gold", "MoleyG"),
            new ("supporter_gold", "AutomattPL"),
            new ("supporter_gold", "T_Wagz"),
            new ("supporter_gold", "Colin_Henry"),
            new ("supporter_gold", "NotValik"),
            new ("supporter_gold", "Switch"),
            new ("supporter_gold", "Melissa"),
            new ("supporter_gold", "pneguin"),
            new ("supporter_gold", "Tomas"),
            new ("supporter_gold", "Xerionix"),
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
