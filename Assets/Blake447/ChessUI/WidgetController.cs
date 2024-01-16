using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetController : MonoBehaviour
{
    public GameObject BoardRoot;
    //public MVNode BoardObject;
    public GameObject CameraPivot;

    public void SetBoard(GameObject prefab)
    {
        for (int i = 0; i < BoardRoot.transform.childCount; i++)
        {
            Destroy(BoardRoot.transform.GetChild(i).gameObject);
        }
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.parent = BoardRoot.transform;
            obj.transform.localPosition = Vector3.zero;
            CenterBoard();
        }
    }

    // Start is called before the first frame update
    void CenterBoard()
    {
        MVNode[] BoardObjects = BoardRoot.GetComponentsInChildren<MVNode>();
        if (BoardObjects != null)
        {
            foreach (MVNode node in BoardObjects)
            {
                Vector3 center = node.GetBoard().GetCenter();
                CameraPivot.transform.position = center;
            }
        }
    }
    
    
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
