using System.Linq;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AATool
{
    public static class Input
    {
        public static bool IsActive { get; private set; }

        public static MouseState MouseNow  { get; private set; }
        public static MouseState MousePrev { get; private set; }
        public static KeyboardState KeyboardNow  { get; private set; }
        public static KeyboardState KeyboardPrev { get; private set; }

        public static Keys[] KeysNow  { get; private set; }
        public static Keys[] KeysPrev { get; private set; }

        public static int ScrollNow  { get; private set; }
        public static int ScrollPrev { get; private set; }
        public static bool CapsLock { get; private set; }

        public static bool LeftClicking => IsActive && MouseNow.LeftButton is ButtonState.Pressed;
        public static bool LeftClicked => IsActive && !LeftClicking && MousePrev.LeftButton is ButtonState.Pressed;
        public static bool LeftClickStarted => IsActive && LeftClicking && MousePrev.LeftButton is ButtonState.Released;
        public static bool RightClicking => IsActive && MouseNow.RightButton is ButtonState.Pressed;
        public static bool RightClicked => IsActive && !RightClicking && MousePrev.RightButton is ButtonState.Pressed;
        public static bool RightClickStarted => IsActive && RightClicking && MousePrev.RightButton is ButtonState.Released;

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

        public static bool IsDown(Keys key) => IsActive && KeysNow.Contains(key);
        public static bool WasDown(Keys key) => IsActive && KeysPrev.Contains(key);
        public static bool Ended(Keys key) => IsActive && (WasDown(key) && !IsDown(key));
        public static bool Started(Keys key) => IsActive && (IsDown(key) && !WasDown(key));

        public static bool ScrolledUp() => ScrollNow > ScrollPrev;
        public static bool ScrolledDown() => ScrollNow < ScrollPrev;

        public static void SupressClicks()
        {
            MouseNow = default;
            MousePrev = default;
        }

        public static void BeginUpdate(bool active)
        {
            IsActive = active;
            MouseNow = Mouse.GetState();
            ScrollNow = MouseNow.ScrollWheelValue;
            if (active)
            {
                KeyboardNow = Keyboard.GetState();
                KeysNow = KeyboardNow.GetPressedKeys();
                KeysPrev = KeyboardPrev.GetPressedKeys();
                
                CapsLock = KeyboardNow.CapsLock;
            }
            else
            {
                KeyboardNow = default;
                KeysNow = new Keys[0];
                KeysPrev = new Keys[0];
            }
        }

        public static void EndUpdate()
        {
            MousePrev = MouseNow;
            KeyboardPrev = KeyboardNow;
            ScrollPrev = ScrollNow;
        }

        public static string GetKeyText(Keys key, bool shift = false)
        {
            string keyString = key.ToString();
            if (keyString.Length == 1 && keyString[0] >= 'A' && keyString[0] <= 'Z')
                return shift ? keyString : keyString.ToLower();
            if (keyString.Length == 2 && keyString[1] >= '0' && keyString[1] <= '9')
                return keyString[1].ToString();
            if (keyString.Length == 7 && keyString[6] >= '0' && keyString[6] <= '9')
                return keyString[6].ToString();

            return key switch {
                Keys.Space => " ",
                Keys.Multiply => "*",
                Keys.Add => "+",
                Keys.Separator => "-",
                Keys.Subtract => "-",
                Keys.Decimal => ".",
                Keys.Divide => "/",
                Keys.OemSemicolon => shift ? ":" : ";",
                Keys.OemPlus => shift ? "+" : "=",
                Keys.OemComma => shift ? "<" : ",",
                Keys.OemMinus => shift ? "_" : "-",
                Keys.OemPeriod => shift ? ">" : ".",
                Keys.OemQuestion => shift ? "?" : "/",
                Keys.OemTilde => shift ? "~" : "`",
                Keys.OemOpenBrackets => shift ? "{" : "[",
                Keys.OemPipe => shift ? "|" : "\\",
                Keys.OemCloseBrackets => shift ? "}" : "]",
                Keys.OemQuotes => shift ? "\"" : "'",
                Keys.OemBackslash => "\\",
                _ => string.Empty
            };
        }
    }
}
