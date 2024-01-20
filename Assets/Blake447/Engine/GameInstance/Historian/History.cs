using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class History
{
    public List<Turn> Turns = new List<Turn>();
    public void SetHistory(int[] serialized)
    {
        Debug.Log("SetHistory(" + Coordinates.CoordinateToString(serialized));

        Turns = new List<Turn>();
        int t = -1;
        int m = -1;
        int c = -1;

        // Unnessecary allocation of memory, but just to be save and ensure non-nullity
        Turn turn = new Turn();
        Move move = new Move();

        int index = 0;
        while (index < serialized.Length)
        {
            int length = serialized[index];
            if (length == 0)
                break;

            if (serialized[index + 1] > t)
            {
                t = serialized[index + 1];
                turn = new Turn();
                Turns.Add(turn);
            }
            if (serialized[index + 2] > m)
            {
                m = serialized[index + 2];
                move = new Move();
            }

            int[] temp = new int[length - 3];
            System.Array.Copy(serialized, index + 3, temp, 0, length - 3);
            int pfrom = temp[0];
            int pto = temp[1];
            int from_length = temp[2];
            int[] from = new int[from_length];
            System.Array.Copy(temp, 3, from, 0, from_length);
            int to_length = temp[3 + from_length];
            int[] to = new int[to_length];
            System.Array.Copy(temp, 3 + from_length + 1, to, 0, to_length);

            Command comm = new Command(pfrom, pto, (int[])from.Clone(), (int[])to.Clone());
            Debug.Log("Command( " + pfrom + ", " + pto + ", " + Coordinates.CoordinateToString(from) + ", " + Coordinates.CoordinateToString(to) + " ) @ " + "(" + t + ", " + m + ")");

            bool isLastCommand = true;
            if (index + length + 2 < serialized.Length)
            {
                isLastCommand = serialized[index + length + 2] != serialized[index + 2];
            }
            move.Add(comm);
            if (isLastCommand)
                turn.Add(move);
           
            index += length;
        }
        PrintHistory();
    }


    public void ReadSerialized(int[] serialized)
    {
        int i = 0;
        while (i < serialized.Length)
        {
            int length = serialized[i];
            if (length == 0)
                break;
            int[] temp = new int[length - 1];
            System.Array.Copy(serialized, i + 1, temp, 0, length - 1);
            Debug.Log("out: " + Coordinates.CoordinateToString(temp));
            i += length;
        }
    }

    public void TestSerialization()
    {
        Debug.Log("Iteration 1: ");
        int[] serialized = (int[])this.Serialized().Clone();
        Debug.Log("Serialized 1: " + Coordinates.CoordinateToString(serialized));
        History history = new History();
        history.SetHistory(serialized);
        Debug.Log("Iteration 2: ");
        serialized = (int[])history.Serialized().Clone();
        Debug.Log("Serialized 2: " + Coordinates.CoordinateToString(serialized));
        history = new History();
        history.SetHistory(serialized);
    }

    public int[] Serialized()
    {
        List<int[]> strings_out = new List<int[]>();
        int turn_count = 0;
        int move_count = 0;
        int comm_count = 0;
        int comm_length = 0;
        
        
        for (int i = 0; i < Turns.Count; i++)
        {
            Move move_sentinel = Turns[i].tail;
            while (move_sentinel != null)
            {
                Command command_sentinel = move_sentinel.tail;
                while (command_sentinel != null)
                {
                    int[] comm_ser = new int[6 + command_sentinel.from.Length + command_sentinel.to.Length];
                    turn_count = i;
                    comm_ser[0] = turn_count;
                    comm_ser[1] = move_count;
                    comm_ser[2] = command_sentinel.pfrom;
                    comm_ser[3] = command_sentinel.pto;
                    comm_ser[4] = command_sentinel.from.Length;
                    System.Array.Copy(command_sentinel.from, 0, comm_ser, 5, command_sentinel.from.Length);
                    comm_ser[5 + command_sentinel.from.Length] = command_sentinel.to.Length;
                    System.Array.Copy(command_sentinel.to, 0, comm_ser, 5 + command_sentinel.from.Length + 1, command_sentinel.to.Length);
                    strings_out.Add(comm_ser);
                    command_sentinel = command_sentinel.next;
                    comm_count++;
                }
                move_sentinel = move_sentinel.next;
                move_count++;
            }
        }
        int length = 0;
        for (int i = 0; i < strings_out.Count; i++)
        {
            length += strings_out[i].Length + 1;
        }
        int[] serialized = new int[length];

        int index = 0;
        for (int i = 0; i < strings_out.Count; i++)
        {
            serialized[index] = strings_out[i].Length + 1;
            index++;
            System.Array.Copy(strings_out[i], 0, serialized, index, strings_out[i].Length);
            index += strings_out[i].Length;
        }

        return (int[])serialized.Clone();
        //return strings_out.ToArray();
    }


    //public int[] GetHead()
    //{
    //    return (int[])Turns[Turns.Count - 1].Serialize().Clone();
    //}
    //public int[] GetItem(int index)
    //{
    //    return (int[])Turns[index].Serialize()?.Clone();
    //}
    public Turn GetTurn(int index)
    {
        return Turns[index];
    }
    public static int Length(History history)
    {
        if (history == null)
            return 0;
        return history.Turns.Count;
    }
    public static int AgreementLength(History a, History b)
    {
        if (a == null || b == null)
            return 0;

        List<Turn> TurnsA = a.Turns;
        List<Turn> TurnsB = b.Turns;
        for (int i = 0; i < Mathf.Min(TurnsA.Count, TurnsB.Count); i++)
        {
            Turn ta = TurnsA[i];
            Turn tb = TurnsB[i];
            Move ma = ta.tail;
            Move mb = tb.tail;
            while (ma != null && mb != null)
            {
                Command ca = ma.tail;
                Command cb = mb.tail;
                while (ca != null && cb != null)
                {
                    if (ca.pfrom != cb.pfrom)
                        return i - 1;
                    if (ca.pto != cb.pto)
                        return i - 1;
                    if (ca.from.Length != cb.from.Length)
                        return i - 1;
                    if (ca.to.Length != cb.to.Length)
                        return i - 1;
                    for (int j = 0; j < ca.from.Length; j++)
                        if (ca.from[j] != cb.from[j])
                            return i - 1;
                    for (int j = 0; j < ca.to.Length; j++)
                        if (ca.to[j] != cb.to[j])
                            return i - 1;
                    ca = ca.next;
                    cb = cb.next;
                    if (ca != cb && (ca == null || cb == null))
                        return i - 1;
                }
                ma = ma.next;
                mb = mb.next;
                if (ma != mb && (ma == null || mb == null))
                    return i - 1;
            }
        }
        return Mathf.Min(TurnsA.Count, TurnsB.Count);
    }

    public void Revert()
    {
        if (Turns.Count > 0)
            Turns.RemoveAt(Turns.Count - 1);
    }

    public void AddTurn(Turn turn)
    {
        Turn new_turn = new Turn();
        Move move_sentinel = turn.tail;
        while (move_sentinel != null)
        {
            Move move = new Move();
            Command command_sentinel = move_sentinel.tail;
            while (command_sentinel != null)
            {
                Command command = new Command(command_sentinel.pfrom, command_sentinel.pto, (int[])command_sentinel.from.Clone(), (int[])command_sentinel.to.Clone());
                move.Add(command);
                command_sentinel = command_sentinel.next;
            }
            //Debug.Log("Adding move: " + Coordinates.CoordinateToString(move.Serialize()));
            new_turn.Add(move);
            move_sentinel = move_sentinel.next;
        }
        Turns.Add(new_turn);
    }

    public string[] HistoryStrings()
    {
        List<string> strings_out = new List<string>();
        for (int i = 0; i < Turns.Count; i++)
        {
            strings_out.Add("Turn");
            Move move_sentinel = Turns[i].tail;
            while (move_sentinel != null)
            {
                strings_out.Add("  move");
                Command command_sentinel = move_sentinel.tail;
                while (command_sentinel != null)
                {
                    strings_out.Add("    " + command_sentinel.CommandToString());
                    command_sentinel = command_sentinel.next;
                }
                move_sentinel = move_sentinel.next;
            }
        }
        return strings_out.ToArray();
    }
    public string HistoryString()
    {
        string output = "";
        for (int i = 0; i < Turns.Count; i++)
        {
            output += "Turn\n";
            Move move_sentinel = Turns[i].tail;
            while (move_sentinel != null)
            {
                output += "  move\n";
                Command command_sentinel = move_sentinel.tail;
                while (command_sentinel != null)
                {
                    output += "    " + command_sentinel.CommandToString() + "\n";
                    command_sentinel = command_sentinel.next;
                }
                move_sentinel = move_sentinel.next;
            }
        }
        return output;
    }
    public void PrintHistory()
    {
        int[] history = this.Serialized();
        Debug.Log("Printing History");
        for (int i = 0; i < Turns.Count; i++)
        {
            Debug.Log("Turn");
            Move move_sentinel = Turns[i].tail;
            while (move_sentinel != null)
            {
                Debug.Log("L_move");
                Command command_sentinel = move_sentinel.tail;
                while (command_sentinel != null)
                {
                    Debug.Log("L___" + command_sentinel.CommandToString());
                    command_sentinel = command_sentinel.next;
                }
                move_sentinel = move_sentinel.next;
            }
        }
    }
}
