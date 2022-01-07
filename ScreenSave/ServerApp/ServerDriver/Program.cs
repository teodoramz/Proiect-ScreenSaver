using CoreApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;

namespace ServerApp
{
    class Program
    {
        static int port = 5000;
        static List<ServerInt> _users;
        static TcpListener _listener;
        static ServerInt currUser;

        static void Main(string[] args)
        {
            
            Console.WriteLine($"[{DateTime.Now}]: Server has started! ");
            _users = new List<ServerInt>();

            var taskWatcher = new ServerInt();

            string MyIP = "";
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    MyIP = Convert.ToString(IP);
                }
            }
            IPAddress localAddr = IPAddress.Parse(MyIP);
            _listener = new TcpListener(localAddr, port);

            
            _listener.Start();

            while (true)
            {
                try
                {
                    var client = new ServerInt(_listener.AcceptTcpClient());
                    _users.Add(client);
                    currUser = client;
                    ConnectionToServerResponse("Connection successful!");

                }
                catch (Exception e)
                {
                     Console.WriteLine("Unable to establish a connection with a client!\n" + e.Message);
                     ConnectionToServerResponse("Connection failed!");
                 }

            /*Broadcast the connection to everyone on the server */

             }
        }

       
        public static void ConnectionToServerResponse(string response)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.ConnectToServerResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }

        public static void LoginApplicationResponse(string response, string userID="")
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.LoginResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            if(userID!="")
            {
                msg.Add(userID);
            }
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
        public static void RegistrationResponse(string response)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.RegisterResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
        public static void LoadGroupsResponse(string response, List<string> responseList)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.LoadGroupsResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            if(responseList.Count == 0)
            {
                msg.Add("NoData");
            }
            else
            {
                foreach(var x in responseList)
                {
                    msg.Add(x);
                }
            }
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
        public static void SavePhotoResponse(string response)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.SendPhotoToGroupResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
        public static void LoadOwnerGroupsResponse(string response, List<string> responseList)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.LoadOwnerGroupsResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            if (responseList.Count == 0)
            {
                msg.Add("NoData");
            }
            else
            {
                foreach (var x in responseList)
                {
                    msg.Add(x);
                }
            }
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
        public static void LoadDeleteGroupsResponse(string response, List<string> responseList)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.LoadDeleteGroupResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            if (responseList.Count == 0)
            {
                msg.Add("NoData");
            }
            else
            {
                foreach (var x in responseList)
                {
                    msg.Add(x);
                }
            }
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
        public static void GenerateCodeResponse(string response)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.GenerateCodeRespone);
            List<string> msg = new List<string>();
            msg.Add(response);
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
        public static void CreateGroupResponse(string response)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.CreateGroupResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
        public static void JoinGroupResponse(string response)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.JoinGroupResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
        public static void LeaveGroupResponse(string response)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.LeaveGroupResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
        public static void LoadImgResponse(string response, List<string> responseList)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.LoadImagesResponse);
            List<string> msg = new List<string>();
            msg.Add(response);
            if (responseList.Count == 0)
            {
                msg.Add("NoData");
            }
            else
            {
                foreach (var x in responseList)
                {
                    msg.Add(x);
                }
            }
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }

        public static void SendBackToUserMessage()
        {

            var msgPacket = new PacketBuilder();
            msgPacket.WritePacketHeaderCode(PacketHeaders.ConectToServerRequest);
            List<string> msg = new List<string>();
            string a = "Teodor Amzuloiu";
            msg.Add(a);
            msgPacket.WriteMessage(msg);
            currUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }

        //public static void BroadcastMessage(string message)
        //{
        //    foreach (var user in _users)
        //    {
        //        var msgPacket = new PacketBuilder();
        //        msgPacket.WriteOpCode(5);
        //        msgPacket.WriteMessage(message);
        //        user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        //    }
        //}
    }
}
