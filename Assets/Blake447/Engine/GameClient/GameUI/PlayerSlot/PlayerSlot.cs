using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Blake447;

public class PlayerSlot : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text player_name_text;

    [SerializeField]
    TMPro.TMP_Text player_timer_text;

    [SerializeField]
    int playerIndex = 0;

    GameClient gameClient;
    GameInstance game;


    public void BindToGame(GameInstance game, int player_index)
    {
        this.game = game;
        playerIndex = player_index;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SendChangePlayerType() // received from Button;
    {
        if (game != null && !game.IsGameInProgress())
        {
            game.CyclePlayerType(playerIndex);
        }
    }
    public void SendChangeTimer()
    {
        if (game != null && !game.IsGameInProgress())
        {
            game.ChangeTimerAmount(playerIndex);
        }
    }


    void UpdateBoundDate()
    {
        PlayerInfo pi = game.GetPlayerInfo(this.playerIndex);
        player_name_text.text = pi.player_name;
        
        double player_time = pi.player_time;
        int time = Mathf.RoundToInt( (float)pi.player_time );
        int seconds = time % 60;
        int minutes = time / 60;
        string time_string = minutes.ToString("00") + ":" + seconds.ToString("00");

        player_timer_text.text = time_string;
    }

    // Update is called once per frame
    void Update()
    {
        bool isBound = game != null;
        if (isBound)
        {

            UpdateBoundDate();
        }
    }
}
