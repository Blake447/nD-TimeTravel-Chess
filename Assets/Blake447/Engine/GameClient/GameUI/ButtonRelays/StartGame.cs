using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : ButtonRelay
{
    GameInstance game;
    bool isBound = false;

    public override void BindButtonToGameStatus(GameInstance game)
    {
        if (game != null)
        {
            this.game = game;
            isBound = true;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isBound)
        {
            bool isGameInProgress = game.IsGameInProgress();
            bool isGameReady = game.IsGameReady();

            this.gameObject.SetActive(!isGameInProgress);
            if (base.button != null)
            {
                button.interactable = isGameReady;
            }

        }
    }
}
