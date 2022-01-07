using CoreApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerApp
{
    class ServerInt
    {
        public string Hostname { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        PacketReader _packetReader;
        private DateTime lastTimeChecked;

        static String connectionString = "Server=.;Database=ScreenSaver;Trusted_Connection=true";
        SqlConnection connection = new SqlConnection(connectionString);


        public ServerInt(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            List<string> message = _packetReader.ReadMessage();
            Hostname = message[1];

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {Hostname}");
            
            Task.Run(() => Process());
           
        }
        public ServerInt()
        {
            lastTimeChecked = DateTime.Now;
            Task.Run(() => DeleteJoiningCodes());
        }
        void DeleteJoiningCodes()
        {
            while(true)
            {
                try { 
                    if(CheckTime())
                    {
                        DeleteExpiredCodes();
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"[{DateTime.Now}] - Time Watcher : " + e.Message);
                    break;
                }
            }
        }
        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case (byte)PacketHeaders.Dofirst:
                            List<string> messaj = _packetReader.ReadMessage();
                            string ceva = messaj[1];
                            Console.WriteLine($"[{DateTime.Now}] - {Hostname} : {ceva}");

                            Program.SendBackToUserMessage();

                            break;
                        case (byte)PacketHeaders.DoSecond:
                            List<string> messaje = _packetReader.ReadMessage();
                            string altceva = messaje[1];
                            Console.WriteLine($"[{DateTime.Now}] - {Hostname} : {altceva}");
                            break;

                        case (byte)PacketHeaders.LoginRequest:
                            {
                                //probabil o sa las un try catch aici
                                //primim mesajul
                                HandleLoginRequest(_packetReader);
                                //ver daca merge asa
                                //daca nu, dau variabila globala list<msg> si transmit de aici mesajul
                            }
                            break;
                        case (byte)PacketHeaders.RegisterRequest:
                            {
                                //probabil o sa las un try catch aici
                                //primim mesajul
                                HandleRegisterRequest(_packetReader);
                                //ver daca merge asa
                                //daca nu, dau variabila globala list<msg> si transmit de aici mesajul
                            }
                            break;
                        case (byte)PacketHeaders.LoadGroupsRequest:
                            {
                                HandleLoadGroupsRequest(_packetReader);
                            }
                            break;
                        case (byte)PacketHeaders.SendPhotoToGroupRequest:
                            {
                                HandleSendPhotoToGroupRequest(_packetReader);
                            }
                            break;
                        case (byte)PacketHeaders.CreateGroupRequest:
                            {
                                HandleCreateGroupRequest(_packetReader);
                            }
                            break;
                        case (byte)PacketHeaders.LoadOwnerGroupsRequest:
                            {
                                HandleLoadOwnerGroupsRequest(_packetReader);
                            }
                            break;
                        case (byte)PacketHeaders.GenerateCodeRequest:
                            {
                                HandleGenerateCodeRequest(_packetReader);
                            }
                            break;
                        case (byte)PacketHeaders.JoinGroupRequest:
                            {
                                HandleJoinGroupRequest(_packetReader);
                            }
                            break;
                        case (byte)PacketHeaders.LoadDeleteGroupRequest:
                            {
                                HandleLoadDeleteGroupRequest(_packetReader);
                            }
                            break;
                        case (byte)PacketHeaders.LeaveGroupRequest:
                            {
                                HandleLeaveGroupRequest(_packetReader);
                            }
                            break;
                        case (byte)PacketHeaders.LoadImagesRequest:
                            {
                                HandleLoadImgRequest(_packetReader);
                            }
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{Hostname}]:Disconnected!");
                    ClientSocket.Close();
                    break;
                }
            }
        }
        #region time watcher and delete joining codes
        private bool CheckTime()
        {
            DateTime reTime = lastTimeChecked.AddHours(8);
            DateTime nowTime = DateTime.Now;
            if (DateTime.Compare(reTime, nowTime) <= 0)
            {
                lastTimeChecked = nowTime;
                return true;
            }
            return false;
        }

        private  void DeleteExpiredCodes()
        {
            List<string> codeIDs = new List<string>();
            DateTime date = DateTime.Now;

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"Select CodeID from GenerateCodes where ExpirationDate < @currDate";
                cmd.Parameters.AddWithValue("@currDate", date);
                bool succes = false;
                using (var result = cmd.ExecuteReader())
                {
                    while (result.Read())
                    {
                        codeIDs.Add(result.GetValue(0).ToString());
                        succes = true;
                    }
                }
                int code = 0;
                if (succes)
                {
                    foreach (var x in codeIDs)
                    {
                        code = Convert.ToInt32(x);
                        cmd.CommandText = @"Delete from GenerateCodes where CodeID = " + code;
                        cmd.ExecuteNonQuery();
                    }
                    Console.WriteLine($"[{DateTime.Now}] - SERVER : Successfully deleted joining codes! ");
                }
                connection.Close();

            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine($"[{DateTime.Now}] - SERVER : Delete joining codes procedure - " + e.Message);
                return;
            }
        }
        #endregion

        #region login
        public void HandleLoginRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Login to app request!");

            List<string> message = packetReader.ReadMessage();
            //pp ca mesajul vine instant, daca nu, asteptam;

            string username = message[1];
            string password = message[2];

            password = username + AES_Server.saltString() + password;
            password = AES_Server.Encrypt256(password);

            string userID = "";

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Select * from Users where Username = @user and Password = @pass";
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);
                bool succes = false;

                using (var result = cmd.ExecuteReader())
                {
                    if(result.Read())
                    {
                        userID = result.GetInt32(result.GetOrdinal("UserID")).ToString();
                        succes = true;
                    }
                }
                if(succes)
                {
                    cmd.CommandText = "Select @@ROWCOUNT";
                    var totalRows = cmd.ExecuteScalar();

                    if (Convert.ToInt32(totalRows) == 1)
                    {
                        //there is just one result of sql query
                        //so login is succesfull 
                        Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Log in with succes!");
                        connection.Close();
                        // send back login response
                        Program.LoginApplicationResponse("LogInSucces!", userID);
                    }
                    else
                    {
                        throw new Exception("Can't log in!");
                    }
                }
                else
                {
                    throw new Exception("Can't log in!");
                }

            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);
                //send back login response
                Program.LoginApplicationResponse("LogInFailed!");
                return;
            }
        }
        #endregion

        #region register
        public void HandleRegisterRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Register client to app request!");

            List<string> message = packetReader.ReadMessage();
            //pp ca mesajul vine instant, daca nu, asteptam;
            
            string username = message[1];
            string email = message[2];
            string password = message[3];

            password = username + AES_Server.saltString() + password;
            password = AES_Server.Encrypt256(password);

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Insert into Users(Username, Email, Password) Values (@user, @email, @pass)";
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@pass", password);

                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);
                connection.Close();
                //send back login response
                Program.RegistrationResponse("RegisterFailed!");
                return;
            }
            connection.Close();

           Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Registered with succes!");

           Program.RegistrationResponse("RegisterSucces!");
        }
        #endregion

        #region load groups
        private void HandleLoadGroupsRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Load user's groups request!");

            List<string> message = packetReader.ReadMessage();
            List<string> resultList = new List<string>();
            string userId = message[1];

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Select Groups.GroupName " +
                    "                           from GroupUsers " +
                    "                           Inner join Groups  " +
                    "                           ON GroupUsers.GroupID = Groups.GroupID" +
                    "                           where GroupUsers.UserID = @userid";
               cmd.Parameters.AddWithValue("@userid", userId);
                using (var result = cmd.ExecuteReader())
                {
                    while (result.Read())
                    {
                        resultList.Add(result.GetString(0));
                    }
                }
                //there is just one result of sql query
                //so login is succesfull 
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Load user's groups with succes!");
                connection.Close();
                // send back login response
                Program.LoadGroupsResponse("LoadGroupsSucces!", resultList);
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);
                //send back login response
                Program.LoadGroupsResponse("LoadGroupsFailed!", resultList);
                return;
            }
        }
        #endregion

        #region save photo to group
        private byte[] ConvertStringByteArray(string data)
        {
            byte[] bytesData = Convert.FromBase64String(data);
            return bytesData;

            
        }
        private void HandleSendPhotoToGroupRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Save photo in db's group request!");

            List<string> message = packetReader.ReadMessage();
            List<string> resultList = new List<string>();
            string filename = message[1];
            byte[] img = ConvertStringByteArray(message[2]);
            string groupName = message[3];
            string date = DateTime.Now.ToString();
            string groupID ="";
            try
            {
                connection.Open();



                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"Select GroupID from Groups where GroupName = @groupName";
                cmd.Parameters.AddWithValue("@groupName", groupName);
                bool succes = false;

                using (var result = cmd.ExecuteReader())
                {
                    if (result.Read())
                    {
                        groupID = result.GetInt32(result.GetOrdinal("GroupID")).ToString();
                        succes = true;
                    }
                }
                if(succes)
                {
                    cmd.CommandText = @"Insert into Images(GroupID, ImageName, Image, Date)
                                            Values(@groupID, @imageName, @image, @date)";
                    cmd.Parameters.AddWithValue("@groupID", groupID);
                    cmd.Parameters.AddWithValue("@imageName", filename);
                    cmd.Parameters.AddWithValue("@image", img);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    throw new Exception("Can't save photo in database!");
                }

                Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Save photo into db with succes!");
                connection.Close();


                Program.SavePhotoResponse("SendPhotoSucces!");
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);
                //send back login response
                Program.SavePhotoResponse("SendPhotoFailed!");
                return;
            }
        }
        #endregion

        #region create group
        public void HandleCreateGroupRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Create group request!");

            List<string> message = packetReader.ReadMessage();

            string userID = message[1];
            string groupName = message[2];
            string groupID = "";

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"Insert into Groups(GroupName, OwnerID) Values (@groupName, @ownerID)";
                cmd.Parameters.AddWithValue("@groupName", groupName);
                cmd.Parameters.AddWithValue("@ownerID", Convert.ToInt32(userID));

                cmd.ExecuteNonQuery();

                cmd.CommandText = @"Select GroupID from Groups where GroupName = @groupName";

                using (var result = cmd.ExecuteReader())
                {
                    if (result.Read())
                    {
                        groupID = result.GetInt32(result.GetOrdinal("GroupID")).ToString();
                    }
                }

                cmd.CommandText = @"Insert into GroupUsers(UserID, GroupID) Values (@userID, @groupID)";
                cmd.Parameters.AddWithValue("@groupID", groupID);
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);
                connection.Close();
                //send back login response
                Program.CreateGroupResponse("CreateGroupFailed!");
                return;
            }
            connection.Close();

            Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Create group succes!");

            Program.CreateGroupResponse("CreateGroupSucces!");
        }
        #endregion

        #region load owner groups
        private void HandleLoadOwnerGroupsRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Load owner's groups request!");

            List<string> message = packetReader.ReadMessage();
            List<string> resultList = new List<string>();
            string userId = message[1];

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"Select GroupName from Groups where OwnerID = @userid";
                cmd.Parameters.AddWithValue("@userid", userId);
                using (var result = cmd.ExecuteReader())
                {
                    while (result.Read())
                    {
                        resultList.Add(result.GetString(0));
                    }
                }
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Load owners's groups with succes!");
                connection.Close();

                Program.LoadOwnerGroupsResponse("LoadOwnerGroupsSucces!", resultList);
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);

                Program.LoadOwnerGroupsResponse("LoadOwnerGroupsFailed!", resultList);
                return;
            }
        }
        #endregion

        #region add generated code in db
        public void HandleGenerateCodeRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Add generated code request!");

            List<string> message = packetReader.ReadMessage();

            string userID = message[1];
            string groupName = message[2];
            string generatedCode = message[3];
            string groupID = "";
            DateTime date = DateTime.Now;
            date=date.AddDays(7);

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"Select GroupID from Groups where GroupName = @groupName";
                cmd.Parameters.AddWithValue("@groupName", groupName);

                using (var result = cmd.ExecuteReader())
                {
                    if (result.Read())
                    {
                        groupID = result.GetInt32(result.GetOrdinal("GroupID")).ToString();
                    }
                }

                cmd.CommandText = @"Select * from GenerateCodes where GroupID = @groupID";
                cmd.Parameters.AddWithValue("@groupID", groupID);
                bool succes = false;
                using (var result = cmd.ExecuteReader())
                {
                    if (result.Read())
                    {
                        succes = true;
                    }
                }

                if(succes)
                {
                    cmd.CommandText = @"Delete from GenerateCodes where GroupID = @groupID";
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = @"Insert into GenerateCodes(GroupID, Code, ExpirationDate, UserID)
                                                    Values (@groupID, @code, @date, @userID)";
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@code", generatedCode);
                cmd.Parameters.AddWithValue("@date", date);

                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);
                connection.Close();
                //send back login response
                Program.GenerateCodeResponse("AddCodeFailed!");
                return;
            }
            connection.Close();

            Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Generated code added succesfully!");

            Program.GenerateCodeResponse("AddCodeSucces!");
        }
        #endregion

        #region join group
        public void HandleJoinGroupRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Join group request!");

            List<string> message = packetReader.ReadMessage();

            string userID = message[1];
            string groupCode = message[2];
            string groupID = "";

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;

                //check if code exist
                cmd.CommandText = @"Select GroupID from GenerateCodes where Code = @groupCode";
                cmd.Parameters.AddWithValue("@groupCode", groupCode);
                bool succes = false;
                using (var result = cmd.ExecuteReader())
                {
                    if (result.Read())
                    {
                        groupID = result.GetInt32(result.GetOrdinal("GroupID")).ToString();
                        succes = true;
                    }
                }
                if (!succes)
                {
                    connection.Close();
                    Program.JoinGroupResponse("InvalidCode!");
                    return;
                }

                //check if user already is in the group
                cmd.CommandText = @"Select * from GroupUsers where UserID = @userID And GroupID = @groupID";
                cmd.Parameters.AddWithValue("@groupID", groupID);
                cmd.Parameters.AddWithValue("@userID", userID);
                succes = false;
                using (var result = cmd.ExecuteReader())
                {
                    if (result.Read())
                    {
                        succes = true;
                    }
                }
                if (succes)
                {
                    connection.Close();
                    Program.JoinGroupResponse("AlreadyJoined!");
                    return;
                }


                //join user in group
                cmd.CommandText = @"Insert into GroupUsers(UserID, GroupID) Values (@userID, @groupID)";
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);
                connection.Close();
                //send back login response
                Program.JoinGroupResponse("JoinGroupFailed!");
                return;
            }
            connection.Close();

            Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Join group succes!");

            Program.JoinGroupResponse("JoinGroupSucces!");
        }
        #endregion

        #region load delete groups
        private void HandleLoadDeleteGroupRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Load delete user's groups request!");

            List<string> message = packetReader.ReadMessage();
            List<string> resultList = new List<string>();
            string userId = message[1];

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"Select Groups.GroupName From Groups where OwnerID = @userid ";
                cmd.Parameters.AddWithValue("@userid", userId);
                using (var result = cmd.ExecuteReader())
                {
                    while (result.Read())
                    {
                        resultList.Add(result.GetString(0));
                    }
                }
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Load user's groups with succes!");
                connection.Close();
 
                Program.LoadDeleteGroupsResponse("LoadGroupsSucces!", resultList);
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);
                //send back login response
                Program.LoadDeleteGroupsResponse("LoadGroupsFailed!", resultList);
                return;
            }
        }
        #endregion


        #region leave group
        public void HandleLeaveGroupRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Leave group request!");

            List<string> message = packetReader.ReadMessage();

            string userID = message[1];
            string groupName = message[2];
            string action = message[3];
            string groupID = "";

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"Select GroupID from Groups where GroupName = @groupName";
                cmd.Parameters.AddWithValue("@groupName", groupName);

                using (var result = cmd.ExecuteReader())
                {
                    if (result.Read())
                    {
                        groupID = result.GetInt32(result.GetOrdinal("GroupID")).ToString();
                    }
                }

                cmd.CommandText = @"Delete from GroupUsers where UserID = @ownerID and GroupID = @groupID";
                cmd.Parameters.AddWithValue("@groupID", groupID);
                cmd.Parameters.AddWithValue("@ownerID", Convert.ToInt32(userID));
                cmd.ExecuteNonQuery();

                if (action == "Action")
                {
                    cmd.CommandText = @"Delete from GenerateCodes where GroupID = @groupID";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"Delete from Images where GroupID = @groupID";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"Delete from Groups where GroupID = @groupID and OwnerID = @ownerID";
                    cmd.ExecuteNonQuery();

                    connection.Close();
                    Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Leave group succes!");

                    Program.LeaveGroupResponse("DeleteGroupSucces!");

                }
                else
                {
                    connection.Close();

                    Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Leave group succes!");

                    Program.LeaveGroupResponse("LeaveGroupSucces!");
                }

                
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);
                connection.Close();
                //send back login response
                Program.LeaveGroupResponse("LeaveGroupFailed!");
                return;
            }
        }
        #endregion

        #region load images
        private void HandleLoadImgRequest(PacketReader packetReader)
        {
            Console.WriteLine($"[{ DateTime.Now}] - { Hostname}: Load user's images request!");

            List<string> message = packetReader.ReadMessage();
            List<string> resultList = new List<string>();
            string groupName = message[1];

            try
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"Select Images.ImageName, Images.Image, Images.Date from Images
                                                   inner join Groups 
                                                        on Images.GroupID = Groups.GroupID 
                                                                Where Groups.GroupName = @groupName
                                                                    Order by Images.Date Desc";
                cmd.Parameters.AddWithValue("@groupName", groupName);
                using (var result = cmd.ExecuteReader())
                {
                    while (result.Read())
                    {
                        resultList.Add(result.GetString(0));
                        resultList.Add(Convert.ToBase64String((byte[])result.GetValue(1)));
                        resultList.Add(result.GetValue(2).ToString());
                    }
                }

                Console.WriteLine($"[{DateTime.Now}] - {Hostname} : Load user's images with succes!");
                connection.Close();

                Program.LoadImgResponse("LoadImgSucces!", resultList);
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine($"[{DateTime.Now}] - {Hostname} :" + e.Message);

                Program.LoadImgResponse("LoadImgFailed!", resultList);
                return;
            }
        }
        #endregion
    }
}
