using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parametric : Board
{
    int offx = 0;
    int offy = 0;
    int[] board_dimensions = new int[2] { 8, 16 };
    int global_horizontal_wrapping = 1;
    int global_vertical_wrapping = 2;
    float phase = 1.20f;
    float radius = 47.41f;
    float offset = 0.170f;
    float min = 0.09f;

    public Vector2 Offsets;



    Vector2 scroll = new Vector2(1.0f / 16.0f, 1.0f / 32.0f) + new Vector2(0.0f, 1.0f / 16.0f) * 8;
    [SerializeField] GameObject HitboxTemplate;
    [SerializeField] GameObject HitboxRoot;
    [SerializeField] Material boardMaterial;
    GameObject[] Hitboxes;


    public override void InitializeBoard(PiecePallete pallete, int[] state, bool isPhysical)
    {
        SetDimensions();
        if (this.pallete == null)
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


    public void ChangeX(float value)
    {
        Offsets = new Vector2(value, Offsets.y);
        OnParameterChange();
    }
    public void ChangeY(float value)
    {
        Offsets = new Vector2(Offsets.x, value);
        OnParameterChange();
    }

    public void OnParameterChange()
    {
        int length = CalculateLength();
        if (isPhysical)
        {
            if (TemplateSquare != null)
            {
                for (int i = 0; i < squares.Length; i++)
                {
                    int[] coordinate = IndexToCoordinate(i);
                    squares[i].transform.position = CoordinateToPosition(coordinate);
                    squares[i].transform.localRotation = CoordinateToRotation(coordinate);
                }
            }
            if (VisualizerTemplate != null)
            {
                for (int i = 0; i < visualizers.Length; i++)
                {
                    int[] coordinate = IndexToCoordinate(i);
                    visualizers[i].transform.position = CoordinateToPosition(coordinate);
                    visualizers[i].transform.localRotation = CoordinateToRotation(coordinate);
                }
            }
        }
        if (boardMaterial != null)
        {
            boardMaterial.SetVector("_Scroll", new Vector4(-Offsets.x, Offsets.y + 0.5075f, 0.0f, 0.0f));
        }
    }


    public override void SetDimensions()
    {
        base.dimensions = (int[])board_dimensions.Clone();
    }
    public override void GenerateSquareArrays(PiecePallete pallete)
    {
        base.GenerateSquareArrays(this.pallete);
        InitilizeHitboxes();
    }
    public void InitilizeHitboxes()
    {
        int length = dimensions[0] * dimensions[1];
        Hitboxes = new GameObject[length];
        for (int i = 0; i < length; i++)
        {
            Hitboxes[i] = Instantiate(HitboxTemplate);
            int x = i % (dimensions[0]);
            int y = i / (dimensions[0]);
            int[] c = new int [] { x, y };
            Hitboxes[i].transform.parent = HitboxRoot.transform;
            Hitboxes[i].transform.localPosition = F(c)*5.0f;
            Hitboxes[i].transform.localRotation = Quaternion.LookRotation(T(c), N(c));
            Vector2 scaling = Scaling(c);
            Hitboxes[i].transform.localScale = new Vector3(scaling.x*3.5f, 0.125f, scaling.y*2.5f);
        }
    }

    // brute force this shit
    public override int[] PositionToCoordinate(Vector3 position)
    {
        int minx = 0;
        int miny = 0;
        float minDist = 1000.0f;
        for (int x = 0; x < dimensions[0]; x++)
        {
            for (int y = 0; y < dimensions[1]; y++)
            {
                Vector3 pos = CoordinateToPosition(new int[] { x, y });
                if (Vector3.Distance(pos, position) < minDist)
                {
                    minDist = Vector3.Distance(pos, position);
                    minx = x;
                    miny = y;
                }
            }
        }
        return new int[] { minx, miny };
    }
    public override Vector3 CoordinateToPosition(int[] coordinate)
    {
        if (coordinate == null)
            return this.transform.position;
        int[] coord = (int[])coordinate.Clone();
        Vector3 position = F(new int[2] { coordinate[0], coordinate[1] }) * 5.0f;
        return this.transform.localToWorldMatrix * new Vector4(position.x, position.y, position.z, 1.0f);
    }
    public override Quaternion CoordinateToRotation(int[] coordinate)
    {
        return Quaternion.LookRotation(N(new int[] { coordinate[0], coordinate[1] }), T(new int[] { coordinate[0], coordinate[1] }));
    }
    public override Vector3 SnapCamera(Vector3 position)
    {
        int[] coordinate = PositionToCoordinate(position);
        if (coordinate != null)
        {
            Vector3 a = CoordinateToPosition(new int[] { 0, 0 });
            Vector3 b = CoordinateToPosition(new int[] { 7, 7 });
            Vector3 m = (a + b) * 0.5f;
            return m;
        }
        return position;
    }
    public override Vector3 GetCenter()
    {
        Vector3 a1 = CoordinateToPosition(new int[] { 0, 0 });
        Vector3 a2 = CoordinateToPosition(new int[] { 7, 7 });
        Vector3 center = (a1 + a2) * 0.5f;
        return center;
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

    public virtual Vector3 F(int[] coord)
    {
        int nx = 8;
        int ny = 16;

        float x = (float)coord[0] / (float)(board_dimensions[0]);
        float y = (float)coord[1] / (float)(board_dimensions[1]);

        x = (x + scroll.x + Offsets.x) - Mathf.Floor(x + scroll.x + Offsets.x);
        y = (y + scroll.y + Offsets.y) - Mathf.Floor(y + scroll.y + Offsets.y);
        if ((y / 2) - Mathf.Floor(y / 2) >= 0.5f)
            x = nx - x;
        else
            x = x - Mathf.Floor(x / nx) * nx;
        y = y - Mathf.Floor(y / ny) * ny;
        Vector3 v = new Vector3(x * 2.0f * Mathf.PI, y * 2.0f * Mathf.PI, 0.0f);
        float phi = v.y + phase;

        
        float t = v.y;
        float t2 = 2*Mathf.PI - t;
        Vector2 scale = new Vector2(0.5f, 1.0f);
        Vector2 S1 = new Vector2(0.25f * t * t * Mathf.Sin(t), 2f * Mathf.Sin(0.5f * t)) * scale;
        Vector2 S2 = new Vector2(0.25f * t2 * t2 * Mathf.Sin(t), 2f * Mathf.Sin(0.5f * t)) * scale;
        Vector2 T1 = new Vector2(0.5f * t * Mathf.Sin(t) + 0.25f * t * t * Mathf.Cos(t), Mathf.Cos(0.5f * t)) * scale;
        Vector2 T2 = new Vector2(-0.5f * t2 * Mathf.Sin(t) + 0.25f * t2 * t2 * Mathf.Cos(t), Mathf.Cos(0.5f * t)) * scale;
        Vector2 S = t > Mathf.PI ? S2 : S1;
        Vector2 T = t > Mathf.PI ? T2 : T1;
        float f = (Mathf.Sin(phi) * radius * .01f + offset);
        float r = 4 * f * f * f * f * f+ min;
        Vector3 start = new Vector3(0.0f, S.x, S.y);
        Vector3 tangent = new Vector3(0.0f, T.x, T.y);
        Vector3 side = Vector3.right;
        Vector3 normal = r * Vector3.Cross(tangent, side).normalized * Mathf.Sin(v.x) + r * side * Mathf.Cos(v.x);
        Vector3 vertex = start + normal;
        return vertex;
    }

    //public virtual Vector3 F(int[] coord)
    //{
    //    float x = (float)coord[0] / (float)(dimensions[0]) + 0.01f;
    //    float y = (float)coord[1] / (float)(dimensions[1]) + 0.01f;

    //    x = (x + scroll.x + Offsets.x) - Mathf.Floor(x + scroll.x + Offsets.x);
    //    y = (y + scroll.y + Offsets.y) - Mathf.Floor(y + scroll.y + Offsets.y);


    //    Vector3 v = new Vector3(x * 2.0f * Mathf.PI, y * 2.0f * Mathf.PI, 0.0f);
    //    float phi = v.y + phase;
    //    Vector2 S = new Vector2(2 * ((1.0f / 16.0f) * Mathf.Sin(v.y) + (1.0f / 32.0f) * Mathf.Sin(2 * v.y)), 0.5f + 0.5f * Mathf.Cos(v.y));
    //    Vector2 T = new Vector2(2 * ((1.0f / 16.0f) * Mathf.Cos(v.y) + (1.0f / 16.0f) * Mathf.Cos(2 * v.y)), 0.0f - 0.5f * Mathf.Sin(v.y));
    //    float f = (Mathf.Sin(phi) * radius * .01f + offset);
    //    float r = 6 * f * f * f + min;
    //    Vector3 start = new Vector3(0.0f, S.x, S.y);
    //    Vector3 tangent = new Vector3(0.0f, T.x, T.y);
    //    Vector3 side = Vector3.right;
    //    Vector3 normal = r * Vector3.Cross(tangent, side).normalized * Mathf.Sin(v.x) + r * side * Mathf.Cos(v.x);
    //    Vector3 vertex = start + normal;
    //    return vertex;
    //}
    public virtual Vector3 T(int[] coord)
    {
        Vector3 F0 = F(new int[] { coord[0] + 0, coord[1] - 1 });
        Vector3 F1 = F(new int[] { coord[0] + 0, coord[1] + 1 });
        return (F1*5.0f - F0*5.0f).normalized;
    }
    public virtual Vector3 N(int[] coord)
    {
        Vector3 F0 = F(new int[] { coord[0] - 1, coord[1] + 0 });
        Vector3 F1 = F(new int[] { coord[0] + 1, coord[1] + 0 });
        Vector3 F2 = F(new int[] { coord[0] + 0, coord[1] + 1 });
        Vector3 F3 = F(new int[] { coord[0] + 0, coord[1] - 1 });

        return -Vector3.Cross(F1 * 5.0f - F0 * 5.0f, F3 * 5.0f - F2 * 5.0f).normalized;
    }
    public Vector2 Scaling(int[] coord)
    {
        Vector3 F0 = F(new int[] { coord[0] - 1, coord[1] + 0 });
        Vector3 F1 = F(new int[] { coord[0] + 1, coord[1] + 0 });
        Vector3 F2 = F(new int[] { coord[0] + 0, coord[1] - 1 });
        Vector3 F3 = F(new int[] { coord[0] + 0, coord[1] + 1 });
        return new Vector2(Vector3.Distance(F0, F1), Vector3.Distance(F2, F3));
    }
    public Vector4 HorizontalWrapping(Vector2Int coordinate, Vector2Int direction)
    {
        int mode = global_horizontal_wrapping;
        int nx = 8;
        int ny = 16; // Number of squares
        bool isLeft = false;
        bool isRight = false;

        coordinate = new Vector2Int(coordinate.x - offx, coordinate.y - offy);

        if (coordinate.x >= nx)
            isRight = true;
        if (coordinate.x < 0)
            isLeft = true;
        if (mode == 1) //forward
        {
            if (isLeft)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(nx, 0, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x + offx, coord.y + offy, dir.x, dir.y);

            }
            else if (isRight)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(-nx, 0, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x + offx, coord.y + offy, dir.x, dir.y);

            }
        }
        if (mode == 2) //reverse
        {
            if (isLeft)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, -1, 0, 0);
                Vector4 col2 = new Vector4(nx, ny-1, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x + offx, coord.y + offy, dir.x, dir.y);

            }
            else if (isRight)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, -1, 0, 0);
                Vector4 col2 = new Vector4(-nx, ny-1, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x + offx, coord.y + offy, dir.x, dir.y);

            }
        }
        return new Vector4((coordinate.x + offx), (coordinate.y + offy), direction.x, direction.y);
    }
    public Vector4 VerticalWrapping(Vector2Int coordinate, Vector2Int direction)
    {
        int[] before = new int[] { coordinate.x, coordinate.y };
        int mode = global_vertical_wrapping;
        int nx = 8;
        int ny = 16; // Number of squares
        bool isTop = false;
        bool isBottom = false;

        coordinate = new Vector2Int(coordinate.x - offx, coordinate.y - offy);

        if (coordinate.y >= ny)
            isTop = true;
        if (coordinate.y < 0)
            isBottom = true;
        if (mode == 1) //forward
        {
            if (isBottom)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(0, ny, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                Debug.Log(Coordinates.CoordinateToString(new int[] { coordinate.x, coordinate.y }) + " -> " + Coordinates.CoordinateToString(new int[] { Mathf.RoundToInt(coord.x), Mathf.RoundToInt(coord.y) }));
                Debug.Log(Coordinates.CoordinateToString(before) + " -> "+ Coordinates.CoordinateToString(new int[] { Mathf.RoundToInt(coord.x) + offx, Mathf.RoundToInt(coord.y) + offy }));
                return new Vector4(coord.x + offx, coord.y + offy, dir.x, dir.y);


            }
            else if (isTop)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(0, -ny, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));
                return new Vector4(coord.x + offx, coord.y + offy, dir.x, dir.y);


            }
        }
        if (mode == 2) //reverse
        {
            if (isBottom)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(nx, ny, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));

                Debug.Log(Coordinates.CoordinateToString(new int[] { coordinate.x, coordinate.y }) + " -> " + Coordinates.CoordinateToString(new int[] { Mathf.RoundToInt(coord.x), Mathf.RoundToInt(coord.y) }));
                return new Vector4(coord.x + offx, coord.y + offy, dir.x, dir.y);
            }
            else if (isTop)
            {
                Vector4 col0 = new Vector4(1, 0, 0, 0);
                Vector4 col1 = new Vector4(0, 1, 0, 0);
                Vector4 col2 = new Vector4(nx, -ny, 0, 0);
                Vector4 col3 = new Vector4(0, 0, 0, 0);

                Matrix4x4 mat = new Matrix4x4(col0, col1, col2, col3);
                Vector4 coord = mat * (new Vector4(coordinate.x, coordinate.y, 1, 0));
                Vector4 dir = mat * (new Vector4(direction.x, direction.y, 0, 0));

                Debug.Log(Coordinates.CoordinateToString(new int[] { coordinate.x, coordinate.y }) + " -> " + Coordinates.CoordinateToString(new int[] { Mathf.RoundToInt(coord.x), Mathf.RoundToInt(coord.y) }));
                return new Vector4(coord.x + offx, coord.y + offy, dir.x, dir.y);
            }
        }
        return new Vector4((coordinate.x + offx), (coordinate.y + offy), direction.x, direction.y);

    }
}
