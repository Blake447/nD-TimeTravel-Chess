using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINode
{
    public AINode parent;
    public AINode principleChild;
    public AINode next;
    public AINode prev;

    public Turn turn;

    public int score;

    public AINode(Turn turn)
    {
        this.turn = turn;
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
