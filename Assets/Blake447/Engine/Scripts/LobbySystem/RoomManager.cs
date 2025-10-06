using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("UI elements")]
    public TMPro.TMP_InputField roomName;

    public static RoomManager instance;
    // Start is called before the first frame update
    private string nickname = "unnamed";
    public string roomSelected = "";
    public HistoryLibrarian librarian;


    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void CatchUpRoomState()
    {
        librarian.gameObject.SetActive(true);
        librarian.RequestGameBoard();
        //librarian.RequestGameState();
        //yield return null;
    }
    public void CreateRoom()
    {
        string name = roomName.text;
        bool createdRoom = PhotonNetwork.CreateRoom(name);
    }
    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }
	public override void OnDisconnected(DisconnectCause cause)
	{
        librarian.gameObject.SetActive(false);
	}
	public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        
        RoomList.instance.OpenGameUI();
    }
	public override void OnConnected()
	{
		librarian.gameObject.SetActive(true);
	}
	public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.IsMasterClient)
        {
            RoomList.instance.OpenGameUI();
        }
        else
        {
            RoomList.instance.DisableMenu();
        }
        CatchUpRoomState();
        
        //StartCoroutine(CatchUpRoomState());
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        librarian.gameObject.SetActive(false);
        base.OnJoinRoomFailed(returnCode, message);
	}
    public void ChangeNickname(string _name)
    {
        nickname = _name;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
