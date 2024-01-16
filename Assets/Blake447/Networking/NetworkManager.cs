using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] bool timing = false;
    float timeout = 10.0f;
    [SerializeField] float timer = 0.0f;
    private const string ROOM_NAME = "room_name";
    private const int MAX_PLAYERS = 2;
    string usernameInput = "";
    string lobbynameInput = "";
    public TMPro.TMP_Text text;
    PrimaryMenu menu;
    public bool hosting = true;
    public GameSpawner gameSpawner;
    public NetworkedGameData gameData;
    private void Awake()
    {
        menu = FindObjectOfType<PrimaryMenu>();
        if (menu == null)
            SceneManager.LoadScene(0);
        PhotonNetwork.AutomaticallySyncScene = true;
        usernameInput = menu.GetUsername();
        lobbynameInput = menu.GetLobbyname();
        hosting = menu.IsHosting();
        Debug.Log("Attempting connection");
        Connect();
    }
    // Start is called before the first frame update
    void Start()
    {
           
    }
    public void Connect()
    {
        timer = timeout;
        timing = true;
        if (usernameInput.Length >= 1 && lobbynameInput.Length >= 1)
        {
            Debug.Log("Input valid, attempting to connect");
            PhotonNetwork.NickName = usernameInput;
            PhotonNetwork.ConnectUsingSettings();
            text.text = "Connecting...";
        }
        else
        {
            menu.LoadMainMenu();
        }
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        text.text = "Connected to server";
        PhotonNetwork.JoinLobby();

    }
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        //menu.LoadMainMenu();
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        if (hosting)
            PhotonNetwork.CreateRoom(lobbynameInput, new Photon.Realtime.RoomOptions() { MaxPlayers = 2 });
        else
            PhotonNetwork.JoinRoom(lobbynameInput);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        timing = false;
        if (hosting)
            gameSpawner.TrySpawnGameMode();
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(newPlayer.NickName + " has joined");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        PhotonNetwork.Disconnect();
        menu.LoadMainMenu();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        PhotonNetwork.Disconnect();
        menu.LoadMainMenu();
    }
    #endregion



    // Update is called once per frame
    void Update()
    {
        if (timing)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                PhotonNetwork.Disconnect();
                menu.LoadMainMenu();
            }
        }
    }
}
