using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace InterfataOsciloscop
{
    public partial class Form1 : Form
    {
        private static string[] porturiUart;

        private void ActualizareInterfata()
        {
            chartVoltage.Series.Clear();
            Series sDummy = chartVoltage.Series.Add("dummy");
            sDummy.Color = Color.Transparent;
            sDummy.IsVisibleInLegend = false;
            sDummy.ChartType = SeriesChartType.Line;
            sDummy.Points.AddXY(1, 1);
            sDummy.Points.AddXY(1, 3.5);
            Series series = new Series();
            series.ChartType = SeriesChartType.Line;
            series.Points.Clear();
            series.IsVisibleInLegend = false;
            for (int i = 0; i < OsciloscopeData.MarimeBufferTensiuni; i++)
            {
                series.Points.AddXY(i+1, ((float)ProgramData.Instance.Data.Tensiuni[i])*3.33/4095);
            }
            chartVoltage.Series.Add(series);
            if (UartPortData.Port.IsOpen)
            {
                labelConnectionStatus.Text = "Connected";
                labelConnectionStatus.BackColor = Color.Green;
            }
            else
            {
                labelConnectionStatus.Text = "Disconnected";
                labelConnectionStatus.BackColor = Color.Red;
            }
        }
        public Form1()
        {
            InitializeComponent();
            /*populare lista porturi UART*/
            porturiUart = SerialPort.GetPortNames();
            comboBoxComPort.Items.Clear();
            if (porturiUart.Length != 0)
            {
                comboBoxComPort.Items.AddRange(porturiUart);
            }
            if (comboBoxComPort.Items != null)
            {
                comboBoxComPort.SelectedIndex = 0;
            }
            /*selectie lista baudrate-uri*/
            comboBoxBaudrate.SelectedIndex = 0;
            /*initializare chart*/
            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.Title = "Timp";
            chartArea.AxisY.Title = "Tensiune";
            chartVoltage.ChartAreas.Add(chartArea);
            chartVoltage.Series.Clear();
            Series series = new Series();
            series.ChartType = SeriesChartType.Line;
            series.Points.Clear();
            chartVoltage.Series.Add(series);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            UartPortData.comPort = comboBoxComPort.Text;
            UartPortData.baudrate = Convert.ToInt32(comboBoxBaudrate.Text);
            UartPortData.shouldBeOpen = !UartPortData.shouldBeOpen;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ActualizareInterfata();
        }
    }
}
