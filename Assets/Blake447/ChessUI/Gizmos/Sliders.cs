using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliders : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    private void Awake()
    {
        canvas = this.GetComponent<Canvas>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.canvas.worldCamera == null)
            this.canvas.worldCamera = Camera.main;
    }
}
