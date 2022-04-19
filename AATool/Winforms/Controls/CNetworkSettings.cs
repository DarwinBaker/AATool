using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Net;
using AATool.Winforms.Forms;

namespace AATool.Winforms.Controls
{
    public partial class CNetworkSettings : UserControl, INetworkController
    {
        public RichTextBox GetConsole() => this.console;
        public Button GetButton()       => this.networkSwitch;
        public ComboBox GetTypeCombo()  => this.networkType;
        public ListView GetUserList()   => this.peers;

        private bool loaded;

        public CNetworkSettings()
        {
            this.InitializeComponent();
        }

        public void LoadSettings()
        {
            this.loaded = false; 
            if (Peer.IsRunning)
            {
                //network is running. pull credentials from current peer instance
                this.mojangName.Text    = Config.Net.MinecraftName;
                this.displayName.Text   = Config.Net.PreferredName;
                this.pronouns.Text      = Peer.Instance.LocalUser.Pronouns;
                this.ip.Text            = Peer.Instance.Address;
                this.port.Text          = Peer.Instance.Port;
                this.networkType.Text   = Peer.IsServer ? "Server" : "Client";
                this.networkSwitch.Text = Peer.IsServer ? "Stop" : "Disconnect";
            }
            else
            {
                //network not running. use saved credentials
                this.mojangName.Text    = Config.Net.MinecraftName;
                this.displayName.Text   = Config.Net.PreferredName;
                this.pronouns.Text      = Config.Net.Pronouns;
                this.ip.Text            = Config.Net.IP;
                this.port.Text          = ((int)Config.Net.Port).ToString();
                this.networkType.Text   = Config.Net.IsServer ? "Server" : "Client";
                this.networkSwitch.Text = this.networkType.Text is "Server" ? "Start" : "Connect";
            }
            this.password.Text          = Config.Net.Password;
            this.autoServerIP.Checked   = Config.Net.AutoServerIP;
            this.UpdateEnabledStates();
            this.loaded = true;
        }

        private void SaveSettings()
        {
            Config.Net.MinecraftName.Set(this.mojangName.Text);
            Config.Net.PreferredName.Set(this.displayName.Text);
            Config.Net.Pronouns.Set(this.pronouns.Text);
            Config.Net.IP.Set(this.ip.Text);
            if (int.TryParse(this.port.Text, out int portNumber))
                Config.Net.Port.Set(portNumber);
            Config.Net.Password.Set(this.password.Text);
            Config.Net.IsServer.Set(this.networkType.Text.ToLower() is "server");
            Config.Net.AutoServerIP.Set(this.autoServerIP.Checked);
            Config.Net.Save();
        }

        private void ToggleIP()
        {
            bool hide = true;
            if (this.ip.UseSystemPasswordChar)
            {
                //show confirmation dialog
                string message = "Be careful about showing IP addresses on stream! ♥\nAre you sure you want to unmask the IP address field?";
                string title   = "IP Reveal Confirmation";
                DialogResult result = MessageBox.Show(this, message, title,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                hide = result is not DialogResult.Yes;
            }
            this.ip.UseSystemPasswordChar = hide;
            this.toggleIP.Text = hide ? "Show IP Address" : "Hide IP Address";
        }

        private void TogglePassword()
        {
            bool hide = true;
            if (this.password.UseSystemPasswordChar)
            {
                //show confirmation dialog
                string message = "Be careful about showing passwords on stream! ♥\nAre you sure you want to unmask the password field?";
                string title   = "Password Reveal Confirmation";
                DialogResult result = MessageBox.Show(this, message, title,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                hide = result is not DialogResult.Yes;
            }
            this.password.UseSystemPasswordChar = hide;
            this.togglePassword.Text = hide ? "Show" : "Hide";
        }

        private void ToggleState()
        {
            if (Peer.IsRunning)
            {
                Peer.StopInstance();
                this.OnIndexChanged(this.networkType, null);
            }
            else
            {
                this.SaveSettings();
                if (this.networkType.Text.ToLower() is "server")
                    Peer.StartInstanceOf<Server>(this.ip.Text, this.port.Text);
                else
                    Peer.StartInstanceOf<Client>(this.ip.Text, this.port.Text);
            }
        }

        private void KickSelectedPlayer()
        {
            if (Server.TryGet(out Server server))
            {
                foreach (ListViewItem peer in this.peers.SelectedItems)
                {
                    if (peer.Tag is Uuid id)
                        server.KickPlayer(id);
                } 
            }
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (sender == this.toggleIP)
            {
                this.ToggleIP();
            }
            else if (sender == this.togglePassword)
            {
                this.TogglePassword();
            }
            else if (sender == this.networkSwitch)
            {
                this.ToggleState();
            }
            else if (sender == this.kick)
            {
                this.KickSelectedPlayer();
            }
            else if (sender == this.runSetup)
            {
                using var dialog = new FNetworkSetup();
                dialog.ShowDialog();
                this.LoadSettings();
            }
        }

        private void OnIndexChanged(object sender, EventArgs e)
        {
            if (sender == this.networkType)
            {
                bool isClient = sender is Control control && control.Text.ToLower() is "client";
                this.networkSwitch.Text = isClient ? "Connect" : "Host";
                this.autoServerIP.Visible = !isClient;
            }
            if (this.loaded)
                this.UpdateEnabledStates();
        }

        private void OnCheckChanged(object sender, EventArgs e)
        {
            if (sender == this.autoServerIP)
            {
                if (sender is CheckBox box && box.Checked)
                {
                    this.ip.Enabled = false;
                    if (NetworkHelper.TryGetLocalIPAddress(out IPAddress address))
                        this.ip.Text = address.ToString();
                }
                else
                {
                    this.ip.Enabled = true;
                    this.ip.Text = Config.Net.IP;
                }
                this.SaveSettings();
            }
            if (this.loaded)
                this.UpdateEnabledStates();
        }

        public void SetControlStates(string buttonText, bool enableButton, bool enableDropDown)
        {
            if (this.IsDisposed)
                return;
            try
            {
                //update network switch button on main thread
                this.Invoke((MethodInvoker)delegate {
                    this.networkSwitch.Text = buttonText;
                    this.networkSwitch.Enabled = enableButton;
                    this.networkType.Enabled = enableDropDown;
                    this.UpdateEnabledStates();
                });
            }
            catch { }
        }

        public void WriteToConsole(string line)
        {
            if (this.IsDisposed)
                return;
            try
            {
                //update console on ui thread
                this.Invoke((MethodInvoker)delegate {
                    if (string.IsNullOrEmpty(this.GetConsole().Text))
                        this.SyncConsole();
                    this.GetConsole().AppendText(line + "\n");
                });
            }
            catch { }
        }

        public void SyncConsole()
        {
            if (this.IsDisposed)
                return;
            try
            {
                //update console on ui thread
                this.Invoke((MethodInvoker)delegate {
                    this.GetConsole().Text = Peer.IsRunning 
                        ? Peer.Instance.ConsoleOutput 
                        : string.Empty;
                });
            }
            catch { }
        }

        public void SyncUserList(IEnumerable<User> users)
        {
            if (this.IsDisposed)
                return;
            try
            {
                //update network switch button on ui thread
                this.Invoke((MethodInvoker)delegate {
                    this.GetUserList().Items.Clear();
                    foreach (User user in users)
                    {
                        var item = new ListViewItem() {
                            Text = user.Name,
                            Tag  = user.Id
                        };
                        this.GetUserList().Items.Add(item);
                    }
                });
            }
            catch { }
        }

        private void UpdateEnabledStates()
        {
            if (Peer.IsRunning)
            {
                this.ip.Enabled           = false;
                this.port.Enabled         = false;
                this.password.Enabled     = false;
                this.mojangName.Enabled   = false;
                this.displayName.Enabled  = false;
                this.pronouns.Enabled     = false;
                this.autoServerIP.Enabled = false;
                this.runSetup.Enabled     = false;
                this.kick.Enabled = Peer.IsServer;
            }
            else
            {
                this.ip.Enabled           = this.networkType.Text.ToLower() is "client" || !this.autoServerIP.Checked;
                this.port.Enabled         = true;
                this.password.Enabled     = true;
                this.mojangName.Enabled   = true;
                this.displayName.Enabled  = true;
                this.pronouns.Enabled     = true;    
                this.autoServerIP.Enabled = true;
                this.runSetup.Enabled     = true;
                this.kick.Enabled         = false;
            }
        }

        private void OnEnabledChanged(object sender, EventArgs e) => this.SaveSettings();

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox box && box.Text.ToLower() is "write your own")
            {
                box.SelectedIndex = -1;
                box.Text = "";
            }
        }

        private void OnLoad(object sender, EventArgs e) => Peer.BindController(this);
    }
}
