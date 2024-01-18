using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPopulator : MonoBehaviour
{
    public PrimaryMenu menuSystem;

    Button[] buttons;
    // Start is called before the first frame update
    void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            GameDescriptor gameDescriptor = menuSystem.GetPrefab(i);
            buttons[i].GetComponentInChildren<TMPro.TMP_Text>().text = gameDescriptor.name;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
