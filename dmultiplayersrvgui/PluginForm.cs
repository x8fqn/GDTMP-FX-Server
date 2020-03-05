using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Darkly.GDTMP
{
    public partial class PluginForm : Form
    {
        public PluginForm()
        {
            InitializeComponent();
            MinimumSize = new Size(Size.Width - 10, Size.Height - 10);
            MaximumSize = new Size(Size.Width - 10, Size.Height - 10);
            Size = new Size(Size.Width - 10, Size.Height - 10);
            Icon = Properties.Resources.gdtmp;

            pluginTreeView.ImageList = new ImageList();
            pluginTreeView.ImageList.Images.Add(Properties.Resources.dot);
            pluginTreeView.ImageList.Images.Add(Properties.Resources.yep);
            pluginTreeView.ImageList.Images.Add(Properties.Resources.nope);

            RefreshPlugins();
        }

        private void changeStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pluginTreeView.SelectedNode.Parent.Text == "Plugins")
            {
                if (changeStateToolStripMenuItem.Text == "Enable")
                    dmultiplayersrv.EnablePlugin(pluginTreeView.SelectedNode.Text);
                else if (changeStateToolStripMenuItem.Text == "Disable")
                    dmultiplayersrv.DisablePlugin(pluginTreeView.SelectedNode.Text);
            }
            else if (pluginTreeView.SelectedNode.Parent.Text == "Extensions")
            {
                if (changeStateToolStripMenuItem.Text == "Enable")
                    dmultiplayersrv.EnableExtension(pluginTreeView.SelectedNode.Text);
                else if (changeStateToolStripMenuItem.Text == "Disable")
                    dmultiplayersrv.DisableExtension(pluginTreeView.SelectedNode.Text);
            }

            RefreshPlugins();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pluginTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            pluginTreeView.SelectedNode = pluginTreeView.GetNodeAt(e.Location);

            if (pluginTreeView.SelectedNode != null && pluginTreeView.SelectedNode.Parent != null && e.Button == MouseButtons.Right)
            {
                if (pluginTreeView.SelectedNode.Parent.Text == "Plugins")
                    changeStateToolStripMenuItem.Text = !dmultiplayersrv.PluginEnabled(pluginTreeView.SelectedNode.Text) ? "Enable" : "Disable";
                else if (pluginTreeView.SelectedNode.Parent.Text == "Extensions")
                    changeStateToolStripMenuItem.Text = !dmultiplayersrv.ExtensionEnabled(pluginTreeView.SelectedNode.Text) ? "Enable" : "Disable";
                else
                    return;

                itemStrip.Show(pluginTreeView, e.Location);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshPlugins();
        }

        private void RefreshPlugins()
        {
            pluginTreeView.Nodes.Clear();

            string[] pluginfiles = Directory.GetFiles(dmultiplayersrv.PLUGINFOLDER, "*" + dmultiplayersrv.JSPLUGIN_EXTENSION, SearchOption.TopDirectoryOnly);
            List<TreeNode> pluginnodes = new List<TreeNode>();
            foreach (string pluginfile in pluginfiles)
            {
                string pluginname = Path.GetFileNameWithoutExtension(pluginfile);
                int image = dmultiplayersrv.PluginEnabled(pluginname) ? 1 : 2;
                pluginnodes.Add(new TreeNode(pluginname, image, image));
            }

            string[] extensionfiles = Directory.GetFiles(dmultiplayersrv.EXTENSIONFOLDER, "*" + dmultiplayersrv.NETPLUGIN_EXTENSION, SearchOption.TopDirectoryOnly);
            List<TreeNode> extensionnodes = new List<TreeNode>();
            foreach (string extensionfile in extensionfiles)
            {
                string pluginname = Path.GetFileNameWithoutExtension(extensionfile);
                int image = dmultiplayersrv.ExtensionEnabled(pluginname) ? 1 : 2;
                extensionnodes.Add(new TreeNode(pluginname, image, image));
            }

            pluginTreeView.Nodes.Add(new TreeNode("Plugins", 0, 0, pluginnodes.ToArray()));
            pluginTreeView.Nodes.Add(new TreeNode("Extensions", 0, 0, extensionnodes.ToArray()));

            pluginTreeView.ExpandAll();
        }

        private void SaveSettings()
        {
            throw new NotImplementedException();
        }
    }
}