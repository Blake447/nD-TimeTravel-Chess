using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	[SerializeField] protected bool isPhysical;
    [SerializeField] protected PiecePallete pallete;
	[SerializeField] protected bool isInitialized;
    [SerializeField] protected int[] pieces;
	[SerializeField] protected int[] flags;
	[SerializeField] protected int[] changes;
	//[SerializeField] protected bool[] enpassant;
	[SerializeField] protected Square[] squares;
	[SerializeField] protected GameObject[] visualizers;
    [SerializeField] protected Square TemplateSquare;
    [SerializeField] protected GameObject SquaresRoot;
	[SerializeField] protected float[] mvtime_offsets = new float[] { 40.0f, 40.0f };
    [SerializeField] protected GameObject VisualizerTemplate;
    [SerializeField] protected GameObject VisualizerRoot;
    [SerializeField] protected int[] dimensions = new int[] {8, 16};
	[SerializeField] int[] previous_state;

    public void ConvertLegacy()
    {
        for (int i = 0; i < GetPieceCount(); i++)
        {
            int[] coord = IndexToCoordinate(i);
            int piece = GetPieceAt(coord);
            if (piece > 31)
            {
                piece = piece % 32 + Overseer.PIECE_COUNT;
                SetAtCoordinate(piece, coord);
            }
        }
    }

    public void MirrorState(int dimension_index, bool negativeToPositive)
    {
        if (dimension_index < dimensions.Length)
        {
            previous_state = (int[])RequestState().Clone();
            for (int i = 0; i < GetPieceCount(); i++)
            {
                int[] coord = IndexToCoordinate(i);
                int[] mirrored = (int[])coord.Clone();
                mirrored[dimension_index] = dimensions[dimension_index] - mirrored[dimension_index] - 1;
                bool flipRegular = coord[dimension_index] <= (dimensions[dimension_index] / 2);
                bool flipNegative = coord[dimension_index] >= (dimensions[dimension_index] / 2);
                bool flip = negativeToPositive ? flipNegative : flipRegular;
                if (flip)
                {
                    int piece_mirror = GetPieceAt(mirrored);
                    if (piece_mirror != 0)
                        SetAtCoordinate((piece_mirror + Overseer.PIECE_COUNT) % (Overseer.PIECE_COUNT*2), coord);
                    else
                        SetAtCoordinate(0, coord);


                }
            }
        }
    }
    public void MirrorStateDouble(int limiting, bool isNegativeToPositive)
    {
        if (limiting < dimensions.Length)
        {
            previous_state = (int[])RequestState().Clone();
            for (int i = 0; i < GetPieceCount(); i++)
            {
                int[] coord = IndexToCoordinate(i);
                int[] mirrored = (int[])coord.Clone();
                mirrored[1] = dimensions[1] - mirrored[1] - 1;
                mirrored[3] = dimensions[3] - mirrored[3] - 1;

                bool flipRegular = coord[limiting] <= (dimensions[limiting] / 2);
                bool flipNegative = coord[limiting] >= (dimensions[limiting] / 2);
                bool flip = isNegativeToPositive ? flipNegative : flipRegular;
                if (flip)
                {
                    int piece_mirror = GetPieceAt(mirrored);
                    if (piece_mirror != 0)
                        SetAtCoordinate((piece_mirror + Overseer.PIECE_COUNT) % (Overseer.PIECE_COUNT*2), coord);
                    else
                        SetAtCoordinate(0, coord);
                }
            }
        }
    }


    #region Initialization
    public virtual void InitializeBoard(PiecePallete pallete, int[] state, bool isPhysical)
    {
        SetDimensions();
        this.pallete = pallete;
        this.isPhysical = isPhysical;
        if (!isInitialized)
        {
            InitializePieceArray();
            GenerateSquareArrays(pallete);
            if (state != null)
            {
                PopulateState(state);
                if (this.isPhysical)
                    SetAllMeshDisplays();
            }
            isInitialized = true;
        }
    }
    public virtual void InitializePieceArray()
    {
        int length = CalculateLength();
        pieces = new int[length];
        flags = new int[length];
        changes = new int[length];
        //enpassant = new bool[length];
    }
    public virtual void GenerateSquareArrays(PiecePallete pallete)
    {
        int length = CalculateLength();
        if (isPhysical)
        {
            squares = new Square[length];
            visualizers = new GameObject[length];
            if (TemplateSquare != null)
            {
                for (int i = 0; i < squares.Length; i++)
                {
                    int[] coordinate = IndexToCoordinate(i);
                    squares[i] = Instantiate(TemplateSquare);
                    squares[i].InitializeSquare(pallete);
                    squares[i].transform.position = CoordinateToPosition(coordinate);
                    string name = "square_" + i.ToString("000");
                    squares[i].name = name;
                    squares[i].transform.parent = SquaresRoot.transform;
                    squares[i].transform.localRotation = CoordinateToRotation(coordinate);
                }
            }
            if (VisualizerTemplate != null)
            {
                for (int i = 0; i < visualizers.Length; i++)
                {
                    int[] coordinate = IndexToCoordinate(i);
                    visualizers[i] = Instantiate(VisualizerTemplate);
                    visualizers[i].transform.position = CoordinateToPosition(coordinate);
                    string name = "visualizer_" + i.ToString("000");
                    visualizers[i].name = name;
                    visualizers[i].transform.parent = VisualizerRoot.transform;
                    visualizers[i].transform.localRotation = CoordinateToRotation(coordinate);
                    visualizers[i].SetActive(false);
                }
            }
        }
    }
    public virtual void SetDimensions()
    { 
    }
    public virtual void PopulateState(int[] piece_array)
    {
        InitializePieceArray();
        for (int i = 0; i < Mathf.Min(piece_array.Length, pieces.Length); i++)
            pieces[i] = piece_array[i];
        if (piece_array.Length != pieces.Length)
            Debug.LogWarning("Warning, piece count mismatch, make sure board state loaded is compatible with board");
    }
    public virtual void SetAllMeshDisplays()
    {
        if (isPhysical)
            for (int i = 0; i < squares.Length; i++)
                squares[i].SetMesh(pieces[i]);
    }
    #endregion

    #region Getters and Setters
    public virtual void TransferState(int[] state, int[] changes, bool[] enpassant)
    {
        if (this.pieces == null)
        {
            this.pieces = new int[state.Length];
        }
        if (this.changes == null)
        {
            this.changes = new int[changes.Length];
        }
        //if (this.enpassant == null)
        //{
        //    this.enpassant = new bool[enpassant.Length];
        //}


        for (int i = 0; i < Mathf.Min(state.Length, pieces.Length); i++)
        {
            pieces[i] = state[i];
            if (pieces[i] % Overseer.PIECE_COUNT == 30) pieces[i] = 0;
            this.changes[i] = changes[i];
            //this.enpassant[i] = enpassant[i];
        }
        if (isPhysical)
            SetAllMeshDisplays();
    }
    public virtual int[] RequestState()
    {
        return (int[])pieces.Clone();
    }
    public virtual int[] RequestChanges()
    {
        return (int[])changes.Clone();
    }
    //public virtual bool[] RequestEnPassant()
    //{
    //    return (bool[])enpassant.Clone();
    //}
    public void ClearGhostPawns()
    {
        int pieceCount = GetPieceCount();
        for (int i = 0; i < pieceCount; i++)
        {
            int[] coord = IndexToCoordinate(i);
            int piece_at = GetPieceAt(i);
            if ( (piece_at % Overseer.PIECE_COUNT) == 30 )
            {
                SetAtCoordinate(0, coord);
            }
        }
    }
    public virtual void SetAtCoordinate(int piece_index, int[] coordinate)
    {
        if (coordinate != null)
        {
            if (IsInBounds(coordinate))
            {
                int index = CoordinateToIndex(coordinate);
                pieces[index] = piece_index;
                if (isPhysical)
                    squares[index].SetMesh(piece_index);
            }
        }
    }
    public virtual void IncrementChangeCounter(int[] coordinate)
    {
        if (IsInBounds(coordinate))
        {
            int index = CoordinateToIndex(coordinate);
            changes[index]++;
        }
    }
    public virtual void DecrementChangeCounter(int[] coordinate)
    {
        if (IsInBounds(coordinate))
        {
            int index = CoordinateToIndex(coordinate);
            changes[index]--;
            changes[index] = Mathf.Max(changes[index], 0);
        }
    }
    public virtual int GetChangeCounter(int[] coordinate)
    {
        if (IsInBounds(coordinate))
        {
            int index = CoordinateToIndex(coordinate);
            return changes[index];
        }
        return -1;
    }
    //public virtual void SetEnpassantFlag(int[] coordinate)
    //{
    //    if (coordinate != null)
    //    {
    //        if (IsInBounds(coordinate))
    //        {
    //            int index = CoordinateToIndex(coordinate);
    //            enpassant[index] = true;
    //        }
    //    }
    //}
    //public virtual void ClearEnpassantFlags()
    //{
    //    for (int i = 0; i < enpassant.Length; i++)
    //        enpassant[i] = false;
    //}
    public void ClearEnpassant()
    {
        for (int i= 0; i < pieces.Length; i++)
        {
            if (pieces[i] % Overseer.PIECE_COUNT == 30)
            {
                pieces[i] = 0;
            }
        }
    }
    //public virtual bool GetEnPassantFlag(int[] coordinate)
    //{
    //    if (coordinate != null)
    //    {
    //        if (IsInBounds(coordinate))
    //        {
    //            int index = CoordinateToIndex(coordinate);
    //            return enpassant[index];
    //        }
    //    }
    //    return false;
    //}
    public virtual void FlagSquare(int[] coordinate)
    {
        int index = CoordinateToIndex(coordinate);
        if (index >= 0 && index < flags.Length)
            flags[index] = 1;
    }
    public virtual void FlagClear()
    {
        for (int i = 0; i < visualizers.Length; i++)
            flags[i] = 0;
    }
    public virtual int GetFlag(int[] coordinate)
    {
        if (coordinate != null)
        {
            int index = CoordinateToIndex(coordinate);
            if (index >= 0 && index < flags.Length)
                return flags[index];
        }
        return 0;
    }
    public virtual int GetPieceAt(int[] coordinate)
    {
        if (IsInBounds(coordinate))
        {
            int index = CoordinateToIndex(coordinate);
            int piece = pieces[index];
            return piece;
        }
        return -1;
    }
    public virtual int GetPieceAt(int index)
    {
        return pieces[index];
    }
    public virtual int GetPieceCount()
    {
        return pieces.Length;
    }
    #endregion
    public virtual int CalculateLength()
    {
        if (dimensions != null)
        {
            int length = 1;
            for (int i = 0; i < dimensions.Length; i++)
                length *= Mathf.Max(1, dimensions[i]);
            return length;
        }
        return 0;
    }
    public virtual int[] GetBoardSize()
    {
        return dimensions;
    }

    #region Board Interface
    public virtual void HighlightSquare(int[] coordinate)
    {
        if (isPhysical)
        {
            int index = CoordinateToIndex(coordinate);
            if (index >= 0 && index < visualizers.Length)
            {
                visualizers[index].SetActive(true);
            }
        }
    }
    public virtual void HighlightClear()
    {
        if (isPhysical)
        {
            if (visualizers != null)
                for (int i = 0; i < visualizers.Length; i++)
                    visualizers[i].SetActive(false);
        }
    }
    #endregion



    #region Overridable
    public virtual Quaternion CoordinateToRotation(int[] coordinate)
    {
        return Quaternion.identity;
    }

    #endregion
    public void InitializeSquare(Square square)
    {

    }

    public virtual void ReprocessCoordinates(int[] coordinate_a, int[] coordinate_b)
    {
        
    }
    public virtual Vector3 GetCenter()
    {
        return this.transform.position;
    }
    public virtual Vector3 SnapCamera(Vector3 vector3)
    {
        return Vector3.zero;
    }
    public virtual int[] PositionToCoordinate(Vector3 position)
    {

        return null;
    }
    public virtual Vector3 CoordinateToPosition(int[] coordinate)
    {

        return Vector3.zero;
    }
    public virtual int CoordinateToIndex(int[] coorindate)
    {
        return -1;
    }
    public virtual int[] IndexToCoordinate(int index)
    {
        return null;
    }
    public virtual bool IsInBounds(int[] coordinate)
    {
        return false;
    }

    public virtual bool HasCoordinateChanged()
    {
        return false;
    }
    public static int[] IndexToCoordinate(int index, int[] dimensions)
    {
        int i = index;
        int[] coordinate = new int[4];
        for (int j = 0; j < coordinate.Length; j++)
        {
            coordinate[j] = i % dimensions[j];
            i = i / dimensions[j];
        }
        return coordinate;
    }
    public static int CoordinateToIndex(int[] coordinate, int[] dimensions)
    {
        int scale = 1;
        int index = 0;
        for (int i = 0; i < Mathf.Min(coordinate.Length, dimensions.Length); i++)
        {
            index += scale * coordinate[i];
            scale *= dimensions[i];
        }
        return index;
    }
    public static int CalculateLength(int[] dimensions)
    {
        int length = 1;
        for (int i = 0; i < dimensions.Length; i++)
            length *= Mathf.Max(1, dimensions[i]);
        return length;
    }
}
