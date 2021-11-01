using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//to enable multithreading
using System.Threading;

//library for real time charts
// Install-Package kayChart.dll -Version 0.4.7 
//https://www.nuget.org/packages/kayChart.dll/
using rtChart;

namespace Fusor_controler_GUI
{
    public partial class CalibrationForm : Form
    {
         
        //Start GUI and start looping threads
        public CalibrationForm(byte[] Mains_Sens)
        {
            InitializeComponent();
            //Set full screen
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;


            Thread testThread = new Thread(TEST);
            testThread.Start();
            testThread.IsBackground = true;

            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 mainForm = new Form1();
            mainForm.Show();
            this.Hide();

            
        }

        private void TEST()
        {

            




        }
    }
}
