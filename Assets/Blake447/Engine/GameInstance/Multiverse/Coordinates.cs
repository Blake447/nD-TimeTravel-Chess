using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates : MonoBehaviour
{
    public static string CoordinateToString(int[] coord)
    {
        if (coord == null)
            return "()";
        string coordstring = "(";
        for (int i = 0; i < coord.Length - 1; i++)
            coordstring = coordstring + coord[i] + ", ";
        coordstring = coordstring + ( coord.Length > 1 ? (coord[coord.Length - 1] + ")") : ")") ;
        return coordstring;
    }
    public static bool Equal(int[] a, int[] b)
    {
        if (a.Length != b.Length)
            return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i])
                return false;
        return true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
