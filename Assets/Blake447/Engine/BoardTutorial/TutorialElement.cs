using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialElement : MonoBehaviour
{
    int indexed_id = 0;
    [SerializeField] GameObject[] TutorialObjects;
    BoardTutorial tutorial;
    public void SetClient(BoardTutorial tutorial, int id)
    {
        this.tutorial = tutorial;
        indexed_id = id;
    }
    public void Activate()
    {
        if (TutorialObjects != null)
        {
            for (int i = 0; i < TutorialObjects.Length; i++)
                TutorialObjects[i].SetActive(true);
        }
    }
    public void Deactivate()
    {
        if (TutorialObjects != null)
        {
            for (int i = 0; i < TutorialObjects.Length; i++)
                TutorialObjects[i].SetActive(false);
        }
    }
    public void OnCompleteState()
    {

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
