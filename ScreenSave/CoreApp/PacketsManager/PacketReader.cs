using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{
    public class PacketReader : BinaryReader
    {
        private NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public List<string> ReadMessage()
        {
            if (_ns.CanRead)
            {
                byte[] msgBuffer;
                var length = ReadInt32();
                msgBuffer = new byte[length];
                _ns.Read(msgBuffer, 0, length);

                var msg = Encoding.ASCII.GetString(msgBuffer);
                return DecodeMessage(msg);
            }
            else
            {
                throw new Exception("Nu se poate executa citirea! ");
            }


            //if (_ns.CanRead)
            //{
            //    byte[] myReadBuffer = new byte[1024];
            //    StringBuilder myCompleteMessage = new StringBuilder();
            //    int numberOfBytesRead = 0;

            //    // Incoming message may be larger than the buffer size.
            //    do
            //    {
            //        numberOfBytesRead = _ns.Read(myReadBuffer, 0, myReadBuffer.Length);

            //        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
            //    }
            //    while (_ns.DataAvailable);

            //    // Print out the received message to the console.
            //    Console.WriteLine("You received the following message : " +
            //                                 myCompleteMessage);
                
            //}
            //else
            //{
            //    Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
            //}

        }
        public List<string> DecodeMessage(string message)
        {
            List<string> msg = new List<string>();
            string[] helper = message.Split('\0');
            foreach(var x in helper)
            {
                msg.Add(x);
            }
            return msg;
        }
    }
}
