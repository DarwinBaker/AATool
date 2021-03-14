using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace AAUpdate
{
    public partial class FMain : Form
    {
        private void SetBar(MinecraftBar bar, int percent) => background.ReportProgress(0, (bar, percent));
        private void SetLabel(Label label, string text)    => background.ReportProgress(0, (label, text));

        private Updater updater;

        public FMain(Updater updater)
        {
            InitializeComponent();
            this.updater = updater;
            updater.ProgressChanged += OnProgressChanged;
            updater.StatusChanged   += OnStatusChanged;
            background.RunWorkerAsync();
        }

        private void OnStatusChanged(int key, string value)
        {
            switch (key)
            {
                case Updater.STATUS_1:
                    SetLabel(statusGeneric, value);
                    break;
                case Updater.STATUS_2:
                    SetLabel(statusSpecific, value);
                    break;
                case Updater.STATUS_3:
                    SetLabel(statusFile, value);
                    break;
            }
        }

        private void OnProgressChanged(int key, (int, int) value)
        {
            MinecraftBar bar = null;
            Label labelTotal = null;
            Label labelPercent = null;
            switch (key)
            {
                case Updater.PROGRESS_1:
                    bar = barOverall;
                    labelTotal = totalOverall;
                    labelPercent = percentOverall;
                    break;
                case Updater.PROGRESS_2:
                    bar = barCurrent;
                    labelTotal = totalCurrent;
                    labelPercent = percentCurrent;
                    break;
            }

            //update label text
            int percent = (int)Math.Round(((double)value.Item1 / value.Item2) * 100);
            if (bar.Value == 0 && percent == 100 || bar.Value == 100 && percent == 100)
                SetLabel(labelTotal, "");
            else
                SetLabel(labelTotal, value.Item1 + " / " + value.Item2);
            SetLabel(labelPercent, percent + "%");

            //update progress bar fill
            SetBar(bar, percent);
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            updater.TryUpdate();
        }

        private void OnReportProgress(object sender, ProgressChangedEventArgs e)
        {
            //update ui thread
            if (e.UserState == null)
                return;
            if (e.UserState.GetType() == typeof(ValueTuple<MinecraftBar, int>))
            {
                var state = (ValueTuple<MinecraftBar, int>)e.UserState;
                state.Item1.SetValue(state.Item2);
            }
            else if (e.UserState.GetType() == typeof(ValueTuple<Label, string>))
            {
                var state = (ValueTuple<Label, string>)e.UserState;
                state.Item1.Text = state.Item2;
            }
        }

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (updater.StartAAToolWhenDone)
                Close();
        }
    }
}
