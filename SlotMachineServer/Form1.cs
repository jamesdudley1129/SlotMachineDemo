using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SlotMachineServer
{
    public partial class Form1 : Form
    {
        ServerMain server;
        NetworkLogic cl;

        public Form1()
        {
            InitializeComponent();
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            server = new ServerMain();
            cl = ServerMain.NetLogic;
        }

        private void On_Update(object sender, EventArgs e)
        {
            richTextBox1.Text = Logger.GetLog();
            cl.Proccess_commands();
        }
    }
}
