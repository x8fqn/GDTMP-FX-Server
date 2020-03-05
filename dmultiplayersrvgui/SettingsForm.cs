using System;
using System.Drawing;
using System.Windows.Forms;

namespace Darkly.GDTMP
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            MinimumSize = new Size(Size.Width - 10, Size.Height - 10);
            MaximumSize = new Size(Size.Width - 10, Size.Height - 10);
            Size = new Size(Size.Width - 10, Size.Height - 10);
            Icon = Properties.Resources.gdtmp;

            nudPort.Value = dmultiplayersrv.Settings.port;
            nudTimeout.Value = dmultiplayersrv.Settings.timeout;

            switch (dmultiplayersrv.Settings.maxmoney)
            {
                case -1:
                    cbMM.SelectedItem = "Off";
                    break;

                case 100000000000:
                    cbMM.SelectedItem = "100B";
                    break;

                case 1000000000000:
                    cbMM.SelectedItem = "1T";
                    break;

                case 10000000000000:
                    cbMM.SelectedItem = "10T";
                    break;

                case 100000000000000:
                    cbMM.SelectedItem = "100T";
                    break;

                case 1000000000000000:
                    cbMM.SelectedItem = "1Q";
                    break;

                default:
                    cbMM.Items.Add("Other");
                    cbMM.SelectedItem = "Other";
                    break;
            }

            chkAPF.Checked = dmultiplayersrv.Settings.autopf;
            chkCMA.Checked = dmultiplayersrv.Settings.cheatmodallowed;
            chkEOP.Checked = dmultiplayersrv.Settings.extendedopprivs;
            chkPOI.Checked = dmultiplayersrv.Settings.permaopip;
            chkSB.Checked = dmultiplayersrv.Settings.srvbrowser;
            chkSSS.Checked = dmultiplayersrv.Settings.serversidesave;
            chkSSS.CheckedChanged += chkSSS_CheckedChanged;

            chkMOTD.Checked = dmultiplayersrv.Settings.motd.ToLower() != "disabled";
            if (chkMOTD.Checked)
                tbMOTD.Text = dmultiplayersrv.Settings.motd;
            else
                tbMOTD.ReadOnly = true;

            chkDesc.Checked = dmultiplayersrv.Settings.description.ToLower() != "disabled";
            if (chkDesc.Checked)
                tbDesc.Text = dmultiplayersrv.Settings.description;
            else
                tbDesc.ReadOnly = true;

            chkOC.Checked = dmultiplayersrv.Settings.offlineconsoles;
            chkSC.Checked = dmultiplayersrv.Settings.syncconsoles;
            chkTS.Checked = dmultiplayersrv.Settings.timesync;

            chkRB.Enabled = chkTS.Checked;
            chkRB.Checked = chkTS.Checked && dmultiplayersrv.Settings.reviewbattle;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkDesc_CheckedChanged(object sender, EventArgs e)
        {
            tbDesc.ReadOnly = !chkDesc.Checked;
        }

        private void chkMOTD_CheckedChanged(object sender, EventArgs e)
        {
            tbMOTD.ReadOnly = !chkMOTD.Checked;
        }

        private void chkSSS_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSSS.Checked && MessageBox.Show(this, "Warning: This feature allows clients to store their save files on your server computer. It's designed to be fairly secure, but it uses much more bandwidth and could still possibly be vulnerable to hacker exploits if you are running a public server, so use it with caution!\n\nDo you want to enable server-side saving anyway?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                chkSSS.Checked = false;
        }

        private void chkTS_CheckedChanged(object sender, EventArgs e)
        {
            chkRB.Enabled = chkTS.Checked;
            if (!chkTS.Checked)
                chkRB.Checked = false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            dmultiplayersrv.Settings.port = Convert.ToUInt16(nudPort.Value);
            dmultiplayersrv.Settings.timeout = Convert.ToUInt16(nudTimeout.Value);

            switch (cbMM.SelectedItem.ToString())
            {
                case "Off":
                    dmultiplayersrv.Settings.maxmoney = -1;
                    break;

                case "1T":
                    dmultiplayersrv.Settings.maxmoney = 1000000000000;
                    break;

                case "10T":
                    dmultiplayersrv.Settings.maxmoney = 10000000000000;
                    break;

                case "100T":
                    dmultiplayersrv.Settings.maxmoney = 100000000000000;
                    break;

                case "1Q":
                    dmultiplayersrv.Settings.maxmoney = 1000000000000000;
                    break;
            }

            dmultiplayersrv.Settings.autopf = chkAPF.Checked;
            dmultiplayersrv.Settings.cheatmodallowed = chkCMA.Checked;
            dmultiplayersrv.Settings.extendedopprivs = chkEOP.Checked;
            dmultiplayersrv.Settings.permaopip = chkPOI.Checked;
            dmultiplayersrv.Settings.srvbrowser = chkSB.Checked;
            dmultiplayersrv.Settings.serversidesave = chkSSS.Checked;

            if (chkMOTD.Checked && tbMOTD.Text.Length > 0)
                dmultiplayersrv.Settings.motd = tbMOTD.Text;
            else
                dmultiplayersrv.Settings.motd = "disabled";

            if (chkDesc.Checked && tbDesc.Text.Length > 0)
                dmultiplayersrv.Settings.description = tbDesc.Text;
            else
                dmultiplayersrv.Settings.description = "disabled";

            dmultiplayersrv.Settings.offlineconsoles = chkOC.Checked;
            dmultiplayersrv.Settings.syncconsoles = chkSC.Checked;
            dmultiplayersrv.Settings.timesync = chkTS.Checked;
            dmultiplayersrv.Settings.reviewbattle = chkRB.Checked;

            dmultiplayersrv.SaveSettings();
            dmultiplayersrv.SendSettings();

            Close();
        }
    }
}