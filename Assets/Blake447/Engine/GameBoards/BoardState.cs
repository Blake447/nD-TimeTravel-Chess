using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardState
{
    public string name;
    public int[] board_dimensions;
    public int[] board_state;
    public float[] dimensional_modifiers;


    public BoardState(string name, int[] board_dimensions, int[] board_state, float[] dimensional_modifiers)
    {
        this.name = name;
        this.board_dimensions = (int[])board_dimensions.Clone();
        this.board_state = (int[])board_state.Clone();
        //this.dimensional_modifiers = (float[])dimensional_modifiers.Clone();
    }


}
