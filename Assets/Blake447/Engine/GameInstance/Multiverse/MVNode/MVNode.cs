using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVNode : MonoBehaviour
{
    public MVNode next;
    //public MVNode prev;
    //public MVNode mvup;
    //public MVNode mvdw;
    public Board board;




    public int m;
    public int t;

    [SerializeField] GameObject Indicator;

    public void SetIndicator(bool active)
    {
        if (Indicator != null)
        {
            Indicator.SetActive(active);
        }
    }
    public Board GetBoard()
    {
        if (board == null)
            Debug.LogWarning("Warning, no board found in node");
        return board;
    }
}
