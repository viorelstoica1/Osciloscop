namespace InterfataOsciloscop
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.labelComPort = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxBaudrate = new System.Windows.Forms.ComboBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.labelConnectionStatus = new System.Windows.Forms.Label();
            this.chartVoltage = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chartVoltage)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxComPort
            // 
            this.comboBoxComPort.FormattingEnabled = true;
            this.comboBoxComPort.Location = new System.Drawing.Point(100, 12);
            this.comboBoxComPort.Name = "comboBoxComPort";
            this.comboBoxComPort.Size = new System.Drawing.Size(121, 28);
            this.comboBoxComPort.TabIndex = 0;
            // 
            // labelComPort
            // 
            this.labelComPort.AutoSize = true;
            this.labelComPort.Location = new System.Drawing.Point(12, 15);
            this.labelComPort.Name = "labelComPort";
            this.labelComPort.Size = new System.Drawing.Size(82, 20);
            this.labelComPort.TabIndex = 1;
            this.labelComPort.Text = "COM Port:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Baudrate:";
            // 
            // comboBoxBaudrate
            // 
            this.comboBoxBaudrate.FormattingEnabled = true;
            this.comboBoxBaudrate.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
            "921600"});
            this.comboBoxBaudrate.Location = new System.Drawing.Point(312, 12);
            this.comboBoxBaudrate.Name = "comboBoxBaudrate";
            this.comboBoxBaudrate.Size = new System.Drawing.Size(121, 28);
            this.comboBoxBaudrate.TabIndex = 3;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(439, 12);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(89, 28);
            this.buttonConnect.TabIndex = 5;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // labelConnectionStatus
            // 
            this.labelConnectionStatus.AutoSize = true;
            this.labelConnectionStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labelConnectionStatus.Location = new System.Drawing.Point(534, 16);
            this.labelConnectionStatus.Name = "labelConnectionStatus";
            this.labelConnectionStatus.Size = new System.Drawing.Size(107, 20);
            this.labelConnectionStatus.TabIndex = 6;
            this.labelConnectionStatus.Text = "Disconnected";
            // 
            // chartVoltage
            // 
            chartArea2.Name = "ChartArea1";
            this.chartVoltage.ChartAreas.Add(chartArea2);
            this.chartVoltage.IsSoftShadows = false;
            legend2.Name = "Legend1";
            this.chartVoltage.Legends.Add(legend2);
            this.chartVoltage.Location = new System.Drawing.Point(12, 46);
            this.chartVoltage.Name = "chartVoltage";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartVoltage.Series.Add(series2);
            this.chartVoltage.Size = new System.Drawing.Size(1421, 601);
            this.chartVoltage.TabIndex = 7;
            this.chartVoltage.TabStop = false;
            this.chartVoltage.Text = "chartVoltage";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1445, 656);
            this.Controls.Add(this.chartVoltage);
            this.Controls.Add(this.labelConnectionStatus);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.comboBoxBaudrate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelComPort);
            this.Controls.Add(this.comboBoxComPort);
            this.Name = "Form1";
            this.Text = "Osciloscop";
            ((System.ComponentModel.ISupportInitialize)(this.chartVoltage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxComPort;
        private System.Windows.Forms.Label labelComPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxBaudrate;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label labelConnectionStatus;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartVoltage;
        private System.Windows.Forms.Timer timer1;
    }
}

