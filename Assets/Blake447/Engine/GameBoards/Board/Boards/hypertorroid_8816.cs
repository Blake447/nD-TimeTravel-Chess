using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hypertorroid_8816 : Board
{
    int[] board_dimensions = new int[] { 16, 8, 8 };
    int[] hypertorroid_state = new int[]
    {
        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        5,4,3,1,2,3,4,5,4,3,4,5,4,3,4,5,
        5,4,3,2,6,3,4,5,4,3,4,5,4,3,4,5,
        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,

        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,

        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,

        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,
        6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,
        6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,
        6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,

        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,
        5+32,4+32,3+32,2+32,6+32,3+32,4+32,5+32,4+32,3+32,4+32,5+32,4+32,3+32,4+32,5+32,
        5+32,4+32,3+32,1+32,2+32,3+32,4+32,5+32,4+32,3+32,4+32,5+32,4+32,3+32,4+32,5+32,
        6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,

        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,
        6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,
        6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,
        6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,6+32,

        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,

        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,

    };

    public override void SetDimensions()
    {
        base.dimensions = (int[])board_dimensions.Clone();
    }

    public override int[] PositionToCoordinate(Vector3 position)
    {
        float major_radius = 1.0f * 3;
        float minor_radius = 0.5f * 3;
        float hyper_radius = 4.0f * 3;

        //Vector3 local_offset = position - this.transform.localPosition;
        Vector3 local_offset = this.transform.worldToLocalMatrix * new Vector4(position.x, position.y, position.z, 1.0f);
        //Debug.Log(transform.worldToLocalMatrix);

        Vector3 rt = -Vector3.right;
        Vector3 fw = Vector3.up;
        Vector3 up = Vector3.forward;

        float hyper_angle = Mathf.Atan2(Vector3.Dot(local_offset, up), Vector3.Dot(local_offset, fw));
        //hyper_angle += -(2.0f * Mathf.PI * 1 / 32.0f);
        //hyper_angle += hyper_angle < 0 ? 2.0f * Mathf.PI : 0;
        int hyper_rounded = Mathf.RoundToInt(hyper_angle / (2.0f * Mathf.PI) * dimensions[2]);
        float hyper_angle_rounded = (float)hyper_rounded * 2.0f * Mathf.PI / (float)dimensions[2];
        Vector3 hyper = (fw * Mathf.Cos(hyper_angle_rounded) + up * Mathf.Sin(hyper_angle_rounded)) * hyper_radius;
        Vector3 major_offset = local_offset - hyper;
        float major_angle = Mathf.Atan2(Vector3.Dot(major_offset, hyper.normalized), Vector3.Dot(major_offset, rt));
        Vector3 center = (rt * Mathf.Cos(major_angle) + hyper.normalized * Mathf.Sin(major_angle)) * major_radius;
        major_angle += -(2.0f * Mathf.PI * 1 / 32.0f);


        Vector3 minor = Vector3.Cross(hyper, rt).normalized; // Gives tangent to hyper axis
        Vector3 minor_offset = major_offset - center;
        Debug.Log("Hyper  : " + hyper.normalized);
        Debug.Log("center : " + center.normalized);
        Debug.Log("minor  : " + minor.normalized);

        float minor_angle = Mathf.Atan2(Vector3.Dot(minor_offset, minor.normalized), Vector3.Dot(minor_offset, center.normalized));
        minor_angle += -(2.0f * Mathf.PI * 1 / 16.0f);
        int major_rounded = Mathf.RoundToInt(major_angle / (2.0f * Mathf.PI) * dimensions[0]);
        int minor_rounded = Mathf.RoundToInt(minor_angle / (2.0f * Mathf.PI) * dimensions[1]);
        Debug.Log(Coordinates.CoordinateToString(new int[] { major_rounded, minor_rounded, hyper_rounded }));
        return new int[] { major_rounded, minor_rounded, hyper_rounded };
    }
    public override Vector3 CoordinateToPosition(int[] coordinate)
    {
        float major_radius = 1.0f * 3;
        float minor_radius = 0.5f * 3;
        float hyper_radius = 4.0f * 3;

        float hyper_angle = (float)(coordinate[2] % board_dimensions[2]) / (float)board_dimensions[2] * 2.0f * Mathf.PI;
        float major_angle = (float)(coordinate[0] % board_dimensions[0]) / (float)board_dimensions[0] * 2.0f * Mathf.PI + (2.0f * Mathf.PI * 1 / 32.0f);
        float minor_angle = (float)(coordinate[1] % board_dimensions[1]) / (float)board_dimensions[1] * 2.0f * Mathf.PI + (2.0f * Mathf.PI * 1 / 16.0f);

        Vector3 rt = -Vector3.right;
        Vector3 fw = Vector3.up;
        Vector3 up = Vector3.forward;

        Vector3 hyper = (fw * Mathf.Cos(hyper_angle) + up * Mathf.Sin(hyper_angle)) * hyper_radius;
        Vector3 center = (rt * Mathf.Cos(major_angle) + hyper.normalized * Mathf.Sin(major_angle)) * major_radius;
        Vector3 minor = Vector3.Cross(hyper, rt).normalized; // Gives tangent to hyper axis
        Vector3 minor_offset = (center.normalized * Mathf.Cos(minor_angle) + minor * Mathf.Sin(minor_angle)) * minor_radius;
        Vector3 position = hyper + center + minor_offset;
        return this.transform.localToWorldMatrix * new Vector4(position.x, position.y, position.z, 1.0f);
    }
    public override Quaternion CoordinateToRotation(int[] coordinate)
    {
        float major_radius = 1.0f * 3;
        float minor_radius = 0.5f * 3;
        float hyper_radius = 4.0f * 3;

        float hyper_angle = (float)(coordinate[2] % dimensions[2]) / (float)dimensions[2] * 2.0f * Mathf.PI;
        float major_angle = (float)(coordinate[0] % dimensions[0]) / (float)dimensions[0] * 2.0f * Mathf.PI + (2.0f * Mathf.PI * 1 / 32.0f);
        float minor_angle = (float)(coordinate[1] % dimensions[1]) / (float)dimensions[1] * 2.0f * Mathf.PI + (2.0f * Mathf.PI * 1 / 16.0f);

        Vector3 rt = -Vector3.right;
        Vector3 fw = Vector3.up;
        Vector3 up = Vector3.forward;

        Vector3 hyper = (fw * Mathf.Cos(hyper_angle) + up * Mathf.Sin(hyper_angle)) * hyper_radius;
        Vector3 center = (rt * Mathf.Cos(major_angle) + hyper.normalized * Mathf.Sin(major_angle)) * major_radius;
        Vector3 minor = Vector3.Cross(hyper, rt).normalized; // Gives tangent to hyper axis
        Vector3 minor_offset = (center.normalized * Mathf.Cos(minor_angle) + minor * Mathf.Sin(minor_angle)) * minor_radius;
        Vector3 direction = minor_offset;
        return Quaternion.FromToRotation(up, direction);
    }
    public override Vector3 SnapCamera(Vector3 position)
    {
        Debug.LogWarning("Snap Camera not functional");
        int[] coordinate = PositionToCoordinate(position);
        if (coordinate != null)
        {
            Vector3 a = CoordinateToPosition(new int[3] { 0, 0, coordinate[2] });
            Vector3 b = CoordinateToPosition(new int[3] { 8, 7, coordinate[2] });
            Vector3 m = (a + b) * 0.5f;
            return m;
        }
        return position;
    }
    public override Vector3 GetCenter()
    {
        //Debug.LogWarning("Get center probably not correct");
        Vector3 a1 = CoordinateToPosition(new int[] { 3, 0, 0 });
        Vector3 a2 = CoordinateToPosition(new int[] { 8, 4, 4 });
        Vector3 center = (a1 + a2) * 0.5f;
        return center;
    }

    public override int[] IndexToCoordinate(int index)
    {
        int length = CalculateLength();
        int i = index % length;
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
