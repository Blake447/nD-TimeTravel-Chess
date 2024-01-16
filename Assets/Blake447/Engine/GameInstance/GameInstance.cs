using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo
{
    public string player_name;
    public int player_type;
    public double player_time;
};

public struct Click
{
    public int player_turn;
    public int piece_from;
    public int piece_to;
    public int[] coord_from;
    public int[] coord_to;
}

public class GameInstance : MonoBehaviour
{
    int localPlayerID;
    const int max_players = 2;
    [SerializeField]
    private string filename;
    Historian historian;
    Multiverse multiverse;
    RuleSet ruleSet;
    AI ai;
    PiecePallete pallete;
    BoardState boardState;
    TimelineArrow timelineArrow;
    //[SerializeField] int[] dimensions;
    //[SerializeField] HistoryLibrarian librarian;
    //[SerializeField] GameDescriptor gameDescriptor;
    [SerializeField] ArrowArray arrowArray;
    GameLoader gameLoader;

    public UnityEngine.UI.Button undo_button;

    int current_players_turn;
    bool isGameInProgress = false;
    bool isInitialized = false;
    string[] player_names;
    int[] player_types;
    int[] players_joined;
    double[] player_timers;
    double[] timer_settings = new double[7] { 0.0, 10.0, 30.0, 60.0, 90.0, 120.0, 180.0 };
    int timer_set = 0;
    bool useTimer = false;
    double start_time = 0.0f;
    double rfrsh_time =  1.0f * 30.0f;
    bool isMaster = true;
    string[] move_history;
    Move stack_head;

    const int PLAYER_NONE = 0;
    const int PLAYER_LOCAL = 1;
    const int PLAYER_REMOTE = 2;
    const int PLAYER_AI = 3;

    int local_client_id = 0;
    int[] lastPlayerClicked = new int[2] { -1, -1 };


    public ToggleArray accordion_white;
    public ToggleArray accordion_black;
    public ToggleArray currentTurn;

    public void SaveGame(string name)
    {
        if (isGameInProgress)
        {
            Historian historian = GetComponent<Historian>();
            if (historian != null)
                historian.SaveGame(name);
            else
                Debug.LogError("Could not find historian");
        }
        else
        {
            Debug.LogError("Unable to save game, game instance not yet progress");
        }
    }
    public void LoadGame(string name)
    {
        if (!isGameInProgress)
        {
            Historian historian = GetComponent<Historian>();
            if (historian != null)
                historian.LoadGame(name);
        }
        else
        {
            Debug.LogError("Unable to load game, game instance already in progress");
        }
    }



    public void SetPlayerLocalID(int id)
    {
        localPlayerID = id;
    }
    public int GetPlayerLocalID()
    {
        return localPlayerID;
    }
    public string CalculateHash()
    {
        RuleSet ruleset = GetComponent<RuleSet>();
        int[] dimensions = ruleset.local_dimensions;
        int[] primes = {3, 5, 7, 11, 13, 17, 19, 23};
        int hash = 1;
        for (int i = 0; i < Mathf.Min(dimensions.Length, primes.Length); i++)
        {
            hash += primes[i] * dimensions[i];
        }
        return filename + hash.ToString();
    }

    //public void SetGameState(History history, int current_player_turn)
    //{
    //    historian.SetFromHistory(history);
    //    this.current_players_turn = current_player_turn;
    //    UpdatePlayerTurn();
    //}


    public void UpdatePlayerTurn()
    {
        if (currentTurn != null)
        {
            bool isInProgress = IsGameInProgress();
            if (!isInProgress)
            {
                currentTurn.EnablePanels(new int[] { 0 });
            }
            else
            {
                int player_turn = GetPlayersTurn();
                currentTurn.EnablePanels(new int[] { player_turn + 1 });
            }
        }
    }

    public void UpdateGameState(int[] players_joined, int[] player_types, bool inProgress)
    {
        if (players_joined != null)
            this.players_joined = (int[])players_joined.Clone();

        for (int i = 0; i < players_joined.Length; i++)
        {
            if (players_joined[i] == localPlayerID)
                this.player_types[i] = PLAYER_LOCAL;
            else if (player_types[i] == PLAYER_AI)
                this.player_types[i] = PLAYER_AI;
            else if (player_types[i] == PLAYER_NONE)
                this.player_types[i] = PLAYER_NONE;
            else
                this.player_types[i] = PLAYER_REMOTE;
        }
        SetColorTypes();
        if (inProgress)
            StartGame();
    }

    public void SetPlayerSet(int[] set, int player_from)
    {
        if (set != null)
        {
            bool remoteCall = localPlayerID != player_from;
            if (remoteCall)
            {
                for (int i = 0; i < Mathf.Min(set.Length, player_types.Length); i++)
                    if (set[i] == PLAYER_LOCAL)
                        player_types[i] = PLAYER_REMOTE;
                    else if (player_types[i] != PLAYER_LOCAL)
                        player_types[i] = set[i];
                SetColorTypes();
            }
            else
            {
                for (int i = 0; i < Mathf.Min(set.Length, player_types.Length); i++)
                    if (player_types[i] != PLAYER_LOCAL)
                        player_types[i] = set[i];
                SetColorTypes();
            }
        }
    }
    public int[] GetPlayerSet()
    {
        return (int[])player_types.Clone();
    }



    public void ClickWhite(int player)
    {
        if (player_types == null)
            player_types = new int[2];


        if (!isGameInProgress)
        {
            int white = 0;
            int color = white;
            if (player_types[color] == PLAYER_NONE)
            {
                lastPlayerClicked[color] = player;
                player_types[color] = PLAYER_LOCAL;
                players_joined[color] = localPlayerID;
            }
            else if (player_types[color] == PLAYER_LOCAL)
            {
                player_types[color] = PLAYER_AI;
                players_joined[color] = -1;
            }
            else if (player_types[color] == PLAYER_AI)
            {
                player_types[color] = PLAYER_NONE;
                players_joined[color] = -1;
            }
            SetColorTypes();
        }
        historian.OnGameStateChanged(players_joined, player_types, isGameInProgress);
    }
    public void ClickBlack(int player)
    {
        if (player_types == null)
            player_types = new int[2];

        if (!isGameInProgress)
        {
            int black = 1;
            int color = black;
            if (player_types[color] == PLAYER_NONE)
            {
                lastPlayerClicked[color] = player;
                player_types[color] = PLAYER_LOCAL;
                players_joined[color] = localPlayerID;
            }
            else if (player_types[color] == PLAYER_LOCAL)
            {
                player_types[color] = PLAYER_AI;
                players_joined[color] = -1;
            }
            else if (player_types[color] == PLAYER_AI)
            {
                player_types[color] = PLAYER_NONE;
                players_joined[color] = -1;
            }
            SetColorTypes();
        }
        historian.OnGameStateChanged(players_joined, player_types, isGameInProgress);
    }
    public int GetPlayerType(int color)
    {
        return player_types[color];
    }
    public void SetColorTypes()
    {
        int whiteType = GetPlayerType(0);
        int blackType = GetPlayerType(1);

        accordion_white.EnablePanels(new int[] {whiteType});
        accordion_black.EnablePanels(new int[] {blackType});
    }


    // Game Scripts
    public Multiverse GetMultiverse()
    {
        return multiverse;
    }
    public Historian GetHistorian()
    {
        return historian;
    }
    public RuleSet GetRuleSet()
    {
        return ruleSet;
    }


    // Game state
    public bool StartGame()
    {
        Debug.Log("Start Game");
        if (IsGameReady())
        {
            SetPlayerStartTimers();
            historian.CalculatePlayersTurn();
            //current_players_turn = 0;
            isGameInProgress = true;
            OnGameStart();
            UpdatePlayerTurn();
            BoardTutorial tutorial = FindObjectOfType<BoardTutorial>();
            if (tutorial != null)
                tutorial.OnGameStart();
            return true;
        }
        return false;
    }
    public void StartGamePublic()
    {
        StartGame();
        historian.OnGameStateChanged(players_joined, player_types, isGameInProgress);
    }
    public void RequestPlayersJoined()
    {
        historian.OnGameStateChanged(players_joined, player_types, isGameInProgress);
    }
    public void SetPlayerStartTimers()
    {
        for (int i = 0; i < player_timers.Length; i++)
            player_timers[i] = start_time * 60.0;
    }
    // player settings
    public void ChangeTimerAmount(int player_index)
    {
        if (!IsGameInProgress())
        {
            int player_type = player_types[player_index];
            if (isMaster)
            {
                timer_set++;
                timer_set = timer_set % timer_settings.Length;
                start_time = timer_settings[timer_set];
                useTimer = timer_set != 0;
                SetPlayerStartTimers();
            }
        }
    }
    // Game state
    public bool IsGameReady()
    {
        if (isGameInProgress)
            return false;
        if (!isInitialized)
            return false;
        if (!ArePlayersReady())
            return false;
        return true;
    }
    public bool ArePlayersReady()
    {
        return player_types[0] != PLAYER_NONE && player_types[1] != PLAYER_NONE;
    }

    // game state
    public void CyclePlayerTurn(int current_players_turn)
    {

        this.current_players_turn = (current_players_turn + 1) % max_players;
        UpdatePlayerTurn();
    }

    // Game state
    public void GameLoop()
    {
        DecrementLocalTurnTimer();
    }
    // Game state
    public void DecrementLocalTurnTimer()
    {
        if (useTimer && player_types[current_players_turn] == PLAYER_LOCAL)
            player_timers[current_players_turn] -= Time.deltaTime;
    }

    public void CyclePlayerType(int player_index)
    {
        int player_type = player_types[player_index];
        if (player_type != PLAYER_REMOTE)
        {
            int next_type = PLAYER_NONE;
            switch (player_type)
            {
                case PLAYER_NONE:
                    next_type = PLAYER_LOCAL;
                    break;
                case PLAYER_LOCAL:
                    next_type = PLAYER_AI;
                    break;
                case PLAYER_AI:
                    next_type = PLAYER_NONE;
                    break;
                default:
                    break;
            }
            player_types[player_index] = next_type;
        }
    }


    #region Events
    public void OnGameStart()
    {
        if (player_types[current_players_turn] == PLAYER_AI)
            ai.StartAISearch(current_players_turn);
    }
    public void OnTurnSubmitted()
    {
        if (player_types[current_players_turn] == PLAYER_AI)
            ai.StartAISearch(current_players_turn);
    }
    #endregion

    #region Historian Communication

    // Historian communication
    public void ApplyTurn(Turn turn)
    {
        historian.ApplyTurn(this, turn);
    }

    // historian comm
    public void SubmitTurn()
    {
        if (player_types[GetPlayersTurn()] == PLAYER_LOCAL)
        {
            historian.SubmitTurn(this);
            OnTurnSubmitted();
        }
    }

    #endregion


    // multiverse communication
    public void ApplyMove(Move move)
    {
        multiverse.ApplyMove(move);
    }
    public void RevertMove(Move move)
    {
        multiverse.RevertMove(move);
    }

    // Game analysis
    public bool IsPlayerInCheck(int player)
    {
        int[][] royalty_coordinates = (int[][])multiverse.GetRoyalty().Clone();
        
        if (royalty_coordinates == null)
            return false;

        for (int i = 0; i < royalty_coordinates.Length; i++)
        {
            int piece = multiverse.GetPieceAt(royalty_coordinates[i]);
            if (piece != 0 && piece / 32 == player)
            {
                if (IsCoordinateInCheck(royalty_coordinates[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Game analysis
    public bool IsCoordinateInCheck(int[] coordinate)
    {
        return ruleSet.GetCheckingCoordinates(multiverse, coordinate) != null;
    }

    // Analysis
    public void HighlightMoves(int[] coordinate)
    {
        multiverse.HighlightClear();
        if (coordinate != null)
        {
            int[][] valid_moves = ruleSet.CreateTargetCoords(multiverse, coordinate);
            if (valid_moves != null)
            {
                multiverse.HightlightMoves((int[][])valid_moves.Clone()); ;
            }
        }
    }
    // Analysis /  Visualization
    public void HighlightPossibleChecking(int[] coordinate)
    {
        Vector3 kingPosition = multiverse.CoordinateToPosition(coordinate);
        int[][] checking_coordinates = ruleSet.GetCheckingCoordinates(multiverse, coordinate);
        if (checking_coordinates != null)
        {
            for (int i = 0; i < checking_coordinates.Length; i++)
            {
                Vector3 fromPosition = multiverse.CoordinateToPosition(checking_coordinates[i]);
                arrowArray.SetArrow(fromPosition, kingPosition);
            }
        }
    }

    // Visualization
    public void HighlightClear()
    {
        multiverse.HighlightClear();
    }

    // Direct commands for the game
    #region Game Commands

    // Game Analysis *highlights possible checks, misnamed*
    public void PrintRoyaltyCoordinates()
    {
        int[][] royalty = (int[][])multiverse.GetRoyalty().Clone();
        arrowArray.ClearArrows();
        if (royalty != null)
        {
            for (int i = 0; i < royalty.Length; i++)
            {
                HighlightPossibleChecking(royalty[i]);
                if (undo_button != null)
                    undo_button.interactable = true;
            }
        }
        else if (undo_button != null)
            undo_button.interactable = false;
    }


    // Players State / Game State


    // player settings







    #endregion
    // Flagging coordinates for highlight and valid moves
    #region Board Flagging

    #endregion
    // Getter methods
    #region Getter Methods



    // Player system
    public PlayerInfo GetPlayerInfo(int player_index)
    {
        PlayerInfo pi;
        if (isInitialized)
        {
            pi.player_type = player_types[player_index];
            pi.player_time = player_timers[player_index];
            if (player_types[player_index] == PLAYER_LOCAL)
                pi.player_name = "Local";
            else if (player_types[player_index] == PLAYER_REMOTE)
                pi.player_name = "Remote";
            else if (player_types[player_index] == PLAYER_AI)
                pi.player_name = "AI";
            else
                pi.player_name = "Join";
        }
        else
        {
            pi.player_type = PLAYER_NONE;
            pi.player_time = 0.0f;
            pi.player_name = "Join";
        }
        return pi;
    }

    // Game state
    public bool IsGameInProgress()
    {
        return isGameInProgress;
    }
    // Game state

    public int GetPlayersTurn()
    {
        return current_players_turn;
    }
    #endregion

    // Handling move changes
    #region Move Handling
    public bool ProcessClick(int[] coord_from, int[] coord_to)
    {
        //bool foundBoards = !(board_from == null || board_to == null);
        bool foundBoards = true;

        bool isPlayersTurn = player_types[current_players_turn] == PLAYER_LOCAL;

        if (isPlayersTurn && foundBoards)
        {
            Click click = new Click();
            click.player_turn = current_players_turn;
            click.piece_from = multiverse.GetPieceAt(coord_from);
            click.piece_to = multiverse.GetPieceAt(coord_to);
            click.coord_from = (int[])coord_from.Clone();
            click.coord_to = (int[])coord_to.Clone();
            bool didClick = historian.ProcessClick(this, click);
            return didClick;
        }
        else
            multiverse.HighlightClear();
        return false;
    }
    #endregion

    // Standard methods (awake, update, etc)
    #region Standard Methods
    void Awake()
    {
        //InitializeGame();
    }
    void Update()
    {
        if (IsGameInProgress())
            GameLoop();
    }
    #endregion

    // Initialization and Resetting
    #region Setup
    public void InitializeGame(GameLoader gameLoader, GameDescriptor gameDescriptor)
    {
        this.filename = gameDescriptor.filename;
        this.gameLoader = gameLoader;
        pallete = GetComponent<PiecePallete>();
        multiverse = GetComponent<Multiverse>();
        ruleSet = GetComponent<RuleSet>();
        historian = GetComponent<Historian>();

        ruleSet.local_dimensions = (int[])gameDescriptor.dimensions.Clone();
        ruleSet.AutoSubmit = !gameDescriptor.isTimeTravel;
        ruleSet.useForwardLateralRule = gameDescriptor.useForwardLateral;
        ruleSet.time_index = gameDescriptor.timeIndex;
        multiverse.SetTemplateNode(gameDescriptor.board);

        boardState = BoardLoader.LoadCustomBoardState(filename);
        int piece_count = 1;
        for (int i = 0; i < ruleSet.local_dimensions.Length; i++)
            piece_count *= ruleSet.local_dimensions[i];
        if (boardState == null)
            boardState = new BoardState("empty", ruleSet.local_dimensions, new int[piece_count], null);
        multiverse.InitializeMultiverse(pallete, boardState.board_state);
        //pallete.InitializePiecePallete(ruleSet.local_dimensions);
        ruleSet.InitializeRuleSet(pallete);
        historian.InitializeHistorian(this);

        Debug.Log("Initialize Game");

        int max_players = 2;

        if (gameDescriptor.isTimeTravel)
        {
            MVTime mvtime = GetComponent<MVTime>();
            if (mvtime != null)
                mvtime.SetOffsets(gameDescriptor.multiverse_offset, gameDescriptor.timetravel_offset);
        }

        Board board = multiverse.GetTemplateNode().board;
        player_names = new string[max_players];
        player_types = new int[max_players];
        players_joined = new int[max_players];
        player_timers = new double[max_players];
        for (int i = 0; i < max_players; i++)
        {
            players_joined[i] = -1;
            player_names[i] = "";
            player_types[i] = PLAYER_NONE;
            player_timers[i] = start_time;
        }
        isInitialized = true;
    }
    #endregion

    // Debugging methods
    #region Debugging

    #endregion

}
