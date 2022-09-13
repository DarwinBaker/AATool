using System;
using System.Linq;
using System.Xml;
using AATool.Configuration;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AATool.UI.Controls
{
    public class UITextInput : UITextBlock
    {
        public delegate void TextChangedEventHandler(UIControl sender);
        public event TextChangedEventHandler OnTextChanged;

        public bool Enabled { get; set; }
        public bool ClearOnLeave { get; set; }
        public ControlState State { get; private set; }
        public int BorderThickness { get; set; }
        public string UserInput { get; private set; } = string.Empty;
        public int Length => this.UserInput.Length;

        private int maxLength = 13;
        private bool selectedAll;
        private bool cursorBlink;
        private bool ignoreInput;
        private int cursor;

        private Timer holdTimer = new (1);
        private Timer repeatTimer = new (0.03);
        private Timer cursorBlinkTimer = new (0.5);
        private Keys holdKey = Keys.None;

        public bool UseCustomColor;
        public bool ShowBorder;

        private void SetState(ControlState newState)
        {
            if (this.State is not ControlState.Released)
            {
                if (newState is ControlState.Released or ControlState.Disabled && this.ClearOnLeave)
                {
                    this.UserInput = string.Empty;
                    this.SetText(string.Empty);
                }
            }
            this.State = newState;
        }

        public void SetFocus(bool active)
        {
            ControlState previousState = this.State;
            this.ignoreInput = true;
            this.SetState(active ? ControlState.Pressed : ControlState.Released);
            if (this.State != previousState)
                OnTextChanged?.Invoke(this);
        }

        public override void SetText(string text)
        {
            bool changed = text != this.rawValue;
            base.SetText(text);
            if (changed)
                OnTextChanged?.Invoke(this);
            if (string.IsNullOrEmpty(text))
                this.TextBounds = Rectangle.Empty;
        }

        public UITextInput()
        {
            this.Enabled = true;
            this.ShowBorder = true;
            this.Layer = Layer.Fore;
        }

        public void Backspace()
        {
            if (this.selectedAll)
            {
                this.UserInput = string.Empty;
                this.cursor = 0;
            }
            else
            {
                this.cursor = Math.Min(this.cursor, this.UserInput.Length);
                if (this.Length > 0 && this.cursor > 0)
                    this.UserInput = this.UserInput.Remove(this.cursor - 1, 1);
                this.cursor = Math.Max(this.cursor - 1, 0);
            }
            this.SetText(this.UserInput);
        }

        public void Delete()
        {
            if (this.selectedAll)
            {
                this.UserInput = string.Empty;
                this.cursor = 0;
            }
            else
            {
                this.cursor = Math.Min(this.cursor, this.UserInput.Length);
                if (this.Length > 0 && this.cursor < this.Length)
                    this.UserInput = this.UserInput.Remove(this.cursor, 1);
            }
            this.SetText(this.UserInput);
        }

        public void Copy()
        {
            try
            {
                System.Windows.Forms.Clipboard.SetText(this.UserInput);
            }
            catch
            {
                //clipboard error, do nothing and move on
            }
        }

        public void Cut()
        {
            try
            {
                System.Windows.Forms.Clipboard.SetText(this.UserInput);
                this.UserInput = string.Empty;
                this.cursor = 0;
                this.selectedAll = false;
                this.SetText(this.UserInput);
            }
            catch
            {
                //clipboard error, do nothing and move on
            }
        }

        public void Paste()
        {
            try
            {
                string clipboard = System.Windows.Forms.Clipboard.GetText();
                if (this.selectedAll)
                {
                    this.UserInput = clipboard;
                }
                else
                {
                    int cappedCursor = Math.Min(this.cursor, this.UserInput.Length);
                    this.UserInput = this.UserInput.Insert(cappedCursor, clipboard);
                }
                this.selectedAll = false;
                this.cursor = this.UserInput.Length;
                this.UserInput = this.UserInput.Substring(0, Math.Min(this.UserInput.Length, this.maxLength));
                this.SetText(this.UserInput);
            }
            catch
            {
                //clipboard error, do nothing and move on
            }
        }

        protected override void UpdateThis(Time time)
        {
            this.holdTimer.Update(time);
            this.repeatTimer.Update(time);

            if (Input.IsDown(Keys.Escape) || !this.Root().HasFocus)
                this.SetFocus(false);

            this.UpdateState();

            if (this.State != ControlState.Pressed)
                return;

            this.UpdateCursor(time);
            this.UpdateKeys(time);
        }

        private void UpdateState()
        {
            bool hovering = this.Bounds.Contains(Input.Cursor(this.Root()));
            if (this.State != ControlState.Pressed)
            {
                if (hovering)
                    this.SetState(ControlState.Hovered);
                else
                    this.SetState(ControlState.Released);
            }

            if (Input.LeftClickStarted)
            {
                if (hovering)
                {
                    if (this.State is not ControlState.Pressed)
                        OnTextChanged?.Invoke(this);
                    this.SetState(ControlState.Pressed);
                }
            }
            else if (Input.LeftClicked && !hovering)
            {
                this.cursorBlink = false;
                this.cursor = this.UserInput.Length;
                this.SetState(ControlState.Released);
            }
        }

        private void UpdateCursor(Time time)
        {
            this.cursorBlinkTimer.Update(time);
            if (this.cursorBlinkTimer.IsExpired)
            {
                this.cursorBlink = !this.cursorBlink;
                this.cursorBlinkTimer.Reset();
            }
        }

        private void UpdateKeys(Time time)
        {
            if (this.ignoreInput)
            {
                this.ignoreInput = false;
                return;
            }

            if (!Input.KeysNow.Any())
                this.holdKey = Keys.None;

            foreach (Keys key in Input.KeysNow)
            {
                if (this.holdKey != key)
                {
                    this.holdKey = key;
                    this.holdTimer.Reset();
                }
                else
                {
                    this.holdTimer.Update(time);
                }

                if (!Input.KeysPrev.Contains(key))
                {
                    this.ProcessKeypress(key);
                }
                else if (this.holdTimer.IsExpired && this.repeatTimer.IsExpired)
                {
                    this.ProcessKeypress(key);
                    this.repeatTimer.Reset();
                }
            }
        }

        private void ProcessKeypress(Keys key)
        {
            this.cursor = MathHelper.Clamp(this.cursor, 0, this.UserInput.Length);
            switch (key)
            {
                case Keys.Back:
                    this.Backspace();
                    this.selectedAll = false;
                    return;
                case Keys.Delete:
                    this.Delete();
                    this.selectedAll = false;
                    return;
                case Keys.Right:
                    bool rightSnap = this.selectedAll || Input.IsDown(Keys.LeftControl) || Input.IsDown(Keys.RightControl);
                    this.cursor = rightSnap ? this.UserInput.Length : Math.Min(this.cursor + 1, this.UserInput.Length);
                    this.selectedAll = false;
                    return;
                case Keys.Left:
                    bool leftSnap = this.selectedAll || Input.IsDown(Keys.LeftControl) || Input.IsDown(Keys.RightControl);
                    this.cursor = leftSnap ? 0 : Math.Max(this.cursor - 1, 0);
                    this.selectedAll = false;
                    return;
                case Keys.A:
                    if (Input.IsDown(Keys.LeftControl) || Input.IsDown(Keys.RightControl))
                    {
                        if (this.UserInput.Length > 0)
                        {
                            this.selectedAll = true;
                            this.cursor = 0;
                        }
                        return;
                    }
                    break;
                case Keys.C:
                    if (Input.IsDown(Keys.LeftControl) || Input.IsDown(Keys.RightControl))
                    {
                        this.Copy();
                        return;
                    }
                    break;
                case Keys.X:
                    if (Input.IsDown(Keys.LeftControl) || Input.IsDown(Keys.RightControl))
                    {
                        this.Cut();
                        return;
                    }
                    break;
                case Keys.V:
                    if (Input.IsDown(Keys.LeftControl) || Input.IsDown(Keys.RightControl))
                    {
                        this.Paste();
                        return;
                    }
                    break;
                case Keys.Home:
                    this.cursor = 0;
                    this.selectedAll = false;
                    return;
                case Keys.End:
                    this.cursor = this.UserInput.Length;
                    this.selectedAll = false;
                    return;
                case Keys.Enter:
                    this.cursorBlink = false;
                    this.cursor = this.UserInput.Length;
                    this.selectedAll = false;
                    this.SetState(ControlState.Released);
                    return;
            }
            bool shift = Input.IsDown(Keys.LeftShift) || Input.IsDown(Keys.RightShift) || Input.CapsLock;
            string keyText = Input.GetKeyText(key, shift);
            if (!string.IsNullOrEmpty(keyText))
            {
                int cappedCursor = Math.Min(this.cursor, this.UserInput.Length);
                this.UserInput = this.selectedAll ? keyText : this.UserInput.Insert(cappedCursor, keyText);
                this.UserInput = this.UserInput.Substring(0, Math.Min(this.UserInput.Length, this.maxLength));
                this.SetText(this.UserInput);
                this.cursor = Math.Min(cappedCursor + keyText.Length, this.maxLength);
                this.selectedAll = false;
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;

            Color backColor = Config.Main.BackColor;
            Color borderColor = Config.Main.BorderColor;
            if (!this.ShowBorder)
                borderColor = backColor;

            switch (this.State)
            {
                case ControlState.Released:
                    canvas.DrawRectangle(this.Bounds, borderColor, backColor, this.BorderThickness, this.Layer);
                    break;
                case ControlState.Hovered:
                    canvas.DrawRectangle(this.Bounds, borderColor * 1.25f, backColor, this.BorderThickness, this.Layer);
                    break;
                case ControlState.Pressed:
                    canvas.DrawRectangle(this.Bounds, borderColor * 1.3f, Config.Main.TextColor, this.BorderThickness, this.Layer);
                    break;
                case ControlState.Disabled:
                    canvas.DrawRectangle(this.Bounds, backColor * 1.2f, backColor, this.BorderThickness, this.Layer);
                    break;
            }
            base.DrawThis(canvas);

            if (this.State is not ControlState.Pressed)
                return;

            if (this.selectedAll)
            {
                var selectionBounds = new Rectangle(this.TextBounds.Left - 2, this.Bounds.Top, this.TextBounds.Width + 2, this.Bounds.Height);
                var leftCursorBounds = new Rectangle(this.TextBounds.Left - 6, this.Bounds.Bottom - 6, 9, 5);
                var rightCursorBounds = new Rectangle(this.TextBounds.Right - 4, this.Bounds.Bottom - 6, 9, 5);
                canvas.DrawRectangle(selectionBounds, ColorHelper.Fade(Config.Main.TextColor, 0.3f), null, 0, Layer.Fore);
                canvas.Draw("text_cursor", leftCursorBounds, Config.Main.TextColor, Layer.Fore);
                canvas.Draw("text_cursor", rightCursorBounds, Config.Main.TextColor, Layer.Fore);
            }
            else
            {
                Color cursorColor = this.cursorBlink ? Config.Main.TextColor : ColorHelper.Fade(Config.Main.TextColor, 0.4f);
                int cappedLength = Math.Min(this.cursor, this.UserInput.Length);
                int textWidth = (int)this.Font.MeasureString(this.UserInput.Substring(0, cappedLength)).X;
                int cursorX = this.TextBounds != Rectangle.Empty ? this.TextBounds.Left + textWidth : this.Center.X;
                var cursorBounds = new Rectangle(cursorX - 4, this.Bounds.Bottom - 6, 9, 5);
                canvas.Draw("text_cursor", cursorBounds, cursorColor, Layer.Fore);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.BorderThickness = Attribute(node, "border_thickness", 1);
            this.ClearOnLeave = Attribute(node, "clear", false);
        }
    }
}
