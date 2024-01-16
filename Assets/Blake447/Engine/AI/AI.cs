using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{

    [SerializeField]
    GameInstance game;
    AINode rootNode;
    [SerializeField]
    Multiverse simulatorMultiverse;



    bool[] AI_Players = new bool[] { true, false, false, false };
    
    bool isInProgress = false;
    [SerializeField]
    GameObject IsThinkingObject;



    int[] piece_values = new int[] { 0, 1000000, 500, 200, 100, 150, 20, 0, 0, 0, 0, 0, 0, 20, 0, 0 };

    public bool isThinking = false;


    //Move[][] pair_buffer;
    //Move[][] sing_buffer;

    void DebugLogCoreFunctions()
    {
        Debug.Log("Testing core functions");
        Debug.Log("Generating possibilities 6 choose 3");
        int[][] fourChooseTwo = GenerateUnpairedCombos(6, 3);
        for (int i = 0; i < fourChooseTwo.Length; i++)
            Debug.Log(Coordinates.CoordinateToString(fourChooseTwo[i]));
        int[] numbers = GenerateSequentialNumbers(10);
        int[] even_indices = new int[] { 0, 2, 4, 6, 8 };
        Debug.Log("Testing Array ops on " + Coordinates.CoordinateToString(numbers));
        int[] evens = ExtractIndices(numbers, even_indices);
        Debug.Log("Even numbers: " + Coordinates.CoordinateToString(evens));
        int[] odds = RemoveIndices(numbers, even_indices);
        Debug.Log("Odd numbers: " + Coordinates.CoordinateToString(odds));
        int[] both = SpliceArrays(evens, odds);
        Debug.Log("Even then odds: " + Coordinates.CoordinateToString(both));
        int[] remap = { 0, 5, 1, 6, 2, 7, 3, 8, 4, 9 };
        Debug.Log("Remapping back in order");
        int[] resorted = RemapIndices(both, remap);
        Debug.Log("Remapped: " + Coordinates.CoordinateToString(resorted));
        Debug.Log("Testing combination index");
        int n = 8;
        int[][] eightChooseTwo = GenerateUnpairedCombos(8, 2);
        for (int i = 0; i < eightChooseTwo.Length; i++)
        {
            Debug.Log(GenerateComboIndex(eightChooseTwo[i][0], eightChooseTwo[i][1], n).ToString("00") + ": " + Coordinates.CoordinateToString(eightChooseTwo[i]));
        }
    }


    public void StartAISearch(int player_turn)
    {
        isThinking = true;
        if (IsThinkingObject != null)
            IsThinkingObject.SetActive(true);
        ConstructTurnTree(player_turn);
    }

    void ConstructTurnTree(int player_turn)
    {
        MirrorGameMultiverse();
        ClearTree();
        AINode current_node = rootNode;
        StartCoroutine( ConstructTree(current_node, player_turn, 1) );
    }

    IEnumerator ConstructTree(AINode current_node, int player_turn, int depth)
    {
        yield return GenerateTurns(current_node, player_turn);
        if (depth > 0)
        {
            if (current_node.principleChild != null)
            {
                AINode previous = null;
                AINode sentinel = current_node.principleChild;
                while (sentinel != null)
                {
                    if (previous != null)
                        simulatorMultiverse.RevertTurn(previous.turn);
                    simulatorMultiverse.ApplyTurn(sentinel.turn);
                    yield return ConstructTree(sentinel, (player_turn + 1) % 2, depth - 1);
                    previous = sentinel;
                    sentinel = sentinel.next;
                }
                if (previous != null)
                    simulatorMultiverse.RevertTurn(previous.turn);
            }
        }
        if (current_node.parent == null)
            OnTreeConstructed(player_turn);
    }
    void OnTreeConstructed(int player_turn)
    {
        int node_score = ScoreNode(rootNode, player_turn);
        List<AINode> good_moves = new List<AINode>();
        AINode sentinel = rootNode.principleChild;
        while (sentinel != null)
        {
            if (sentinel.score == node_score)
                good_moves.Add(sentinel);
            sentinel = sentinel.next;
        }
        AINode[] moves = good_moves.ToArray();
        Turn turn = moves[Random.Range(0, moves.Length)].turn;
        game.ApplyTurn(turn);
        isThinking = false;
        if (IsThinkingObject != null)
            IsThinkingObject.SetActive(false);
    }
    int ScoreNode(AINode node, int player_turn)
    {
        int node_score = 0;
        if (node.turn != null)
        {
            node_score += EvaluateTurn(node.turn);
        }
        if (node.principleChild != null)
        {
            bool isMaximizing = IsMaximizing(player_turn);
            AINode sentinel = node.principleChild;
            int child_score = 0;
            if (isMaximizing)
                while (sentinel != null)
                {
                    child_score = Mathf.Max(child_score, ScoreNode(sentinel, (player_turn + 1) % 2));
                    sentinel = sentinel.next;
                }
            else
                while (sentinel != null)
                {
                    child_score = Mathf.Min(child_score, ScoreNode(sentinel, (player_turn + 1) % 2));
                    sentinel = sentinel.next;
                }
            node_score += child_score;
        }

        node.score = node_score;
        return node_score;
    }
    bool IsMaximizing(int player_turn)
    {
        return player_turn == 0;
    }


    int EvaluateTurn(Turn turn)
    {
        Move move = turn.tail;
        if (move != null)
        {
            Command command = move.tail;
            if (command != null)
            {
                return piece_values[(command.pto % 32)] * (IsMaximizing(command.pto / 32) ? -1 : 1);
            }
        }
        return 0;
    }


    void OnGenerateTurnsComplete(AINode current_node, int player_turn)
    {
        int player_count = 2;
        player_turn = (player_turn + 1) % player_count;
        if (current_node.principleChild != null)
        {
            StartCoroutine( CycleAllTurns(current_node.principleChild) );
        }
    }
    IEnumerator CycleAllTurns(AINode starting_node)
    {
        AINode previous_node = null;
        AINode sentinel_node = starting_node;
        int counter = 0;
        while (sentinel_node != null)
        {
            if (previous_node != null)
                simulatorMultiverse.RevertTurn(previous_node.turn);
            simulatorMultiverse.ApplyTurn(sentinel_node.turn);
            counter++;
            previous_node = sentinel_node;
            sentinel_node = sentinel_node.next;
            yield return null;
        }
        if (previous_node != null)
            simulatorMultiverse.RevertTurn(previous_node.turn);
    }


    IEnumerator GenerateTurns(AINode current_node, int player_turn)
    {
        bool hasFailedEarly = false;

        MVNode[] playable_nodes = null;
        if (!hasFailedEarly)
        {
            playable_nodes = simulatorMultiverse.GetPlayableNodes(player_turn);
            if (playable_nodes == null)
                hasFailedEarly = true;
        }

        int board_count = 0;
        Move[] move_list = null;
        if (!hasFailedEarly)
        {
            board_count = playable_nodes.Length;
            move_list = GetPlayableMoves(playable_nodes, player_turn);
            if (move_list == null)
                hasFailedEarly = true;
        }

        Move[][] pair_buffer = null;
        Move[][] sing_buffer = null;
        int[][][] turn_combination_seq = null;
        if (!hasFailedEarly)
        {
            pair_buffer = GeneratePairBuffer(board_count);
            sing_buffer = GenerateSingBuffer(board_count);
            StoreMovesIntoBuffers(playable_nodes, move_list, pair_buffer, sing_buffer, board_count);
            turn_combination_seq = GenerateTurnCombinations(board_count);
            if (turn_combination_seq == null)
                hasFailedEarly = true;
        }

        int yield_time = 65536;
        //int yield_time = 8912;
        int yield_timer = yield_time;
        if (!hasFailedEarly)
        {
            for (int turn_seq = 0; turn_seq < turn_combination_seq.Length; turn_seq++)
            {
                int[] paired_seq = turn_combination_seq[turn_seq][0];
                int[] single_seq = turn_combination_seq[turn_seq][1];
                int paired_count = paired_seq == null ? 0 : paired_seq.Length / 2;
                int single_count = single_seq == null ? 0 : single_seq.Length;

                int[] current_paired_moves = paired_count > 0 ? new int[paired_count] : null;
                int[] current_single_moves = single_count > 0 ? new int[single_count] : null;

                int[] maximum_paired_moves = paired_count > 0 ? new int[paired_count] : null;
                int[] maximum_single_moves = single_count > 0 ? new int[single_count] : null;

                bool hasFailed = false;
                for (int i = 0; i < paired_count; i++)
                {
                    int board_from = paired_seq[2 * i];
                    int board_to = paired_seq[2 * i + 1];
                    int index = GenerateComboIndex(board_from, board_to, board_count);
                    if (pair_buffer[index] != null)
                        maximum_paired_moves[i] = pair_buffer[index].Length;
                    else
                        hasFailed = true;
                }
                for (int i = 0; i < single_count; i++)
                {
                    if (sing_buffer[i] != null)
                        maximum_single_moves[i] = sing_buffer[i].Length;
                    else
                        hasFailed = true;
                }
                while (!hasFailed && (current_paired_moves == null || current_paired_moves[0] >= 0))
                {
                    if (single_count > 0)
                        System.Array.Clear(current_single_moves, 0, current_single_moves.Length);
                    while ( current_single_moves == null || current_single_moves[0] >= 0)
                    {
                        Turn turn = new Turn();
                        string turnGenerateingSequence = Coordinates.CoordinateToString(current_paired_moves) + ":" + Coordinates.CoordinateToString(current_single_moves);
                        for (int i = 0; i < paired_count; i++)
                        {
                            int index = GenerateComboIndex(paired_seq[2 * i], paired_seq[2 * i + 1], board_count);
                            Move move = pair_buffer[index][current_paired_moves[i]];
                            turn.Add(move);
                        }
                        for (int i = 0; i < single_count; i++)
                        {
                            int index = single_seq[i];
                            Move move = sing_buffer[index][current_single_moves[i]];
                            turn.Add(move);
                        }
                        //Debug.Log(turnGenerateingSequence + " -> " + turn.TurnToString());
                        if (turn.tail != null)
                        {
                            AINode node = new AINode(turn);
                            ParentNode(node, current_node);
                        }

                        if (current_single_moves == null)
                            current_single_moves = new int[] { -1 };
                        else
                            current_single_moves = IncrementArray(current_single_moves, maximum_single_moves);

                        yield_timer--;
                        if (yield_timer <= 0)
                        {
                            yield_timer = yield_time;
                            yield return null;
                        }

                    }
                    if (current_paired_moves == null)
                        current_paired_moves = new int[] { -1 };
                    else
                        current_paired_moves = IncrementArray(current_paired_moves, maximum_paired_moves);
                    
                    yield_timer--;
                    if (yield_timer <= 0)
                    {
                        yield_timer = yield_time;
                        yield return null;
                    }
                }
            }
            //OnGenerateTurnsComplete(current_node, player_turn);
        }
    }
    void MirrorGameMultiverse()
    {
        Historian historian = game.GetHistorian();
        historian.HardSetMultiverse(simulatorMultiverse);
    }
    int[][][] GenerateTurnCombinations(int board_count)
    {
        List<int[][]> TurnCombinations = new List<int[][]>();
        int max_pair_count = board_count / 2;
        int[][][] unpaired_seq = new int[max_pair_count + 1][][];
        int[][][] pairing_seq = new int[max_pair_count + 1][][];
        for (int pairs = 1; pairs <= max_pair_count; pairs++)
        {
            // generate ways to choose b - 2p items to leave unpaired
            unpaired_seq[pairs] = GenerateUnpairedCombos(board_count, board_count - pairs * 2);
            // generate unique ways to pair 2p items
            pairing_seq[pairs] = GeneratePairCombinations(pairs);
        }
        unpaired_seq[0] = new int[1][];
        unpaired_seq[0][0] = GenerateSequentialNumbers(board_count);
        for (int pair_count = 0; pair_count <= max_pair_count; pair_count++)
        {
            int[] boards = GenerateSequentialNumbers(board_count);
            int unpaired_combination_count = unpaired_seq[pair_count] == null ? 1 : unpaired_seq[pair_count].Length;
            for (int unpaired_combination = 0; unpaired_combination < unpaired_combination_count; unpaired_combination++)
            {
                int[] unpaired_boards = null;
                if (unpaired_seq[pair_count] != null)
                    unpaired_boards = ExtractIndices(boards, unpaired_seq[pair_count][unpaired_combination]);

                int[] pairable_boards = null;
                if (unpaired_seq[pair_count] != null)
                    pairable_boards = RemoveIndices(boards, unpaired_seq[pair_count][unpaired_combination]);
                else
                    pairable_boards = (int[])boards.Clone();

                int paired_combination_count = pairing_seq[pair_count] == null ? 1 : pairing_seq[pair_count].Length;
                for (int current_paired_seq = 0; current_paired_seq < paired_combination_count; current_paired_seq++)
                {
                    int[] paired_boards = null;
                    if (pairing_seq[pair_count] != null)
                        paired_boards = RemapIndices(pairable_boards, pairing_seq[pair_count][current_paired_seq]);

                    // int[] turn_node_sequence = SpliceArrays(paired_boards, unpaired_boards);
                    int[][] turn_seq = new int[2][];
                    turn_seq[0] = paired_boards;
                    turn_seq[1] = unpaired_boards;
                    TurnCombinations.Add(turn_seq);
                }
            }
        }
        if (TurnCombinations.Count > 0)
            return TurnCombinations.ToArray();
        return null;
    }
    int[][] GenerateUnpairedCombos(int board_count, int unpaired_board_count)
    {
        if (unpaired_board_count == 0)
            return null;

        int n = board_count;
        int c = unpaired_board_count;

        List<int[]> combinations = new List<int[]>();
        int[] combination = GenerateSequentialNumbers(c);
        int[] maximums = GenerateSequentialNumbers(c, n - c);
        int index = combination.Length - 1;
        while (combination[0] <= maximums[0] && index >= 0)
        {
            for (int i = 0; (index + i) < combination.Length; i++)
                combination[index + i] = combination[index] + i;
            combinations.Add((int[])combination.Clone());

            index = combination.Length - 1;
            combination[index]++;
            while (index >= 0 && combination[index] > maximums[index])
            {
                index--;
                if (index >= 0)
                    combination[index]++;
            }
        }
        if (combinations.Count > 0)
            return combinations.ToArray();
        return null;
    }
    int[][] GeneratePairCombinations(int pair_count)
    {
        int board_count = pair_count * 2;
        if (board_count > 1)
        {
            int[] choices = new int[board_count / 2];
            int[] choice_max = new int[board_count / 2];
            int product = 1;
            for (int i = 0; i < choice_max.Length; i++)
            {
                choice_max[i] = board_count - 1 - 2 * i;
                product *= choice_max[i];
                choices[i] = 1;
            }
            int[][] generating_strings = new int[product][];
            int[][] generated_pairs = new int[product][];

            int counter = 0;
            while (choices[0] <= choice_max[0])
            {
                generating_strings[counter] = (int[])choices.Clone();
                counter++;
                int index = choices.Length - 1;
                choices[index]++;
                while (choices[index] > choice_max[index] && index > 0)
                {
                    choices[index] = 1;
                    index--;
                    choices[index]++;
                }
            }
            for (int i = 0; i < generating_strings.Length; i++)
            {
                int[] accumulator = new int[board_count];
                int[] generated = new int[board_count];
                for (int j = 0; j < generating_strings[i].Length; j++)
                {
                    int first = 0;
                    int index = 0;
                    int number = 0;
                    while (number <= 0)
                    {
                        if (accumulator[index] == 0)
                            number++;
                        first = index;
                        index++;
                    }
                    int second = 0;
                    index = 0;
                    number = 0;
                    while (number <= generating_strings[i][j])
                    {
                        if (accumulator[index] == 0)
                            number++;
                        second = index;
                        index++;
                    }

                    generated[2 * j] = first;
                    generated[2 * j + 1] = second;

                    accumulator[first] = 1;
                    accumulator[second] = 1;
                }
                generated_pairs[i] = generated;
            }
            return generated_pairs;
        }
        return null;
    }
    Move[] GetPlayableMoves(MVNode[] playable_nodes, int player_turn)
    {
        RuleSet ruleSet = game.GetRuleSet();
        List<Move> move_list = new List<Move>();

        for (int i = 0; i < playable_nodes.Length; i++)
        {
            MVNode node = playable_nodes[i];
            int[][] pieces_to_move = GetMovablePieceCoordinates(node, player_turn);
            if (pieces_to_move != null)
            {
                for (int piece_coord = 0; piece_coord < pieces_to_move.Length; piece_coord++)
                {
                    int[] coord_from = pieces_to_move[piece_coord];
                    int[][] coords_to = GetPossibleMoveLocations(coord_from);
                    if (coords_to != null)
                    {
                        for (int target_coord = 0; target_coord < coords_to.Length; target_coord++)
                        {
                            if (coords_to[target_coord] != null)
                            {
                                Click click = new Click();
                                click.coord_from = coord_from;
                                click.coord_to = coords_to[target_coord];
                                click.piece_from = simulatorMultiverse.GetPieceAt(coord_from);
                                click.piece_to = simulatorMultiverse.GetPieceAt(coords_to[target_coord]);
                                Move move = ruleSet.CanMakeMove(simulatorMultiverse, player_turn, click);
                                move_list.Add(move);
                            }
                        }
                    }
                }
            }
        }
        if (move_list.Count > 0)
            return move_list.ToArray();
        return null;
    }
    int[][] GetMovablePieceCoordinates(MVNode node, int player_turn)
    {
        if (node != null)
        {
            int node_index = 0;
            Board board = node.GetBoard();
            int[][] search_coordinates = new int[board.GetPieceCount()][];
            int count = 0;
            for (int i = 0; i < board.GetPieceCount(); i++)
            {
                int[] coordinate = board.IndexToCoordinate(i);
                int piece_index = board.GetPieceAt(coordinate);
                if (piece_index != 0 && (piece_index / 32) == player_turn)
                {
                    search_coordinates[count] = new int[coordinate.Length + 2];
                    System.Array.Copy(coordinate, search_coordinates[count], coordinate.Length);
                    search_coordinates[count][search_coordinates[count].Length - 2] = node.m;
                    search_coordinates[count][search_coordinates[count].Length - 1] = node.t;
                    count++;
                }
            }
            int[][] coordinates = new int[count][];
            System.Array.Copy(search_coordinates, coordinates, count);
            return coordinates;
        }
        return null;
    }
    int[][] GetPossibleMoveLocations(int[] coordinate_from)
    {
        List<int[]> listOfTargets = new List<int[]>();
        RuleSet ruleset = game.GetRuleSet();
        int[][] targetCoords = ruleset.CreateTargetCoords(simulatorMultiverse, coordinate_from);
        if (targetCoords != null)
        {
            for (int i = 0; i < targetCoords.Length; i++)
            {
                if (simulatorMultiverse.IsInBounds(targetCoords[i]))
                {
                    listOfTargets.Add(targetCoords[i]);
                }
            }
            if (listOfTargets.Count > 0)
                return listOfTargets.ToArray();
        }
        return null;
    }
    Move[][] GeneratePairBuffer(int board_count)
    {
        if (board_count <= 1)
            return null;
        int combo_count = ((board_count) * (board_count - 1)) / 2;
        Move[][] pair_buffer = new Move[combo_count][];
        return pair_buffer;
    }
    Move[][] GenerateSingBuffer(int board_count)
    {
        if (board_count <= 0)
            return null;
        Move[][] sing_buffer = new Move[board_count][];
        return sing_buffer;
    }
    void StoreMovesIntoBuffers(MVNode[] playable_nodes, Move[] move_list, Move[][] pair_buffer, Move[][] sing_buffer, int board_count)
    {
        for (int i = 0; i < move_list.Length; i++)
        {
            StoreMoveIntoBuffer(playable_nodes, move_list[i], pair_buffer, sing_buffer, board_count);
        }
    }
    void StoreMoveIntoBuffer(MVNode[] playable_nodes, Move move, Move[][] pair_buffer, Move[][] sing_buffer, int board_count)
    {
        if (playable_nodes != null && move != null)
        {
            int node_from = PlayableNodeFrom(playable_nodes, move);
            int node_to = PlayableNodeTo(playable_nodes, move);
            if (node_from >= 0)
            {
                if (node_from == node_to && sing_buffer != null)
                    AddToBuffer(move, sing_buffer, node_from);
                else if (node_to == -1 && sing_buffer != null)
                    AddToBuffer(move, sing_buffer, node_from);
                else if (node_to >= 0 && pair_buffer != null)
                    AddToBuffer(move, pair_buffer, GenerateComboIndex(node_from, node_to, board_count));
            }
            else
            {
                Debug.LogWarning("Warning, node from is -1, shouldnt happen.");
            }
        }
    }
    void AddToBuffer(Move move, Move[][] buffer, int index)
    {
        if (buffer[index] == null)
            buffer[index] = new Move[] { move };
        else
        {
            Move[] new_moves = new Move[buffer[index].Length + 1];
            System.Array.Copy(buffer[index], new_moves, buffer[index].Length);
            new_moves[new_moves.Length - 1] = move;
            buffer[index] = new_moves;
        }
    }
    int GenerateComboIndex(int board0, int board1, int board_count)
    {
        int min = Mathf.Min(board0, board1);
        int max = Mathf.Max(board0, board1);

        int n = board_count;
        int f = min;
        return ((n) * (n - 1) - (n - f) * (n - 1 - f)) / 2 + (max - 1 - min);
    }
    int PlayableNodeFrom(MVNode[] playable_nodes, Move move)
    {
        int m = move.tail.from[move.tail.from.Length - 2];
        int t = move.tail.from[move.tail.from.Length - 1];
        return FindByCoordinate(playable_nodes, m, t);
    }
    int PlayableNodeTo(MVNode[] playable_nodes, Move move)
    {
        int m = move.tail.to[move.tail.to.Length - 2];
        int t = move.tail.to[move.tail.to.Length - 1];
        return FindByCoordinate(playable_nodes, m, t);
    }
    int FindByCoordinate(MVNode[] playableNodes, int m, int t)
    {
        for (int i = 0; i < playableNodes.Length; i++)
        {
            if ( (playableNodes[i].m == m) && (playableNodes[i].t == t) )
                return i;
        }
        return -1;
    }
    int[] GenerateSequentialNumbers(int n)
    {
        int[] seq = new int[n];
        for (int i = 0; i < n; i++)
            seq[i] = i;
        return seq;
    }
    int[] GenerateSequentialNumbers(int n, int offset)
    {
        int[] seq = new int[n];
        for (int i = 0; i < n; i++)
            seq[i] = i + offset;
        return seq;
    }
    int[] ExtractIndices(int[] array, int[] indices)
    {
        int[] extracted = new int[indices.Length];
        for (int i = 0; i < extracted.Length; i++)
            extracted[i] = array[indices[i]];
        return extracted;
    }
    int[] RemoveIndices(int[] array, int[] indices)
    {
        int[] array_clone = (int[])array.Clone();
        int[] removed = new int[array.Length - indices.Length];
        for (int i = 0; i < indices.Length; i++)
            array_clone[indices[i]] = -1;
        int counter = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array_clone[i] != -1)
            {
                removed[counter] = array_clone[i];
                counter++;
            }
        }
        return removed;
    }
    int[] RemapIndices(int[] array, int[] indices)
    {
        int[] remapped = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
            remapped[i] = array[indices[i]];
        return remapped;
    }
    int[] SpliceArrays(int[] arr1, int[] arr2)
    {
        if (arr2 == null)
            return (int[])arr1.Clone();
        if (arr1 == null)
            return (int[])arr2.Clone();
        int[] array = new int[arr1.Length + arr2.Length];
        System.Array.Copy(arr1, 0, array, 0, arr1.Length);
        System.Array.Copy(arr2, 0, array, arr1.Length, arr2.Length);
        return array;
    }
    int[] IncrementArray(int[] array, int[] maximums)
    {
        if (array == null)
            return null;

        int index = array.Length - 1;
        array[index]++;
        while (index >= 0 && array[index] >= maximums[index])
        {
            array[index] = 0;
            index--;
            if (index >= 0)
                array[index]++;
            else
            {
                for (int i = 0; i < array.Length; i++)
                    array[i] = -1;
                break;
            }
        }
        return array;
    }





    #region Turn Tree Data Structure
    void ClearTree()
    {
        ClearNode(rootNode);
        if (rootNode == null)
            Plant();
    }
    void ClearNode(AINode node)
    {
        if (node != null)
        {
            if (node.principleChild != null)
            {
                AINode sentinel = node.principleChild;
                while (sentinel != null)
                {
                    AINode next = sentinel.next;
                    ClearNode(sentinel);
                    sentinel = next;
                }
            }
            if (node == rootNode)
                rootNode = null;
            node.parent = null;
            node.next = null;
            node.prev = null;
            node.principleChild = null;
        }
    }

    
    void Plant()
    {
        AINode root = new AINode(null);
        SetRootNode(root);
    }
    void SetRootNode(AINode root)
    {
        rootNode = root;
    }
    void ParentNode(AINode child, AINode parent)
    {
        child.parent = parent;
        if (parent.principleChild == null)
        {
            parent.principleChild = child;
            child.next = null;
            child.prev = null;
        }
        else
        {
            parent.principleChild.prev = child;
            child.next = parent.principleChild;
            child.prev = null;
            parent.principleChild = child;
        }
    }
    void RemoveNode(AINode node)
    {
        if (node.parent != null)
        {
            if (node.next == null && node.prev == null)
            {
                AINode parent = node.parent;
                if (parent.principleChild != node)
                    Debug.LogWarning("Removing lone node that is not principle child of parent");
                parent.principleChild = null;
                node.parent = null;
            }
            else
            {
                AINode next = node.next;
                AINode prev = node.prev;
                if (next != null)
                    next.prev = prev;
                if (prev != null)
                    prev.next = next;
                if (prev == null)
                    node.parent.principleChild = next;
                node.parent = null;
                node.prev = null;
                node.next = null;
            }
        }
    }
    #endregion
}
