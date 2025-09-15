using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVTime : Multiverse
{
    PiecePallete pallete;
    int player_count = 2;
    [SerializeField]
    float time_offset = -20.0f;
    [SerializeField]
    float mv_offset = -40.0f;
    public GameObject NodeRoot;

    public MVNode first_node;
    public MVNode front_node;

    public MVNode[] NodeStack;

    public List<MVNode> GameStack = new List<MVNode>();

    public PurpleArrow TimeLineArrowPrefab;
    public List<PurpleArrow> TimeLineArrows = new List<PurpleArrow>();

    public void SetOffsets(float m, float t)
    {
        mv_offset = -m;
        time_offset = -t;
    }

    List<MVNode> last_node_split = new List<MVNode>();
    
    public MVNode[][] multiverse;
    int[] state = null;

    public MVNode stack_head;

    public Vector3 GetBoardOffset()
    {
        Board board = GetRootBoard();
        return Vector3.Scale(new Vector3(1,0,1), board.GetCenter());
    }
    public Vector2 GetOffsets()
    {
        return new Vector2(-time_offset, -mv_offset);
    }
    public Vector3 MTCenter(int m, int t)
    {
        return GetBoard(m, t).GetCenter();
    }


    public override void ApplyTurn(Turn turn)
    {
        Move move = turn.tail;
        while (move != null)
        {
            ApplyMove(move);
            move = move.next;
        }
    }
    public override void RevertTurn(Turn turn)
    {
        Move move = turn.head;
        while (move != null)
        {
            RevertMove(move);
            move = move.prev;
        }
    }

    public override void InitializeMultiverse(PiecePallete pallete, int[] board_state)
    {
        this.pallete = pallete;
        this.state = (int[])board_state.Clone();
    }
    public override MVNode[] GetPlayableNodes(int players_turn)
    {
        if (multiverse != null)
        {
            List<MVNode> nodes = new List<MVNode>();
            for (int i = 0; i < multiverse.Length; i++)
            {
                if (multiverse[i] != null)
                {
                    int t = multiverse[i].Length - 1;
                    MVNode node = multiverse[i][t];
                    if (node != null)
                    {
                        if (IsTimePlayers(t, players_turn))
                            nodes.Add(node);
                    }
                    else
                    {
                        Debug.Log("Failed to find at (" + IndexToTimeline(i) + ", " + t + ")");
                    }
                }
            }
            if (nodes.Count > 0)
                return nodes.ToArray();
        }
        return null;
    }

    public override void HardReset()
    {
        if (multiverse != null)
        {
            for (int i = 0; i < multiverse.Length; i++)
            {
                if (multiverse[i] != null)
                {
                    for (int j = 0; j < multiverse[i].Length; j++)
                    {
                        if (multiverse[i][j] != null && (i != 0 || j != 0) )
                        {
                            Destroy(multiverse[i][j].gameObject);
                        }
                    }
                }
            }
        }
        MVNode root = GetNode(0, 0);
        multiverse = new MVNode[1][];
        multiverse[0] = new MVNode[1];
        multiverse[0][0] = root;
    }

    public override int[][] GetRoyalty()
    {
        List<int[]> royalty = new List<int[]>();
        if (multiverse != null)
        {
            for (int i = 0; i < multiverse.Length; i++)
            {
                int m = IndexToTimeline(i);
                MVNode[] timeline = multiverse[i];
                if (timeline != null)
                {
                    for (int t = 0; t < timeline.Length; t++)
                    {
                        MVNode node = timeline[t];
                        if (node != null)
                        {
                            Board board = node.GetBoard();
                            if (board != null)
                            {
                                for (int s = 0; s < board.GetPieceCount(); s++)
                                {
                                    int piece = board.GetPieceAt(s);
                                    int king = 1;
                                    int sking = 32;
                                    if ( (piece % Overseer.PIECE_COUNT) == king || (piece % Overseer.PIECE_COUNT) == sking )
                                    {
                                        int[] coordinate = board.IndexToCoordinate(s);
                                        int[] coordinate_mv = new int[coordinate.Length + 2];
                                        System.Array.Copy(coordinate, 0, coordinate_mv, 0, coordinate.Length);
                                        coordinate_mv[coordinate_mv.Length - 2] = m;
                                        coordinate_mv[coordinate_mv.Length - 1] = t;
                                        royalty.Add(coordinate_mv);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (royalty.Count > 0)
        {
            int[][] royalty_array = royalty.ToArray();
            //Debug.Log("Found " + royalty_array.Length + " pieces of royalty");
            return royalty_array;
        }
        else
        {
            Debug.Log("did not find any royalty");
        }
        return null;
    }

    void PrintMultiverse()
    {
        if (multiverse != null)
        {
            for (int i = 0; i < multiverse.Length; i++)
            {
                Debug.Log("Timeline " + IndexToTimeline(i) + " is " + multiverse[i]);
                if (multiverse[i] != null)
                {
                    for (int j = 0; j < multiverse[i].Length; j++)
                    {
                        Debug.Log("node " + multiverse[i][j]);
                    }
                }
            }
        }
        else
        {
            Debug.Log("multiverse null");
        }
    }

    void AddToMultiverse(MVNode node, int m, int t)
    {
        int index = TimelineToIndex(m);
        if (multiverse == null)
        {
            multiverse = new MVNode[1][];
        }
        if ( !(index < multiverse.Length) )
        {
            MVNode[][] new_multiverse = new MVNode[index + 1][];
            System.Array.Copy(multiverse, 0, new_multiverse, 0, multiverse.Length);
            multiverse = new_multiverse;
        }
        if (multiverse[index] == null)
        {
            MVNode[] timeline = new MVNode[node.t + 1];
            timeline[node.t] = node;
            multiverse[index] = timeline;
        }
        if ( !(node.t < multiverse[index].Length) )
        {
            MVNode[] timeline = multiverse[index];
            MVNode[] new_timeline = new MVNode[node.t + 1];
            System.Array.Copy(timeline, 0, new_timeline, 0, timeline.Length);
            multiverse[index] = new_timeline;
        }
        multiverse[index][node.t] = node;
        if (index == 0 && node.t == 0)
            RootNode = node;
    }
    MVNode GetNodeFromMultiverse(int m, int t)
    {
        int index = TimelineToIndex(m);
        if (multiverse != null)
            if (index < multiverse.Length)
                if (multiverse[index] != null)
                    if (t >= 0 && t < multiverse[index].Length)
                        return multiverse[index][t];
        return null;
    }

    int TimelineToIndex(int m)
    {
        int offset = m < 0 ? -1 : 0;
        return Mathf.Abs(m) * 2 + offset;
    }
    int IndexToTimeline(int m)
    {
        int sign = (m % 2) == 1 ? -1 : 1;
        return ( (m + 1) / 2 ) * sign;
    }

    int CalculatePresentWidth()
    {
        if (multiverse == null)
            return 0;

        int pos_max = 0;
        int neg_max = 0;

        for (int i = 0; i < multiverse.Length; i++)
        {

            int m = IndexToTimeline(i);
            MVNode[] timeline = multiverse[i];
            if (timeline != null && m > 0)
            {
                pos_max = Mathf.Max(pos_max, m);
            }
            if (timeline != null && m < 0)
            {
                neg_max = Mathf.Max(neg_max, -m);
            }
        }

        return Mathf.Min(pos_max, neg_max) + 1;
    }
    int CalculatePresentTime()
    {
        int m_max = CalculatePresentWidth();

        if (multiverse == null)
            return 0;
        if (multiverse[0] == null)
            return 0;


        int min_time = multiverse[0].Length - 1;
        for (int i = 0; i < multiverse.Length; i++)
        {
            int m = IndexToTimeline(i);
            MVNode[] timeline = multiverse[i];
            if (timeline != null && Mathf.Abs(m) <= m_max)
            {
                min_time = Mathf.Min(min_time, timeline.Length - 1);
            }
        }
        return min_time;
    }

    public void SetAllIndicators(bool active)
    {
        if (multiverse != null)
        {
            for (int i = 0; i < multiverse.Length; i++)
            {
                int m = IndexToTimeline(i);
                MVNode[] timeline = multiverse[i];
                if (timeline != null)
                {
                    for (int t = 0; t < timeline.Length; t++)
                    {
                        MVNode node = GetNode(m, t);
                        if (node != null)
                        {
                            node.SetIndicator(active);
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (Time.frameCount % 15 == 0)
        {
            IndicatedRequiredBoards();
        }
    }

    public void IndicatedRequiredBoards()
    {
        SetAllIndicators(false);
        int max_m = CalculatePresentWidth();
        int present = CalculatePresentTime();
        if (multiverse != null)
        {
            for (int i = 0; i < multiverse.Length; i++)
            {
                int m = IndexToTimeline(i);
                MVNode[] timeline = multiverse[i];
                if (timeline != null)
                {
                    MVNode node = GetNode(m, timeline.Length - 1);
                    if (node != null && node.t <= present && Mathf.Abs(m) <= max_m)
                    {
                        node.SetIndicator(true);
                    }
                }
            }
        }
    }
    public override int CountPlayersActiveBoards(int player)
    {
        int max_m = CalculatePresentWidth();
        int present = CalculatePresentTime();
        int players_required_boards = 0;
        if (multiverse != null)
        {
            for (int i = 0; i < multiverse.Length; i++)
            {
                int m = IndexToTimeline(i);
                MVNode[] timeline = multiverse[i];
                if (timeline != null)
                {
                    MVNode node = GetNode(m, timeline.Length - 1);
                    if (node != null && node.t <= present && Mathf.Abs(m) <= max_m)
                    {
                        if (IsTimePlayers(node.t, player))
                        {
                            players_required_boards++;
                        }
                    }
                }
            }
        }
        return players_required_boards;
    }

    bool IsTimePlayers(int t, int player)
    {
        return (t % player_count) == player;
    }

    public override bool IsPlayersBoard(int[] coordinate, int player)
    {
        int m = coordinate[coordinate.Length - 2];
        int t = coordinate[coordinate.Length - 1];
        MVNode node = GetNode(m, t);
        if (node == null)
            return false;

        return IsTimePlayers(t, player);
    }

    public override bool IsBoardActive(int[] coordinate)
    {
        int m = coordinate[coordinate.Length - 2];
        int t = coordinate[coordinate.Length - 1];
        
        MVNode node = GetNode(m, t);
        if (node == null)
            return false;

        int index = TimelineToIndex(m);
        if (index >= multiverse.Length)
            return false;

        MVNode[] timeline = multiverse[index];

        if (timeline == null)
            return false;

        return (timeline.Length - 1) == t;
    }


    MVNode Node(MVNode source, int m, int t)
    {
        MVNode node = Instantiate(source);
        node.m = m;
        node.t = t;
        node.name = "Node-" + Tuple(node.m, node.t);
        node.GetBoard().name = "Board-" + Tuple(node.m, node.t);
        //node.GetBoard().InitializeBoard(pallete, state, isPhysical);
        //int[] source_state = (int[])source.GetBoard().RequestState();
        //int[] source_changes = (int[])source.GetBoard().RequestChanges();
        //bool[] source_enpassant = (bool[])source.GetBoard().RequestEnPassant();
        //node.GetBoard().TransferState(source_state, source_changes, source_enpassant);
        node.GetBoard().ClearEnpassant();
        node.GetBoard().SetAllMeshDisplays();
        node.transform.parent = NodeRoot.transform;
        node.transform.localPosition = TemplateNode.transform.position + -TemplateNode.transform.TransformVector(NodeRoot.transform.position) + MTPosition(m, t);
        return node;
    }
    MVNode Node(int m, int t)
    {
        MVNode node = Instantiate(TemplateNode);
        node.m = m;
        node.t = t;
        node.name = "Node-" + Tuple(node.m, node.t);
        node.GetBoard().name = "Board-" + Tuple(node.m, node.t);
        node.GetBoard().InitializeBoard(pallete, state, isPhysical);
        node.transform.parent = NodeRoot.transform;
        node.transform.localPosition = node.transform.localPosition + MTPosition(m, t);
        return node;
    }

    public Vector3 MTPosition(float m, float t)
    {
        return Vector3.forward * mv_offset * m + Vector3.right * time_offset * t; ;
    }
    public void AddBoard()
    {
        MVNode node = Node(0, 0);
        GameStack.Add(node);
        AddToMultiverse(node, node.m, node.t);
    }
    public void RevertTimeline(int m, bool isTimeTravel)
    {
        Debug.Log("Reverting timline " + m);
        int index = TimelineToIndex(m);
        if (multiverse != null && index < multiverse.Length)
        {
            MVNode[] timeline = multiverse[index];
            if (timeline != null)
            {
                MVNode last = timeline[timeline.Length - 1];
                if (last != null)
                {
                    bool isOnlyBoard = m == 0 && last.t == 0;
                    if (!isOnlyBoard)
                    {
                        Destroy(last.gameObject);
                        timeline[timeline.Length - 1] = null;
                        bool hasNonNull = false;
                        for (int t = 0; t < timeline.Length; t++)
                        {
                            hasNonNull = hasNonNull || (timeline[t] != null);
                        }
                        if (hasNonNull)
                        {
                            MVNode[] new_timeline = new MVNode[timeline.Length - 1];
                            System.Array.Copy(timeline, 0, new_timeline, 0, new_timeline.Length);
                            multiverse[index] = new_timeline;
                        }
                        else
                        {
                            Debug.Log("Removed last mvnode in " + m);
                            multiverse[index] = null;
                            bool isLastMultiverse = true;
                            //for (int sentinel = index; sentinel < multiverse.Length; sentinel++)
                            //{
                            //    if (multiverse[sentinel] != null)
                            //    {
                            //        isLastMultiverse = false;
                            //    }
                            //}
                            //if (isLastMultiverse && index > 1)
                            //{
                            //    Debug.Log("Detected index " + index + " to be the furthest multiverse out, removing branch")
                            //    MVNode[][] new_multiverse = new MVNode[index - 1][];
                            //    System.Array.Copy(multiverse, 0, new_multiverse, 0, new_multiverse.Length);
                            //    this.multiverse = new_multiverse;
                            //}
                        }
                    }
                }
            }
        }
    }
    public MVNode AdvanceTimeline(int m)
    {
        if (multiverse == null)
            return null;
        int index = TimelineToIndex(m);
        if (multiverse[index] == null)
            return null;

        MVNode[] timeline = multiverse[index];

        if (timeline[timeline.Length - 1] == null)
            return null;

        MVNode source = timeline[timeline.Length - 1];
        MVNode node = Node(source, m, source.t + 1);
        GameStack.Add(node);
        AddToMultiverse(node, node.m, node.t);
        return node;
    }

    public MVNode SplitTimeline(MVNode board_traveled_to)
    {
        if (multiverse == null)
            return null;
        int m_target = (board_traveled_to.t % 2) == 0 ? 1 : -1;
        int m = 0;
        bool exitFlag = false;
        while (!exitFlag)
        {
            if (m_target > 0)
            {
                m++;
                int index = TimelineToIndex(m);
                if (index < 0 || index >= multiverse.Length || multiverse[index] == null)
                    exitFlag = true;
            }
            if (m_target < 0)
            {
                m--;
                int index = TimelineToIndex(m);
                if (index < 0 || index >= multiverse.Length || multiverse[index] == null)
                    exitFlag = true;
            }
        }
        MVNode node = Node(board_traveled_to, m, board_traveled_to.t + 1);
        GameStack.Add(node);
        AddToMultiverse(node, node.m, node.t);
        return node;
    }

    public override void RevertMove(Move move)
    {
        Command command = move.head;
        if (command != null)
        {
            int piece_from = command.pfrom;
            int piece_to = command.pto;
            int[] coord_from = (int[])command.from.Clone();
            int[] coord_to = (int[])command.to.Clone();

            if (GameStack.Count > 0)
            {
                MVNode FirstLastAdded = GameStack[Mathf.Max(GameStack.Count - 1, 0)];
                MVNode SecondLastAdded = GameStack[Mathf.Max(GameStack.Count - 2, 0)];

                MVNode node_from = GetNode(coord_from[coord_from.Length - 2], coord_from[coord_from.Length - 1]);
                MVNode node_to = GetNode(coord_to[coord_to.Length - 2], coord_to[coord_to.Length - 1]);

                bool isSingle = node_from == node_to;

                // TODO: Implement turn reversal
                if (isSingle)
                {
                    RevertTimeline(FirstLastAdded.m, false);
                    GameStack.Remove(FirstLastAdded);
                }
                else
                {
                    RevertTimeline(FirstLastAdded.m, false);
                    RevertTimeline(SecondLastAdded.m, false);
                    GameStack.Remove(FirstLastAdded);
                    GameStack.Remove(SecondLastAdded);
                }
            }
        }
    }
    public override void ApplyMove(Move move)
    {
        Command command = move.tail;

        int piece_from = command.pfrom;
        int piece_to = command.pto;
        int[] coord_from = (int[])command.from.Clone();
        int[] coord_to = (int[])command.to.Clone();

        MVNode node_from = GetNode(coord_from[coord_from.Length - 2], coord_from[coord_from.Length - 1]);
        MVNode node_to = GetNode(coord_to[coord_to.Length - 2], coord_to[coord_to.Length - 1]);

        MVNode next_from = GetNode(coord_from[coord_from.Length - 2], coord_from[coord_from.Length - 1] + 1);
        MVNode next_to = GetNode(coord_to[coord_to.Length - 2], coord_to[coord_to.Length - 1] + 1);

        bool isStandard = node_from == node_to;
        bool isAdvanceFrom = next_from == null;
        bool isAdvanceTo = next_to == null;
        bool isTimeTravel = next_to != null;

        if (isStandard)
        {
            MVNode node = AdvanceTimeline(node_to.m);
            if (node != null)
            {
                coord_from[coord_from.Length - 2] = node.m;
                coord_from[coord_from.Length - 1] = node.t;

                coord_to[coord_to.Length - 2] = node.m;
                coord_to[coord_to.Length - 1] = node.t;
            }
        }
        else
        {
            if (isAdvanceTo)
            {
                MVNode node0 = AdvanceTimeline(node_to.m);
                last_node_split.Add(node0);
                coord_to[coord_from.Length - 2] = node0.m;
                coord_to[coord_from.Length - 1] = node0.t;
            }
            else if (isTimeTravel)
            {
                MVNode node0 = SplitTimeline(node_to);
                coord_to[coord_from.Length - 2] = node0.m;
                coord_to[coord_from.Length - 1] = node0.t;
            }
            MVNode node1 = AdvanceTimeline(node_from.m);
            coord_from[coord_from.Length - 2] = node1.m;
            coord_from[coord_from.Length - 1] = node1.t;

        }

        int mf = coord_from[coord_from.Length - 2];
        int tf = coord_from[coord_from.Length - 1];

        int mt = coord_to[coord_to.Length - 2];
        int tt = coord_to[coord_to.Length - 1];

        while (command != null)
        {
            coord_from = (int[])command.from.Clone();
            coord_to = (int[])command.to.Clone();

            coord_from[coord_from.Length - 2] = mf;
            coord_from[coord_from.Length - 1] = tf;

            coord_to[coord_to.Length - 2] = mt;
            coord_to[coord_to.Length - 1] = tt;

            int pfrom = command.pfrom;
            int pto = command.pto;

            SetPieceAt(0, coord_from);
            IncrementChangeCounter(coord_from);
            SetPieceAt(pfrom, coord_to);
            IncrementChangeCounter(coord_to);
            command = command.next;
        }
    }


    MVNode GetNode(int m, int t)
    {
        MVNode node = GetNodeFromMultiverse(m, t);
        return GetNodeFromMultiverse(m, t);
    }

    public override bool IsInBounds(int[] coordinate)
    {
        Board board = GetBoardFromCoordinate(coordinate);
        if (board == null)
            return false;
        return board.IsInBounds(coordinate);
    }
    public override int GetPieceAt(int[] coordinate)
    {
        Board board = GetBoardFromCoordinate(coordinate);
        if (board != null && coordinate != null)
        {
            return board.GetPieceAt(coordinate);
        }
        return -1;

    }

    Vector3 root_position = new Vector3(-2.0f, 0.0f, -11.0f);
    public override Vector3 WorldToLocalPosition(Vector3 position)
    {
        MVNode node = GetNearestNode(position);
        if (node != null)
            return position - Vector3.forward * mv_offset * node.m - Vector3.right * time_offset * node.t;
        return Vector3.zero;
    }
    

    public override int[] PositionToCoordinate(Vector3 position)
    {
        Board rootBoard = GetRootBoard();
        if (rootBoard == null)
            return null;
        Vector3 center = rootBoard.GetCenter();
        Vector3 offset = position - center;

        int m = Mathf.RoundToInt(offset.z / mv_offset);
        int t = Mathf.RoundToInt(offset.x / time_offset);
        Board board = GetBoard(m, t);
        if (board == null)
            return null;
        //Vector3 localPosition = WorldToLocalPosition(position);
        int[] board_coord = board.PositionToCoordinate(position);
        if (board_coord == null)
            return null;
        int[] mv_coord = new int[board_coord.Length + 2];
        System.Array.Copy(board_coord, 0, mv_coord, 0, board_coord.Length);
        mv_coord[mv_coord.Length - 2] = m;
        mv_coord[mv_coord.Length - 1] = t;
        return (int[])mv_coord.Clone();
    }
    public override Vector3 CoordinateToPosition(int[] coordinate)
    {
        if (coordinate == null)
            return Vector3.zero;
        int m = coordinate[coordinate.Length - 2];
        int t = coordinate[coordinate.Length - 1];
        Board board = GetBoard(m, t);
        if (board == null)
            return Vector3.zero;
        return board.CoordinateToPosition(coordinate);
    }

    private void Start()
    {
        AddBoard();
        
    }

    string Tuple(int m, int t)
    {
        return "(" + m + ", " + t + ")";
    }
    
    public override void SetPieceAt(int piece, int[] coordinate)
    {
        Board board = GetBoardFromCoordinate(coordinate);
        if (board != null)
            board.SetAtCoordinate(piece, coordinate);
    }
    public override void IncrementChangeCounter(int[] coordinate)
    {
        Board board = GetBoardFromCoordinate(coordinate);
        if (board != null)
            board.IncrementChangeCounter(coordinate);
    }
    public override void DecrementChangeCounter(int[] coordinate)
    {
        Board board = GetBoardFromCoordinate(coordinate);
        if (board != null)
            board.DecrementChangeCounter(coordinate);
    }
    protected override string CoordinateToString(int[] coord)
    {
        if (coord == null)
            return "null_coord";
        string coordstring = "(";
        for (int i = 0; i < coord.Length - 1; i++)
            coordstring = coordstring + coord[i] + ", ";
        coordstring = coordstring + coord[coord.Length - 1] + ")";
        return coordstring;
    }
    public override int[] GetStrip(int[] coord_from, int[] coord_to, int time_index)
    {
        if (coord_from == null || coord_to == null)
            return null;
        int[] offset = new int[Mathf.Min(coord_from.Length, coord_to.Length)];
        for (int i = 0; i < offset.Length; i++)
            offset[i] = coord_to[i] - coord_from[i];
        if (time_index < offset.Length)
            offset[time_index] /= 2;
        int max = Mathf.Max(offset);
        int min = Mathf.Min(offset);
        max = Mathf.Abs(max > -min ? max : min);
        int gcf = 1;
        for (int i = 1; i <= max; i++)
        {
            bool divisible = true;
            for (int j = 0; j < offset.Length; j++)
                divisible = divisible && Mathf.Abs(offset[j]) % i == 0;
            gcf = divisible ? i : gcf;
        }
        for (int i = 0; i < offset.Length; i++)
            offset[i] = offset[i] / gcf;
        int steps = gcf;
        int[] strip = new int[steps];
        int[] sentinel = (int[])coord_from.Clone();
        if (time_index < offset.Length)
            offset[time_index] *= 2;
        for (int i = 0; i < steps; i++)
        {
            for (int j = 0; j < offset.Length; j++)
                sentinel[j] = coord_from[j] + offset[j] * (i + 1);
            Board board = GetBoardFromCoordinate(coord_from);
            if (board != null)
            {
                strip[i] = GetPieceAt(sentinel);
            }
        }
        return (int[])strip.Clone();
    }
    public override int[][] GetStripCoordinate(int[] coord_from, int[] coord_to)
    {
        if (coord_from == null || coord_to == null)
            return null;
        int[] offset = new int[Mathf.Min(coord_from.Length, coord_to.Length)];
        for (int i = 0; i < offset.Length; i++)
            offset[i] = coord_to[i] - coord_from[i];
        int max = Mathf.Max(offset);
        int min = Mathf.Min(offset);
        max = Mathf.Abs(max > -min ? max : min);
        int gcf = 1;
        for (int i = 1; i <= max; i++)
        {
            bool divisible = true;
            for (int j = 0; j < offset.Length; j++)
                divisible = divisible && Mathf.Abs(offset[j]) % i == 0;
            gcf = divisible ? i : gcf;
        }
        for (int i = 0; i < offset.Length; i++)
            offset[i] = offset[i] / gcf;
        int steps = gcf;
        int[][] strip = new int[steps][];
        int[] sentinel = (int[])coord_from.Clone();
        for (int i = 0; i < steps; i++)
        {
            for (int j = 0; j < offset.Length; j++)
                sentinel[j] = coord_from[j] + offset[j] * (i + 1);
            strip[i] = (int[])sentinel.Clone();
        }
        return (int[][])strip.Clone();
    }

    public override Board GetBoardFromCoordinate(int[] coordinate)
    {
        int m = coordinate[coordinate.Length - 2];
        int t = coordinate[coordinate.Length - 1];
        return GetBoard(m, t);
    }

    Board GetBoard(int m, int t)
    {
        MVNode node = GetNode(m, t);
        if (node == null)
            return null;
        return node.GetBoard();
    }
    

    public override void HightlightMoves(int[][] coordinates)
    {
        if (coordinates != null)
        { 
            for (int i = 0; i < coordinates.Length; i++)
            {
                if (coordinates[i] != null)
                {
                    Board board = GetBoardFromCoordinate(coordinates[i]);
                    if (board != null)
                    {
                        if (board.IsInBounds(coordinates[i]))
                        {
                            board.HighlightSquare(coordinates[i]);
                        }
                    }
                    else
                    {
                        Debug.Log("Was not able to find board for target coordinate " + Coordinates.CoordinateToString(coordinates[i]));
                    }
                }
            }
        }
    }
    public override void FlagCoordinatesForPieces(int[][] coordinates)
    {
        if (coordinates != null)
        {
            for (int i = 0; i < coordinates.Length; i++)
            {
                Board board = GetBoardFromCoordinate(coordinates[i]);
                if (board != null)
                {
                    board.HighlightClear();
                    if (board.IsInBounds(coordinates[i]))
                    board.FlagSquare(coordinates[i]);
                }
            }
        }
    }
    public override void ClearFlag()
    {
        Board board = GetRootBoard();
        board.FlagClear();
    }
    public override bool GetFlag(int[] coordinate)
    {
        Board board = GetBoardFromCoordinate(coordinate);
        if (board != null)
            return board.GetFlag(coordinate) == 1;
        return false;
    }
    public override void HighlightClear()
    {
        if (multiverse != null)
        {
            for (int m = 0; m < multiverse.Length; m++)
            {
                if (multiverse[m] != null)
                {
                    for(int t = 0; t < multiverse[m].Length; t++)
                    {
                        int mv = IndexToTimeline(m);
                        Board board = GetBoard(mv, t);
                        if (board != null)
                        {
                            board.HighlightClear();
                        }
                    }
                }
            }
        }
    }
    public override Board GetNearestBoard(Vector3 position)
    {
        MVNode node = GetNearestNode(position);
        if (node == null)
            return null;
        return node.GetBoard();
    }
    MVNode GetNearestNode(Vector3 position)
    {
        Board rootBoard = GetRootBoard();
        Vector3 positionA = rootBoard.CoordinateToPosition(new int[] { 0, 0, 0, 0 });
        Vector3 positionB = rootBoard.CoordinateToPosition(new int[] { 3, 3, 3, 3 });
        Vector3 center = (positionA + positionB) * 0.5f;

        Vector3 offset = position - center;
        int m = Mathf.RoundToInt(offset.z / mv_offset);
        int t = Mathf.RoundToInt(offset.x / time_offset);

        return GetNode(m, t);
    }
    protected override string ArrayToString(int[] coordinate)
    {
        if (coordinate == null)
            return "()";
        string ret = "(";
        for (int i = 0; i < coordinate.Length - 1; i++)
        {
            ret += coordinate[i] + ", ";
        }
        ret += coordinate[coordinate.Length - 1] + ")";
        return ret;
    }
    public override bool IsStarting(int[] coordinate)
    {
        if (coordinate == null)
            return false;
        Board board = GetBoardFromCoordinate(coordinate);
        if (board != null)
            return board.GetChangeCounter(coordinate) == 0;
        return false;
    }
}
