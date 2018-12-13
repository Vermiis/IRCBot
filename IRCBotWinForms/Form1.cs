using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRCBotWinForms
{
    public partial class Form1 : Form
    {
        IRCbot ircBot;
        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            ircBot = new IRCbot(
               server: textBoxServerAddr.Text,
               port: Int32.Parse(textBoxServerPort.Text),
               user: "USER IRCbot 0 * :IRCbot",
               nick: textBoxNickName.Text,
               channel: textBoxChannel.Text
               );

            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();


        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {               
            ircBot.Start();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            var x = ircBot.ShowMsgOut();
            foreach (var item in x)
            {
                richTextBoxServerMessages.Text += Environment.NewLine + item;
            }
        }
    }
}
