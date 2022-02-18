using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AATool
{
    public static class Input
    {
        public static MouseState MouseNow  { get; private set; }
        public static MouseState MousePrev { get; private set; }

        public static void BeginUpdate() => MouseNow = Mouse.GetState();
        public static void EndUpdate() => MousePrev = MouseNow;

        public static bool LeftClicked => !LeftClicking && MousePrev.LeftButton is ButtonState.Pressed;
        public static bool LeftClicking => MouseNow.LeftButton is ButtonState.Pressed;
        public static bool RightClicked => !RightClicking && MousePrev.RightButton is ButtonState.Pressed;
        public static bool RightClicking => MouseNow.RightButton is ButtonState.Pressed;

        public static Point Cursor(UIScreen screen)
        {
            Point cursor = MouseNow.Position;
            if (screen == Main.PrimaryScreen)
                return cursor;

            //normalize cursor position on secondary windows
            return new Point(
                cursor.X + Main.PrimaryScreen.Form.Location.X - screen.Form.Location.X,
                cursor.Y + Main.PrimaryScreen.Form.Location.Y - screen.Form.Location.Y);
        }
    }
}
