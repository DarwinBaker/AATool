using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.Configuration;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AATool
{
    public static class Input
    {
        public static MouseState MouseNow  { get; private set; }
        public static MouseState MousePrev { get; private set; }
        public static KeyboardState KeyboardCurrent  { get; private set; }
        public static KeyboardState KeyboardPrevious { get; private set; }

        private static Keys[] KeysNow;
        private static Keys[] KeysPrev;

        public static bool LeftClicked => !LeftClicking && MousePrev.LeftButton is ButtonState.Pressed;
        public static bool LeftClicking => MouseNow.LeftButton is ButtonState.Pressed;
        public static bool RightClicked => !RightClicking && MousePrev.RightButton is ButtonState.Pressed;
        public static bool RightClicking => MouseNow.RightButton is ButtonState.Pressed;

        public static Point Cursor(UIScreen screen)
        {
            if (screen == Main.PrimaryScreen)
                return MouseNow.Position / new Point(Config.Main.DisplayScale);

            //normalize cursor position on secondary windows
            return new Point(
                MouseNow.Position.X + Main.PrimaryScreen.Form.Location.X - screen.Form.Location.X,
                MouseNow.Position.Y + Main.PrimaryScreen.Form.Location.Y - screen.Form.Location.Y);
        }

        public static bool IsDown(Keys key) => KeysNow.Contains(key);
        public static bool WasDown(Keys key) => KeysPrev.Contains(key);
        public static bool Ended(Keys key) => WasDown(key) && !IsDown(key);
        public static bool Started(Keys key) => IsDown(key) && !WasDown(key);

        public static void BeginUpdate(bool active)
        {
            if (active)
            {
                MouseNow = Mouse.GetState();
                KeyboardCurrent = Keyboard.GetState();
                KeysNow = KeyboardCurrent.GetPressedKeys();
                KeysPrev = KeyboardPrevious.GetPressedKeys();
            }
            else
            {
                MouseNow = default;
                KeyboardCurrent = default;
                KeysNow = new Keys[0];
                KeysPrev = new Keys[0];
            }
        }

        public static void EndUpdate()
        {
            MousePrev = MouseNow;
            KeyboardPrevious = KeyboardCurrent;
        }
    }
}
