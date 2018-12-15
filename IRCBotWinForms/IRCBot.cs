using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace IRCBotWinForms
{
    public class IRCbot
    {
        // server to connect to (edit at will)
        private readonly string _server;
        // server port (6667 by default)
        private readonly int _port;
        // user information defined in RFC 2812 (IRC: Client Protocol) is sent to the IRC server 
        private readonly string _user;

        // the bot's nickname
        private readonly string _nick;
        private readonly string _channel;
        

        private readonly int _maxRetries;
        public Messages comm = new Messages();
        public List<string> names = new List<string>();
       
        public IRCbot(string server, int port, string user, string nick, string channel, int maxRetries = 3)
        {
            _server = server;
            _port = port;
            _user = user;
            _nick = nick;
            _channel = channel;
            _maxRetries = maxRetries;
        }

        public void Start()
        {
          //  workerSend.DoWork += workerSend_DoWork;
            var retry = false;
            var users = new List<string>();
            var retryCount = 0;
            do
            {
                try
                {
                    using (var irc = new TcpClient(_server, _port))
                    using (var stream = irc.GetStream())
                    using (var reader = new StreamReader(stream))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine("NICK " + _nick);
                        comm.messagesOut.Enqueue("NICK " + _nick);
                        writer.Flush();
                        writer.WriteLine(_user);
                        comm.messagesOut.Enqueue(_user);
                        writer.Flush();
                        

                        while (irc.Connected)
                        {
                            string inputLine;
                            string outputLine;
                           


                            while ((inputLine = reader.ReadLine()) != null)
                            {
                                Console.WriteLine("<- " + inputLine);
                                comm.messagesIn.Enqueue("<- " + inputLine);
                               

                                // split the lines sent from the server by spaces (seems to be the easiest way to parse them)
                                string[] splitInput = inputLine.Split(new Char[] { ' ' });
                                string[] getnick = splitInput[0].Split(new char[] {':', '!', '@' });

                                if (splitInput[0] == "PING")
                                {
                                    string PongReply = splitInput[1];
                                    //Console.WriteLine("->PONG " + PongReply);
                                    writer.WriteAsync("PONG " + PongReply);
                                    comm.messagesIn.Enqueue("PONG " +PongReply);
                                    writer.Flush();
                                    //continue;
                                }

                                if (splitInput[1] == "001")
                                {
                                    writer.WriteLine("JOIN " + _channel);
                                    writer.Flush();
                                }
                                //else if (splitInput[1] == "353")
                                //{
                                //    names.Clear();                                    
                                //    var namesonChanel = splitInput;
                                //    foreach (var item in namesonChanel)
                                //    {
                                //        names.Add(item);
                                //    }
                                //}
                                else if (splitInput[1] == "PRIVMSG" && splitInput[3].Contains(_nick))
                                {
                                    writer.WriteLine("PRIVMSG " + _channel + " PUHUPUHU");
                                    writer.Flush();
                                }
                                else if (splitInput[1] == "QUIT"  || splitInput[1] == "PART" )
                                {
                                    writer.WriteLine("PRIVMSG " + _channel + " :( " + getnick[1]);                                   
                                    writer.Flush();
                                    writer.WriteLine("NAMES " + _channel);
                                    writer.Flush();
                                }
                                else if (splitInput[1] == "JOIN")
                                {
                                    writer.WriteLine("PRIVMSG " + _channel + " Hej " + getnick[1]);                             
                                    writer.Flush();
                                    writer.WriteLine("NAMES " + _channel);
                                    writer.Flush();
                                }

                                while (comm.messagesToSend.TryDequeue(out outputLine))
                                {
                                    Send(outputLine, writer);                                   
                                }



                            }


                            


                        }
                    }
                }
                catch (Exception e)
                {
                    // shows the exception, sleeps for a little while and then tries to establish a new connection to the IRC server
                    Console.WriteLine(e.ToString());
                    Thread.Sleep(5000);
                    retry = ++retryCount <= _maxRetries;
                }
            } while (retry);
        }

        public List<string> ShowMsgOut()
        {
            string result;
            List<string> msgout = new List<string>();
            while (comm.messagesOut.TryDequeue(out result))
            {
                try
                {
                    msgout.Add(result);
                }
                catch (Exception)
                {
                    throw;
                }                               
            }
            return msgout;

        }

        public List<string> ShowMsgIn()
        {
            string result;
            List<string> msgin = new List<string>();
            while (comm.messagesIn.TryDequeue(out result))
            {
                try
                {
                    msgin.Add(result);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return msgin;

        }

        public void AddMsgToQueue( string msg)
        {
            comm.messagesOut.Enqueue(msg);
        }

        public void Send (string message, StreamWriter sw)
        {
            if (message.Contains("/join") || message.Contains("/part"))
            {
                sw.WriteLine(message);
            }
            else if ((message.Contains("/quit")))
            {
                sw.WriteLine(message);
            }
            else
            {
                sw.WriteLine("PRIVMSG " + _channel + " " + message);
            }
        }

        private void workerSend_DoWork(object sender, DoWorkEventArgs e)
        {
            var timer = new System.Timers.Timer(500);
            timer.Elapsed += EventSend;
            timer.Enabled = true;
        }

        public void EventSend(object sender, ElapsedEventArgs e)
        {
            string outputLine;
            while (comm.messagesToSend.TryDequeue(out outputLine))
            {
              //  Send(outputLine, sw)
            }
        }
    }    
}
