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
        Dictionary<string, CheckBox> advancementBoxes;
        Dictionary<string, CheckBox> criterionBoxes;
        Dictionary<string, CheckBox> statisticBoxes;

        Dictionary<string, string> pngs;
        Dictionary<string, string> gifs;

        public FPickFavorites(AdvancementTracker advancementTracker, AchievementTracker achievementTracker, StatisticsTracker statisticsTracker)
        {
            InitializeComponent();
            LoadImageDictionaries();
            Populate(advancementTracker, achievementTracker, statisticsTracker);
            Text += " (" + TrackerSettings.Instance.GameVersion + ")";
        }

        private void LoadImageDictionaries()
        {
            //get dictionary of all png images
            pngs = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(Paths.DIR_TEXTURES, "*.png", SearchOption.AllDirectories))
                pngs[Path.GetFileNameWithoutExtension(file)] = file;

            //get dictionary of all gif images
            gifs = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(Paths.DIR_TEXTURES, "*.gif", SearchOption.AllDirectories))
                gifs[Path.GetFileNameWithoutExtension(file)] = file;
        }

        private void Populate(AdvancementTracker advancementTracker, AchievementTracker achievementTracker, StatisticsTracker statisticsTracker)
        {
            advancementBoxes = new Dictionary<string, CheckBox>();
            criterionBoxes = new Dictionary<string, CheckBox>();
            statisticBoxes = new Dictionary<string, CheckBox>();

            if (TrackerSettings.IsPostExplorationUpdate)
            {
                foreach (var advancement in advancementTracker.FullAdvancementList)
                {
                    AddAdvancement(advancement.Key, advancement.Value);
                    foreach (var criterion in advancement.Value.Criteria)
                        AddCriterion(criterion);
                }
            }
            else
            {
                foreach (var achievement in achievementTracker.FullAchievementList)
                {
                    AddAdvancement(achievement.Key, achievement.Value);
                    foreach (var criterion in achievement.Value.Criteria)
                        AddCriterion(criterion);
                }
            }
            
            foreach (var statistic in statisticsTracker.ItemCountList)
                AddStatistic(statistic);
            CheckBoxes();
        }

        private void AddAdvancement(string key, Advancement advancement)
        {
            //create new check box for this advancement
            var box = new CheckBox();
            box.Text = "     " + advancement.Name;
            box.Width = 175;
            box.Margin = new Padding(0, 0, 1, 1);
            box.ImageAlign = ContentAlignment.MiddleLeft;
            box.Appearance = Appearance.Button;
            box.AutoSize = false;
            box.Tag = advancement.Type;
            box.CheckedChanged += OnCheckedChanged;

            //find appropriate image
            if (pngs.TryGetValue(advancement.Icon, out string icon))
                box.Image = SpriteSheet.BitmapFromFile(icon, 16, 16);
            else if (pngs.TryGetValue(advancement.Icon + SpriteSheet.RESOLUTION_PREFIX + "16", out icon))
                box.Image = SpriteSheet.BitmapFromFile(icon, 16, 16);
            else if (gifs.TryGetValue(advancement.Icon, out icon))
                box.Image = Image.FromFile(icon);

            advancementBoxes[key] = box;
            advancements.Controls.Add(box);
        }

        private void AddCriterion(KeyValuePair<string, Criterion> criterion)
        {
            //create new check box for this criterion
            var box = new CheckBox();
            box.Text = "     " + criterion.Value.Name;
            box.Width = 120;
            box.Margin = new Padding(0, 0, 1, 1);
            box.ImageAlign = ContentAlignment.MiddleLeft;
            box.Appearance = Appearance.Button;
            box.AutoSize = false;

            //find appropriate image
            if (pngs.TryGetValue(criterion.Value.Icon, out string icon))
                box.Image = SpriteSheet.BitmapFromFile(icon, 16, 16);
            else if (pngs.TryGetValue(criterion.Value.Icon + SpriteSheet.RESOLUTION_PREFIX + "16", out icon))
                box.Image = SpriteSheet.BitmapFromFile(icon, 16, 16);
            else if (gifs.TryGetValue(criterion.Value.Icon, out icon))
                box.Image = Image.FromFile(icon);

            criterionBoxes[criterion.Value.ParentID + "/" + criterion.Key] = box;
            criteria.Controls.Add(box);
        }

        private void AddStatistic(KeyValuePair<string, Statistic> statistic)
        {
            //create new check box for this advancement
            var box = new CheckBox();
            box.Text = "     " + statistic.Value.Name;
            box.Width = 120;
            box.Margin = new Padding(0, 0, 1, 1);
            box.ImageAlign = ContentAlignment.MiddleLeft;
            box.Appearance = Appearance.Button;
            box.AutoSize = false;
            box.CheckedChanged += OnCheckedChanged;

            //find appropriate image
            if (pngs.TryGetValue(statistic.Value.Icon, out string icon))
                box.Image = SpriteSheet.BitmapFromFile(icon, 16, 16);
            else if (pngs.TryGetValue(statistic.Value.Icon + SpriteSheet.RESOLUTION_PREFIX + "16", out icon))
                box.Image = SpriteSheet.BitmapFromFile(icon, 16, 16);
            else if (gifs.TryGetValue(statistic.Value.Icon, out icon))
                box.Image = Image.FromFile(icon);

            statisticBoxes[statistic.Key] = box;
            statistics.Controls.Add(box);
        }

        private void CheckBoxes()
        {
            //start appropriate boxes checked
            foreach (var advancement in OverlaySettings.Instance.Favorites.Advancements)
                if (advancementBoxes.ContainsKey(advancement))
                    advancementBoxes[advancement].Checked = true;
            foreach (var criterion in OverlaySettings.Instance.Favorites.Criteria)
                if (criterionBoxes.ContainsKey(criterion))
                    criterionBoxes[criterion].Checked = true;
            foreach (var statistic in OverlaySettings.Instance.Favorites.Statistics)
                if (statisticBoxes.ContainsKey(statistic))
                    statisticBoxes[statistic].Checked = true;
        }


        private void UpdateLabels()
        {
            int total, normal, goal, challenge, minecraft, nether, end, adventure, husbandry;
            total = normal = goal = challenge = minecraft = nether = end = adventure = husbandry = 0;
            foreach (var box in advancementBoxes)
            {
                if (!box.Value.Checked)
                    continue;

                total++;
                switch ((FrameType)box.Value.Tag)
                {
                    case FrameType.Normal:
                        normal++;
                        break;
                    case FrameType.Goal:
                        goal++;
                        break;
                    case FrameType.Challenge:
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
                OverlaySettings.Instance.Favorites.Clear();
                foreach (var box in advancementBoxes)
                    if (box.Value.Checked)
                        OverlaySettings.Instance.Favorites.Advancements.Add(box.Key);
                foreach (var box in criterionBoxes)
                    if (box.Value.Checked)
                        OverlaySettings.Instance.Favorites.Criteria.Add(box.Key);
                foreach (var box in statisticBoxes)
                    if (box.Value.Checked)
                        OverlaySettings.Instance.Favorites.Statistics.Add(box.Key);
                OverlaySettings.Instance.Save();
                Close();
            }
            else if (sender == cancel)
                Close();
            else if (sender == advancementsAll)
                foreach (var box in advancementBoxes.Values)
                    box.Checked = true;
            else if (sender == advancementsNone)
                foreach (var box in advancementBoxes.Values)
                    box.Checked = false;
            else if (sender == criteriaAll)
                foreach (var box in criterionBoxes.Values)
                    box.Checked = true;
            else if (sender == criteriaNone)
                foreach (var box in criterionBoxes.Values)
                    box.Checked = false;
        }
    }
}
