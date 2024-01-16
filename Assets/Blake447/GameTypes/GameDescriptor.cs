using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDescriptor : MonoBehaviour
{
    public string game_name;
    public string filename;
    public MVNode board;
    public bool isTimeTravel;

    public bool useForwardLateral = true;
    public bool allowPromotions = true;
    public bool friendlyVisualizer = true;
    public bool enemyVisualizer = true;

    public float multiverse_offset = 40.0f;
    public float timetravel_offset = 20.0f;

    public int timeIndex;

    public int[] dimensions;
    public int[] board_state;


    public int[] forwards;
    public int[] laterals;
}
