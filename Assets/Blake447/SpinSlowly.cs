using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSlowly : MonoBehaviour
{
    public float spinSpeed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0.0f, spinSpeed * Time.deltaTime, 0.0f);   
    }
}
