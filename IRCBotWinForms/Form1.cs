using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace IRCBotWinForms
{
    public partial class Form1 : Form
    {
        //private static System.Timers.Timer aTimer;
        IRCbot ircBot;


        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker3.DoWork += backgroundWorker3_DoWork;

        }
        public void CreateTimer()
        {
            var timer = new System.Timers.Timer(500); 
            timer.Elapsed += HandleTimerElapsed;
        }

        public void HandleTimerElapsedNames(object sender, ElapsedEventArgs e)
        {
            if (ircBot.names.Count > 0)
            {
                richTextBoxNames.Invoke((MethodInvoker)delegate
                {
                    richTextBoxNames.Text = "";
                });
                // richTextBoxNames.Text = "";
                foreach (var item in ircBot.names)
                    richTextBoxNames.Invoke((MethodInvoker)delegate
                    {
                        richTextBoxNames.Text += Environment.NewLine + item;
                    });
            }
        }

        public void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var messages = ircBot.ShowMsgOut();
            foreach (var item in messages)
            {
                richTextBoxServerMessages.Invoke((MethodInvoker)delegate
                {
                    richTextBoxServerMessages.Text += Environment.NewLine + item;
                });
            }
            var messages2 = ircBot.ShowMsgIn();
            foreach (var item in messages2)
            {
                richTextBoxServerMessages.Invoke((MethodInvoker)delegate
                {
                    richTextBoxMsgOut.Text += Environment.NewLine + item;
                });
            }
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
            buttonConnect.Enabled = false;
            backgroundWorker2.RunWorkerAsync();
            backgroundWorker3.RunWorkerAsync();
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ircBot.Start();
            
        }

        

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            var timer = new System.Timers.Timer(500); 
            timer.Elapsed += HandleTimerElapsed;
            timer.Enabled = true;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            ircBot.comm.messagesToSend.Enqueue(richTextBoxMessageToSend.Text);
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            var timer = new System.Timers.Timer(500); 
            timer.Elapsed += HandleTimerElapsedNames;
            timer.Enabled = true;
        }
    }
}
