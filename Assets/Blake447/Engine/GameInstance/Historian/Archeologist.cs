using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archeologist : MonoBehaviour
{
    static Archeologist archeologist;
    int[] lastSerializedHistory;

    public void StoreHistory(int[] serializedHistory)
    {
        lastSerializedHistory = (int[])serializedHistory.Clone();
    }
    public void StoreRecovery()
    {
        if (lastSerializedHistory != null)
            BoardLoader.SaveRecovery(lastSerializedHistory);
        Debug.Log(Coordinates.CoordinateToString(lastSerializedHistory));
    }

    private void Awake()
    {
        if (archeologist == null)
            archeologist = this;
        else
            Destroy(this.gameObject);
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
