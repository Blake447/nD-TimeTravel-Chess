using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class topological : Board
{
    public string fileName = "standard";

    float xwidth = 1.0f;
    float ywidth = 1.0f;
    float zwidth = 2.0f;
    float wwidth = 6.0f;

    int[] board_dimensions = new int[2] { 8, 8 };
    int king = 1;

    int global_horizontal_wrapping = 1;
    int global_vertical_wrapping = 2;


    public Vector4 HorizontalWrapping(Vector2Int coordinate, Vector2Int direction)
    {
        int mode = global_horizontal_wrapping;
        int n = 8; // Number of squares
        bool isLeft = false;
        bool isRight = false;

        if (coordinate.x >= n)
            isRight = true;
        if (coordinate.x < 0)
            isLeft = true;
        if (mode == 1) //forward
        {
            if (isLeft)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(n, 0, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x, coord.y, dir.x, dir.y);
            }
            else if (isRight)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(-n, 0, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x, coord.y, dir.x, dir.y);
            }
        }
        if (mode == 2) //reverse
        {
            if (isLeft)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, -1, 0, 0);
                Vector4 col2 = new Vector4(n, n-1, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x, coord.y, dir.x, dir.y);
            }
            else if (isRight)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, -1, 0, 0);
                Vector4 col2 = new Vector4(-n, n-1, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x, coord.y, dir.x, dir.y);
            }
        }
        return new Vector4(coordinate.x, coordinate.y, direction.x, direction.y);
    }

    public Vector4 VerticalWrapping(Vector2Int coordinate, Vector2Int direction)
    {
        int mode = global_vertical_wrapping;
        int n = 8; // Number of squares
        bool isTop = false;
        bool isBottom = false;

        if (coordinate.y >= n)
            isTop = true;
        if (coordinate.y < 0)
            isBottom = true;
        if (mode == 1) //forward
        {
            if (isBottom)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(0, n, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x, coord.y, dir.x, dir.y);
            }
            else if (isTop)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(0, -n, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x, coord.y, dir.x, dir.y);
            }
        }
        if (mode == 2) //reverse
        {
            if (isBottom)
            {
                Vector4 col0 = new Vector4(-1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(n-1, n, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x, coord.y, dir.x, dir.y);
            }
            else if (isTop)
            {
                Vector4 col0 = new Vector4(-1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(n-1, -n, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x, coord.y, dir.x, dir.y);
            }
        }
        return new Vector4(coordinate.x, coordinate.y, direction.x, direction.y);
    }


    public override void ReprocessCoordinates(int[] coordinate_a, int[] coordinate_b)
    {
        base.ReprocessCoordinates(coordinate_a, coordinate_b);


        int[] ac = new int[] { coordinate_a[0], coordinate_a[1] };
        int[] bc = new int[] { coordinate_b[0], coordinate_b[1] };

        ac = (int[])BlindProcessHorizontal(ac).Clone();
        ac = (int[])BlindProcessVertical(ac).Clone();
        ac = (int[])BlindProcessHorizontal(ac).Clone();
        ac = (int[])BlindProcessVertical(ac).Clone();

        bc = (int[])BlindProcessHorizontal(bc).Clone();
        bc = (int[])BlindProcessVertical(bc).Clone();
        bc = (int[])BlindProcessHorizontal(bc).Clone();
        bc = (int[])BlindProcessVertical(bc).Clone();

        coordinate_a[0] = ac[0];
        coordinate_a[1] = ac[1];
        coordinate_b[0] = bc[0];
        coordinate_b[1] = bc[1];
    }
    public override int[] RequestState()
    {
        return (int[])pieces.Clone();
    }
    private void Start()
    {
        //InitializeBoard();
    }
    public override Vector3 GetCenter()
    {
        Vector3 a1 = CoordinateToPosition(new int[] { 0, 0 });
        Vector3 a2 = CoordinateToPosition(new int[] { 7, 7 });
        Vector3 center = (a1 + a2) * 0.5f;
        return center;
    }
    public override void SetAtCoordinate(int piece_index, int[] coordinate)
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
    public override void HighlightSquare(int[] coordinate)
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
    public override void HighlightClear()
    {
        if (isPhysical)
        {
            if (visualizers != null)
                for (int i = 0; i < visualizers.Length; i++)
                    visualizers[i].SetActive(false);
        }
    }
    public override void FlagSquare(int[] coordinate)
    {
        int index = CoordinateToIndex(coordinate);
        if (index >= 0 && index < flags.Length)
            flags[index] = 1;
    }
    public override void FlagClear()
    {
        for (int i = 0; i < visualizers.Length; i++)
            flags[i] = 0;
    }
    public override int GetFlag(int[] coordinate)
    {
        if (coordinate != null)
        {
            int index = CoordinateToIndex(coordinate);
            if (index >= 0 && index < flags.Length)
                return flags[index];
        }
        return 0;
    }
    //public override void SetEnpassantFlag(int[] coordinate)
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
    //public override void ClearEnpassantFlags()
    //{
    //    for (int i = 0; i < enpassant.Length; i++)
    //        enpassant[i] = false;
    //}
    //public override bool GetEnPassantFlag(int[] coordinate)
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
    public override void IncrementChangeCounter(int[] coordinate)
    {
        if (IsInBounds(coordinate))
        {
            int index = CoordinateToIndex(coordinate);
            changes[index]++;
        }
    }
    public override void DecrementChangeCounter(int[] coordinate)
    {
        if (IsInBounds(coordinate))
        {
            int index = CoordinateToIndex(coordinate);
            changes[index]--;
            changes[index] = Mathf.Max(changes[index], 0);
        }
    }
    public override int GetChangeCounter(int[] coordinate)
    {
        if (IsInBounds(coordinate))
        {
            int index = CoordinateToIndex(coordinate);
            return changes[index];
        }
        return -1;
    }

    public override Vector3 SnapCamera(Vector3 position)
    {
        int[] coordinate = PositionToCoordinate(position);
        if (coordinate != null)
        {
            Vector3 a = CoordinateToPosition(new int[] { 0, 0 } );
            Vector3 b = CoordinateToPosition(new int[] { 7, 7 } );
            Vector3 m = (a + b) * 0.5f;
            return m;
        }
        return position;
    }
    public override int GetPieceAt(int[] coordinate)
    {
        if (IsInBounds(coordinate) )
        {
            int index = CoordinateToIndex(coordinate);
            int piece = pieces[index];
            return piece;
        }
        return 0;
    }
    public override int GetPieceCount()
    {
        return pieces.Length;
    }
    public override int GetPieceAt(int index)
    {
        return pieces[index];
    }
    bool IsInBounds(int index)
    {
        return (index >= 0 && index < pieces.Length);
    }
    private int CalculateLength()
    {
        int length = 1;
        for (int i = 0; i < dimensions.Length; i++)
            length *= Mathf.Max(1, dimensions[i]);
        return length;
    }
    public override int[] IndexToCoordinate(int index)
    {
        int i = index;
        int[] coordinate = new int[2];
        for (int j = 0; j < coordinate.Length; j++)
        {
            coordinate[j] = i % dimensions[j];
            i = i / dimensions[j];
        }
        return coordinate;
    }
    private void GenerateSquareArrays(PiecePallete pallete)
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
                    squares[i].transform.localRotation = Quaternion.identity;
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
                    visualizers[i].transform.localRotation = Quaternion.identity;
                    visualizers[i].SetActive(false);
                }
            }
        }
    }
    public void SetAllMeshDisplays()
    {
        if (isPhysical)
        {
            for (int i = 0; i < squares.Length; i++)
                squares[i].SetMesh(pieces[i]);
        }
    }
    public override int[] GetBoardSize()
    {
        return dimensions;
    }
    public override void InitializeBoard(PiecePallete pallete, int[] state, bool isPhysical)
    {
        this.isPhysical = isPhysical;
        if (!isInitialized)
        {
            InitializePieceArray();
            GenerateSquareArrays(pallete);
            //BoardState state = BoardLoader.LoadBoardState(fileName + ".data");
            
            //BoardState state = BoardLoader.LoadBoardState("standard2D" + ".data");
            if (state != null)
            {
                PopulateState(state);
                if (this.isPhysical)
                    SetAllMeshDisplays();
            }
            isInitialized = true;
        }
    }
    public override int CoordinateToIndex(int[] coordinate)
    {
        int scale = 1;
        int index = 0;

        coordinate = (int[])BlindProcessHorizontal(coordinate).Clone();
        coordinate = (int[])BlindProcessVertical(coordinate).Clone();
        coordinate = (int[])BlindProcessHorizontal(coordinate).Clone();
        coordinate = (int[])BlindProcessVertical(coordinate).Clone();


        for (int i = 0; i < Mathf.Min(coordinate.Length, dimensions.Length); i++)
        {
            index += scale * (coordinate[i] % dimensions[i]);
            scale *= dimensions[i];
        }
        return index;
    }
    int[] BlindProcessHorizontal(int[] coordinate)
    {
        Vector4 multiplied = HorizontalWrapping(new Vector2Int(coordinate[0], coordinate[1]), Vector2Int.zero);
        return new int[2] { Mathf.RoundToInt(multiplied.x), Mathf.RoundToInt(multiplied.y) };
    }
    int[] BlindProcessVertical(int[] coordinate)
    {
        Vector4 multiplied = VerticalWrapping(new Vector2Int(coordinate[0], coordinate[1]), Vector2Int.zero);
        return new int[2] { Mathf.RoundToInt(multiplied.x), Mathf.RoundToInt(multiplied.y) };
    }
    public override bool IsInBounds(int[] coordinate)
    {

        if (coordinate == null)
            return false;
        coordinate = (int[])BlindProcessHorizontal(coordinate).Clone();
        coordinate = (int[])BlindProcessVertical(coordinate).Clone();
        coordinate = (int[])BlindProcessHorizontal(coordinate).Clone();
        coordinate = (int[])BlindProcessVertical(coordinate).Clone();

        for (int i = 0; i < Mathf.Min(coordinate.Length, dimensions.Length); i++)
            if (coordinate[i] < 0 || coordinate[i] >= dimensions[i])
                return false;
        return true;
    }
    public void InitializePieceArray()
    {
        int length = CalculateLength();
        pieces = new int[length];
        flags = new int[length];
        changes = new int[length];
        //enpassant = new bool[length];
    }
    public override void PopulateState(int[] piece_array)
    {
        InitializePieceArray();
        for (int i = 0; i < Mathf.Min(piece_array.Length, pieces.Length); i++)
            pieces[i] = piece_array[i];
        if (piece_array.Length != pieces.Length)
            Debug.LogWarning("Warning, piece count mismatch, make sure board state loaded is compatible with board");
    }
    public override int[] PositionToCoordinate(Vector3 position)
    {
        Vector3 local_offset = this.transform.worldToLocalMatrix * new Vector4(position.x, position.y, position.z, 1.0f);
        local_offset = Vector3.Scale(local_offset, new Vector3(-1, 1, 1));
        Vector3 offset = local_offset;
        //offset = offset + new Vector3(2, 2, 0);
        int offset_x = Mathf.FloorToInt(offset.x);
        int offset_y = Mathf.FloorToInt(offset.y);
        int[] coordinate = new int[2] { offset_x, offset_y };
        if (IsWithinBounds(coordinate))
            return coordinate;
        return null;
    }
    public override Vector3 CoordinateToPosition(int[] coordinate)
    {
        if (coordinate == null)
            return this.transform.position;
        int[] coords = new int[2] { 0, 0 };
        for (int i = 0; i < Mathf.Min(coords.Length, coordinate.Length); i++)
            coords[i] = coordinate[i];
        int x = coords[0];
        int y = coords[1];
        Vector3 rt = new Vector3(1 , 0, 0);
        Vector3 up = new Vector3(0 , 0, 1);
        Vector3 fw = new Vector3(0 , 1, 0);
        Vector3 position = x * rt + y * fw + new Vector3(0.5f, 0.5f, 0);
        position = Vector3.Scale(position, new Vector3(-1, 1, 1));
        return this.transform.localToWorldMatrix * new Vector4(position.x, position.y, position.z, 1.0f);
    }
    public bool IsWithinBounds(int[] coordinate)
    {
        return true;

        int[] bounds = (int[])dimensions.Clone();
        for (int i = 0; i < Mathf.Min(coordinate.Length, bounds.Length); i++)
            if (coordinate[i] < 0 || coordinate[i] >= bounds[i])
                return false;
        return true;
    }
}
