using System.Data;
using System;
using System.Linq;
using System.Xml;
using AATool.Data;
using AATool.Net;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using AATool.Winforms.Controls;
using System.Windows.Forms;

namespace AATool.UI.Controls
{
    internal class UISupporterList : UIFlowPanel
    {
        struct Supporter
        {
            public readonly Uuid Uuid;
            public readonly string Name;
            public readonly string Role;

            public Supporter(Uuid uuid, string name, string role)
            {
                this.Uuid = uuid;
                this.Name = name;
                this.Role = role;
            }
        }

        bool activeOnly;
        int columns;
        int rows;
        int scrollOffset;
        int maxAvatars;

        readonly List<Supporter> netherite = new();
        readonly List<Supporter> diamond = new();
        readonly List<Supporter> gold = new();

        List<Supporter> all = new();

        public void ScrollUp(int rows) => this.TryScroll(-rows);
        public void ScrollDown(int rows) => this.TryScroll(rows);

        public override void InitializeThis(UIScreen screen)
        {
            base.InitializeThis(screen);

            this.gold.Clear();
            this.diamond.Clear();
            this.netherite.Clear();
            
            foreach (Credit supporter in Credits.All)
            {
                if (supporter.Active == this.activeOnly)
                {
                    string role = this.activeOnly ? supporter.CurrentRole : supporter.HighestRole;
                    this.TryAdd(supporter.Uuids.FirstOrDefault(), supporter.Name, role);
                }
            }

            this.all = this.netherite.Union(this.diamond).Union(this.gold).ToList();
        }

        public override void ResizeThis(Rectangle parent)
        {
            base.ResizeThis(parent);

            const int AvatarWidth = 70;
            const int AvatarHeight = 70;
            this.columns = this.Inner.Width / AvatarWidth;
            this.rows = this.Inner.Height / AvatarHeight;
            this.maxAvatars = this.rows * this.columns;

            this.Populate();
        }

        protected override void UpdateThis(Time time)
        {
            base.UpdateThis(time);

            this.UpdateScrollWheel();
        }

        private void TryAdd(Uuid uuid, string name, string role)
        {
            var supporter = new Supporter(uuid, name, role);
            if (role is Credits.GoldTier)
            {
                this.gold.Add(supporter);
            }
            else if (role is Credits.DiamondTier)
            {
                this.diamond.Add(supporter);
            }
            else if (role is Credits.NetheriteTier)
            {
                this.netherite.Add(supporter);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.activeOnly = Attribute(node, "active_only", this.activeOnly);
        }

        private void Populate()
        {
            this.ClearControls();

            int start = this.columns * this.scrollOffset;
            int end = Math.Min(start + this.maxAvatars, this.all.Count);
            for (int i = start; i < end; i++)
            {
                Supporter supporter = this.all[i];
                var avatar = new UISupporterAvatar(supporter.Uuid, supporter.Name, supporter.Role);
                this.AddControl(avatar);
                avatar.InitializeRecursive(this.Root());
                avatar.ResizeRecursive(this.Inner);
            }
            this.ReflowChildren();
        }

        protected virtual void TryScroll(int rows)
        {
            int maxOffset = Math.Max((this.all.Count - this.maxAvatars) / this.columns, 0);
            this.scrollOffset = MathHelper.Clamp(this.scrollOffset + rows, 0, maxOffset);
        }

        private void UpdateScrollWheel()
        {
            int oldOffset = this.scrollOffset;
            if (this.Bounds.Contains(Input.Cursor(this.Root())))
            {
                if (Input.ScrolledUp())
                    this.ScrollUp(1);
                else if (Input.ScrolledDown())
                    this.ScrollDown(1);

                if (this.scrollOffset != oldOffset)
                    this.Populate();
            }
        }
    }
}
