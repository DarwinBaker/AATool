namespace AATool
{
    //data structures
    public enum FrameType       { Normal, Goal, Challenge }

    public enum SaveFolderState
    {
        Valid,
        NoWorlds,
        NonExistentPath,
        EmptyPath,
        InvalidPath,
        PathTooLong,
        PermissionError
    }

    public enum SyncState
    {
        Ready,
        Connecting,
        ServerProperties,
        LastAutoSave,
        Advancements,
        Statistics,
    }

    //ui
    public enum HorizontalAlign { Center, Left, Right }
    public enum VerticalAlign   { Center, Top, Bottom }
    public enum FlowDirection   { LeftToRight, RightToLeft, TopToBottom, BottomToTop }
    public enum SizeMode        { Absolute, Relative }
    public enum DrawMode        { All, ThisOnly, ChildrenOnly, None }
    public enum UIButtonState   { Released, Hovered, Pressed, Disabled }
    public enum UIRefreshStyle  { Compass, Xp }

    //graphics
    public enum Layer           { Main, Glow, Fore }

    //misc
    public enum Ease            { Back, Bounce, Circular, Cubic, Elastic, Exponential, Quadratic, Quartic, Quintic, Sinusoidal }
    public enum SupportTier     { Developer, BetaTester, PatreonGold, PatreonDiamond, PatreonNetherite }
}
