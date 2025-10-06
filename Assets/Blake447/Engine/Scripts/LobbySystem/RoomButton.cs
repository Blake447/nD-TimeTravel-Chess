using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomButton : MonoBehaviour
{
    private string roomName;
    public void SetRoomName(string roomName)
    {
        this.roomName = roomName;
    }
    public void SelectRoom()
    {
        RoomList.instance.SelectRoom(roomName);
    }
}
