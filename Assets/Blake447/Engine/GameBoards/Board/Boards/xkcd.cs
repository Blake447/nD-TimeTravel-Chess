using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xkcd : Board
{
    int[] board_dimensions = new int[] {8, 8, 8, 8, 8 };
    
    public override void SetDimensions()
    {
        base.dimensions = (int[])board_dimensions.Clone();
    }


    float x_scale = 1f;
    float y_scale = 1f;
    float z_scale = 12.0f;
    float w_scale = 1.5f;
    float u_scale = 12.0f;


    public override int[] PositionToCoordinate(Vector3 position)
    {
        Vector3 local_offset = position;
        local_offset = this.transform.worldToLocalMatrix * new Vector4(local_offset.x, local_offset.y, local_offset.z, 1.0f);
        local_offset = Vector3.Scale(local_offset, new Vector3(-1, 1, 1));
        Vector3 offset = local_offset;
        int offset_u = Mathf.FloorToInt(offset.y / u_scale);
        offset = local_offset - offset_u * Vector3.up * u_scale;

        int offset_x = Mathf.FloorToInt(offset.x / x_scale);
        int offset_y = Mathf.FloorToInt(offset.y / y_scale);
        int offset_z = Mathf.RoundToInt(offset.z / z_scale);
        offset = local_offset - offset_z * Vector3.forward * z_scale;
        int offset_w = Mathf.RoundToInt(offset.z / w_scale);

        int[] coordinate = new int[5] { offset_x, offset_y, offset_z + 3, offset_w + 3, offset_u + 3};
        //Debug.Log("Coordinate: " + Coordinates.CoordinateToString(coordinate));
        if (IsInBounds(coordinate))
            return coordinate;
        return null;
    }
    public override Vector3 CoordinateToPosition(int[] coordinate)
    {
        if (coordinate == null)
            return this.transform.position;
        int[] coords = new int[] { 0, 0, 0, 0, 0 };
        for (int i = 0; i < Mathf.Min(coords.Length, coordinate.Length); i++)
            coords[i] = coordinate[i];
        int x = coords[0];
        int y = coords[1];
        int z = coords[2];
        int w = coords[3];
        int u = coords[4];
        Vector3 rt = new Vector3(1, 0, 0);
        Vector3 up = new Vector3(0, 0, 1);
        Vector3 fw = new Vector3(0, 1, 0);
        Vector3 position = Vector3.zero;
        position += u * fw * u_scale - fw * u_scale * 3;
        position += x * rt * x_scale;
        position += y * fw * y_scale;
        position += rt * x_scale * 0.5f + fw * y_scale * 0.5f;

        position += z * up * z_scale - up*z_scale*3;
        position += w * up * w_scale - up * w_scale * 3;
        return this.transform.localToWorldMatrix * new Vector4(-position.x, position.y, position.z, 1.0f);
    }
    public override Vector3 SnapCamera(Vector3 position)
    {
        int[] coordinate = PositionToCoordinate(position);
        if (coordinate != null)
        {
            Vector3 a = CoordinateToPosition(new int[5] { 0, 0, coordinate[2], coordinate[3], coordinate[4] });
            Vector3 b = CoordinateToPosition(new int[5] { 7, 7, coordinate[2], coordinate[3], coordinate[4] });
            Vector3 m = (a + b) * 0.5f;
            return m;
        }
        return position;
    }
    public override Vector3 GetCenter()
    {
        Vector3 a1 = CoordinateToPosition(new int[] { 0, 0, 0, 0, 0 });
        Vector3 a2 = CoordinateToPosition(new int[] { 7, 7, 6, 6, 6 });
        Vector3 center = (a1 + a2) * 0.5f;
        return center;
    }

    public override int[] IndexToCoordinate(int index)
    {
        int i = index;
        int[] coordinate = new int[dimensions.Length];
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
        for (int i = 0; i < Mathf.Min(coordinate.Length, dimensions.Length); i++)
        {
            index += scale * coordinate[i];
            scale *= dimensions[i];
        }
        return index;
    }


    public override bool IsInBounds(int[] coordinate)
    {
        if (coordinate == null)
            return false;
        for (int i = 0; i < Mathf.Min(coordinate.Length, dimensions.Length); i++)
        {
            int rank = 3 - Mathf.FloorToInt(Mathf.Abs(coordinate[0] - 3.5f));
            bool norm = coordinate[i] == 3;
            if ((i > 1) && ((i - 1) > rank) && !norm)
                return false;

            if (coordinate[i] < 0 || coordinate[i] >= dimensions[i])
                return false;
        }
        return true;
    }

}
