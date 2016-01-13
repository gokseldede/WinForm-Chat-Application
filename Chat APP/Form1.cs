using System;
using System.Security.Cryptography;//Encryption Decryption
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
namespace Chat_APP
{
    public partial class Form1 : Form
    {
        Socket mySocket;
        EndPoint epLocal, epRemote;
        byte[] buffer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;

            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            mySocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);


            txtLocalIP.Text = GetLocalIP();
            txtRemoteIP.Text = GetLocalIP();
            groupBox1.Text = Class1.i;

        }
        private string GetLocalIP()//local Ip 127.0.0.1 
        {
            IPHostEntry myHost;
            myHost = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in myHost.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtLocalPort.Text != "" && txtRemotePort.Text != "")
                {

                    epLocal = new IPEndPoint(IPAddress.Parse(txtLocalIP.Text), Convert.ToInt32(txtLocalPort.Text));
                    mySocket.Bind(epLocal);


                    epRemote = new IPEndPoint(IPAddress.Parse(txtRemoteIP.Text), Convert.ToInt32(txtRemotePort.Text));
                    mySocket.Connect(epRemote);


                    buffer = new byte[1500];
                    mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);


                    BtnConnect.Text = "Connected";
                    BtnConnect.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Please After filling in the required fields");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MessageCallBack(IAsyncResult aResult)
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;
            try
            {
                byte[] RecivedData = new byte[Class1.j];
                RecivedData = (byte[])aResult.AsyncState;


                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string RecivedMessage = aEncoding.GetString(RecivedData);
                string decmessage = dencryptus(RecivedMessage, key.Text.ToString());

                ListMessages.Items.Add(decmessage);
                string path = System.IO.Directory.GetCurrentDirectory() + @"\inchat.mp3";
                WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
                wplayer.URL = path;
                wplayer.controls.play();
                buffer = new byte[1500];

                mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;
            try
            {
                if (key.Text != "")
                {

                    string x = encryptus(groupBox1.Text.ToString() + ":  " + txtMessage.Text, key.Text.ToString());
                    int l = System.Text.ASCIIEncoding.Unicode.GetByteCount(x);
                    Class1.j = l;
                    byte[] SendingMessage = new byte[l];
                    ASCIIEncoding aEncoding = new ASCIIEncoding();

                    SendingMessage = aEncoding.GetBytes(x);


                    mySocket.Send(SendingMessage);


                    ListMessages.Items.Add(groupBox1.Text.ToString() + ": " + txtMessage.Text);
                    txtMessage.Text = null;
                }
                else {
                    MessageBox.Show("Password Required....");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        public string encryptus(string x, string keyai)
        {
            try
            {

                string y = x;
                byte[] etext = UTF8Encoding.UTF8.GetBytes(y);
                string key = keyai;
                MD5CryptoServiceProvider mdhash = new MD5CryptoServiceProvider();
                byte[] keyarray = mdhash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                TripleDESCryptoServiceProvider tds = new TripleDESCryptoServiceProvider();
                tds.Key = keyarray;
                tds.Mode = CipherMode.ECB;
                tds.Padding = PaddingMode.PKCS7;

                ICryptoTransform itransform = tds.CreateEncryptor();
                byte[] result = itransform.TransformFinalBlock(etext, 0, etext.Length);
                string encryptresult = Convert.ToBase64String(result);
                return encryptresult.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message + "Possible Reason,Password problem";
            }
        }
        public string dencryptus(string x, string keyai)
        {
            try
            {
                string y = x.Replace("\0", null);
                byte[] etext = Convert.FromBase64String(y);
                string key = keyai;
                MD5CryptoServiceProvider mdhash = new MD5CryptoServiceProvider();
                byte[] keyarray = mdhash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                TripleDESCryptoServiceProvider tds = new TripleDESCryptoServiceProvider();
                tds.Key = keyarray;
                tds.Mode = CipherMode.ECB;
                tds.Padding = PaddingMode.PKCS7;

                ICryptoTransform itransform = tds.CreateDecryptor();
                byte[] result = itransform.TransformFinalBlock(etext, 0, etext.Length);
                string dencryptresult = UTF8Encoding.UTF8.GetString(result);
                return dencryptresult.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message + "Possible Reason,Password problem";
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }


        private void sAMEFRIENDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Class1.ip = txtRemoteIP.Text.ToString();
            Form newForm = new CALL();
            newForm.ShowDialog(this);
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void differentFriendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Class1.ip = "";
            Form newForm = new CALL();
            newForm.ShowDialog(this);
        }

    }
}
