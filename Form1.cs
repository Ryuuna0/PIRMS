using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialChat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            cboPort.Text = Properties.Settings.Default.PortName;
            cboBaudRate.Text = Properties.Settings.Default.BaudRate;
            txtNick.Text = Properties.Settings.Default.Nick;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.PortName = cboPort.Text;
            Properties.Settings.Default.BaudRate = cboBaudRate.Text;
            Properties.Settings.Default.Nick = txtNick.Text;
            Properties.Settings.Default.Save();
        }

        private void btnOpenClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnOpenClose.Text == "Připojit")
                {
                    serialPort1.PortName = cboPort.Text;
                    serialPort1.BaudRate = int.Parse(cboBaudRate.Text);
                    serialPort1.Open();
                    btnOpenClose.Text = "Odpojit";
                }
                else
                {
                    serialPort1.Close();
                    btnOpenClose.Text = "Připojit";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string start = ((char)0x02).ToString();
                string end = ((char)0x03).ToString();

                serialPort1.Write(start);
                serialPort1.Write(txtNick.Text);
                serialPort1.Write("~");
                serialPort1.Write(txtMessage.Text);
                serialPort1.Write(end);


                ListViewItem li = new ListViewItem(
                    txtNick.Text + ": " + txtMessage.Text);
                li.ForeColor = Color.Pink;
                listView1.Items.Add(li);
                txtMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        string buffer = string.Empty;

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen == false)
                    return;
                if (serialPort1.BytesToRead == 0)
                    return;

                buffer += serialPort1.ReadExisting();
                string[] messages = buffer.Split((char)0x03);
                buffer = messages[messages.Length - 1];

                for (int i = 0; i < messages.Length - 1; i++)
                {
                    string message = messages[i].TrimStart((char)0x02);
                    string[] vals = message.Split('~');
                    string nick = vals[0];
                    string msg = vals[1];

                    ListViewItem li = new ListViewItem(nick + ": " + msg);
                    li.ForeColor = Color.Blue;
                    listView1.Items.Add(li);
                }


            }
            catch (Exception)
            {

                // TODO
            }
        }
    }
}
