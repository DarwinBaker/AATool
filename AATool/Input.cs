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
        public static KeyboardState KeyboardNow  { get; private set; }
        public static KeyboardState KeyboardPrev { get; private set; }

        public static Keys[] KeysNow  { get; private set; }
        public static Keys[] KeysPrev { get; private set; }

        public static int ScrollNow  { get; private set; }
        public static int ScrollPrev { get; private set; }

        public static bool LeftClicked => !LeftClicking && MousePrev.LeftButton is ButtonState.Pressed;
        public static bool LeftClicking => MouseNow.LeftButton is ButtonState.Pressed;
        public static bool RightClicked => !RightClicking && MousePrev.RightButton is ButtonState.Pressed;
        public static bool RightClicking => MouseNow.RightButton is ButtonState.Pressed;

        public static Point Cursor(UIScreen screen)
        {
            if (screen == Main.PrimaryScreen)
            {
                //normalize cursor position to primary window viewport
                float ratioX = (float)screen.Width / screen.GraphicsDevice.Viewport.Width;
                float ratioY = (float)screen.Height / screen.GraphicsDevice.Viewport.Height;
                return new Point((int)(MouseNow.X * ratioX), (int)(MouseNow.Y * ratioY));
            }
            else
            {
                //normalize cursor position on secondary windows
                return new Point(
                    MouseNow.Position.X + Main.PrimaryScreen.Form.Location.X - screen.Form.Location.X,
                    MouseNow.Position.Y + Main.PrimaryScreen.Form.Location.Y - screen.Form.Location.Y);
            }
        }

        public static bool IsDown(Keys key) => KeysNow.Contains(key);
        public static bool WasDown(Keys key) => KeysPrev.Contains(key);
        public static bool Ended(Keys key) => WasDown(key) && !IsDown(key);
        public static bool Started(Keys key) => IsDown(key) && !WasDown(key);

        public static bool ScrolledUp() => ScrollNow > ScrollPrev;
        public static bool ScrolledDown() => ScrollNow < ScrollPrev;

        public static void BeginUpdate(bool active)
        {
            if (active)
            {
                MouseNow = Mouse.GetState();
                KeyboardNow = Keyboard.GetState();
                KeysNow = KeyboardNow.GetPressedKeys();
                KeysPrev = KeyboardPrev.GetPressedKeys();
                ScrollNow = MouseNow.ScrollWheelValue;
            }
            else
            {
                MouseNow = default;
                KeyboardNow = default;
                KeysNow = new Keys[0];
                KeysPrev = new Keys[0];
                ScrollNow = ScrollPrev;
            }
        }

        public static void EndUpdate()
        {
            MousePrev = MouseNow;
            KeyboardPrev = KeyboardNow;
            ScrollPrev = ScrollNow;
        }
    }
}
