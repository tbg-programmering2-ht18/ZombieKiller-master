using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZombieKillerMenu
{
    public partial class Menu : Form
    {
        TheGame FrmTheGame = new TheGame();
        private TcpClient client; //Listens for connections from TCP network clients.
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string textToSend;

        public Menu()
        {
            InitializeComponent();
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress adress in localIP)
            {
                //Checks for IPv4
                if (adress.AddressFamily == AddressFamily.InterNetwork)
                {
                    edtServerIP.Text = adress.ToString();
                }
            }

        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (FrmTheGame.ShowDialog() == DialogResult.OK)
            {
                // Clear the textboxes, it looks good
                this.Show();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
                 
                if(edtServerIP.Text != "")
                {
                    try
                    {
                        //Check with ex Wireshark 
                        TcpListener listener = new TcpListener(IPAddress.Any, 5001);
                        listener.Start();
                        this.BackColor = Color.Green;
                        this.Update();
                        client = listener.AcceptTcpClient(); //Accept a pending connection request 
                        STR = new StreamReader(client.GetStream());
                        STW = new StreamWriter(client.GetStream());
                        STW.AutoFlush = true;
                        backgroundWorker1.RunWorkerAsync();
                        backgroundWorker2.WorkerSupportsCancellation = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }

            
                else if(edtClientIP.Text != "")
                {
                    try
                    {
                        client = new TcpClient();

                        //Represents a network endpoint as an IP address and a port number.
                        IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(edtClientIP.Text), 5001);

                        //Connects the client to a remote TCP host using the specified host name and port number.
                        client.Connect(ipEnd);
                        if (client.Connected)
                        {
                            this.BackColor = Color.Green;
                            this.Update();
                            STR = new StreamReader(client.GetStream());
                            STW = new StreamWriter(client.GetStream());
                            STW.AutoFlush = true;
                            backgroundWorker1.RunWorkerAsync();
                            backgroundWorker2.WorkerSupportsCancellation = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                } 
        }
    }
}

