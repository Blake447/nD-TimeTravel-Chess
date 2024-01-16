using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{

    public Move next = null;
    public Move prev = null;
    public Command head = null;
    public Command tail = null;
    //public Command current;

    string numbers = "12345678";
    string letters = "abcdefgh";
    string pieces = "xkqbnrp____";

    //public int[] Serialize()
    //{
    //    int size = 0;
    //    Command sentinel = tail;
    //    while(sentinel != null)
    //    {
    //        size += sentinel.Size();
    //        sentinel = sentinel.next;
    //    }
    //    if (size == 0)
    //        return null;
    //    int[] serialized = new int[size];
    //    sentinel = tail;
    //    int index = 0;
    //    while (sentinel != null)
    //    {
    //        int[] command_ser = (int[])sentinel.Serialize().Clone();
    //        System.Array.Copy(command_ser, 0, serialized, index, size);
    //        index += size;
    //        sentinel = sentinel.next;
    //    }
    //    return serialized;
    //}
    public Move(int[] serialized)
    {
        int i = 0;
        while (i < serialized.Length)
        {
            int from_size = serialized[i + 2];
            int to_size = serialized[i + 2 + from_size];
            int[] command = new int[4 + from_size + to_size];
            System.Array.Copy(command, 0, serialized, i, command.Length);
            this.Add(new Command(command));
            i += command.Length;
        }
    }
    public int Size()
    {
        int size = 0;
        Command sentinel = tail;
        while (sentinel != null)
        {
            size += sentinel.Size();
            sentinel = sentinel.next;
        }
        return size;
    }


    public Move()
    {
        
    }
    public Move(Command command)
    {
        Add(command);
    }
    public void Add(Command command)
    {
        if (head == null)
        {
            head = command;
            tail = command;
            command.next = null;
            command.prev = null;
        }
        else
        {
            head.next = command;
            command.prev = head;
            command.next = null;
            head = command;
        }
    }
}
