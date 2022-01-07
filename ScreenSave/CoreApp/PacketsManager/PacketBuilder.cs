using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{
    public class PacketBuilder
    {
        MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WritePacketHeaderCode(PacketHeaders header)
        {
            byte opcode = Convert.ToByte(header);
            _ms.WriteByte(opcode);
        }

        public void WriteMessage(List<string> packetMessage)
        {
            var msg = GetPacketMessage(packetMessage);
            var msgLenght = msg.Length;
            _ms.Write(BitConverter.GetBytes(msgLenght));
            _ms.Write(Encoding.ASCII.GetBytes(msg));

        }

        public string GetPacketMessage(List<string> packetMessage)
        {
            string message = "";
            char delimitator = '\0';   // schimb in cazul in care nu merge  // trebuie sa am grija
                                       // ce caracter aleg => sql image
            message = packetMessage.Count.ToString() + '\0';
           string message2 = string.Join(delimitator, packetMessage.ToArray());
            return message + message2;
        }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
}

