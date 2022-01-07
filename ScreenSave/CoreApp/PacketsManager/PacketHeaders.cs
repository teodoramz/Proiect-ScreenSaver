using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp
{
    public enum PacketHeaders
    {
        ConectToServerRequest = 1,
        ConnectToServerResponse = 2,
        Dofirst = 3,
        DoSecond = 4,
        LoginRequest = 5,
        LoginResponse = 6,
        RegisterRequest = 7,
        RegisterResponse = 8,
        LoadGroupsRequest = 9,
        LoadGroupsResponse = 10,
        SendPhotoToGroupRequest = 11,
        SendPhotoToGroupResponse = 12,
        CreateGroupRequest = 13,
        CreateGroupResponse = 14,
        LoadOwnerGroupsRequest = 15,
        LoadOwnerGroupsResponse = 16,
        GenerateCodeRequest = 17,
        GenerateCodeRespone = 18,
        JoinGroupRequest = 19,
        JoinGroupResponse = 20,
        LoadDeleteGroupRequest = 21,
        LoadDeleteGroupResponse = 22,
        LeaveGroupRequest = 23,
        LeaveGroupResponse = 24,
        LoadImagesRequest = 25,
        LoadImagesResponse = 26,
    }
}
