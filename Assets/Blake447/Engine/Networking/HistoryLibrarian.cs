using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Runtime.Serialization.Formatters.Binary;
public class HistoryLibrarian : MonoBehaviour
{
    public int players = 2;
    public GameInstance game;
    //public History[] histories;
    private PhotonView photonView;
    History proxyHistory;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        proxyHistory = new History();
    }
    public void InitializeLibrarian(GameInstance game)
    {
        this.game = game;
        game.SetPlayerLocalID(PhotonNetwork.LocalPlayer.GetHashCode());
    }

    public void RequestGameState()
    {
        photonView.RPC(nameof(RPC_RequestGameState), RpcTarget.MasterClient, new object[] { });
    }
    [PunRPC]
    public void RPC_RequestGameState()
    {
        Debug.Log("Remote player requesting updated game state, sending serialized history");
        if (game != null && game.GetHistorian() != null)
        {
            Historian historian = game.GetHistorian();
            if (historian.IsWritable())
            {
                History history = historian.ConstructHistory();
                int[] serialized = (int[])history.Serialized().Clone();
                photonView.RPC(nameof(RPC_SendHistory), RpcTarget.Others, new object[] { serialized });
                game.RequestPlayersJoined();
            }
        }
    }
    public void PushHistory()
    {
        if (game != null && game.GetHistorian() != null)
        {
            Historian historian = game.GetHistorian();
            if (historian.IsWritable())
            {
                History history = historian.ConstructHistory();
                int[] serialized = (int[])history.Serialized().Clone();
                photonView.RPC(nameof(RPC_SendHistory), RpcTarget.Others, new object[] { serialized });
                game.RequestPlayersJoined();
            }
        }
    }
    [PunRPC]
    public void RPC_SendHistory(int[] serialized)
    {
        Debug.Log("Remote client received history");
        Debug.Log(Coordinates.CoordinateToString(serialized));
        History history = new History();
        history.SetHistory(serialized);
        if (game == null)
            FindObjectOfType<GameInstance>();
        game.GetHistorian().SetFromHistory(history);
    }

    
    public void UpdateGameState(int[] players_joined, int[] player_types, bool inProgress)
    {
        photonView.RPC(nameof(RPC_UpdateGameState), RpcTarget.All, new object[] { players_joined, player_types, inProgress });
    }
    [PunRPC]
    public void RPC_UpdateGameState(int[] players_joined, int[] player_types, bool inProgress)
    {
        if (game == null)
            game = FindObjectOfType<GameInstance>();
        game.UpdateGameState(players_joined, player_types, inProgress);
    }




    public void SetGameState(int player1, int player2, int player_from, bool inProgress)
    {
        //game.SetPlayerSet(new int[] { player1, player2 }, false);
        //if (inProgress)
        //    game.StartGame();
        photonView.RPC(nameof(RPC_SetGameState), RpcTarget.All, new object[] { player1, player2, player_from, inProgress });
    }
    [PunRPC]
    public void RPC_SetGameState(int player1, int player2, int player_from, bool inProgress)
    {
        if (game == null)
            game = FindObjectOfType<GameInstance>();
        game.SetPlayerSet(new int[] { player1, player2 }, player_from);
        if (inProgress)
            game.StartGame();
    }

    public void ProcessClick(Click click)
    {
        int[] serialized = new int[5 + click.coord_from.Length + click.coord_to.Length];
        serialized[0] = click.player_turn;
        serialized[1] = click.piece_from;
        serialized[2] = click.piece_to;
        serialized[3] = click.coord_from.Length;
        serialized[4 + click.coord_from.Length] = click.coord_to.Length;
        System.Array.Copy(click.coord_from, 0, serialized, 4, click.coord_from.Length);
        System.Array.Copy(click.coord_to, 0, serialized, 5 + click.coord_from.Length, click.coord_to.Length);
        Debug.Log("Processing click: " + Coordinates.CoordinateToString(serialized));
        photonView.RPC(nameof(RPC_ProcessClick), RpcTarget.All, new object[] { serialized });

    }
    [PunRPC]
    public void RPC_ProcessClick(int[] serialized)
    {
        Click click = new Click();
        click.player_turn = serialized[0];
        click.piece_from = serialized[1];
        click.piece_to = serialized[2];
        click.coord_from = new int[serialized[3]];
        click.coord_to = new int[serialized[4 + click.coord_from.Length]];
        System.Array.Copy(serialized, 4, click.coord_from, 0, click.coord_from.Length);
        System.Array.Copy(serialized, 5 + click.coord_from.Length, click.coord_to, 0, click.coord_to.Length);
        Debug.Log("RPC_Processing click: " + Coordinates.CoordinateToString(serialized));
        Debug.Log("Coord from: " + Coordinates.CoordinateToString(click.coord_from));
        Debug.Log("Coord to: " + Coordinates.CoordinateToString(click.coord_to));

        game.GetHistorian().ProcessClickLocal(game, click);
    }

    public void SpawnGameMode(int prefab_index)
    {
        Debug.Log("Sending rpc SpawnGameMode(" + prefab_index + ")");
        photonView.RPC(nameof(RPC_SpawnGameMode), RpcTarget.AllBuffered, new object[] { prefab_index });
    }
    [PunRPC]
    public void RPC_SpawnGameMode(int prefab_index)
    {
        GameSpawner spawner = FindObjectOfType<GameSpawner>();
        if (spawner != null)
            spawner.SpawnGameModeLocal(prefab_index);
    }


    public void UndoMove()
    {
        photonView.RPC(nameof(RPC_UndoMove), RpcTarget.All, new object[] { });
    }
    [PunRPC]
    public void RPC_UndoMove()
    {
        game.GetHistorian().UndoLocal(game);
    }
    public void SubmitTurn()
    {
        photonView.RPC(nameof(RPC_SubmitTurn), RpcTarget.All, new object[] { });
    }
    [PunRPC]
    public void RPC_SubmitTurn()
    {
        game.GetHistorian().SubmitTurnLocal(game);
    }

}
