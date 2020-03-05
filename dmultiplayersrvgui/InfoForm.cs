using System;
using System.Drawing;
using System.Windows.Forms;

namespace Darkly.GDTMP
{
    public partial class InfoForm : Form
    {
        private int clientindex;

        public InfoForm(int clientindex)
        {
            InitializeComponent();
            MinimumSize = new Size(Size.Width - 10, Size.Height - 10);
            MaximumSize = new Size(Size.Width - 10, Size.Height - 10);
            Size = new Size(Size.Width - 10, Size.Height - 10);
            Icon = Properties.Resources.gdtmp;

            this.clientindex = clientindex;
            RefreshInfo();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ocbMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            ocbMods.SelectedIndex = -1;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            try
            {
                dmultiplayersrv.Client client = dmultiplayersrv.Clients[clientindex];
                Text = client.Name + " (Player Information)";

                olblCN.Text = client.Name;
                olblCB.Text = client.Boss;
                olblCash.Text = dmultiplayersrv.ShortNumber(client.Cash);
                olblFans.Text = dmultiplayersrv.ShortNumber(client.Fans);
                olblRP.Text = client.ResearchPoints.ToString();
                olblCW.Text = dmultiplayersrv.WeekString(client.CurrentWeek);
                olblOp.Text = client.Op ? "Yes" : "No";
                olblID.Text = client.ID.ToString();
                olblIP.Text = client.Context.ConnectionInfo.ClientIpAddress;

                ocbMods.Items.Clear();
                foreach (dmultiplayersrv.ClientMod mod in client.Mods)
                    ocbMods.Items.Add(mod.Name);
            }
            catch
            {
                Close();
            }
        }
    }
}