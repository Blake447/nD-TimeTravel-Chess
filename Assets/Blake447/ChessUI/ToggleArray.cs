using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleArray : MonoBehaviour
{
    public GameObject[] panels;

    public void EnablePanel(int panel)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == panel);
        }
    }

    public void EnablePanels(int[] panels_to_set)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        if (panels_to_set != null && panels_to_set.Length != 0)
            for (int j = 0; j < panels_to_set.Length; j++)
                if (panels_to_set[j] >= 0 && panels_to_set[j] < panels.Length)
                    panels[panels_to_set[j]].SetActive(true);
    }
    public void TogglePanels(int[] panels_to_set)
    {
        if (panels_to_set != null && panels_to_set.Length != 0)
            for (int j = 0; j < panels_to_set.Length; j++)
                if (panels_to_set[j] >= 0 && panels_to_set[j] < panels.Length)
                    panels[panels_to_set[j]].SetActive(!panels[panels_to_set[j]].activeInHierarchy);
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
