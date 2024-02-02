using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shogi : Board
{
    int[] board_dimensions = new int[3] { 9, 9, 3 };

    public override void SetDimensions()
    {
        base.dimensions = (int[])board_dimensions.Clone();
    }

    public override int[] PositionToCoordinate(Vector3 position)
    {
        Vector3 local_offset = this.transform.worldToLocalMatrix * new Vector4(position.x, position.y, position.z, 1.0f);
        local_offset = Vector3.Scale(local_offset, new Vector3(-1, 1, 1));
        Vector3 offset = local_offset;
        int z = Mathf.FloorToInt(local_offset.x / 12.0f);
        offset = offset -= z * new Vector3(12.0f, -5.0f, 0.0f);
        int offset_x = Mathf.FloorToInt(offset.x);
        int offset_y = Mathf.FloorToInt(offset.y);
        int[] coordinate = new int[3] { offset_x, offset_y, z + 1 };
        if (IsInBounds(coordinate))
            return coordinate;
        return null;
    }
    public override Vector3 CoordinateToPosition(int[] coordinate)
    {
        if (coordinate == null)
            return this.transform.position;
        int[] coords = new int[3] { 0, 0, 0 };
        for (int i = 0; i < Mathf.Min(coords.Length, coordinate.Length); i++)
            coords[i] = coordinate[i];
        int x = coords[0];
        int y = coords[1];
        int z = (coords[2]);
        Vector3 rt = new Vector3(1, 0, 0);
        Vector3 up = new Vector3(0, 0, 1);
        Vector3 fw = new Vector3(0, 1, 0);
        Vector3 position = x * rt + y * fw + new Vector3(0.5f, 0.5f, 0) + new Vector3(12.0f, -5.0f, 0.0f) * (z - 1);
        position = Vector3.Scale(position, new Vector3(-1, 1, 1));
        return this.transform.localToWorldMatrix * new Vector4(position.x, position.y, position.z, 1.0f);
    }
    public override Vector3 SnapCamera(Vector3 position)
    {
        int[] coordinate = PositionToCoordinate(position);
        if (coordinate != null)
        {
            Vector3 a = CoordinateToPosition(new int[] { 0, 0, 0 });
            Vector3 b = CoordinateToPosition(new int[] { 8, 8, 2 });
            Vector3 m = (a + b) * 0.5f;
            return m;
        }
        return position;
    }
    public override Vector3 GetCenter()
    {
        Vector3 a1 = CoordinateToPosition(new int[] { 0, 0, 0 });
        Vector3 a2 = CoordinateToPosition(new int[] { 8, 8, 2 });
        Vector3 center = (a1 + a2) * 0.5f;
        return center;
    }

    public override int[] IndexToCoordinate(int index)
    {
        int i = index;
        int[] coordinate = new int[3];
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
            if (coordinate[i] < 0 || coordinate[i] >= dimensions[i])
                return false;
        return true;
    }
}
