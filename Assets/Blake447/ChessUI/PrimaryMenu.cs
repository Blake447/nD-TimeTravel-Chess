using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PrimaryMenu : MonoBehaviour
{
    public UIWidgets.Custom.DataTypeGameModeNS.ListViewDataTypeGameMode gamemodeList;
    public GameObject persistant;
    public GameObject toggled;
    public GameObject MainMenu;
    public GameObject GameMenu;
    static PrimaryMenu menu;
    int MenuState = 0;
    public static int STATE_MAIN = 0;
    public static int STATE_GAME = 1;
    public int selectedGameMode = -1;
    public GameDescriptor[] GameDescriptors;
    string username = "";
    string lobbyname = "";
    string file_name = "";
    public bool useForwardLateralSelected = true;
    bool isHosting;
    bool isLocal;
    int SCENE_MENU = 0;
    int SCENE_LOCAL = 1;
    int SCENE_NETWORK = 2;
    int SCENE_EDITOR = 5;
    int SCENE_TUTORIAL = 7;

    public WidgetController widgetController;
    public GameObject GameInfoRoot;
    public GameObject CheckMark;

    public WidgetController multiverseWidgetController;
    public GameObject multiverse_model_singularity;
    public GameObject multiverse_model_MVTime;



    // Awake, Toggle menu, etc
    #region Menu Functions
    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_ANDROID
        Screen.orientation = ScreenOrientation.LandscapeLeft;
#endif
        // Construct singleton menu
        if (menu == null)
        {
            DontDestroyOnLoad(this.gameObject);
            menu = this;
        }
        else
            Destroy(this.gameObject);

        // Populate menu list with gamedata of assigned game descriptors
        foreach (GameDescriptor gamemode in GameDescriptors)
        {
            DataTypeGameMode gamedata = new DataTypeGameMode();
            int dimensions = gamemode.dimensions.Length;
            if (gamemode.isTimeTravel)
                dimensions -= 2;
            gamedata.dimensions = dimensions + "D" + (gamemode.isTimeTravel ? " + MT" : "");
            gamedata.GameType = gamemode.game_name;
            gamedata.type = "chess";
            gamedata.multiverse = gamemode.isTimeTravel ? "MV Time" : "Singularity";
            gamedata.players = 2;
            gamemodeList.Add(gamedata);
        }
    }
    // Toggle the menu
    public void ToggleMenu()
    {
        toggled.SetActive(!toggled.activeInHierarchy);
    }
    // Lock or unlock the menu when clicking inside input field
    public void TryLockMenu()
    {
        CameraRig cameraRig = FindObjectOfType<CameraRig>();
        if (cameraRig != null)
            cameraRig.Lock();
    }
    public void TryUnlockMenu()
    {
        CameraRig cameraRig = FindObjectOfType<CameraRig>();
        if (cameraRig != null)
            cameraRig.Unlock();
    }
    public void SetMenuState(int state)
    {
        if (state == STATE_MAIN)
        {
            toggled.SetActive(true);
            MainMenu.SetActive(true);
            GameMenu.SetActive(false);
            if (widgetController != null)
                widgetController.gameObject.SetActive(true);
            if (multiverseWidgetController != null)
                multiverseWidgetController.gameObject.SetActive(true);
        }
        else
        {
            toggled.SetActive(false);
            MainMenu.SetActive(false);
            GameMenu.SetActive(true);
            if (widgetController != null)
                widgetController.gameObject.SetActive(false);
            if (multiverseWidgetController != null)
                multiverseWidgetController.gameObject.SetActive(false);
        }
    }
    #endregion
    // Get and set game settings for loading
    #region Getters and Setters
    // Set the filename for saving games
    public void SetFileName(string name)
    {
        file_name = name;
    }
    // Attempt to save to set filename
    public void AttemptSaveGame()
    {
        GameInstance game = FindObjectOfType<GameInstance>();
        if (game != null)
            if (file_name != "")
                game.SaveGame(file_name);
            else
                Debug.Log("Please specify save name");
        else
            Debug.LogError("Could not find game instance");
    }
    // Attempt to load from set filename
    public void AttemptLoadGame()
    {
        GameInstance game = FindObjectOfType<GameInstance>();
        if (game != null)
            if (file_name != "")
                game.LoadGame(file_name);
            else
                Debug.Log("Please specify save name");
        else
            Debug.LogError("Could not find game instance");
        toggled.SetActive(false);
    }
    // Set game settings. Set and Get username, lobby name
    public void SetUsername(string username)
    {
        this.username = username;
    }
    public string GetUsername()
    {
        return username;
    }
    public void SetLobbyName(string lobbyname)
    {
        this.lobbyname = lobbyname;
    }
    public string GetLobbyname()
    {
        return lobbyname;
    }
    // Get networking and game setting information
    public bool IsHosting()
    {
        return isHosting;
    }
    public bool IsLocal()
    {
        return isLocal;
    }
    public void SetForwardLateral(bool userForwardLateral)
    {
        useForwardLateralSelected = userForwardLateral;
    }
    // Get and set selected game mode
    public void SetSelectedGameMode(int gamemode, UIWidgets.ListViewItem item)
    {
        selectedGameMode = gamemode;
        if (widgetController != null)
        {
            widgetController.gameObject.SetActive(true);
            widgetController.SetBoard(GetPrefab(gamemode).board.gameObject);
        }
        if (multiverseWidgetController != null)
        {
            multiverseWidgetController.gameObject.SetActive(true);
            if (GameDescriptors[gamemode].isTimeTravel)
            {
                multiverseWidgetController.SetBoard(multiverse_model_MVTime);
            }
            else
            {
                multiverseWidgetController.SetBoard(multiverse_model_singularity);
            }


        }
        for (int i = 1; i < 5; i++)
        {
            GameObject textRoot = GameInfoRoot.transform.GetChild(i).gameObject;
            GameObject text = textRoot.transform.GetChild(0).gameObject;
            UIWidgets.Custom.DataTypeGameModeNS.ListViewComponentDataTypeGameMode gameData = gamemodeList.GetItemInstance(gamemode);
            TMPro.TMP_Text tmp_text = text.GetComponent<TMPro.TMP_Text>();
            switch(i)
            {
                case 1:
                    tmp_text.text = gameData.type.text;
                    break;
                case 2:
                    tmp_text.text = gameData.multiverse.text;
                    break;
                case 3:
                    tmp_text.text = gameData.dimensions.text;
                    break;
                case 4:
                    UnityEngine.UI.Toggle toggle = text.GetComponent<UnityEngine.UI.Toggle>();
                    toggle.isOn = !GameDescriptors[gamemode].useForwardLateral; 
                    break;
                default:
                    break;
            }
        }
    }
    public int GetSelectedGameMode()
    {
        return selectedGameMode;
    }
    // Get selected game descriptor
    public GameDescriptor GetPrefab(int index)
    {
        if (GameDescriptors != null && index >= 0 && index < GameDescriptors.Length)
            return GameDescriptors[index];
        return null;
    }
    #endregion
    // Methods to load between the various scenes and main menu
    #region SceneManagment
    // Load the main menu
    public void LoadMainMenu()
    {
        if (widgetController != null)
            widgetController.gameObject.SetActive(true);
        if (multiverseWidgetController != null)
            multiverseWidgetController.gameObject.SetActive(true);
        // Load the main menu scene
        SceneManager.LoadScene(SCENE_MENU); // Load menu
        BoardLoader.ClearBoardState(); // clear board state loaded from file
        this.isHosting = false; // unset "master" setting
        this.isLocal = true; // set the game to local mode
        SetMenuState(PrimaryMenu.STATE_MAIN); // set the menu to main menu state
        Archeologist archeologist = FindObjectOfType<Archeologist>(); // find the recovery script, "archeologist"
        if (archeologist != null)
            archeologist.StoreRecovery(); // Store recovery info if we can find the archeologist
        else
            Debug.LogError("Failed to find archeologist"); // Throw error if we cant
        // Attempt to disconnect from photon network;
        try {
            if (Photon.Pun.PhotonNetwork.IsConnected)
                Photon.Pun.PhotonNetwork.Disconnect();
        }
        catch {
            Debug.Log("Throwing error");
        }
    }
    public void LaunchLocal()
    {
        if (widgetController != null)
            widgetController.gameObject.SetActive(false);
        if (multiverseWidgetController != null)
            multiverseWidgetController.gameObject.SetActive(false);
        SceneManager.LoadScene(SCENE_LOCAL); // Load a local loading scene
        this.isHosting = false; // unset "master" setting
        this.isLocal = true; // set the game to local mode
        SetMenuState(PrimaryMenu.STATE_GAME); // set the menu to game state
        // Attempt to discconect from photon network
        try
        {
            if (Photon.Pun.PhotonNetwork.IsConnected)
                Photon.Pun.PhotonNetwork.Disconnect();
        }
        catch
        {
            Debug.Log("Throwing error");
        }
    }
    public void LaunchCreate()
    {
        if (widgetController != null)
            widgetController.gameObject.SetActive(false);
        if (multiverseWidgetController != null)
            multiverseWidgetController.gameObject.SetActive(false);
        if (username.Length == 0)
            username = "username"; // set default username
        if (lobbyname.Length == 0)
            lobbyname = "room"; // set default room
        this.isHosting = true; // set "master mode"
        this.isLocal = true; // set the game to local mode ?
        SetMenuState(PrimaryMenu.STATE_GAME); // Set the menu to game state
        if (selectedGameMode != -1)
            SceneManager.LoadScene(SCENE_NETWORK); // load the network scene as the host client 
        // Attempt to discconect from photon network
        try
        {
            if (Photon.Pun.PhotonNetwork.IsConnected)
                Photon.Pun.PhotonNetwork.Disconnect();
        }
        catch
        {
            Debug.Log("Throwing error");
        }
    }
    public void LaunchJoin()
    {
        if (widgetController != null)
            widgetController.gameObject.SetActive(false);
        if (multiverseWidgetController != null)
            multiverseWidgetController.gameObject.SetActive(false);
        if (username.Length == 0)
            username = "username"; // set default username
        if (lobbyname.Length == 0)
            lobbyname = "room"; // set default room
        this.isHosting = false; // unset "master mode"
        this.isLocal = false; // unset local mode
        SetMenuState(PrimaryMenu.STATE_GAME); // Set the menu to game state
        SceneManager.LoadScene(SCENE_NETWORK); // load the network scene as the joining client
    }
    public void LaunchEditor()
    {
        if (widgetController != null)
            widgetController.gameObject.SetActive(false);
        if (multiverseWidgetController != null)
            multiverseWidgetController.gameObject.SetActive(false);
        SceneManager.LoadScene(SCENE_EDITOR); // load the editor scene
        this.isHosting = false; // unset "master" mode
        this.isLocal = true; // set the game to local mode
        SetMenuState(PrimaryMenu.STATE_GAME); // Set the menu to game state
        // Attempt to discconect from photon network
        try
        {
            if (Photon.Pun.PhotonNetwork.IsConnected)
                Photon.Pun.PhotonNetwork.Disconnect();
        }
        catch
        {
            Debug.Log("Throwing error");
        }
    }
    public void LaunchTutorial()
    {
        if (widgetController != null)
            widgetController.gameObject.SetActive(false);
        if (multiverseWidgetController != null)
            multiverseWidgetController.gameObject.SetActive(false);
        SceneManager.LoadScene(SCENE_TUTORIAL); // Load the tutorial loading scene
        this.isHosting = false; // unset "master mode"
        this.isLocal = true; // set the game to local mode
        SetMenuState(PrimaryMenu.STATE_GAME); // set the menu to game state
        // Attempt to discconect from photon network
        try
        {
            if (Photon.Pun.PhotonNetwork.IsConnected)
                Photon.Pun.PhotonNetwork.Disconnect();
        }
        catch
        {
            Debug.Log("Throwing error");
        }
    }
    #endregion




}
