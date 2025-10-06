using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardLayout
{
    [NonSerialized] public Board board;
    [SerializeField] public string boardName;
    [SerializeField] public string displayName;

    [SerializeField] public int[] dimensions;
    [SerializeField] public bool[] forwards;
    [SerializeField] public bool[] laterals;

    [SerializeField] public int[] state;

    public static byte[] memLayout;
    public static byte[] SerializeLayout(object layoutObj)
    {
        BoardLayout layout = (BoardLayout)layoutObj;
        int dimLength = layout.dimensions.Length;
        int[] dim = layout.dimensions;
        int[] fwd = new int[dimLength];
        int[] lat = new int[dimLength];
        int[] state = layout.state;


        int DIM_SIZE = 5;
        int ARR_SIZE = 3 * (dim.Length & 15);
        int LEN_SIZE = 5;
        int STA_SIZE = Mathf.Clamp(state.Length, 0, (1 << 15) - 1 - 55);
        short MEM_SIZE = (short)(DIM_SIZE + ARR_SIZE + LEN_SIZE + STA_SIZE);
        memLayout = new byte[MEM_SIZE];
        byte[] bytes = memLayout;

        int index = 0;
        byte[] memInt = new byte[5];
        BitConverter.TryWriteBytes(memInt, dimLength);
        if (BitConverter.IsLittleEndian) Array.Reverse(memInt);
        System.Array.Copy(memInt, 0, bytes, index, 5);
        index += 5;

        for (int i = 0; i < dimLength; i++)
        {
            bytes[index] = (byte)(dim[i] & 255);
            index++;
			bytes[index] = (byte)(fwd[i] & 255);
			index++;
			bytes[index] = (byte)(lat[i] & 255);
			index++;
		}
        
        BitConverter.TryWriteBytes(memInt, state.Length);
		if (BitConverter.IsLittleEndian) Array.Reverse(memInt);
		System.Array.Copy(memInt, 0, bytes, index, 5);
		index += 5;
        for (int i = 0; i < STA_SIZE; i++)
        {
            bytes[index] = (byte)(state[i] & 255);
            index++;
        }

        return bytes;
	}
    public static BoardLayout DeserializeLayout(byte[] data)
    {
        BoardLayout boardLayout = new BoardLayout();
        byte[] bytes = data;
        int index = 0;
        byte[] memInt = new byte[5];
        System.Array.Copy(bytes, index, memInt, 0, 5);
		if (BitConverter.IsLittleEndian) Array.Reverse(memInt);
        int dimLength = BitConverter.ToInt32(memInt, 0);
        index += 5;

        int[] dimByte = new int[dimLength];
        int[] fwdByte = new int[dimLength];
        int[] latByte = new int[dimLength];
                
        for (int i = 0; i < dimLength; i++)
        {
            dimByte[i] = bytes[index];
            index++;
            fwdByte[i] = bytes[index];
            index++;
            latByte[i] = bytes[index];
            index++;
        }

        System.Array.Copy(bytes, index, memInt, 0, 5);
		if (BitConverter.IsLittleEndian) Array.Reverse(memInt);
        int stateLength = BitConverter.ToInt32(memInt, 0);
        index += 5;
        int[] state = new int[stateLength];
		for (int i = 0; i < stateLength; i++)
        {
            state[i] = bytes[index];
            index++;
        }
        boardLayout.state = (int[])state.Clone();

        boardLayout.dimensions = (int[])dimByte.Clone();
        boardLayout.forwards = new bool[dimLength];
        for (int i = 0; i < dimLength; i++) boardLayout.forwards[i] = fwdByte[i] == 1;
        boardLayout.laterals = new bool[dimLength];
		for (int i = 0; i < dimLength; i++) boardLayout.laterals[i] = latByte[i] == 1;
        return boardLayout;
	}

    public void SetDimensions(int[] dimensions)
    {
        this.dimensions = (int[])dimensions.Clone();
        forwards = new bool[dimensions.Length];
        laterals = new bool[dimensions.Length];
        int length = 1;
        for (int i = 0; i < dimensions.Length; i++)
        {
            length *= dimensions[i];
        }
        state = new int[length];
    }
    public void SetForwards(bool[] forwards)
    {
        this.forwards = (bool[])forwards.Clone();
    }
    public void SetLaterals(bool[] laterals)
    {
        this.laterals = (bool[])laterals.Clone();
    }
    public void SetState(int[] state)
    {
        System.Array.Copy(state, this.state, Mathf.Min(state.Length, this.state.Length));
    }
    public void SetPiece(int index, int piece)
    {
        state[index] = piece;
    }
}
