using System;

namespace Darkly.GDTMP
{
    partial class PluginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.closeButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.pluginTreeView = new System.Windows.Forms.TreeView();
            this.itemStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(167, 253);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(145, 23);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(12, 253);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(145, 23);
            this.refreshButton.TabIndex = 2;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // pluginTreeView
            // 
            this.pluginTreeView.Location = new System.Drawing.Point(12, 12);
            this.pluginTreeView.Name = "pluginTreeView";
            this.pluginTreeView.Size = new System.Drawing.Size(300, 235);
            this.pluginTreeView.TabIndex = 4;
            this.pluginTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pluginTreeView_MouseDown);
            // 
            // itemStrip
            // 
            this.itemStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeStateToolStripMenuItem});
            this.itemStrip.Name = "playerListStrip";
            this.itemStrip.Size = new System.Drawing.Size(153, 48);
            // 
            // changeAutoRunToolStripMenuItem
            // 
            this.changeStateToolStripMenuItem.Name = "changeAutoRunToolStripMenuItem";
            this.changeStateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.changeStateToolStripMenuItem.Click += new System.EventHandler(this.changeStateToolStripMenuItem_Click);
            // 
            // PluginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 288);
            this.Controls.Add(this.pluginTreeView);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(340, 322);
            this.MinimumSize = new System.Drawing.Size(340, 322);
            this.Name = "PluginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Plugins/Extensions";
            this.itemStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.TreeView pluginTreeView;
        private System.Windows.Forms.ContextMenuStrip itemStrip;
        private System.Windows.Forms.ToolStripMenuItem changeStateToolStripMenuItem;
    }
}