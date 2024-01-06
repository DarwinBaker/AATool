using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AATool.Data;
using AATool.Net;
using AATool.UI.Badges;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UISupporterAvatar : UIControl
    {
        Uuid uuid;
        string name;
        string tier;

        UIAvatar avatar;
        UITextBlock label;

        public UISupporterAvatar(Uuid uuid, string name, string tier)
        {
            this.BuildFromTemplate();
            this.uuid = uuid;
            this.name = name;
            this.tier = tier;
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);

            this.avatar = this.First<UIAvatar>();
            this.label = this.First<UITextBlock>("label");

            this.avatar.LockBadgeAndFrame = true;
            this.avatar.SetPlayer(this.uuid);

            this.label.SetText(this.name);
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.avatar.SetBadge(new SupporterBadge(this.tier));
            this.avatar.SetFrame(this.tier, true);
        }
    }
}
