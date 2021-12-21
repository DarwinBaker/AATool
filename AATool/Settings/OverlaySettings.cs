using AATool.Data;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AATool.Settings
{
    //i don't love the way these classes work, i might change them later
    public class OverlaySettings : SettingsGroup
    {
        public static OverlaySettings Instance = new ();

        private const string ENABLED           = "enabled";
        private const string SHOW_LABELS       = "show_labels";
        private const string SHOW_CRITERIA     = "show_criteria";
        private const string SHOW_ITEMS        = "show_items";
        private const string SHOW_IGT          = "show_igt";
        private const string CLARIFY_AMBIGUOUS = "clarify_ambiguous";
        private const string RIGHT_TO_LEFT     = "right_to_left";
        private const string SCALE             = "scale";
        private const string SPEED             = "speed";
        private const string WIDTH             = "width";
        private const string BACK_COLOR        = "back_color";
        private const string TEXT_COLOR        = "text_color";

        public bool Enabled                 { get => this.Get<bool>(ENABLED);       set => this.Set(ENABLED, value); }
        public bool ShowLabels              { get => this.Get<bool>(SHOW_LABELS);   set => this.Set(SHOW_LABELS, value); }
        public bool ShowCriteria            { get => this.Get<bool>(SHOW_CRITERIA); set => this.Set(SHOW_CRITERIA, value); }
        public bool ShowCounts              { get => this.Get<bool>(SHOW_ITEMS);    set => this.Set(SHOW_ITEMS, value); }
        public bool ShowIgt                 { get => this.Get<bool>(SHOW_IGT);      set => this.Set(SHOW_IGT, value); }
        public bool ClarifyAmbiguous        { get => this.Get<bool>(CLARIFY_AMBIGUOUS); set => this.Set(CLARIFY_AMBIGUOUS, value); }
        public bool RightToLeft             { get => this.Get<bool>(RIGHT_TO_LEFT); set => this.Set(RIGHT_TO_LEFT, value); }
        public int Scale                    { get => this.Get<int>(SCALE);          set => this.Set(SCALE, value); }
        public int Speed                    { get => this.Get<int>(SPEED);          set => this.Set(SPEED, value); }
        public int Width                    { get => this.Get<int>(WIDTH);          set => this.Set(WIDTH, value); }
        public Color BackColor              { get => this.Get<Color>(BACK_COLOR);   set => this.Set(BACK_COLOR, value); }
        public Color TextColor              { get => this.Get<Color>(TEXT_COLOR);   set => this.Set(TEXT_COLOR, value); }

        public bool EnabledChanged()      => Instance.ValueChanged(ENABLED);
        public bool DirectionChanged()    => Instance.ValueChanged(RIGHT_TO_LEFT);
        public bool SpeedChanged()        => Instance.ValueChanged(SPEED);
        public bool WidthChanged()        => Instance.ValueChanged(WIDTH);
        public bool ShowLabelsChanged()   => Instance.ValueChanged(SHOW_LABELS);
        public bool ShowCriteriaChanged() => Instance.ValueChanged(SHOW_CRITERIA);
        public bool ShowItemsChanged()    => Instance.ValueChanged(SHOW_ITEMS);

        private OverlaySettings() 
        {
            this.Load("overlay");
        }

        public override void ResetToDefaults()
        {
            this.Set(ENABLED, false);
            this.Set(SHOW_LABELS, true);
            this.Set(SHOW_CRITERIA, true);
            this.Set(SHOW_ITEMS, true);
            this.Set(SHOW_IGT, true);
            this.Set(CLARIFY_AMBIGUOUS, true);
            this.Set(RIGHT_TO_LEFT, true);
            this.Set(SCALE, 3);
            this.Set(SPEED, 2);
            this.Set(WIDTH, 1920);
            this.Set(BACK_COLOR, Color.FromNonPremultiplied(0, 170, 0, 255));
            this.Set(TEXT_COLOR, Color.White);
        }
    }
}
