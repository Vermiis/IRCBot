using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCBotWinForms
{
    public partial class Messages
    {
        public ConcurrentQueue<string> messagesOut = new ConcurrentQueue<string>();
        public ConcurrentQueue<string> messagesIn = new ConcurrentQueue<string>();
        public ConcurrentQueue<string> messagesServer = new ConcurrentQueue<string>();
        public ConcurrentQueue<string> messagesPriv = new ConcurrentQueue<string>();
        public ConcurrentQueue<string> messagesChannel = new ConcurrentQueue<string>();
        public ConcurrentQueue<string> messagesToSend = new ConcurrentQueue<string>();

        public string SendTime(string channel)
            {
            return DateTime.Now.ToString();
            }
    }
}
