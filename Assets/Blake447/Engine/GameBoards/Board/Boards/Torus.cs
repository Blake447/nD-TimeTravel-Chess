using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torus : Board
{
    int[] board_dimensions = new int[] { 8, 16 };

    public override void SetDimensions()
    {
        base.dimensions = (int[])board_dimensions.Clone();
    }

    public override int[] PositionToCoordinate(Vector3 position)
    {
        float major_radius = 1.0f * 3;
        float minor_radius = 0.5f * 3;

        Vector3 local_offset = this.transform.worldToLocalMatrix * new Vector4(position.x, position.y, position.z, 1.0f);


        //Vector3 local_offset = position - this.transform.localPosition;
        //local_offset = this.transform.worldToLocalMatrix.MultiplyVector(local_offset);
        //Debug.Log(transform.worldToLocalMatrix);

        Vector3 rt = Vector3.right;
        Vector3 fw = Vector3.up;
        Vector3 up = Vector3.forward;

        float major_angle = Mathf.Atan2(Vector3.Dot(local_offset, fw), Vector3.Dot(local_offset, rt)) ;
        //if (major_angle < 0)
        //    major_angle += Mathf.PI * 2.0f;
        major_angle += -(2.0f * Mathf.PI * 1 / 32.0f);
        Vector3 center = (rt * Mathf.Cos(major_angle) + fw * Mathf.Sin(major_angle)) * major_radius;

        Vector3 minor_offset = local_offset - center;
        float minor_angle = Mathf.Atan2(Vector3.Dot(minor_offset, up), Vector3.Dot(minor_offset, center.normalized));
        //if (minor_angle < 0)
        //    minor_angle += Mathf.PI * 2.0f;
        minor_angle += -(2.0f * Mathf.PI * 1 / 16.0f);
        //Debug.Log("Angles = <" + major_angle + ", " + minor_angle + ">");

        int major_rounded = Mathf.RoundToInt(major_angle / (2.0f * Mathf.PI) * dimensions[1]);
        int minor_rounded = Mathf.RoundToInt(minor_angle / (2.0f * Mathf.PI) * dimensions[0]);
        return new int[] { minor_rounded, major_rounded };
    }
    public override Vector3 CoordinateToPosition(int[] coordinate)
    {
        float major_radius = 1.0f * 3;
        float minor_radius = 0.5f * 3;

        float minor_angle = (float)(coordinate[0] % board_dimensions[0]) / (float)board_dimensions[0] * 2.0f * Mathf.PI + (2.0f * Mathf.PI * 1 / 16.0f);
        float major_angle = (float)(coordinate[1] % board_dimensions[1]) / (float)board_dimensions[1] * 2.0f * Mathf.PI + (2.0f * Mathf.PI * 1 / 32.0f);
        Vector3 rt = Vector3.right;
        Vector3 fw = Vector3.up;
        Vector3 up = Vector3.forward;

        Vector3 center = (rt * Mathf.Cos(major_angle) + fw * Mathf.Sin(major_angle)) * major_radius;
        Vector3 minor_offset = (center.normalized * Mathf.Cos(minor_angle) + up * Mathf.Sin(minor_angle))*minor_radius;
        Vector3 position = center + minor_offset;
        return this.transform.localToWorldMatrix * new Vector4(position.x, position.y, position.z, 1.0f);
    }
    public override Quaternion CoordinateToRotation(int[] coordinate)
    {
        float major_radius = 1.0f * 3;
        float minor_radius = 0.5f * 3;


        float minor_angle = (float)(coordinate[0] % dimensions[0]) / (float)dimensions[0] * 2.0f * Mathf.PI + (2.0f * Mathf.PI * 1 / 16.0f); ;
        float major_angle = (float)(coordinate[1] % dimensions[1]) / (float)dimensions[1] * 2.0f * Mathf.PI + (2.0f * Mathf.PI * 1 / 32.0f); ;
        Vector3 rt = Vector3.right;
        Vector3 fw = Vector3.up;
        Vector3 up = Vector3.forward;

        Vector3 center = (rt * Mathf.Cos(major_angle) + fw * Mathf.Sin(major_angle)) * major_radius;
        Vector3 minor_offset = (center.normalized * Mathf.Cos(minor_angle) + up * Mathf.Sin(minor_angle)) * minor_radius;
        Vector3 direction = minor_offset;
        return Quaternion.FromToRotation(up, direction);
    }
    public override Vector3 SnapCamera(Vector3 position)
    {
        Debug.LogWarning("Snap Camera not functional");
        int[] coordinate = PositionToCoordinate(position);
        if (coordinate != null && coordinate.Length <= 2)
        {
            Vector3 a = CoordinateToPosition(new int[2] { 0, 0 });
            Vector3 b = CoordinateToPosition(new int[2] { 3, 3 });
            Vector3 m = (a + b) * 0.5f;
            return m;
        }
        else if (coordinate != null && coordinate.Length == 4)
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
        //Debug.LogWarning("Get center probably not correct");
        Vector3 a1 = CoordinateToPosition(new int[] { 3, 0 });
        Vector3 a2 = CoordinateToPosition(new int[] { 4, 8 });
        Vector3 center = (a1 + a2) * 0.5f;
        return center;
    }

    public override int[] IndexToCoordinate(int index)
    {
        int length = CalculateLength();
        int i = index % length;
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
        for (int i = 0; i < Mathf.Min(coordinate.Length, dimensions.Length); i++)
        {
            index += scale * ( (coordinate[i] + dimensions[i] * 16)  % dimensions[i]);
            scale *= dimensions[i];
        }
        return index;
    }

    public override bool IsInBounds(int[] coordinate)
    {
        return true;
    }
    public override void ReprocessCoordinates(int[] coordinate_a, int[] coordinate_b)
    {
        base.ReprocessCoordinates(coordinate_a, coordinate_b);
        for (int i = 0; i < Mathf.Min(coordinate_a.Length, dimensions.Length); i++)
        {
            coordinate_a[i] = (coordinate_a[i] + dimensions[i] * 4) % dimensions[i];
            coordinate_b[i] = (coordinate_b[i] + dimensions[i] * 4) % dimensions[i];
        }
    }
   
}
