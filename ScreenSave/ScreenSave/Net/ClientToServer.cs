using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CoreApp;

namespace ClientApp
{
    public class ClientToServer
    {
        private string ip = "127.0.0.1";
        int port = 5000;
        public List<string> returnMessage { get; set; }
        private TcpClient _client;
        public PacketReader PacketReader;
        private static ClientToServer instance = null;
        public static ClientToServer getInstance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ClientToServer();
                }
                return instance;
            }
        }
        private ClientToServer()
        {
            if (_client == null)
            {
                _client = new TcpClient();
            }
            
            ConnectToServer();
            
        }
        public bool isConnected()
        {
            //if (_client != null)              //later x1 probl
            //{
            //    if (_client.Connected)
            //        return true;
            //}

            if (_client.Connected)
                return true;
            return false;
        }

        public void clearAll()
        {
            if (this.returnMessage != null)
            {
                this.returnMessage.Clear();
            }
            return;
        }

        #region connect server and read cases
        public void ConnectToServer()
        {
            try
            {
                if (!_client.Connected)
                {
                    string Hostname = null;
                    if (ip == "127.0.0.1")
                    {
                        string MyIP = "";
                        IPHostEntry Host = default(IPHostEntry);
                        Hostname = System.Environment.MachineName;
                        Host = Dns.GetHostEntry(Hostname);
                        foreach (IPAddress IP in Host.AddressList)
                        {
                            if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                MyIP = Convert.ToString(IP);
                            }
                        }
                        ip = MyIP;
                    }
                    try
                    {
                        _client.Connect(ip, port);
                        PacketReader = new PacketReader(_client.GetStream());
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("Unable to connect to Server! Server down \n" + e.Message);
                        instance = null;
                        _client = null;
                        return;
                    }
                    if (!string.IsNullOrEmpty(Hostname))
                    {
                        List<string> message = new List<string>();
                        var connectPacket = new PacketBuilder();
                        connectPacket.WritePacketHeaderCode(PacketHeaders.ConectToServerRequest);
                        message.Add(Hostname);
                        connectPacket.WriteMessage(message);
                        _client.Client.Send(connectPacket.GetPacketBytes());
                    }

                    ReadPackets();

                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Unable to connect to Server! Server down \n" + e.Message);
                instance = null;
                _client = null;
                return;
            }
        }
 
        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var opcode = PacketReader.ReadByte();
                        switch (opcode)
                        {
                            case 1:
                                //
                                List<string> messaje = PacketReader.ReadMessage();
                                returnMessage = messaje;
                                break;
                            case (byte)PacketHeaders.ConnectToServerResponse:
                                ConnectToServerResponse();
                                break;
                            case (byte)PacketHeaders.LoginResponse:
                                LoginApplicationResponse();
                                break;
                            case (byte)PacketHeaders.RegisterResponse:
                                RegisterResponse();
                                break;
                            case (byte)PacketHeaders.LoadGroupsResponse:
                                LoadGroupsResponse();
                                break;
                            case (byte)PacketHeaders.SendPhotoToGroupResponse:
                                SavePhotoToGroupResponse();
                                break;
                            case (byte)PacketHeaders.CreateGroupResponse:
                                CreateGroupResponse();
                                break;
                            case (byte)PacketHeaders.LoadOwnerGroupsResponse:
                                LoadOwnerGroupsResponse();
                                break;
                            case (byte)PacketHeaders.GenerateCodeRespone:
                                GenerateCodeResponse();
                                break;
                            case (byte)PacketHeaders.JoinGroupResponse:
                                JoinGroupResponse();
                                break;
                            case (byte)PacketHeaders.LoadDeleteGroupResponse:
                                JoinGroupResponse();
                                break;
                            case (byte)PacketHeaders.LeaveGroupResponse:
                                LeaveGroupResponse();
                                break;
                            case (byte)PacketHeaders.LoadImagesResponse:
                                LoadImgResponse();
                                break;

                            default:
                                Console.WriteLine("ah, yes .. ");
                                break;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Server down!");
                        instance = null;
                        _client = null;
                        break;
                    }
                }
            });
        }

        public void ConnectToServerResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            while(msg == null)
            {
                //just wait
            }
            while(msg.Count == 0)
            {
                //just wait
            }
            string response = msg[1];
            if(response == "Connection failed!")
            {
                MessageBox.Show("Unable to connect to server!");
            }
        }

        #endregion

        #region memories
        public void FirstFunct()
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.Dofirst);
            List<string> message = new List<string>();
            message.Add("Primul");
            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }

        public void SecondFunc()
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.DoSecond);
            List<string> message = new List<string>();
            message.Add("Second");
            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        #endregion

        #region login
        public void LoginRequestFunct(string username, string password)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.LoginRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(username);
            message.Add(AES_Client.Encrypt256(password));

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void LoginApplicationResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

        #region register
        public void RegisterRequestFunct(string username, string email, string password)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.RegisterRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(username);
            message.Add(email);
            message.Add(AES_Client.Encrypt256(password));

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void RegisterResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

        #region load groups
        public void LoadGroupsRequestFunct(string userID)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.LoadGroupsRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(userID);

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void LoadGroupsResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

        #region save photo to group
        public void SavePhotoToGroupRequestFunct(string filename, string imageString, string groupName)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.SendPhotoToGroupRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(filename);
            message.Add(imageString);
            message.Add(groupName);

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void SavePhotoToGroupResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

        #region create group
        public void CreateGroupRequestFunct(string userID, string groupName)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.CreateGroupRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(userID);
            message.Add(groupName);

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void CreateGroupResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

        #region load owner groups
        public void LoadOwnerGroupsRequestFunct(string userID)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.LoadOwnerGroupsRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(userID);

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void LoadOwnerGroupsResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

        #region generate code
        public void GenerateCodeRequestFunct(string userID, string groupName, string codeGenerated)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.GenerateCodeRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(userID);
            message.Add(groupName);
            message.Add(codeGenerated);

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void GenerateCodeResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

        #region join group
        public void JoinGroupRequestFunct(string userID, string groupCode)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.JoinGroupRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(userID);
            message.Add(groupCode);

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void JoinGroupResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

        #region load delete groups
        public void LoadDeleteGroupsRequestFunct(string userID)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.LoadDeleteGroupRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(userID);

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void LoadDeleteGroupsResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

        #region leave group
        public void LeaveGroupRequestFunct(string userID,string groupName, string action)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.LeaveGroupRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(userID);
            message.Add(groupName);
            message.Add(action);

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void LeaveGroupResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

        #region load images
        public void LoadImgRequestFunct(string groupName)
        {
            var connectPacket = new PacketBuilder();
            connectPacket.WritePacketHeaderCode(PacketHeaders.LoadImagesRequest);
            List<string> message = new List<string>();

            //messages to transmit
            message.Add(groupName);

            connectPacket.WriteMessage(message);
            _client.Client.Send(connectPacket.GetPacketBytes());
        }
        public void LoadImgResponse()
        {
            List<string> msg = PacketReader.ReadMessage();
            returnMessage = msg;
        }
        #endregion

    }
}
