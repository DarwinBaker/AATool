using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Utilities;

namespace AATool.Winforms.Forms
{

    public partial class FStyleDialog : Form
    {
        private int ButtonWidth => this.isOverlay ? 84 : 80;
        private int ButtonHeight => this.isOverlay ? 130 : 105;

        private readonly Dictionary<string, Control> styleButtons = new ();
        private readonly Dictionary<string, CheckBox> prideCheckBoxes = new ();
        private readonly HashSet<string> highlightedButtons = new ();
        private readonly HashSet<string> prideFrameList = new ();
        private readonly bool isOverlay;
        private readonly Color back;
        private readonly Color fore;
        private readonly Color text;

        private string style;

        public FStyleDialog(bool overlay)
        {
            this.InitializeComponent();
            this.isOverlay = overlay;

            this.back = ColorHelper.ToDrawing(Config.Main.BackColor);
            this.fore = ColorHelper.ToDrawing(Config.Main.BorderColor);
            this.text = ColorHelper.ToDrawing(Config.Main.TextColor);

            this.closeOnSelect.Checked = Config.Main.CloseFramesOnSelection;
            this.closeOnSelect.Location = new Point(this.Right - this.closeOnSelect.Width - 8, this.Top + 8);
            this.closeOnSelect.BackColor = this.back;
            this.closeOnSelect.ForeColor = this.text;

            this.frames.BackColor = this.back;
            this.frames.Controls.Clear();

            this.Text = this.isOverlay ? "Overlay Frame Style" : "Main Window Frame Style";
            this.Width = this.isOverlay ? 945 : 825;
            this.Height = this.isOverlay ? 710 : 610;

            this.style = this.isOverlay ? Config.Overlay.FrameStyle : Config.Main.FrameStyle;
            string prideList = this.isOverlay ? Config.Overlay.PrideFrameList : Config.Main.PrideFrameList;
            foreach (string flag in prideList.Split(','))
                this.prideFrameList.Add(flag);

            if (this.isOverlay)
            {
                this.Populate("Solid Colors");
                this.Populate("Other");
                this.Populate("Pride Flags");
            }
            else
            {
                this.Populate("Minimalist");
                this.Populate("Game Inspired");
                this.Populate("Pride Flags");
            }
            this.UpdateHighlighted();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys key)
        {
            if (key == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref message, key);
        }

        private string GetFramesFolder(string group)
        {
            return this.isOverlay
                ? Path.Combine(Paths.System.WinformsAssets, "frames_overlay", group)
                : Path.Combine(Paths.System.WinformsAssets, "frames_main", group);
        }

        private void UpdateHighlighted()
        {
            if (string.IsNullOrEmpty(this.style))
                return;

            this.highlightedButtons.Clear();

            //reset all buttons to deselected color
            foreach (Control styleButton in this.styleButtons.Values)
                styleButton.BackColor = this.back;

            if (this.style.Contains("Pride"))
            {
                //highlight all checked pride flag buttons
                foreach (string flag in this.prideFrameList)
                {
                    if (this.styleButtons.TryGetValue(flag, out Control prideButton))
                        prideButton.BackColor = this.fore;
                }
                if (this.prideFrameList.Count > 0)
                    return;
            }

            //highlight single button
            if (this.styleButtons.TryGetValue(this.style, out Control button))
                button.BackColor = this.fore;
        }

        private void UpdatePrideList()
        {
            this.prideFrameList.Clear();
            foreach (KeyValuePair<string, CheckBox> flag in this.prideCheckBoxes)
            {
                if (flag.Value.Checked)
                    this.prideFrameList.Add(flag.Key);
            }
        }

        private void SaveConfig()
        {
            this.UpdatePrideList();
            string prideFlags = string.Join(",", this.prideFrameList);
            if (this.style.Contains("Pride"))
            {
                if (this.prideFrameList.Count is 1)
                    this.style = this.prideFrameList.First();
                else if (this.prideFrameList.Count > 1)
                    this.style = "Multi-Pride";
            }

            if (this.isOverlay)
            {
                Config.Overlay.FrameStyle.Set(this.style);
                Config.Overlay.SetPrideList(prideFlags);
                Config.Overlay.TrySave();
            }
            else
            {
                Config.Main.FrameStyle.Set(this.style);
                Config.Main.SetPrideList(prideFlags);
                Config.Main.TrySave();
            }
        }

        private void Populate(string group)
        {
            Control header = this.CreateGroupHeader(group);
            this.frames.Controls.Add(header);
            try
            {
                foreach (string file in Directory.GetFiles(this.GetFramesFolder(group)))
                {
                    var image = Image.FromFile(file);
                    string name = Path.GetFileNameWithoutExtension(file);
                    name = name.Split('_').LastOrDefault();
                    Control button = this.CreateFrameButton(image, name);
                    button.Click += this.OnClick;
                    this.frames.Controls.Add(button);
                }
            }
            catch
            {
                MessageBox.Show("There was an error loading frame styles. Run AAUpdate.exe to repair your installation.",
                    "Error Loading Styles", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Control CreateGroupHeader(string name) 
        {
            var label = new Label () {
                Text = name,
                Width = this.frames.Width,
                Height = 32,
                Font = new Font("Segoi UI", 16),
                Margin = new Padding(0, 8, 0, 0),
                Padding = new Padding(8, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = text,
            };
            return label;
        }

        private Control CreateFrameButton(Image image, string name)
        {
            Color color = name == this.style 
                ? ColorHelper.ToDrawing(Config.Main.BorderColor)
                : this.frames.BackColor;

            var button = new Button() {
                Image = image,
                Text = name.Replace(" Pride", "\nPride"),
                Tag = name,
                Width = this.ButtonWidth,
                Height = this.ButtonHeight,
                Padding = new Padding(0, 8, 0, 8),
                BackColor = color,
                ForeColor = text,
                ImageAlign = ContentAlignment.TopCenter,
                TextAlign = ContentAlignment.BottomCenter,
                FlatStyle = FlatStyle.Flat,
            };

            //checkbox for selecting more than one flag <3
            if (name.ToLower().Contains("pride"))
            {
                var checkBox = new CheckBox {
                    Checked = this.prideFrameList.Contains(name),
                    AutoSize = true,
                    Tag = name,
                };
                checkBox.CheckedChanged += this.OnCheckedChanged;
                button.Controls.Add(checkBox);
                checkBox.Location = new Point(button.Width - checkBox.Width, 0);
                this.prideCheckBoxes[name] = checkBox;
            }

            this.styleButtons[name] = button;
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void OnClick(object sender, EventArgs e)
        {
            if ((sender as Control)?.Tag is not string style)
                return;

            if (style.Contains("Pride") && this.prideCheckBoxes.TryGetValue(style, out CheckBox flag))
            {
                if (!this.style.Contains("Pride"))
                {
                    if (this.prideFrameList.Count > 0)
                    {
                        //check newly selected flag if already multi-select
                        this.prideFrameList.Add(style);
                        flag.Checked = true;
                    }

                }
                else if (this.prideFrameList.Count > 0 && !Config.Main.CloseFramesOnSelection)
                {
                    flag.Checked ^= true;
                }
            }
            this.style = style;
            this.SaveConfig();
            this.UpdateHighlighted();

            if (Config.Main.CloseFramesOnSelection)
                this.Close();
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if (sender == this.closeOnSelect)
            {
                Config.Main.CloseFramesOnSelection.Set(this.closeOnSelect.Checked);
                Config.Main.TrySave();
            }
            else if (sender is CheckBox flagCheckBox && flagCheckBox.Tag is string style)
            {
                if (flagCheckBox.Checked)
                {
                    this.style = style;
                    this.SaveConfig();
                }
                else
                {
                    this.SaveConfig();
                }
                this.UpdateHighlighted();
            }
        }
    }
}
