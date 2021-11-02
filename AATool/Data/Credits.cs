using System.Collections.Generic;

namespace AATool.Data
{
    public static class Credits
    {
        public static readonly HashSet<Supporter> Supporters = new ()
        {
            new Supporter("developer", "CTM", "https://www.patreon.com/_ctm"),
            new Supporter("special_dedication", "Wroxy"),

            new Supporter("beta_testers", "Elysaku", "https://www.twitch.tv/elysaku"),
            new Supporter("beta_testers", "Churro the Cat :3", "https://www.instagram.com/theelysaku/"),

            new Supporter("netherite", "Toshio"),
            new Supporter("emerald", "NiceTwice"),
            new Supporter("emerald", "Cube1337x"),
            new Supporter("gold", "Antoine"),
            new Supporter("gold", "TheSwordElf"),
            new Supporter("gold", "MeisterMaki"),
            new Supporter("gold", "Deadpool"),
            new Supporter("gold", "Feinberg"),
            new Supporter("gold", "Infernalord"),
            new Supporter("gold", "MoleyG"),
            new Supporter("gold", "AutomattPL"),
            new Supporter("gold", "T_Wagz"),
        };
    }

    public struct Supporter
    {
        public string Name;
        public string Role;
        public string Link;

        public Supporter(string role, string name, string link = "")
        {
            this.Name = name;
            this.Role = role;
            this.Link = link;
        }
    }
}
