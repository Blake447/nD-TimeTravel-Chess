using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class GameSpawner : MonoBehaviour
{
    PrimaryMenu menu;
    private GameDescriptor spawnedGamemode;
    [SerializeField] bool isEditor;
    [SerializeField] bool isTutorial;
    int SCENE_SINGULARITY = 3;
    int SCENE_MVTIME = 4;
    int SCENE_TUTORIAL = 8;
    int SCENE_EDITOR = 6;

    // Start is called before the first frame update
    void Awake()
    {
        menu = FindObjectOfType<PrimaryMenu>(); // find the menu system
        if (menu == null)
            Debug.LogError("Could not find menu");// if we can't, throw an error
        if (FindObjectOfType<HistoryLibrarian>() == null) // If we can't find a librarian (networked object)
        {
            int prefab_index = menu.GetSelectedGameMode(); // Get the selected prefab from the menu
            SpawnGameModeLocal(prefab_index); // and spawn the appropriate game mode locally
        }
    }
    // Locally Load in the addidive scene
    public void SpawnGameModeLocal(int prefab_index)
    {
        // Instantiate a descriptor script that the additive scene will do a FindObjectCall<>() for
        spawnedGamemode = Instantiate(menu.GetPrefab(prefab_index)); // get the index from the menu
        if (spawnedGamemode == null) // throw an error if we failed to spawn a game mode
            Debug.LogError("Failed to spawn prefab index " + prefab_index);
        else // otherwise loade the appropriate additive scene
        {
            if (isTutorial)
                SceneManager.LoadScene(SCENE_TUTORIAL, LoadSceneMode.Additive);
            else if (isEditor)
                SceneManager.LoadScene(SCENE_EDITOR, LoadSceneMode.Additive);
            else if (spawnedGamemode.isTimeTravel)
                SceneManager.LoadScene(SCENE_MVTIME, LoadSceneMode.Additive);
            else
                SceneManager.LoadScene(SCENE_SINGULARITY, LoadSceneMode.Additive);
        }
    }
    // Runs OnRoomJoined()
    public void TrySpawnGameMode()
    {
        HistoryLibrarian librarian = FindObjectOfType<HistoryLibrarian>(); // Find the librarian
        int prefab_index = menu.GetSelectedGameMode(); // Get the selected game mode. Runs if hosting
        if (librarian != null)
            librarian.SpawnGameMode(prefab_index); // have the librarian spawn the approprate gamemode. Sends RPC all buffered.
    }
}
