using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ChatApplication
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        public Form1()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            txtIP1.Text = GetLocalIP();
            txtIP2.Text = GetLocalIP();
            lblUser1.Font = new Font(lblUser1.Font, FontStyle.Bold);
            lblUser2.Font = new Font(lblUser2.Font, FontStyle.Bold);

        }

        public String GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "10.86.31.168";
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] receivedData = new byte[1464];
                    receivedData = (byte[])aResult.AsyncState;
                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receivedMessage = eEncoding.GetString(receivedData);


                    lstClient1.Items.Add("Friend : " + receivedMessage);


                }

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer,
                    0,
                    buffer.Length,
                    SocketFlags.None,
                    ref epRemote,
                    new AsyncCallback(MessageCallBack),
                    buffer);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }


        public String GetPortNumber()
        {
            String Port="";
            return Port;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(txtIP1.Text), Convert.ToInt32(txtPort1.Text));
                sck.Bind(epLocal);

                epRemote = new IPEndPoint(IPAddress.Parse(txtIP2.Text), Convert.ToInt32(txtPort2.Text));
                sck.Connect(epRemote);

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer,
                    0,
                    buffer.Length,
                    SocketFlags.None,
                    ref epRemote,
                    new AsyncCallback(MessageCallBack),
                    buffer);

                btnStart.Text = "Connected!";
                btnStart.BackColor = Color.Coral;
                btnStart.Font = new Font(btnStart.Font, FontStyle.Bold);
                btnStart.Enabled = false;
                btnSend.Enabled = true;
                txtMessage1.Focus();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Form1 frm = new Form1();
            //frm.Show();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

                byte[] msg = new byte[1500];
                msg = enc.GetBytes(txtMessage1.Text);

                sck.Send(msg);
                lstClient1.Items.Add("Me : " + txtMessage1.Text); //Person 1
                txtMessage1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
