using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightArrow : MonoBehaviour
{
    [SerializeField] GameObject ArrowBase;
    [SerializeField] GameObject ArrowTip;


    public void SetArrow(Vector3 start, Vector3 end)
    {
        float scale = this.gameObject.transform.lossyScale.z;
        float rscale = 1.0f / scale;

        Vector3 offset = (end - start);
        Quaternion towards_to = Quaternion.FromToRotation(Vector3.up, offset.normalized);
        this.gameObject.transform.rotation = towards_to;
        ArrowTip.transform.localPosition = Vector3.up * (offset.magnitude - 0.75f) * rscale;
        ArrowBase.transform.localScale = new Vector3(100, 100 * (offset.magnitude - 0.75f) * rscale, 100);
        this.transform.position = start + Vector3.up * 0.5f;
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
