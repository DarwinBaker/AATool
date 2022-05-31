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
        public string SelectedFrame => this.selected?.Tag as string;

        private bool overlay;

        private int ButtonWidth => this.overlay ? 84 : 80;
        private int ButtonHeight => this.overlay ? 130 : 105;

        private Control selected;
        private Color back;
        private Color fore;
        private Color text;

        public FStyleDialog(bool overlay)
        {
            this.InitializeComponent();
            this.overlay = overlay;

            this.back = ColorHelper.ToDrawing(Config.Main.BackColor);
            this.fore = ColorHelper.ToDrawing(Config.Main.BorderColor);
            this.text = ColorHelper.ToDrawing(Config.Main.TextColor);

            this.closeOnSelect.Checked = Config.Main.CloseFramesOnSelection;
            this.closeOnSelect.Location = new Point(this.Right - this.closeOnSelect.Width - 8, this.Top + 8);
            this.closeOnSelect.BackColor = this.back;
            this.closeOnSelect.ForeColor = this.text;

            this.frames.BackColor = this.back;
            this.frames.Controls.Clear();

            this.Text = overlay ? "Overlay Frame Style" : "Main Window Frame Style";
            this.Width = overlay ? 945 : 825;
            this.Height = overlay ? 710 : 610;

            if (overlay)
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
        }

        private string GetFramesFolder(string group)
        {
            return this.overlay
                ? Path.Combine(Paths.System.WinformsAssets, "frames_overlay", group)
                : Path.Combine(Paths.System.WinformsAssets, "frames_main", group);
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
            string current = this.overlay ? Config.Overlay.FrameStyle : Config.Main.FrameStyle;
            Color color = name == current 
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
            button.FlatAppearance.BorderSize = 0;
            if (current == (button.Tag as string))
                this.selected = button;
            return button;
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (this.selected is not null)
                this.selected.BackColor = this.back;
            this.selected = sender as Control;
            this.selected.BackColor = this.fore;

            string style = (sender as Control)?.Tag as string;
            if (this.overlay)
            {
                Config.Overlay.FrameStyle.Set(style);
                Config.Overlay.Save();
            }
            else
            {
                Config.Main.FrameStyle.Set(style);
                Config.Main.Save();
            }
            if (Config.Main.CloseFramesOnSelection)
                this.Close();
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            Config.Main.CloseFramesOnSelection.Set(this.closeOnSelect.Checked);
            Config.Main.Save();
        }
    }
}
