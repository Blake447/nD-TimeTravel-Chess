using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    public Command next = null;
    public Command prev = null;

    public int pfrom;
    public int pto;
    public int[] from;
    public int[] to;

    static public string numbers = "12345678";
    static public string letters = "abcdefgh";
    static public string pieces = "xkqbnrp____";


    public int[] Serialize()
    {
        int[] serialized = new int[4 + from.Length + to.Length];
        serialized[0] = pfrom;
        serialized[1] = pto;
        serialized[2] = from.Length;
        serialized[3 + from.Length] = to.Length;
        System.Array.Copy(from, 0, serialized, 3, from.Length);
        System.Array.Copy(to, 0, serialized, 3 + from.Length + 1, to.Length);
        return serialized;
    }
    public Command(int[] serialized)
    {
        this.pfrom = serialized[0];
        this.pto = serialized[1];
        int from_length = serialized[2];
        this.from = new int[from_length];

        int to_length = serialized[3 + from.Length];
        this.to = new int[to_length];
        
        System.Array.Copy(serialized, 3, from, 0, from_length);
        System.Array.Copy(serialized, 3 + from.Length + 1, to, 0, to_length);
    }
    public int Size()
    {
        return 4 + from.Length + to.Length;
    }


    public Command ()
    {
        this.pfrom = 0;
        this.pto = 0;
        this.from = null;
        this.to = null;

    }
    public Command(int pfrom, int pto, int[] from, int[] to)
    {
        this.pfrom = pfrom;
        this.pto = pto;
        this.from = (int[])from.Clone();
        this.to = (int[])to.Clone();
    }

    public string CommandToString()
    {
        return pfrom.ToString() + "-" + pto.ToString() + " : " + Coordinates.CoordinateToString(from) + " -> " + Coordinates.CoordinateToString(to);
        int piece_from = pfrom % Overseer.PIECE_COUNT;
        int color_from = pfrom / Overseer.PIECE_COUNT;
        string pf = "" + pieces[piece_from];
        if (color_from == 0 && piece_from != 0)
            pf = (pf).ToUpper();
        int piece_to = pto % Overseer.PIECE_COUNT;
        int color_to = pto / Overseer.PIECE_COUNT;
        string pt = "" + pieces[piece_to];
        if (color_to == 0 && piece_to != 0)
            pt = (pt).ToUpper();
        string mv_from = "(" + from[0] + ", " + from[1] + ")";
        string mv_to = "(" + to[0] + ", " + to[1] + ")";
        string cf = "";
        string ct = "";

        return pf + pt + " " + Coordinates.CoordinateToString(from) + " " + Coordinates.CoordinateToString(to);



        return "()";

        //for (int i = 0; i < from.Length; i++)
        //{
        //    int from_i = Mathf.Clamp(from[i], 0, letters.Length - 1);
        //    cf += (i % 2) == 0 ? letters[from_i] : numbers[from_i];
        //}
        //for (int i = 0; i < to.Length; i++)
        //{
        //    int to_i = Mathf.Clamp(to[i], 0, letters.Length);
        //    ct += (i % 2) == 0 ? letters[to_i] : numbers[to_i];
        //}
        //for (int i = 0; i < from.Length; i++)
        //    cf += (i % 2) == 0 ? from[i] : from[i];
        //for (int i = 0; i < to.Length; i++)
        //    ct += (i % 2) == 0 ? to[i] : to[i];
        //return pf + pt + " " + cf + " " + ct;
    }
}
