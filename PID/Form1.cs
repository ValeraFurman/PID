using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
namespace PID
{
    public partial class Form1 : Form
    {
        float Sensor = 37.3f;
        float Step = 0.01f;
        double[] DW_Data = new double[50];
        float err; float integral = 0, prevErr = 0;
        float D;

        public Form1()
        {
            InitializeComponent();
        }
        public  float computePID(float input, float setpoint, float kp, float ki, float kd, float dt, int minOut, int maxOut)
        {
            err = setpoint - input;
            integral = Constrain(integral + (float)err * dt * ki, minOut, maxOut);
            D = (err - prevErr) / dt;
            prevErr = err;
            return Constrain(err * kp + integral + D * kd, minOut, maxOut);
        }
        public float Constrain(float amt, float low, float high)
        {
           return ((amt) < (low) ? (low) : ((amt) > (high) ? (high) : (amt)));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var model = new PlotModel();
            var s1 = new TwoColorAreaSeries();
            s1.Color = OxyColors.Green;
            s1.Limit = 0;

            Array.Copy(DW_Data, 1, DW_Data, 0, DW_Data.Length - 1);
            DW_Data[DW_Data.Length - 1] = Convert.ToDouble(PMW_OUT.Text);

            for (int i = 0; i < DW_Data.Length; i++)
            {
                s1.Points.Add(new DataPoint(i + 1, DW_Data[i]));

            }
            model.Series.Add(s1);
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });

            plotView1.Model = model;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sensor = 37.5f;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Sensor = Sensor + 0.01f;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Sensor = Sensor - 0.01f;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            timer2.Interval = Int32.Parse(Dt_TIME.Text);
            timer1.Interval = Int32.Parse(loopTIME.Text);
            timer2.Enabled = true;  timer1.Enabled = true; 

        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer2.Enabled = false; timer1.Enabled = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            PMW_OUT.Text = Convert.ToString(computePID(Sensor, float.Parse(tSetup.Text),
float.Parse(KpSetup.Text), float.Parse(KiSetup.Text), float.Parse(KdSetup.Text), Int32.Parse(Dt_TIME.Text), 0, 255));
            tSensor.Text = Convert.ToString(Sensor);
        }
    }
}
