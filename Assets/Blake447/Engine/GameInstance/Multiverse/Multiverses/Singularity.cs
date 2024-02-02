using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singularity : Multiverse
{
    public override void InitializeMultiverse(PiecePallete pallete, int[] board_state)
    {
        RootNode = Instantiate(TemplateNode);
        RootNode.name = "root_node";
        RootNode.GetBoard().name = "root_board";
        RootNode.GetBoard().InitializeBoard(pallete, board_state, isPhysical);
        int length = board_state.Length;
        int[] changes = new int[length];
        bool[] enpassant = new bool[length];
        RootNode.GetBoard().TransferState(board_state, changes, enpassant);
        RootNode.transform.parent = this.transform;
        //RootNode.transform.localPosition = Vector3.zero;
    }

    public override void RevertMove(Move move)
    {
        Board board = GetRootBoard();
        board.ClearGhostPawns();
        Command command = move.head;
        while (command != null)
        {
            int piece_from = command.pfrom;
            int piece_to = command.pto;
            int[] coord_from = command.from;
            int[] coord_to = command.to;
            SetPieceAt(piece_from, coord_from);
            DecrementChangeCounter(coord_from);
            SetPieceAt(piece_to, coord_to);
            DecrementChangeCounter(coord_to);
            command = command.prev;
        }
    }

    private void Start()
    {
    }
    public override int[][] GetRoyalty()
    {
        List<int[]> royalty = new List<int[]>();
        Board board = GetRootBoard();
        if (board != null)
        {
            for (int s = 0; s < board.GetPieceCount(); s++)
            {
                int piece = board.GetPieceAt(s);
                int king = 1;
                int sking = 32;
                if ((piece % Overseer.PIECE_COUNT) == king || (piece % Overseer.PIECE_COUNT) == sking)
                {
                    int[] coordinate = board.IndexToCoordinate(s);
                    royalty.Add(coordinate);
                }
            }
        }
        if (royalty.Count > 0)
        {
            int[][] royalty_array = royalty.ToArray();
            return royalty_array;
        }
        return null;
    }
    
    public override int CountPlayersActiveBoards(int player)
    {
        return 0;
    }
    public override bool IsPlayersBoard(int[] coordinate, int player)
    {
        return true;
    }
    public override bool IsBoardActive(int[] coordinate)
    {
        return true;
    }

    public override void ApplyMove(Move move)
    {
        Board board = GetRootBoard();
        board.ClearGhostPawns();
        Command command = move.tail;
        while (command != null)
        {
            int piece_from = command.pfrom;
            int piece_to = command.pto;
            int[] coord_from = command.from;
            int[] coord_to = command.to;
            SetPieceAt(0, coord_from);
            IncrementChangeCounter(coord_from);
            SetPieceAt(piece_from, coord_to);
            IncrementChangeCounter(coord_to);

            command = command.next;
        }
    }
    public override int[] PositionToCoordinate(Vector3 position)
    {
        Board board = GetRootBoard();
        int[] coordinate = board.PositionToCoordinate(position);
        if (coordinate == null)
            return null;
        return (int[])coordinate.Clone();
    }

    public override bool IsInBounds(int[] coordinate)
    {
        Board board = GetRootBoard();
        return board.IsInBounds(coordinate);
    }

    public override int GetPieceAt(int[] coordinate)
    {
        Board board = GetRootBoard();
        return board.GetPieceAt(coordinate);
    }
    public override Vector3 CoordinateToPosition(int[] coordinate)
    {
        Board board = GetRootBoard();
        return board.CoordinateToPosition(coordinate);
    }
    public override Vector3 WorldToLocalPosition(Vector3 position)
    {
        return position;
    }

    public override void SetPieceAt(int piece, int[] coordinate)
    {
        Board board = GetRootBoard();
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
        for (int i = 0; i < steps; i++)
        {
            for (int j = 0; j < offset.Length; j++)
                sentinel[j] = coord_from[j] + offset[j] * (i + 1);
            Board board = GetBoardFromCoordinate(coord_from);
            if (board != null)
            {
                strip[i] = board.GetPieceAt(sentinel);
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
        return GetRootBoard();
    }

    public override void HightlightMoves(int[][] coordinates)
    {
        Board board = GetRootBoard();
        board.HighlightClear();
        if (coordinates != null)
            for (int i = 0; i < coordinates.Length; i++)
                if (board.IsInBounds(coordinates[i]))
                    board.HighlightSquare(coordinates[i]);
    }
    public override void FlagCoordinatesForPieces(int[][] coordinates)
    {
        Board board = GetRootBoard();
        board.HighlightClear();
        if (coordinates != null)
            for (int i = 0; i < coordinates.Length; i++)
                if (board.IsInBounds(coordinates[i]))
                    board.FlagSquare(coordinates[i]);
    }
    public override void ClearFlag()
    {
        Board board = GetRootBoard();
        board.FlagClear();
    }
    public override bool GetFlag(int[] coordinate)
    {
        Board board = GetRootBoard();
        if (board != null)
            return board.GetFlag(coordinate) == 1;
        return false;
    }
    public override void HighlightClear()
    {
        Board board = GetRootBoard();
        if (board != null)
            board.HighlightClear();
    }
    public override Board GetNearestBoard(Vector3 position)
    {
        return GetRootBoard();
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
