using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

                        while (true)
                        {
                            string inputLine;
                            string msgToSend;
                            while ((inputLine = reader.ReadLine()) != null)
                            {
                                Console.WriteLine("<- " + inputLine);
                                comm.messagesIn.Enqueue("<- " + inputLine);
                                try
                                {
                                    comm.messagesToSend.TryDequeue(out msgToSend);
                                    writer.WriteLine("PRIVMSG " + _channel + msgToSend);
                                }
                                catch (Exception)
                                {

                                    throw;
                                }

                                // split the lines sent from the server by spaces (seems to be the easiest way to parse them)
                                string[] splitInput = inputLine.Split(new Char[] { ' ' });
                                string[] getnick = splitInput[0].Split(new char[] {':', '!', '@' });

                                if (splitInput[0] == "PING")
                                {
                                    string PongReply = splitInput[1];
                                    //Console.WriteLine("->PONG " + PongReply);
                                    writer.WriteLine("PONG " + PongReply);
                                    comm.messagesIn.Enqueue("PONG " +PongReply);
                                    writer.Flush();
                                    //continue;
                                }
                                if (splitInput[1] == "PRIVMSG" && splitInput[3].Contains(_nick))
                                {
                                    writer.WriteLine("PRIVMSG " + _channel + " PUHUPUHU");
                                    writer.Flush();
                                }
                                else if (splitInput[1] == "QUIT"  || splitInput[1] == "PART" )
                                {
                                    writer.WriteLine("PRIVMSG " + _channel + " :( " + getnick[1]);
                                    writer.Flush();
                                }
                                else if (splitInput[1] == "JOIN")
                                {
                                    writer.WriteLine("PRIVMSG " + _channel + " Hej " + getnick[1]);
                                    writer.Flush();
                                }

                                switch (splitInput[1])
                                {
                                    case "001":
                                        writer.WriteLine("JOIN " + _channel);
                                        writer.Flush();
                                        break;
                                    default:
                                        break;
                                    case "333":
                                       // writer.WriteLine("JOIN " + _channel);
                                       // writer.Flush();
                                        break;

                                    case "PRIVMSG":
                                       //  writer.WriteLine("PRIVMSG " + _channel +" PUHUPUHU");
                                       //  writer.Flush();
                                        break;

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

    }    
}
