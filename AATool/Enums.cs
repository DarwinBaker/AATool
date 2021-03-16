namespace AATool
{
    //data structures
    public enum AdvancementType { Normal, Goal, Challenge }

    //ui
    public enum HorizontalAlign { Center, Left, Right }
    public enum VerticalAlign   { Center, Top, Bottom }
    public enum FlowDirection   { LeftToRight, RightToLeft, TopToBottom, BottomToTop }
    public enum SizeMode        { Absolute, Relative }
    public enum DrawMode        { All, ThisOnly, ChildrenOnly, None }
    public enum UIButtonState   { Released, Hovered, Pressed }

    //graphics
    public enum Layer { Main, Glow, Fore }

    //misc
    public enum SupportTier { Developer, BetaTester, PatreonGold, PatreonDiamond, PatreonNetherite }
}
