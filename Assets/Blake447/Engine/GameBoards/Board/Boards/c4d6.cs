using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class c4d6 : Board
{
    int[] board_dimensions = new int[4] { 6, 6, 6, 6 };
    
    public override void SetDimensions()
    {
        base.dimensions = (int[])board_dimensions.Clone();
    }

    public override int[] PositionToCoordinate(Vector3 position)
    {
        Vector3 local_offset = this.transform.worldToLocalMatrix * new Vector4(position.x, position.y, position.z, 1.0f);
        local_offset = Vector3.Scale(local_offset, new Vector3(-1, 1, 1));
        Vector3 offset = local_offset;
        offset = offset + new Vector3(1, 1, 0);
        offset = offset / 8.0f;
        int offset_w = Mathf.FloorToInt(offset.y);
        offset = local_offset;
        offset = offset / 6.0f;
        int offset_z = Mathf.FloorToInt(offset.z);
        offset = local_offset;
        offset = offset - Vector3.up * 8.0f * offset_w;
        offset = offset - Vector3.forward * 6.0f * offset_z;
        int offset_x = Mathf.FloorToInt(offset.x);
        int offset_y = Mathf.FloorToInt(offset.y);
        int[] coordinate = new int[4] { offset_x, offset_y, offset_z, offset_w };
        if (IsWithinBounds(coordinate))
            return coordinate;
        return null;
    }
    public override Vector3 CoordinateToPosition(int[] coordinate)
    {
        if (coordinate == null)
            return this.transform.position;
        int[] coords = new int[4] { 0, 0, 0, 0 };
        for (int i = 0; i < Mathf.Min(coords.Length, coordinate.Length); i++)
            coords[i] = coordinate[i];
        int x = coords[0];
        int y = coords[1];
        int z = coords[2];
        int w = coords[3];
        Vector3 rt = new Vector3(1, 0, 0);
        Vector3 up = new Vector3(0, 0, 1);
        Vector3 fw = new Vector3(0, 1, 0);
        Vector3 position = x * rt + y * fw + new Vector3(0.5f, 0.5f, 0);
        position = position + z * new Vector3(0, 0, 6);
        position = position + w * new Vector3(0, 8, 0);
        position = Vector3.Scale(position, new Vector3(-1, 1, 1));
        return this.transform.localToWorldMatrix * new Vector4(position.x, position.y, position.z, 1.0f);
    }
    public override Vector3 SnapCamera(Vector3 position)
    {
        int[] coordinate = PositionToCoordinate(position);
        if (coordinate != null)
        {
            Vector3 a = CoordinateToPosition(new int[4] { 0, 0, coordinate[2], coordinate[3] });
            Vector3 b = CoordinateToPosition(new int[4] { 3, 3, coordinate[2], coordinate[3] });
            Vector3 m = (a + b) * 0.5f;
            return m;
        }
        return position;
    }
    public override Vector3 GetCenter()
    {
        Vector3 a1 = CoordinateToPosition(new int[] { 0, 0, 0, 0 });
        Vector3 a2 = CoordinateToPosition(new int[] { 5, 5, 5, 5 });
        Vector3 center = (a1 + a2) * 0.5f;
        return center;
    }

    public override int[] IndexToCoordinate(int index)
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
    public bool IsWithinBounds(int[] coordinate)
    {
        int[] bounds = new int[4] { 6, 6, 6, 6 };
        for (int i = 0; i < Mathf.Min(coordinate.Length, bounds.Length); i++)
            if (coordinate[i] < 0 || coordinate[i] >= bounds[i])
                return false;
        return true;
    }
}
