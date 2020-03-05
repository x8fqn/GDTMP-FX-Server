namespace Darkly.GDTMP
{
    partial class SrvForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.outputBox = new System.Windows.Forms.TextBox();
            this.inputBox = new System.Windows.Forms.TextBox();
            this.playerListGroupBox = new System.Windows.Forms.GroupBox();
            this.playerTabs = new System.Windows.Forms.TabControl();
            this.connectedTab = new System.Windows.Forms.TabPage();
            this.playerListBox = new System.Windows.Forms.ListBox();
            this.playerListStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.opToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.banToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSeperator = new System.Windows.Forms.ToolStripSeparator();
            this.moreInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bannedTab = new System.Windows.Forms.TabPage();
            this.bannedBox = new System.Windows.Forms.ListBox();
            this.bannedStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.unbanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.permaOpTab = new System.Windows.Forms.TabPage();
            this.permaOpBox = new System.Windows.Forms.ListBox();
            this.permaOpStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.permaDeopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphsGroupBox = new System.Windows.Forms.GroupBox();
            this.lengthBox = new System.Windows.Forms.ComboBox();
            this.resourceChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.playerChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.settingsButton = new System.Windows.Forms.Button();
            this.outputGroupBox = new System.Windows.Forms.GroupBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.updateLinkLabel = new System.Windows.Forms.LinkLabel();
            this.pluginButton = new System.Windows.Forms.Button();
            this.playerListGroupBox.SuspendLayout();
            this.playerTabs.SuspendLayout();
            this.connectedTab.SuspendLayout();
            this.playerListStrip.SuspendLayout();
            this.bannedTab.SuspendLayout();
            this.bannedStrip.SuspendLayout();
            this.permaOpTab.SuspendLayout();
            this.permaOpStrip.SuspendLayout();
            this.graphsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resourceChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerChart)).BeginInit();
            this.outputGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // outputBox
            // 
            this.outputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputBox.BackColor = System.Drawing.SystemColors.Window;
            this.outputBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.outputBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputBox.HideSelection = false;
            this.outputBox.Location = new System.Drawing.Point(6, 19);
            this.outputBox.MaxLength = 4000000;
            this.outputBox.Multiline = true;
            this.outputBox.Name = "outputBox";
            this.outputBox.ReadOnly = true;
            this.outputBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputBox.Size = new System.Drawing.Size(518, 486);
            this.outputBox.TabIndex = 0;
            this.outputBox.WordWrap = false;
            // 
            // inputBox
            // 
            this.inputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputBox.BackColor = System.Drawing.Color.White;
            this.inputBox.Location = new System.Drawing.Point(6, 511);
            this.inputBox.Multiline = true;
            this.inputBox.Name = "inputBox";
            this.inputBox.Size = new System.Drawing.Size(434, 20);
            this.inputBox.TabIndex = 1;
            this.inputBox.TextChanged += new System.EventHandler(this.inputBox_TextChanged);
            this.inputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputBox_KeyDown);
            // 
            // playerListGroupBox
            // 
            this.playerListGroupBox.Controls.Add(this.playerTabs);
            this.playerListGroupBox.Location = new System.Drawing.Point(12, 12);
            this.playerListGroupBox.Name = "playerListGroupBox";
            this.playerListGroupBox.Size = new System.Drawing.Size(218, 238);
            this.playerListGroupBox.TabIndex = 2;
            this.playerListGroupBox.TabStop = false;
            this.playerListGroupBox.Text = "Players";
            // 
            // playerTabs
            // 
            this.playerTabs.Controls.Add(this.connectedTab);
            this.playerTabs.Controls.Add(this.bannedTab);
            this.playerTabs.Controls.Add(this.permaOpTab);
            this.playerTabs.Location = new System.Drawing.Point(6, 19);
            this.playerTabs.Name = "playerTabs";
            this.playerTabs.SelectedIndex = 0;
            this.playerTabs.Size = new System.Drawing.Size(206, 219);
            this.playerTabs.TabIndex = 1;
            // 
            // connectedTab
            // 
            this.connectedTab.Controls.Add(this.playerListBox);
            this.connectedTab.Location = new System.Drawing.Point(4, 22);
            this.connectedTab.Name = "connectedTab";
            this.connectedTab.Padding = new System.Windows.Forms.Padding(3);
            this.connectedTab.Size = new System.Drawing.Size(198, 193);
            this.connectedTab.TabIndex = 0;
            this.connectedTab.Text = "Connected";
            this.connectedTab.UseVisualStyleBackColor = true;
            // 
            // playerListBox
            // 
            this.playerListBox.ContextMenuStrip = this.playerListStrip;
            this.playerListBox.FormattingEnabled = true;
            this.playerListBox.HorizontalScrollbar = true;
            this.playerListBox.Location = new System.Drawing.Point(3, 3);
            this.playerListBox.Name = "playerListBox";
            this.playerListBox.Size = new System.Drawing.Size(193, 186);
            this.playerListBox.TabIndex = 0;
            this.playerListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.playerListBox_MouseDown);
            // 
            // playerListStrip
            // 
            this.playerListStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opToolStripMenuItem,
            this.deopToolStripMenuItem,
            this.kickToolStripMenuItem,
            this.banToolStripMenuItem,
            this.tsSeperator,
            this.moreInfoToolStripMenuItem});
            this.playerListStrip.Name = "playerListStrip";
            this.playerListStrip.Size = new System.Drawing.Size(153, 142);
            this.playerListStrip.Opening += new System.ComponentModel.CancelEventHandler(this.playerListStrip_Opening);
            // 
            // opToolStripMenuItem
            // 
            this.opToolStripMenuItem.Name = "opToolStripMenuItem";
            this.opToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.opToolStripMenuItem.Text = "Op";
            this.opToolStripMenuItem.Click += new System.EventHandler(this.opToolStripMenuItem_Click);
            // 
            // deopToolStripMenuItem
            // 
            this.deopToolStripMenuItem.Name = "deopToolStripMenuItem";
            this.deopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deopToolStripMenuItem.Text = "De-op";
            this.deopToolStripMenuItem.Click += new System.EventHandler(this.deopToolStripMenuItem_Click);
            // 
            // kickToolStripMenuItem
            // 
            this.kickToolStripMenuItem.Name = "kickToolStripMenuItem";
            this.kickToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.kickToolStripMenuItem.Text = "Kick";
            this.kickToolStripMenuItem.Click += new System.EventHandler(this.kickToolStripMenuItem_Click);
            // 
            // banToolStripMenuItem
            // 
            this.banToolStripMenuItem.Name = "banToolStripMenuItem";
            this.banToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.banToolStripMenuItem.Text = "Ban";
            this.banToolStripMenuItem.Click += new System.EventHandler(this.banToolStripMenuItem_Click);
            // 
            // tsSeperator
            // 
            this.tsSeperator.Name = "tsSeperator";
            this.tsSeperator.Size = new System.Drawing.Size(149, 6);
            // 
            // moreInfoToolStripMenuItem
            // 
            this.moreInfoToolStripMenuItem.Name = "moreInfoToolStripMenuItem";
            this.moreInfoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.moreInfoToolStripMenuItem.Text = "More info";
            this.moreInfoToolStripMenuItem.Click += new System.EventHandler(this.moreInfoToolStripMenuItem_Click);
            // 
            // bannedTab
            // 
            this.bannedTab.Controls.Add(this.bannedBox);
            this.bannedTab.Location = new System.Drawing.Point(4, 22);
            this.bannedTab.Name = "bannedTab";
            this.bannedTab.Padding = new System.Windows.Forms.Padding(3);
            this.bannedTab.Size = new System.Drawing.Size(198, 193);
            this.bannedTab.TabIndex = 1;
            this.bannedTab.Text = "Banned";
            this.bannedTab.UseVisualStyleBackColor = true;
            // 
            // bannedBox
            // 
            this.bannedBox.ContextMenuStrip = this.bannedStrip;
            this.bannedBox.FormattingEnabled = true;
            this.bannedBox.Location = new System.Drawing.Point(3, 3);
            this.bannedBox.Name = "bannedBox";
            this.bannedBox.Size = new System.Drawing.Size(192, 186);
            this.bannedBox.TabIndex = 1;
            this.bannedBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bannedBox_MouseDown);
            // 
            // bannedStrip
            // 
            this.bannedStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unbanToolStripMenuItem});
            this.bannedStrip.Name = "playerListStrip";
            this.bannedStrip.Size = new System.Drawing.Size(110, 26);
            this.bannedStrip.Opening += new System.ComponentModel.CancelEventHandler(this.bannedStrip_Opening);
            // 
            // unbanToolStripMenuItem
            // 
            this.unbanToolStripMenuItem.Name = "unbanToolStripMenuItem";
            this.unbanToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.unbanToolStripMenuItem.Text = "Unban";
            this.unbanToolStripMenuItem.Click += new System.EventHandler(this.unbanToolStripMenuItem_Click);
            // 
            // permaOpTab
            // 
            this.permaOpTab.Controls.Add(this.permaOpBox);
            this.permaOpTab.Location = new System.Drawing.Point(4, 22);
            this.permaOpTab.Name = "permaOpTab";
            this.permaOpTab.Size = new System.Drawing.Size(198, 193);
            this.permaOpTab.TabIndex = 2;
            this.permaOpTab.Text = "Permanent ops";
            this.permaOpTab.UseVisualStyleBackColor = true;
            // 
            // permaOpBox
            // 
            this.permaOpBox.ContextMenuStrip = this.permaOpStrip;
            this.permaOpBox.FormattingEnabled = true;
            this.permaOpBox.Location = new System.Drawing.Point(3, 3);
            this.permaOpBox.Name = "permaOpBox";
            this.permaOpBox.Size = new System.Drawing.Size(192, 186);
            this.permaOpBox.TabIndex = 2;
            this.permaOpBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.permaOpBox_MouseDown);
            // 
            // permaOpStrip
            // 
            this.permaOpStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.permaDeopToolStripMenuItem});
            this.permaOpStrip.Name = "playerListStrip";
            this.permaOpStrip.Size = new System.Drawing.Size(108, 26);
            this.permaOpStrip.Opening += new System.ComponentModel.CancelEventHandler(this.permaOpStrip_Opening);
            // 
            // permaDeopToolStripMenuItem
            // 
            this.permaDeopToolStripMenuItem.Name = "permaDeopToolStripMenuItem";
            this.permaDeopToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.permaDeopToolStripMenuItem.Text = "De-op";
            this.permaDeopToolStripMenuItem.Click += new System.EventHandler(this.permaDeopToolStripMenuItem_Click);
            // 
            // graphsGroupBox
            // 
            this.graphsGroupBox.Controls.Add(this.lengthBox);
            this.graphsGroupBox.Controls.Add(this.resourceChart);
            this.graphsGroupBox.Controls.Add(this.playerChart);
            this.graphsGroupBox.Location = new System.Drawing.Point(12, 256);
            this.graphsGroupBox.Name = "graphsGroupBox";
            this.graphsGroupBox.Size = new System.Drawing.Size(218, 259);
            this.graphsGroupBox.TabIndex = 3;
            this.graphsGroupBox.TabStop = false;
            this.graphsGroupBox.Text = "Graphs";
            // 
            // lengthBox
            // 
            this.lengthBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lengthBox.FormattingEnabled = true;
            this.lengthBox.Items.AddRange(new object[] {
            "Past Hour",
            "Past 24 Hours",
            "Since Started"});
            this.lengthBox.Location = new System.Drawing.Point(6, 231);
            this.lengthBox.Name = "lengthBox";
            this.lengthBox.Size = new System.Drawing.Size(206, 21);
            this.lengthBox.TabIndex = 3;
            this.lengthBox.SelectedIndexChanged += new System.EventHandler(this.lengthBox_SelectedIndexChanged);
            // 
            // resourceChart
            // 
            this.resourceChart.BorderlineColor = System.Drawing.SystemColors.ControlDark;
            this.resourceChart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.AxisX.LabelStyle.Enabled = false;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisY.Maximum = 100D;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.AxisY.Title = "CPU (%)";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY2.IsLabelAutoFit = false;
            chartArea1.AxisY2.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY2.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY2.Title = "RAM (MB)";
            chartArea1.AxisY2.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.Name = "chartArea";
            this.resourceChart.ChartAreas.Add(chartArea1);
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend1.IsTextAutoFit = false;
            legend1.Name = "graphLegend";
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend2.Enabled = false;
            legend2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend2.IsTextAutoFit = false;
            legend2.Name = "graphLegend24";
            legend3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend3.Enabled = false;
            legend3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend3.IsTextAutoFit = false;
            legend3.Name = "graphLegend0";
            this.resourceChart.Legends.Add(legend1);
            this.resourceChart.Legends.Add(legend2);
            this.resourceChart.Legends.Add(legend3);
            this.resourceChart.Location = new System.Drawing.Point(6, 19);
            this.resourceChart.Name = "resourceChart";
            this.resourceChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series1.ChartArea = "chartArea";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.DarkBlue;
            series1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            series1.Legend = "graphLegend";
            series1.LegendText = "CPU";
            series1.Name = "cpuSeries";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series1.YValuesPerPoint = 4;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series2.ChartArea = "chartArea";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.ForestGreen;
            series2.Legend = "graphLegend";
            series2.LegendText = "Working Set (RAM)";
            series2.Name = "memorySeries";
            series2.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            series3.ChartArea = "chartArea";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.DarkBlue;
            series3.Enabled = false;
            series3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            series3.Legend = "graphLegend24";
            series3.LegendText = "CPU";
            series3.Name = "cpuSeries24";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series3.YValuesPerPoint = 4;
            series3.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series4.ChartArea = "chartArea";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Color = System.Drawing.Color.ForestGreen;
            series4.Enabled = false;
            series4.Legend = "graphLegend24";
            series4.LegendText = "Working Set (RAM)";
            series4.Name = "memorySeries24";
            series4.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            series5.ChartArea = "chartArea";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Color = System.Drawing.Color.DarkBlue;
            series5.Enabled = false;
            series5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            series5.Legend = "graphLegend0";
            series5.LegendText = "CPU";
            series5.Name = "cpuSeries0";
            series5.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series5.YValuesPerPoint = 4;
            series5.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series6.ChartArea = "chartArea";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series6.Color = System.Drawing.Color.ForestGreen;
            series6.Enabled = false;
            series6.Legend = "graphLegend0";
            series6.LegendText = "Working Set (RAM)";
            series6.Name = "memorySeries0";
            series6.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.resourceChart.Series.Add(series1);
            this.resourceChart.Series.Add(series2);
            this.resourceChart.Series.Add(series3);
            this.resourceChart.Series.Add(series4);
            this.resourceChart.Series.Add(series5);
            this.resourceChart.Series.Add(series6);
            this.resourceChart.Size = new System.Drawing.Size(206, 100);
            this.resourceChart.TabIndex = 2;
            this.resourceChart.Text = "Resource Usage";
            title1.Name = "chartTitle";
            title1.Text = "Resource Usage";
            this.resourceChart.Titles.Add(title1);
            // 
            // playerChart
            // 
            this.playerChart.BorderlineColor = System.Drawing.SystemColors.ControlDark;
            this.playerChart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea2.AxisX.LabelStyle.Enabled = false;
            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea2.Name = "chartArea";
            this.playerChart.ChartAreas.Add(chartArea2);
            this.playerChart.Location = new System.Drawing.Point(6, 125);
            this.playerChart.Name = "playerChart";
            this.playerChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series7.ChartArea = "chartArea";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Color = System.Drawing.Color.Teal;
            series7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            series7.Name = "chartSeries";
            series7.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series7.YValuesPerPoint = 4;
            series7.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series8.ChartArea = "chartArea";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series8.Color = System.Drawing.Color.Teal;
            series8.Enabled = false;
            series8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            series8.Name = "chartSeries24";
            series8.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series8.YValuesPerPoint = 4;
            series8.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series9.ChartArea = "chartArea";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series9.Color = System.Drawing.Color.Teal;
            series9.Enabled = false;
            series9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            series9.Name = "chartSeries0";
            series9.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series9.YValuesPerPoint = 4;
            series9.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            this.playerChart.Series.Add(series7);
            this.playerChart.Series.Add(series8);
            this.playerChart.Series.Add(series9);
            this.playerChart.Size = new System.Drawing.Size(206, 100);
            this.playerChart.TabIndex = 1;
            this.playerChart.Text = "Player Count";
            title2.Name = "chartTitle";
            title2.Text = "Player Count";
            this.playerChart.Titles.Add(title2);
            // 
            // settingsButton
            // 
            this.settingsButton.Location = new System.Drawing.Point(12, 521);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(106, 23);
            this.settingsButton.TabIndex = 4;
            this.settingsButton.Text = "Settings";
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // outputGroupBox
            // 
            this.outputGroupBox.Controls.Add(this.sendButton);
            this.outputGroupBox.Controls.Add(this.outputBox);
            this.outputGroupBox.Controls.Add(this.inputBox);
            this.outputGroupBox.Location = new System.Drawing.Point(242, 12);
            this.outputGroupBox.Name = "outputGroupBox";
            this.outputGroupBox.Size = new System.Drawing.Size(530, 537);
            this.outputGroupBox.TabIndex = 5;
            this.outputGroupBox.TabStop = false;
            this.outputGroupBox.Text = "Output/Commands";
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(446, 509);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(78, 23);
            this.sendButton.TabIndex = 5;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // updateLinkLabel
            // 
            this.updateLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateLinkLabel.Location = new System.Drawing.Point(242, 551);
            this.updateLinkLabel.Name = "updateLinkLabel";
            this.updateLinkLabel.Size = new System.Drawing.Size(530, 23);
            this.updateLinkLabel.TabIndex = 6;
            this.updateLinkLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.updateLinkLabel.Visible = false;
            this.updateLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.updateLinkLabel_LinkClicked);
            // 
            // pluginButton
            // 
            this.pluginButton.Location = new System.Drawing.Point(124, 521);
            this.pluginButton.Name = "pluginButton";
            this.pluginButton.Size = new System.Drawing.Size(106, 23);
            this.pluginButton.TabIndex = 7;
            this.pluginButton.Text = "Plugins/Extensions";
            this.pluginButton.UseVisualStyleBackColor = true;
            this.pluginButton.Click += new System.EventHandler(this.pluginButton_Click);
            // 
            // SrvForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 560);
            this.Controls.Add(this.pluginButton);
            this.Controls.Add(this.outputGroupBox);
            this.Controls.Add(this.updateLinkLabel);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.graphsGroupBox);
            this.Controls.Add(this.playerListGroupBox);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "SrvForm";
            this.Text = "GDTMP Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.srvForm_FormClosing);
            this.Load += new System.EventHandler(this.srvForm_Load);
            this.ResizeEnd += new System.EventHandler(this.srvForm_ResizeEnd);
            this.playerListGroupBox.ResumeLayout(false);
            this.playerTabs.ResumeLayout(false);
            this.connectedTab.ResumeLayout(false);
            this.playerListStrip.ResumeLayout(false);
            this.bannedTab.ResumeLayout(false);
            this.bannedStrip.ResumeLayout(false);
            this.permaOpTab.ResumeLayout(false);
            this.permaOpStrip.ResumeLayout(false);
            this.graphsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resourceChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerChart)).EndInit();
            this.outputGroupBox.ResumeLayout(false);
            this.outputGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TextBox outputBox;
        private System.Windows.Forms.TextBox inputBox;
        private System.Windows.Forms.GroupBox playerListGroupBox;
        private System.Windows.Forms.ListBox playerListBox;
        private System.Windows.Forms.GroupBox graphsGroupBox;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.GroupBox outputGroupBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart playerChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart resourceChart;
        private System.Windows.Forms.ContextMenuStrip playerListStrip;
        private System.Windows.Forms.ToolStripMenuItem opToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kickToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem banToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator tsSeperator;
        private System.Windows.Forms.ToolStripMenuItem moreInfoToolStripMenuItem;
        private System.Windows.Forms.ComboBox lengthBox;
        private System.Windows.Forms.TabControl playerTabs;
        private System.Windows.Forms.TabPage connectedTab;
        private System.Windows.Forms.TabPage bannedTab;
        private System.Windows.Forms.ListBox bannedBox;
        private System.Windows.Forms.ContextMenuStrip bannedStrip;
        private System.Windows.Forms.ToolStripMenuItem unbanToolStripMenuItem;
        private System.Windows.Forms.TabPage permaOpTab;
        private System.Windows.Forms.ListBox permaOpBox;
        private System.Windows.Forms.ContextMenuStrip permaOpStrip;
        private System.Windows.Forms.ToolStripMenuItem permaDeopToolStripMenuItem;
        private System.Windows.Forms.LinkLabel updateLinkLabel;
        private System.Windows.Forms.Button pluginButton;
    }
}

