using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.Data.Objectives;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    internal class UIArmorTrimCriterion : UICriterion
    {
        UIPicture smithingIcon;

        float smithingBrightness;

        public UIArmorTrimCriterion() : base()
        {
        }

        public override void InitializeThis(UIScreen screen)
        {
            base.InitializeThis(screen);
            smithingIcon = this.First<UIPicture>("smithing_icon");
        }

        protected override void UpdateThis(Time time)
        {
            base.UpdateThis(time);
            if (this.Objective is not ArmorTrimCriterion trim)
                return;

            float smithingTarget = trim.Applied ? 1f : 0;
            this.smithingBrightness = MathHelper.Lerp(this.smithingBrightness, smithingTarget, (float)(10 * time.Delta));
            this.smithingIcon?.SetTint(Color.White * this.smithingBrightness);
        }
    }
}
