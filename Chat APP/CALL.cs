using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Voice;
using System.Net;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace Chat_APP
{
    public partial class CALL : Form
    {
        #region variables
        private Socket r;
        private Thread t;
        private bool connected = false;
        private System.ComponentModel.Container components = null;
        #endregion

        #region CALL
        public CALL()
        {
            InitializeComponent();
            r = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            t = new Thread(new ThreadStart(Voice_In));
        }
        #endregion

        private void CALL_Load(object sender, EventArgs e)
        {
            textBox1.Text = Class1.ip;
        }
        [STAThread]
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "")
            {
                if(connected == false)
                {
                    t.Start();
                    connected = true;
                Start();
                }
            }
            else
            {
                MessageBox.Show("Some Fields Are Missing OR Exception Occured");
            }
        }
        #region Voice_In()
        private void Voice_In()
        {
            try
            {
                byte[] br;
                r.Bind(new IPEndPoint(IPAddress.Any, int.Parse(this.textBox3.Text)));
                while (true)
                {
                    br = new byte[16384];
                    r.Receive(br);
                    m_Fifo.Write(br, 0, br.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No Response From Your Friend.!!!");
            }
        }
        #endregion
        #region Voice_Out()

        private void Voice_Out(IntPtr data, int size)
        {
           
            if (m_RecBuffer == null || m_RecBuffer.Length < size)
                m_RecBuffer = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy(data, m_RecBuffer, 0, size);

            r.SendTo(m_RecBuffer, new IPEndPoint(IPAddress.Parse(this.textBox1.Text), int.Parse(this.textBox2.Text)));
            label4.Visible = true;
        }

        #endregion

        private WaveOutPlayer m_Player;
        private WaveInRecorder m_Recorder;
        private FifoStream m_Fifo = new FifoStream();

        private byte[] m_PlayBuffer;
        private byte[] m_RecBuffer;


        private void Start()
        {
            Stop();
            try
            {
                WaveFormat fmt = new WaveFormat(44100, 16, 2);
                m_Player = new WaveOutPlayer(-1, fmt, 16384, 3, new BufferFillEventHandler(Filler));
                m_Recorder = new WaveInRecorder(-1, fmt, 16384, 3, new BufferDoneEventHandler(Voice_Out));
            }
            catch
            {
                Stop();
                throw;
            }
        }

        private void Stop()
        {
            if (m_Player != null)
                try
                {
                    m_Player.Dispose();
                }
                finally
                {
                    m_Player = null;
                }
            if (m_Recorder != null)
                try
                {
                    m_Recorder.Dispose();
                }
                finally
                {
                    m_Recorder = null;
                }
            m_Fifo.Flush(); // clear all pending data
        }

        private void Filler(IntPtr data, int size)
        {
            if (m_PlayBuffer == null || m_PlayBuffer.Length < size)
                m_PlayBuffer = new byte[size];
            if (m_Fifo.Length >= size)
                m_Fifo.Read(m_PlayBuffer, 0, size);
            else
                for (int i = 0; i < m_PlayBuffer.Length; i++)
                    m_PlayBuffer[i] = 0;
            System.Runtime.InteropServices.Marshal.Copy(m_PlayBuffer, 0, data, size);
            
        }

        private void Form1_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            t.Abort();
            r.Close();
            Stop();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Stop();
            this.Close();
        }
        private string GetLocalIP()
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
    }
}