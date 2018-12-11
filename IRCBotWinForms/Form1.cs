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
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            var ircBot = new IRCbot(
                server: "irc.freenode.net",
                port: 6667,
                user: "USER IRCbot 0 * :IRCbot",
                nick: "IRCbot",
                channel: "#opers"
                );

            ircBot.Start();
            ircBot.comm.messagesIn.Count();

        }
    }
}
