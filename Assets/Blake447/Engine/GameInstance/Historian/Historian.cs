using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Historian : MonoBehaviour
{
    //[SerializeField]
    //TMPro.TMP_Text text;

    [SerializeField]
    ArrowArray move_arrows;

    [SerializeField] TimelineArrow timelineArrow;

    public Turn head = null;
    public Turn tail = null;
    public Turn current = null;

    GameInstance game;
    HistoryLibrarian librarian;
    Archeologist archeologist;

    public GizmoVerse gizmoVerse;

    #region Double Linked List <Turn>


    private void TestGizmoVerse()
    {
        
        //GizmoVerse verse = GetComponent<GizmoVerse>();
        //this.gizmoVerse = verse;
        if (gizmoVerse != null)
        {
            gizmoVerse.ClearGizmoVerse();
            Turn turn = tail;
            while (turn != null)
            {
                Move move = turn.tail;
                while (move != null)
                {
                    gizmoVerse.AddMoveToHistory(move);
                    move = move.next;
                }
                turn = turn.next;
            }
            gizmoVerse.PrintTimelines();
        }
    }




    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //        PrintSerializedTurns();
    //}

    //public void PrintSerializedTurns()
    //{
    //    Turn turn = tail;
    //    while (turn != null)
    //    {
    //        int[] serialized = (int[])turn.Serialize()?.Clone();
    //        if (serialized != null)
    //        {
    //            string empty = "";
    //            for (int i = 0; i < serialized.Length; i++)
    //                empty += serialized[i] + " ";
    //        }
    //        turn = turn.next;
    //    }
    //}


    public UIWidgets.ListViewString listViewString;
    public void OnGameStateChanged(int[] players_joined, int[] player_types, bool inProgress)
    {
        if (librarian != null)
            librarian.UpdateGameState(players_joined, player_types, inProgress);



    }
    public void PushHistory()
    {
        if (librarian != null)
        {
            librarian.PushHistory();
        }
    }
    
    public void InitializeHistorian(GameInstance game)
    {
        this.game = game;
        librarian = FindObjectOfType<HistoryLibrarian>();
        if (librarian != null)
            librarian.InitializeLibrarian(game);
    }

    void OnHistoryUpdate(GameInstance game)
    {
        //History history = ConstructHistory();
        //BoardLoader.SaveGame(history, "test_game");
        //BoardLoader.LoadGame("test_game");
        //history.TestSerialization();
        
        //history.PrintHistory();
    }

    public void SetListView(History history)
    {
        if (listViewString != null && history != null)
        {
            listViewString.DataSource.Clear();
            string[] strings = history.HistoryStrings();
            for (int i = 0; strings != null && i < strings.Length; i++)
                listViewString.DataSource.Add(strings[i]);
        }
    }

    public void SetFromHistory(History history, bool pushToNetwork=false)
    {
        if (history != null)
        {
            Debug.Log("Constructing local history");
            History LocalHistory = ConstructHistory();
            StartCoroutine(SetFromHistoryRoutine( LocalHistory, history,pushToNetwork));

            //if (LocalHistory != null)
            //{
            //    for (int i = 0; i < LocalHistory.Turns.Count; i++)
            //        RevertTurn();
            //}
            //for (int j = 0; j < history.Turns.Count; j++)
            //{
            //    Turn proxy_sentinel = history.Turns[j];
            //    ApplyTurn(game, proxy_sentinel);
        }
    }
        //if (pushToNetwork)

    IEnumerator SetFromHistoryRoutine(History localHistory, History networkHistory, bool pushToNetwork)
    {
        float delay = 0.10f;
        if (networkHistory != null)
        {
            if (localHistory != null)
                for (int i = 0; i < localHistory.Turns.Count; i++)
                {
                    RevertTurn();
                    yield return new WaitForSeconds(delay);
                }
            for (int j = 0; j < networkHistory.Turns.Count; j++)
            {
                Turn proxy_sentinel = networkHistory.Turns[j];
                ApplyTurn(game, proxy_sentinel);
                yield return new WaitForSeconds(delay);
            }
            if (pushToNetwork)
                PushHistory();
        }
    }

    public void CalculatePlayersTurn()
    {
        if (head != null && head.tail == null && head.prev != null && head.prev.tail != null && head.prev.tail.tail != null)
        {
            int prev_color = head.prev.tail.tail.pfrom / 32;
            Debug.Log("Previous color: " + prev_color);
            game.CyclePlayerTurn(prev_color);
        }
        else
        {
            Debug.LogWarning("Failed to calculate players turn");
        }
    }

    public bool IsWritable()
    {
        return !(tail == null || tail.tail == null || tail.tail.tail == null);
    }


    public void SaveGame(string name)
    {
        if (IsWritable())
        {
            History history = ConstructHistory();
            BoardLoader.SaveGame(history, name);
        }
        else
        {
            Debug.LogError("No moves recorded to history to save");
        }
    }
    public void LoadGame(string name)
    {
        BoardLoader.LoadGame(name, this);
    }

    private void Update()
    {

    }


    public History ConstructHistory()
    {
        if (tail == null)
            return null;
        History history = new History();
        Turn turn = tail;
        while (turn != null)
        {
            history.AddTurn(turn);
            turn = turn.next;
        }
        SetListView(history);
        return history;
    }

    public void UpdateArrows(GameInstance game)
    {
        if (move_arrows != null)
        {
            Multiverse multiverse = game.GetMultiverse();

            move_arrows.ClearArrows();

            // TODO: Fix this, this is kind of hacky
            bool isTimeTravel = game.GetRuleSet().time_index < game.GetRuleSet().local_dimensions.Length;
            //Debug.Log("Time Travel? " + isTimeTravel);
            
            if (isTimeTravel)
            {
                int white_m =  1;
                int black_m = -1;


                Turn turnSentinel = this.tail;
                Move moveSentinel = null;
                while (turnSentinel != null)
                {
                    moveSentinel = turnSentinel.tail;
                    while (moveSentinel != null)
                    {
                        int[] from = moveSentinel.tail.from;
                        int[] to = moveSentinel.tail.to;

                        Vector3 vFrom = multiverse.CoordinateToPosition(from);
                        Vector3 vTo = multiverse.CoordinateToPosition(to);
                        move_arrows.SetArrow(vFrom, vTo);

                        moveSentinel = moveSentinel.next;
                    }
                    turnSentinel = turnSentinel.next;
                }
            }
            else
            {
                Turn turnSentinel = this.head;
                if (turnSentinel != null)
                {
                    Move moveSentinel = turnSentinel.tail;
                    if (moveSentinel == null)
                    {
                        turnSentinel = turnSentinel.prev;
                        moveSentinel = turnSentinel?.tail;
                    }
                    if (moveSentinel != null)
                    {
                        int[] from = moveSentinel.tail.from;
                        int[] to = moveSentinel.tail.to;

                        Vector3 vFrom = multiverse.CoordinateToPosition(from);
                        Vector3 vTo = multiverse.CoordinateToPosition(to);
                        move_arrows.SetArrow(vFrom, vTo);
                    }
                }
            }
        }
        TestGizmoVerse();
    }


    public void Add(Turn turn)
    {
        if (head == null)
        {
            head = turn;
            tail = turn;
            turn.next = null;
            turn.prev = null;
            current = tail;
        }
        else
        {
            head.next = turn;
            turn.prev = head;
            turn.next = null;
            head = turn;
        }
    }
    public bool Advance()
    {
        if (current == null)
            return false;
        if (current.next != null)
        {
            current = current.next;
            return true;
        }
        return false;
    }
    #endregion

    public bool RemoveTurn()
    {
        Turn turn = head;
        if (turn == null)
            return false;

        if (turn.prev != null)
        {
            head = turn.prev;
            head.next = null;
            turn.prev = null;
            turn.next = null;
            current = head;
        }
        else
        {
            turn.prev = null;
            turn.next = null;
            head = null;
            tail = null;
            current = null;
        }
        return true;
    }

    public bool Remove()
    {
        Turn turn = head;
        if (turn == null)
            return false;

        if (turn.head != null)
        {
            Move move = turn.head;
            if (move != null)
            {
                if (move.prev != null)
                {
                    turn.head = move.prev;
                    move.prev = null;
                    turn.head.next = null;
                    return true;
                }
                else
                {
                    move.prev = null;
                    turn.head = null;
                    turn.tail = null;
                    return true;
                }
            }
            else
                return false;
        }
        else
            return false;
    }

    public void ApplyTurn(GameInstance game, Turn turn)
    {
        RuleSet ruleSet = game.GetRuleSet();
        Multiverse multiverse = game.GetMultiverse();
        int player = game.GetPlayersTurn();
        Move sentinel = turn.tail;
        while (sentinel != null)
        {
            if (current == null)
            {
                Turn curr_turn = new Turn(sentinel);
                Add(turn);
                OnHistoryUpdate(game);
            }
            else
            {
                current.Add(sentinel);
                OnHistoryUpdate(game);
            }
            game.ApplyMove(sentinel);
            sentinel = sentinel.next;
        }
        multiverse.IndicateActiveBoards(player);
        game.PrintRoyaltyCoordinates();
        UpdateArrows(game);
        SubmitTurn(game);
    }

    public bool RevertTurn()
    {
        if (head != null)
        {
            Move lastMove = head.head;
            while (lastMove != null)
            {
                game.RevertMove(lastMove);
                lastMove = lastMove.prev;

                if (lastMove != null)
                {
                    game.RevertMove(lastMove);
                    game.ApplyMove(lastMove);
                }
            }
            return RemoveTurn();
        }
        return false;
    }

    public bool ProcessClickLocal(GameInstance game, Click click)
    {
        game.HighlightClear();
        RuleSet ruleSet = game.GetRuleSet();
        Multiverse multiverse = game.GetMultiverse();
        int player = game.GetPlayersTurn();
        Move move = ruleSet.CanMakeMove(multiverse, player, click);

        if (move != null)
        {
            if (current == null)
            {
                Turn turn = new Turn(move);
                Add(turn); 
                OnHistoryUpdate(game);
            }
            else
            {
                current.Add(move);
            }

            game.ApplyMove(move);
            multiverse.IndicateActiveBoards(player);
            game.PrintRoyaltyCoordinates();

            UpdateArrows(game);

            if (ruleSet.AutoSubmit)
            {
                SubmitTurn(game);
            }
            return true;
        }
        PrintHistory();
        return false;
    }
    public bool ProcessClick(GameInstance game, Click click)
    {
        if (librarian != null)
        {
            Multiverse multiverse = game.GetMultiverse();
            RuleSet ruleset = game.GetRuleSet();
            Move move = ruleset.CanMakeMove(multiverse, game.GetPlayersTurn(), click);
            if (move != null)
                librarian.ProcessClick(click);
            return move != null;
        }
        else
        {
            return ProcessClickLocal(game, click);
        }
    }


    public void HardSetMultiverse(Multiverse multiverse)
    {
        multiverse.HardReset();
        Turn turnSentinel = this.tail;
        Move moveSentinel = null;
        Command commandSentinel = null;
        while (turnSentinel != null)
        {
            moveSentinel = turnSentinel.tail;
            while (moveSentinel != null)
            {
                multiverse.ApplyMove(moveSentinel);
                moveSentinel = moveSentinel.next;
            }
            turnSentinel = turnSentinel.next;
        }
    }




    public void UndoMove(GameInstance game)
    {
        if (librarian != null)
        {
            librarian.UndoMove();
        }
        else
        {
            UndoLocal(game);
        }
        //game.OnStateUpdate();
        PrintHistory();
    }
    public void UndoLocal(GameInstance game)
    {
        if (current != null)
        {
            Turn turn = current;
            if (turn.head != null)
            {
                Move move = turn.head;
                game.RevertMove(move);
                game.PrintRoyaltyCoordinates();
                turn = current.prev;
                Remove();

                if (turn != null && turn.head != null)
                {
                    if (GetComponent<Singularity>())
                    {
                        move = turn.head;
                        game.RevertMove(move);
                        game.ApplyMove(move);
                        game.PrintRoyaltyCoordinates();
                    }
                }


                OnHistoryUpdate(game);
            }
        }
        UpdateArrows(game);
    }


    public void SubmitTurn(GameInstance game)
    {
        if (librarian != null && CanSubmitTurn(game))
        {
            librarian.SubmitTurn();
        }
        else
        {
            SubmitTurnLocal(game);
        }
    }
    public void SubmitTurnLocal(GameInstance game)
    {
        //game.SendHistory(ConstructHistory(), 0);
        if (CanSubmitTurn(game))
        {
            Turn turn_applied = current;
            Turn turn = new Turn();
            Add(turn);
            OnHistoryUpdate(game);
            Advance();
            Multiverse multiverse = game.GetMultiverse();
            int player = game.GetPlayersTurn();
            multiverse.IndicateActiveBoards(player);
            PrintHistory();
            History history = ConstructHistory();
            if (archeologist == null)
                archeologist = FindObjectOfType<Archeologist>();
            if (archeologist != null)
                archeologist.StoreHistory((int[])history.Serialized().Clone());

            //game.OnStateUpdate();
            RuleSet ruleSet = game.GetRuleSet();
            int player_from = 0;
            Command command = turn_applied?.head?.tail;
            if (command != null)
            {
                player_from = command.pfrom / 32;
                if (command.pfrom == 0)
                    Debug.LogWarning("Warning, submitting turn moving a blank square");
            }
            else
            {
                Debug.LogWarning("could not receive command from submitted turn");
            }
            game.CyclePlayerTurn(player_from);
        }
    }


    public bool CanSubmitTurn(GameInstance game)
    {
        if (current == null)
            return false;

        if (current.tail == null)
            return false;

        Multiverse multiverse = game.GetMultiverse();
        
        int currentPlayer = game.GetPlayersTurn();
        multiverse.IndicateActiveBoards(currentPlayer);
        int activeBoards = multiverse.CountPlayersActiveBoards(currentPlayer);

        if (game.IsPlayerInCheck(currentPlayer))
            return false;

        if (activeBoards > 0)
            return false;

        return true;
    }

    public void PrintHistory()
    {

        
        //int counter = 0;
        //Turn turn_sentinel = tail;
        //Move move_sentinel = null;
        //Command command_sentinel = null;

        //string output = "";

        //while (turn_sentinel != null && counter < 1000)
        //{   
        //    output += (counter.ToString("000") + ": Turn " + "\n");
        //    counter++;
        //    move_sentinel = turn_sentinel.tail;
        //    while (move_sentinel != null && counter < 1000)
        //    {
        //        output += (counter.ToString("000") + ": L_ Move " + "\n");
        //        counter++;
        //        command_sentinel = move_sentinel.tail;
        //        while (command_sentinel != null && counter < 1000)
        //        {
        //            output += (counter.ToString("000") + ": L___ Comm " + command_sentinel.CommandToString() + "\n");
        //            counter++;
        //            command_sentinel = command_sentinel.next;
        //        }
        //        move_sentinel = move_sentinel.next;
        //    }
        //    turn_sentinel = turn_sentinel.next;
        //}

        //if (text != null)
        //    text.text = output;
    }

}
