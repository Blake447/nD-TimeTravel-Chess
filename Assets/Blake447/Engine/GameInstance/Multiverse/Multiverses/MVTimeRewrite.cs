using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVTimeRewrite : Multiverse
{
    int PLAYER_COUNT = 2;
    float time_offset;
    float mv_offset;
    PiecePallete pallete;
    int[] state;

    List<MVNode> root_up = new List<MVNode>();
    List<MVNode> root_down = new List<MVNode>();
    List<MVNode> front_up = new List<MVNode>();
    List<MVNode> front_down = new List<MVNode>();
    List<MVNode> game_stack = new List<MVNode>();
    List<MVNode>[] timelines;


    public MVNode[][] multiverse;


    void AddToMultiverse(MVNode node, int m, int t)
    {
        int index = TimelineToIndex(m);
        if (multiverse == null)
        {
            multiverse = new MVNode[1][];
        }
        if (!(index < multiverse.Length))
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
        if (!(node.t < multiverse[index].Length))
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
                        if (multiverse[i][j] != null && (i != 0 || j != 0))
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





    public override void InitializeMultiverse(PiecePallete pallete, int[] board_state)
    {
        this.pallete = pallete;
        this.state = (int[])board_state.Clone();
    }
    public void SetOffsets(float m, float t)
    {
        mv_offset = -m;
        time_offset = -t;
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
                                    int sking = 16;
                                    if ((piece % 32) == king || (piece % 32) == sking)
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
    MVNode GetNode(int m, int t)
    {
        MVNode node = GetNodeFromMultiverse(m, t);
        return GetNodeFromMultiverse(m, t);
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





    bool IsTimePlayers(int t, int player)
    {
        return (t % 2) == player;
    }
    int TimelineToIndex(int m)
    {
        int offset = m < 0 ? -1 : 0;
        return Mathf.Abs(m) * 2 + offset;
    }
    int IndexToTimeline(int m)
    {
        int sign = (m % 2) == 1 ? -1 : 1;
        return ((m + 1) / 2) * sign;
    }
}
