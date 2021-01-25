using AATool.DataStructures;
using AATool.Graphics;
using AATool.Settings;
using AATool.Trackers;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FPickFavorites : Form
    {
        Dictionary<string, CheckBox> boxes;

        public FPickFavorites(AdvancementTracker advancementTracker)
        {
            InitializeComponent();
            Populate(advancementTracker);
        }

        private void Populate(AdvancementTracker advancementTracker)
        {
            var pngs = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(Paths.DIR_TEXTURES, "*.png", SearchOption.AllDirectories))
                pngs[Path.GetFileNameWithoutExtension(file)] = file;

            var gifs = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(Paths.DIR_TEXTURES, "*.gif", SearchOption.AllDirectories))
                gifs[Path.GetFileNameWithoutExtension(file)] = file;

            boxes = new Dictionary<string, CheckBox>();
            foreach (var advancement in advancementTracker.AdvancementList)
            {
                var box = new CheckBox();
                box.Text = "     " + advancement.Value.Name;
                box.Width = 175;
                box.Margin = new Padding(0, 0, 1, 1);
                box.ImageAlign = ContentAlignment.MiddleLeft;
                box.Appearance = Appearance.Button;
                box.AutoSize = false;
                box.Tag = advancement.Value.Type;
                box.CheckedChanged += OnCheckedChanged;
                string file;
                if (pngs.TryGetValue(advancement.Value.Icon, out file))
                    box.Image = TextureSet.BitmapFromFile(file, 16, 16);
                else if (gifs.TryGetValue(advancement.Value.Icon, out file))
                    box.Image = Image.FromFile(file);

                boxes[advancement.Key] = box;
                flow.Controls.Add(box);
            }
            foreach (var important in OverlaySettings.Instance.Favorites)
                boxes[important].Checked = true;
        }

        private void UpdateLabels()
        {
            int total, normal, goal, challenge, minecraft, nether, end, adventure, husbandry;
            total = normal = goal = challenge = minecraft = nether = end = adventure = husbandry = 0;
            foreach (var box in boxes)
            {
                if (!box.Value.Checked)
                    continue;

                total++;
                switch ((AdvancementType)box.Value.Tag)
                {
                    case AdvancementType.Normal:
                        normal++;
                        break;
                    case AdvancementType.Goal:
                        goal++;
                        break;
                    case AdvancementType.Challenge:
                        challenge++;
                        break;
                }

                if (box.Key.Contains(":nether"))
                    nether++;
                else if (box.Key.Contains(":end"))
                    end++;
                else if (box.Key.Contains(":adventure"))
                    adventure++;
                else if (box.Key.Contains(":husbandry"))
                    husbandry++;
                else
                    minecraft++;
            }
            countTotal.Text     = total.ToString();
            countMinecraft.Text = minecraft.ToString();
            countNether.Text    = nether.ToString();
            countEnd.Text       = end.ToString();
            countAdventure.Text = adventure.ToString();
            countHusbandry.Text = husbandry.ToString();
            countNormal.Text    = normal.ToString();
            countGoal.Text     = goal.ToString();
            countChallenge.Text = challenge.ToString();
        }

        private void OnCheckedChanged(object sender, System.EventArgs e)
        {
            UpdateLabels();
        }

        private void OnClick(object sender, System.EventArgs e)
        {
            if (sender == save)
            {
                var important = new HashSet<string>();
                foreach (var box in boxes)
                    if (box.Value.Checked)
                        important.Add(box.Key);
                OverlaySettings.Instance.Favorites = important;
                OverlaySettings.Instance.Save();
                Close();
            }
            else if (sender == cancel)
                Close();
            else if (sender == all)
                foreach (var box in boxes.Values)
                    box.Checked = true;
            else if (sender == none)
                foreach (var box in boxes.Values)
                    box.Checked = false;
            else if (sender == defaults)
            {
                foreach (var box in boxes.Values)
                    box.Checked = false;
                foreach (var favorite in OverlaySettings.Instance.DefaultFavorites)
                    if (boxes.TryGetValue(favorite, out var box))
                        box.Checked = true;
            }
                
        }
    }
}
