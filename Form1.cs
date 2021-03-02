using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPS_ShowCase
{
    public partial class Form1 : Form
    {
        int i=0;
        private readonly object x = new object();
        public Form1()
        {
            InitializeComponent();
            listView1.Columns.Add("MyColumn", -2, HorizontalAlignment.Left);
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.View = System.Windows.Forms.View.List;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            SerialPort mySerialPort = new SerialPort();

                mySerialPort.BaudRate = 4800;
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = true;

                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);


                foreach (string s in SerialPort.GetPortNames())
                {
                    Console.WriteLine(s);
                    mySerialPort.PortName = s;
                }

                mySerialPort.Open();
                   
        }
        private void DataReceivedHandler(
                                object sender,
                                SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine();
            string[] SplitData = indata.Split(',');
            if (SplitData[0] == "$GPRMC")
            {
                Console.WriteLine(indata);
                Console.WriteLine("Latitude: " , SplitData[3]);
                Console.WriteLine("Longitude: " , SplitData[5]);
                string lat = SplitData[3];
                string longi = SplitData[5];
                Thread t = new Thread(()=>AddToList(lat, longi));
                t.Start();
                t.Join();

            }
        }
        private void AddToList(string lat, string longi)
        {
            lock (x)
            {
                string mylab;
                mylab = "Latitude: " + lat + "\nLongitude: " + longi;
                ListViewItem MyItem = new ListViewItem();
                MyItem.Text = mylab;
                listView1.Invoke(new MethodInvoker(delegate { 
                listView1.Items.Add(MyItem);
                listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent); ;
                }));
                
                
                i++;
            }
            
        }
    }
}
