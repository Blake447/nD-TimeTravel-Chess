using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messanger : MonoBehaviour
{

    public static Messanger singleton;
    public TMPro.TMP_Text output;

    // Start is called before the first frame update
    void Start()
    {
        if (singleton == null)
            singleton = this;
    }

    public static void DisplayMessage(string message)
    {
        singleton.output.gameObject.SetActive(true);
        singleton.output.text = message;
        singleton.StartCoroutine(singleton.FadeMessage());
    }
    IEnumerator FadeMessage()
    {
        float timer = 5.0f;
        while (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            output.color = new Color(output.color.r, output.color.g, output.color.b, Mathf.Clamp01(timer));
            yield return null;
        }
        output.gameObject.SetActive(false);
        //yield return null;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
