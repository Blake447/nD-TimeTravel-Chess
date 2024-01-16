using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multiverse : MonoBehaviour
{

    public virtual void InitializeMultiverse(PiecePallete pallete, int[] board_state)
    {
       
    }

    public void SetTemplateNode(MVNode node)
    {
        TemplateNode = node;
    }

    public int[] lastPawnPosition;
    protected MVNode TemplateNode;
    protected MVNode RootNode;
    public bool isPhysical = true;

    public virtual void ApplyTurn(Turn turn)
    {

    }
    public virtual void RevertTurn(Turn turn)
    {

    }

    public virtual MVNode[] GetPlayableNodes(int players_turn)
    {
        return null;
    }

    public int[] GetBoardSize()
    {
        Board board = GetRootBoard();
        return (int[])board.GetBoardSize().Clone();
    }

    public virtual void HardReset()
    {

    }
    public virtual int[][][] ItemizeMultiverseState()
    {
        return null;
    }
    public virtual bool IsBoardRequired(int[] coordinate)
    {
        return true;
    }
    public virtual void RevertMove(Move move)
    {

    }
    public virtual int[][] GetRoyalty()
    {
        return null;
    }
    public virtual void IndicateActiveBoards(int player)
    {

    }
    public virtual int CountPlayersActiveBoards(int player)
    {
        return 0;
    }
    public virtual bool IsPlayersBoard(int[] coordinate, int player)
    {
        return true;
    }
    public virtual bool IsBoardActive(int[] coordinate)
    {
        return true;
    }

    public virtual void ApplyMove(Move move)
    {

    }
    public MVNode GetTemplateNode()
    {
        return TemplateNode;
    }
    public virtual bool IsInBounds(int[] coordinate)
    {
        Debug.LogWarning("Feature not implemented");
        return false;   
    }

    public virtual int GetPieceAt(int[] coordinate)
    {
        Debug.LogWarning("Feature not implemented");
        return 0;
    }

    public virtual Vector3 CoordinateToPosition(int[] coordinate)
    {
        Debug.LogWarning("Feature not implemented");
        return Vector3.zero;
    }
    public virtual int[] PositionToCoordinate(Vector3 position)
    {
        Debug.LogWarning("Feature not implemented");
        return null;
    }

    public virtual Vector3 WorldToLocalPosition(Vector3 position)
    {
        Debug.LogWarning("Feature not implemented");
        return position;
    }

    public virtual void SetPieceAt(int piece, int[] coordinate)
    {
        Debug.LogWarning("Feature not implemented");
    }
    public virtual void IncrementChangeCounter(int[] coordinate)
    {
        Debug.LogWarning("Feature not implemented");

    }
    public virtual void DecrementChangeCounter(int[] coordinate)
    {
        Debug.LogWarning("Feature not implemented");
    }
    protected virtual string CoordinateToString(int[] coord)
    {
        Debug.LogWarning("Feature not implemented");
        return "no override for coordinate to string";
    }
    public virtual int[] GetStrip(int[] coord_from, int[] coord_to, int time_index)
    {
        Debug.LogWarning("Feature not implemented");
        return null;
    }
    public virtual int[][] GetStripCoordinate(int[] coord_from, int[] coord_to)
    {
        Debug.LogWarning("Feature not implemented");
        return null;
    }
    public virtual Board GetBoardFromCoordinate(int[] coordinate)
    {
        Debug.LogWarning("Feature not implemented");
        return null;
    }

    public virtual void HightlightMoves(int[][] coordinates)
    {
        Debug.LogWarning("Feature not implemented");
    }
    public virtual void FlagCoordinatesForPieces(int[][] coordinates)
    {
        Debug.LogWarning("Feature not implemented");
    }
    public virtual void ClearFlag()
    {
        Debug.LogWarning("Feature not implemented");
    }
    public virtual bool GetFlag(int[] coordinate)
    {
        Debug.LogWarning("Feature not implemented");
        return false;
    }
    public virtual void HighlightClear()
    {
        Debug.LogWarning("Feature not implemented");
    }
    protected virtual string ArrayToString(int[] coordinate)
    {
        Debug.LogWarning("Feature not implemented");
        return "array to string not overridden for multiverse";
    }
    public virtual Board GetNearestBoard(Vector3 position)
    {
        Debug.LogWarning("Feature not implemented");
        return null;
    }
    public virtual bool IsStarting(int[] coordinate)
    {
        Debug.LogWarning("Feature not implemented");
        return false;
    }
    public Board GetRootBoard()
    {
        if (RootNode != null)
        {
            Board board = RootNode.GetBoard();
            return board;
        }
        else
        {
            Debug.LogWarning("Warning, no root node found");
        }
        return null;
    }
    public virtual void SetEnPassant(int[] coordinate)
    {

    }
}
