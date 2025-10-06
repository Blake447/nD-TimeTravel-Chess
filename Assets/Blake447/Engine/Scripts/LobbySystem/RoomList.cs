using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class RoomList : MonoBehaviourPunCallbacks
{
    public static RoomList instance;


    [Header("UI")] public Transform roomListParent;
    public GameObject MainUI;
    public GameObject LobbyUI;
    public GameObject ConnectingUI;
    public GameObject CreateGameUI;

    public GameObject roomListItemPrefab;
    public TMP_Text RoomNameDisplay;
    public TMP_Text PlayCountDisplay;
    public Button roomJoinButton;

    public bool editor = false;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    private string roomSelected;

    public TMP_InputField nicknameField;

    public GameSetupMenu gameSetupMenu;

    public HistoryLibrarian librarian;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        PhotonPeer.RegisterType(typeof(GameSettings), (byte)'S', GameSettings.SerializeSettings, GameSettings.DeserializeSettings);
        PhotonPeer.RegisterType(typeof(BoardLayout), (byte)'L', BoardLayout.SerializeLayout, BoardLayout.DeserializeLayout);
    }
    public void SetNickname(string _name)
    {
        PhotonNetwork.NickName = _name;
    }
    public void SetNickname()
    {
        PhotonNetwork.NickName = nicknameField.text;
    }

    public void JoinSelectedRoom()
    {
        bool validRoom = false;
        for (int i = 0; i < cachedRoomList.Count; i++)
        {
            if (roomSelected == cachedRoomList[i].Name)
            {
                validRoom = true;
            }
        }
        RoomManager.instance.JoinRoom(roomSelected);
        editor = false;
    }
    public void SelectRoom(string roomName)
    {
        for (int i = 0; i < cachedRoomList.Count; i++)
        {
            if (roomName == cachedRoomList[i].Name)
            {
                roomSelected = roomName;
                PlayCountDisplay.text = "Player count: " + cachedRoomList[i].PlayerCount.ToString();
                RoomNameDisplay.text = "Room name: " + cachedRoomList[i].Name;
                roomJoinButton.interactable = true;
            }
        }
    }
    public void DeselectRoom()
    {
        roomSelected = "";
        roomJoinButton.interactable = false;
    }

    public void StartConnectToLobby()
    {
        MainUI.SetActive(false);
        LobbyUI.SetActive(false);
        CreateGameUI.SetActive(false);
        ConnectingUI.SetActive(true);
        StartCoroutine(ConnectToLobby());
        editor = false;
    }
    IEnumerator ConnectToLobby()
    {
        // precautions
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.Disconnect();
        }
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
        editor = false;
    }
    public void DisconnectFromLobby()
    {
        PhotonNetwork.Disconnect();
        editor = false;
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        ConnectingUI.SetActive(false);
        CreateGameUI.SetActive(false);
        LobbyUI.SetActive(true);
        MainUI.SetActive(false);
        editor = false;
		librarian.gameObject.SetActive(true);
	}
	public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        ConnectingUI.SetActive(false);
        LobbyUI.SetActive(false);
        MainUI.SetActive(true);
        CreateGameUI.SetActive(false);
        cachedRoomList.Clear();
        editor = false;
		librarian.gameObject.SetActive(false);
	}
	public void DisableMenu()
    {
		MainUI.SetActive(false);
		LobbyUI.SetActive(false);
		ConnectingUI.SetActive(false);
		CreateGameUI.SetActive(false);
	}
    public void ReturnToMainMenu()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        MainUI.SetActive(true);
        LobbyUI.SetActive(false);
        ConnectingUI.SetActive(false);
        CreateGameUI.SetActive(false);
        gameSetupMenu.CleanupBoard();
        gameSetupMenu.UpdateBoards();
        //gameSetupMenu.RefreshLayouts();
        editor = false;
    }
    public void OpenGameUI()
    {
        MainUI.SetActive(false);
        LobbyUI.SetActive(false);
        ConnectingUI.SetActive(false);
        CreateGameUI.SetActive(true);
        editor = false;
    }
    public void OpenGameUIEditor()
    {
		MainUI.SetActive(false);
		LobbyUI.SetActive(false);
		ConnectingUI.SetActive(false);
		CreateGameUI.SetActive(true);
        editor = true;
	}
    public void OpenLocal()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        OpenGameUI();
    }
    //IEnumerator Start()
    //{
    //    // precautions
    //    if (PhotonNetwork.InRoom)
    //    {
    //        PhotonNetwork.LeaveLobby();
    //        PhotonNetwork.Disconnect();
    //    }
    //    yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

    //    PhotonNetwork.ConnectUsingSettings();
    //}



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            foreach (RoomInfo room in roomList)
            {
                if (room.RemovedFromList)
                {
                    for (int i = 0; i < cachedRoomList.Count; i++)
                    {
                        if (room.Name == cachedRoomList[i].Name)
                        {
                            cachedRoomList.RemoveAt(i);
                            if (room.Name == roomSelected)
                            {
                                DeselectRoom();
                            }
                            break;
                        }
                    }
                }
                else
                {
                    bool isInList = false;
                    for (int i = 0; i < cachedRoomList.Count; i++)
                    {
                        isInList = isInList || (cachedRoomList[i].Name == room.Name);
                    }
                    if (!isInList)
                    {
                        cachedRoomList.Add(room);
                    }
                }



                //for (int i = 0; i < cachedRoomList.Count; i++)
                //{
                //    if (cachedRoomList[i].Name == room.Name)
                //    {
                //        List<RoomInfo> newList = cachedRoomList;
                //        if (room.RemovedFromList)
                //        {
                //            newList.Remove(newList[i]);
                //            if (room.Name == roomSelected)
                //            {
                //                DeselectRoom();
                //            }
                //        }
                //        else
                //        {
                //            newList[i] = room;
                //        }
                //        cachedRoomList = newList;
                //    }
                //}
                
            }
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        foreach (Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }
        foreach (RoomInfo room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);
            roomItem.GetComponent<RoomButton>().SetRoomName(room.Name);
            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/16";
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
