using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Darkly.GDTMP
{
    public partial class SrvForm : Form
    {
        private List<long> clientIDs = new List<long>();
        private bool firstLBChange = true;
        private double? prevCPU;
        private float prevFScaleX = 1F;
        private float prevFScaleY = 1F;
        private SizeF prevSize;
        private int updateCount = 0;

        public SrvForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.gdtmp;

            foreach (Control control in Controls.Cast<Control>())
                control.TabStop = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Tab:
                case (Keys.Shift | Keys.Tab):
                    return true;

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void addPoint(Series series, double? x, double? y, int max)
        {
            series.Points.AddXY(x, y);

            if (series.Points.Count > max && max != 0)
                series.Points.RemoveAt(0);
        }

        private void bannedBox_MouseDown(object sender, MouseEventArgs e)
        {
            bannedBox.SelectedIndex = bannedBox.IndexFromPoint(e.Location);

            if (bannedBox.SelectedIndex > -1 && e.Button == MouseButtons.Right)
                bannedStrip.Show(bannedBox, e.Location);
        }

        private void bannedStrip_Opening(object sender, CancelEventArgs e)
        {
            if (bannedBox.SelectedIndex < 0)
                e.Cancel = true;
        }

        private void banToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dmultiplayersrv.Client client = dmultiplayersrv.Clients[dmultiplayersrv.GetIndex(clientIDs[playerListBox.SelectedIndex])];
                if (MessageBox.Show(this, "Are you sure you want to ban " + client.Name + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    dmultiplayersrv.Command("ban " + clientIDs[playerListBox.SelectedIndex], null);
            }
            catch
            {
            }
        }

        private void deopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dmultiplayersrv.Client client = dmultiplayersrv.Clients[dmultiplayersrv.GetIndex(clientIDs[playerListBox.SelectedIndex])];
                if (!client.Op || MessageBox.Show(this, "Are you sure you want to remove operator powers from " + dmultiplayersrv.Clients[dmultiplayersrv.GetIndex(clientIDs[playerListBox.SelectedIndex])].Name + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    dmultiplayersrv.Command("deop " + clientIDs[playerListBox.SelectedIndex], null);
            }
            catch
            {
            }
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                SendData();
                e.Handled = true;
            }
        }

        private void inputBox_TextChanged(object sender, EventArgs e)
        {
            inputBox.Text = inputBox.Text.Replace("\n", string.Empty).Replace("\r", string.Empty);
        }

        private void kickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dmultiplayersrv.Client client = dmultiplayersrv.Clients[dmultiplayersrv.GetIndex(clientIDs[playerListBox.SelectedIndex])];
                if (MessageBox.Show(this, "Are you sure you want to kick " + client.Name + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    dmultiplayersrv.Command("kick " + clientIDs[playerListBox.SelectedIndex], null);
            }
            catch
            {
            }
        }

        private void lengthBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firstLBChange)
            {
                firstLBChange = false;
                return;
            }

            switch (lengthBox.SelectedIndex)
            {
                case 0:
                    resourceChart.Series[0].Enabled = true;
                    resourceChart.Series[1].Enabled = true;
                    resourceChart.Series[2].Enabled = false;
                    resourceChart.Series[3].Enabled = false;
                    resourceChart.Series[4].Enabled = false;
                    resourceChart.Series[5].Enabled = false;
                    playerChart.Series[0].Enabled = true;
                    playerChart.Series[1].Enabled = false;
                    playerChart.Series[2].Enabled = false;

                    resourceChart.Legends[0].Enabled = true;
                    resourceChart.Legends[1].Enabled = false;
                    resourceChart.Legends[2].Enabled = false;

                    break;

                case 1:
                    resourceChart.Series[0].Enabled = false;
                    resourceChart.Series[1].Enabled = false;
                    resourceChart.Series[2].Enabled = true;
                    resourceChart.Series[3].Enabled = true;
                    resourceChart.Series[4].Enabled = false;
                    resourceChart.Series[5].Enabled = false;
                    playerChart.Series[0].Enabled = false;
                    playerChart.Series[1].Enabled = true;
                    playerChart.Series[2].Enabled = false;

                    resourceChart.Legends[0].Enabled = false;
                    resourceChart.Legends[1].Enabled = true;
                    resourceChart.Legends[2].Enabled = false;
                    break;

                case 2:
                    resourceChart.Series[0].Enabled = false;
                    resourceChart.Series[1].Enabled = false;
                    resourceChart.Series[2].Enabled = false;
                    resourceChart.Series[3].Enabled = false;
                    resourceChart.Series[4].Enabled = true;
                    resourceChart.Series[5].Enabled = true;
                    playerChart.Series[0].Enabled = false;
                    playerChart.Series[1].Enabled = false;
                    playerChart.Series[2].Enabled = true;

                    resourceChart.Legends[0].Enabled = false;
                    resourceChart.Legends[1].Enabled = false;
                    resourceChart.Legends[2].Enabled = true;
                    break;
            }
        }

        private void moreInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dmultiplayersrv.GetIndex(clientIDs[playerListBox.SelectedIndex]);

                if (index > -1)
                    new InfoForm(index).Show();
            }
            catch
            {
            }
        }

        private void opToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                dmultiplayersrv.Client client = dmultiplayersrv.Clients[dmultiplayersrv.GetIndex(clientIDs[playerListBox.SelectedIndex])];
                if (client.Op || MessageBox.Show(this, "Are you sure you want to give operator powers to " + dmultiplayersrv.Clients[dmultiplayersrv.GetIndex(clientIDs[playerListBox.SelectedIndex])].Name + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    dmultiplayersrv.Command("op " + clientIDs[playerListBox.SelectedIndex], null);
            }
            catch
            {
            }
        }

        private void permaDeopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = (string)permaOpBox.SelectedItem;
                if (MessageBox.Show(this, "Are you sure you want to remove operator powers from " + ip + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dmultiplayersrv.Command("deopip " + ip, null);
                    ResetLists();
                }
            }
            catch
            {
            }
        }

        private void permaOpBox_MouseDown(object sender, MouseEventArgs e)
        {
            permaOpBox.SelectedIndex = permaOpBox.IndexFromPoint(e.Location);

            if (permaOpBox.SelectedIndex > -1 && e.Button == MouseButtons.Right)
                permaOpStrip.Show(permaOpBox, e.Location);
        }

        private void permaOpStrip_Opening(object sender, CancelEventArgs e)
        {
            if (permaOpBox.SelectedIndex < 0)
                e.Cancel = true;
        }

        private void playerListBox_MouseDown(object sender, MouseEventArgs e)
        {
            playerListBox.SelectedIndex = playerListBox.IndexFromPoint(e.Location);

            if (playerListBox.SelectedIndex > -1 && e.Button == MouseButtons.Right)
                playerListStrip.Show(playerListBox, e.Location);
        }

        private void playerListStrip_Opening(object sender, CancelEventArgs e)
        {
            if (playerListBox.SelectedIndex < 0)
                e.Cancel = true;
        }

        private void pluginButton_Click(object sender, EventArgs e)
        {
            new PluginForm().ShowDialog();
        }

        private void ResetLists()
        {
            bannedBox.Items.Clear();
            permaOpBox.Items.Clear();

            foreach (string ip in dmultiplayersrv.BannedIPs)
                bannedBox.Items.Add(ip);
            foreach (string ip in dmultiplayersrv.OppedIPs)
                permaOpBox.Items.Add(ip);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            SendData();
        }

        private void SendData()
        {
            outputBox.AppendText("> " + inputBox.Text + "\r\n");
            dmultiplayersrv.Command(inputBox.Text);
            inputBox.Text = string.Empty;
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            new SettingsForm().ShowDialog();
        }

        private void srvForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dmultiplayersrv.Command("exit");
        }

        private void srvForm_Load(object sender, EventArgs e)
        {
            UpdateFormTitle();

            prevSize = Size;
            lengthBox.SelectedIndex = 0;

            Timer cutimer = new Timer();
            cutimer.Interval = 1000;
            cutimer.Tick += Update;
            cutimer.Start();

            dmultiplayersrv.InitServer(new OutWriter(outputBox));

            dmultiplayersrv.AddEventHandler(dmultiplayersrv.Events.AFTERCOMMAND, new Func<object[], bool>((args) =>
            {
                ResetLists();
                return false;
            }));
            dmultiplayersrv.AddEventHandler(dmultiplayersrv.Events.ONUPDATEAVAILABLE, new Func<object[], bool>((args) =>
            {
                MinimumSize = new Size(MinimumSize.Width, MinimumSize.Height + 6);
                Height += 6;
                updateLinkLabel.Text = "Server version " + args[0] + " available";
                updateLinkLabel.Visible = true;
                FlashWindowEx(this);
                return false;
            }));

            ResetLists();
        }

        private void srvForm_ResizeEnd(object sender, EventArgs e)
        {
            float curFScaleX = (float)Width / MinimumSize.Width;
            float curFScaleY = (float)Height / MinimumSize.Height;

            foreach (Control control in Controls)
                control.Scale(new SizeF(curFScaleX / prevFScaleX, curFScaleY / prevFScaleY));

            updateLinkLabel.Left = outputGroupBox.Right - updateLinkLabel.Width;

            prevFScaleX = curFScaleX;
            prevFScaleY = curFScaleY;
            prevSize = Size;
        }

        private void unbanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = (string)bannedBox.SelectedItem;
                if (MessageBox.Show(this, "Are you sure you want to unban " + ip + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dmultiplayersrv.Command("unban " + ip, null);
                    ResetLists();
                }
            }
            catch
            {
            }
        }

        private void Update(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Process currentprocess = Process.GetCurrentProcess();

            if (prevCPU != null)
            {
                addPoint(resourceChart.Series[0], updateCount, (currentprocess.TotalProcessorTime.TotalMilliseconds - prevCPU) / 10 / Environment.ProcessorCount, 3600);
                addPoint(resourceChart.Series[1], updateCount, (float)currentprocess.WorkingSet64 / 1024 / 1024, 3600);
                addPoint(playerChart.Series[0], updateCount, dmultiplayersrv.GetPlayerCount(), 3600);

                if (updateCount % 10 == 0)
                {
                    addPoint(resourceChart.Series[2], updateCount, (currentprocess.TotalProcessorTime.TotalMilliseconds - prevCPU) / 10 / Environment.ProcessorCount, 3600 * 24 / 10);
                    addPoint(resourceChart.Series[3], updateCount, (float)currentprocess.WorkingSet64 / 1024 / 1024, 3600 * 24 / 10);
                    addPoint(playerChart.Series[1], updateCount, dmultiplayersrv.GetPlayerCount(), 3600 * 24 / 10);
                }

                if (updateCount % 30 == 0)
                {
                    addPoint(resourceChart.Series[4], updateCount, (currentprocess.TotalProcessorTime.TotalMilliseconds - prevCPU) / 10 / Environment.ProcessorCount, 0);
                    addPoint(resourceChart.Series[5], updateCount, (float)currentprocess.WorkingSet64 / 1024 / 1024, 0);
                    addPoint(playerChart.Series[2], updateCount, dmultiplayersrv.GetPlayerCount(), 0);
                }

                resourceChart.ResetAutoValues();
                playerChart.ResetAutoValues();

                UpdateFormTitle();

                if (dmultiplayersrv.GetPlayerCount() < 20 || updateCount % 10 == 0)
                {
                    object prevselected = playerListBox.SelectedItem;

                    ListBox.ObjectCollection olditems = playerListBox.Items;
                    List<long> oldids = new List<long>();
                    oldids.AddRange(clientIDs);

                    List<long> idstoremove = new List<long>();
                    int i = 0;
                    foreach (long id in clientIDs)
                    {
                        bool remove = true;
                        foreach (dmultiplayersrv.Client client in dmultiplayersrv.Clients)
                        {
                            if (client.ID == id)
                                remove = false;
                        }

                        if (remove)
                        {
                            try
                            {
                                playerListBox.Items.RemoveAt(i);
                                idstoremove.Add(id);
                            }
                            catch
                            {
                            }
                        }

                        i++;
                    }

                    foreach (long id in idstoremove)
                        clientIDs.Remove(id);

                    try
                    {
                        i = 0;
                        foreach (dmultiplayersrv.Client client in dmultiplayersrv.Clients)
                        {
                            if (client.Name != null)
                            {
                                int clientindex = clientIDs.IndexOf(client.ID);
                                string clienttext = (client.Op ? "[OP] " : string.Empty) + client.Name + " (" + client.Boss + ")";

                                if (clientindex < 0)
                                {
                                    playerListBox.Items.Add(clienttext);
                                    clientIDs.Add(client.ID);
                                }
                                else if ((string)playerListBox.Items[clientindex] != clienttext)
                                    playerListBox.Items[clientindex] = clienttext;
                            }
                            i++;
                        }
                    }
                    catch
                    {
                        try
                        {
                            playerListBox.Items.Clear();
                            playerListBox.Items.AddRange(olditems);
                            clientIDs.Clear();
                            clientIDs.AddRange(oldids);
                        }
                        catch
                        {
                        }
                    }

                    if (prevselected != null && playerListBox.Items.Contains(prevselected))
                        playerListBox.SelectedItem = prevselected;
                }
            }

            updateCount++;
            prevCPU = currentprocess.TotalProcessorTime.TotalMilliseconds;
        }

        private void UpdateFormTitle()
        {
            int playercount = dmultiplayersrv.GetPlayerCount();
            Text = dmultiplayersrv.GetPlayerCount() + " player" + (playercount != 1 ? "s" : "") + " connected - GDTMP Server " + Assembly.GetEntryAssembly().GetName().Version;
        }

        private void updateLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dmultiplayersrv.Command("site");
        }

        private class NativeMethods
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
        }

        private class OutWriter : TextWriter
        {
            private static object lockobj = new object();
            private TextBox output;

            public OutWriter(TextBox textbox)
            {
                output = textbox;
            }

            public override Encoding Encoding
            {
                get
                {
                    return Encoding.UTF8;
                }
            }

            public override void Write(string value)
            {
                lock (lockobj)
                {
                    output.BeginInvoke(new MethodInvoker(delegate { output.AppendText(value); }));
                }
            }

            public override void WriteLine()
            {
                Write(Environment.NewLine);
            }

            public override void WriteLine(string value)
            {
                if (value.StartsWith("[") || !value.Contains("System.AggregateException") || !value.Contains("System.ObjectDisposedException"))
                    Write(value + Environment.NewLine);
            }
        }

        #region from http://pinvoke.net/default.aspx/user32.FlashWindowEx

        public enum FlashWindow : uint
        {
            FLASHW_STOP = 0,
            FLASHW_CAPTION = 1,
            FLASHW_TRAY = 2,
            FLASHW_ALL = 3,
            FLASHW_TIMER = 4,
            FLASHW_TIMERNOFG = 12
        }

        public static bool FlashWindowEx(Form frm)
        {
            IntPtr hWnd = frm.Handle;
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = (uint)FlashWindow.FLASHW_ALL | (uint)FlashWindow.FLASHW_TIMERNOFG;
            fInfo.uCount = uint.MaxValue;
            fInfo.dwTimeout = 0;

            return NativeMethods.FlashWindowEx(ref fInfo);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        #endregion from http://pinvoke.net/default.aspx/user32.FlashWindowEx
    }
}