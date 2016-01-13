using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Chat_APP
{
    public partial class Name : Form
    {
        public Name()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (nametxt.Text != "")
            {
                Class1.i = nametxt.Text.ToString();
                Form1 frm2 = new Form1();
                frm2.FormClosed += new FormClosedEventHandler(frm2_FormClosed);
                frm2.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("It is mandatory to enter a name for Chat");
            }
        }

        private void Name_Load(object sender, EventArgs e)
        {

        }
        private void frm2_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }
    }
}
