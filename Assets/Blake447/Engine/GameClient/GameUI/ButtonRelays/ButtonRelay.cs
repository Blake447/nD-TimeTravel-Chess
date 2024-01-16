using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class ButtonRelay : MonoBehaviour
{
    protected UnityEngine.UI.Button button;
    
    private void Awake()
    {
        button = this.GetComponent<UnityEngine.UI.Button>();
        if (button == null)
        {
            Debug.LogWarning("Warning, failed to assign button");
        }
    }

    public virtual void BindButtonToGameStatus(GameInstance game)
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
