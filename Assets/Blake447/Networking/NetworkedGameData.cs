using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class NetworkedGameData : MonoBehaviour
{
    private PhotonView photonView;
    int gamemode;
    bool isTimeTravel;
    GameInstance game;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }
    public void SetGameMode(int gamemode)
    {
        this.gamemode = gamemode;
        photonView.RPC(nameof(RPC_SetGameMode), RpcTarget.AllBuffered, new object[] { gamemode } );
    }
    [PunRPC]
    private object RPC_SetGameMode(int gamemode)
    {
        this.gamemode = gamemode;
        GameDescriptor gameDescriptor = FindObjectOfType<GameDescriptor>();
        PrimaryMenu menu = FindObjectOfType<PrimaryMenu>();
        if (gameDescriptor == null)
        {
            GameDescriptor spawnedGamemode = Instantiate(menu.GetPrefab(gamemode));
            if (spawnedGamemode == null)
                menu.LoadMainMenu();
            else
            {
                if (spawnedGamemode.isTimeTravel)
                    SceneManager.LoadScene(4, LoadSceneMode.Additive);
                else
                    SceneManager.LoadScene(3, LoadSceneMode.Additive);
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
