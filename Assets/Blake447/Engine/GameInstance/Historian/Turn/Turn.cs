using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn
{
    public Turn next = null;
    public Turn prev = null;
    public Move head = null;
    public Move tail = null;

    string numbers = "12345678";
    string letters = "abcdefgh";
    string pieces = "xkqbnrp____";

    const int TERMINAL = -9411;

    //public int[] Serialize()
    //{
    //    Debug.Log("Serializing Turn ");
    //    int move_count = 0;
    //    int size = 0;
    //    Move sentinel = tail;
    //    Debug.Log("Calculating size for turn");
    //    while (sentinel != null)
    //    {
    //        Debug.Log("Iterating over move");
    //        Debug.Log("Move: " + Coordinates.CoordinateToString(sentinel.Serialize()));
    //        size += sentinel.Size() + 1;
    //        sentinel = sentinel.next;
    //    }
    //    Debug.Log("Size calculation complete");
    //    if (size == 0)
    //        return null;

    //    sentinel = tail;
    //    int[] serialized = new int[size];
    //    sentinel = tail;
    //    int index = 0;
    //    while (sentinel != null)
    //    {
    //        Debug.Log(size);
    //        int[] sentinel_ser = (int[])sentinel.Serialize()?.Clone();
    //        System.Array.Copy(sentinel_ser, 0, serialized, index, size - 1);
    //        index += size - 1;
    //        serialized[index] = TERMINAL;
    //        index++;
    //        sentinel = sentinel.next;
    //    }
    //    Debug.Log("Serialized turn " + Coordinates.CoordinateToString(serialized));
    //    return serialized;
    //}
    public Turn(int[] serialized)
    {
        int i = 0;
        Move move_sentinel = new Move();
        Debug.Log("Serialized turn received: " + Coordinates.CoordinateToString(serialized));
        while (i < serialized.Length)
        {
            int from_size = serialized[i + 2];
            int to_size = serialized[i + 2 + from_size + 1];
            int[] command = new int[4 + from_size + to_size];
            System.Array.Copy(serialized, i, command, 0, command.Length);
            Command command_obj = new Command((int[])command.Clone());
            move_sentinel.Add(command_obj);
            i += command.Length;
            if (serialized[i] == TERMINAL)
            {
                this.Add(move_sentinel);
                move_sentinel = new Move();
                i++;
            }
        }
    }
    



    public Turn()
    {

    }
    public Turn(Move move)
    {
        Add(move);
    }
    public void Add(Move move)
    {
        Move new_move = new Move();
        Command sentinel = move.tail;
        while (sentinel != null)
        {
            Command command = new Command(sentinel.pfrom, sentinel.pto, (int[])sentinel.from.Clone(), (int[])sentinel.to.Clone());
            new_move.Add(command);
            sentinel = sentinel.next;
        }
        if (head == null)
        {
            head = new_move;
            tail = new_move;
            new_move.next = null;
            new_move.prev = null;
        }
        else
        {
            head.next = new_move;
            new_move.prev = head;
            new_move.next = null;
            head = new_move;
        }
    }
    public string TurnToString()
    {
        string output = "[ ";
        Move sentinel = tail;
        while (sentinel != null)
        {
            Command command = sentinel.tail;
            while (command != null)
            {
                output = output + command.CommandToString() + " ";
                command = command.next;
            }
            sentinel = sentinel.next;
        }
        output += "]";
        return output;
    }
}
