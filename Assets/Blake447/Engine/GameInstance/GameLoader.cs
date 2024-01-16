using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    public int BoardIndex;
    public bool IsTimeTravel;
    [SerializeField] GameDescriptor[] gameDescriptors;
    GameInstance game;
    private void Awake()
    {
        GameDescriptor gameDescriptor = FindObjectOfType<GameDescriptor>(); // Find instantiated game descriptor
        GameInstance game = FindObjectOfType<GameInstance>(); // Find the game instance loaded
        if (gameDescriptor != null)
            game.InitializeGame(this, gameDescriptor); // initialize the game instance
        BoardEditor editor = FindObjectOfType<BoardEditor>(); // search for an editor
        if (editor != null)
            editor.InitializeClient(game); // initialize if found
        HistoryLibrarian librarian = FindObjectOfType<HistoryLibrarian>(); // search for librarian
        if (librarian != null)
            librarian.RequestGameState(); // if found, request RPC for game update (for joining) from master
    }
}
