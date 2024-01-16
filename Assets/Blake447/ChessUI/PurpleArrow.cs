using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleArrow : MonoBehaviour
{

    public void PointArrow(Vector3 start, Vector3 end)
    {
        Vector3 start_swizzle = new Vector3(start.z, start.y, start.x);
        Vector3 end_swizzle = new Vector3(end.z, end.y, end.x);



        SetTimelineArrow(start, end);
    }

    public void SetTimelineArrow(Vector3 from, Vector3 target)
    {
        float main_scale = 1.0f;

        GameObject arrow_root = this.gameObject;
        GameObject arrow = this.gameObject;

        arrow_root.SetActive(true);
        arrow_root.transform.position = from;

        float lateral_distance = 0.75f;
        float forward_distance = 0.50f;
        float segment_distance = 0.25f;
        float epsilon = .01f;

        float scaling = arrow_root.transform.localScale.x;

        Vector3 offset = (target - arrow.transform.position) / scaling;
        float lateral_projection = Vector3.Dot(offset, arrow_root.transform.right) / main_scale;
        float forward_projection = Vector3.Dot(offset, arrow_root.transform.forward) / main_scale;

        forward_projection = Mathf.Clamp(forward_projection, forward_distance, 10000000.0f);
        if (lateral_projection < 0.0f)
        {
            lateral_projection = Mathf.Clamp(lateral_projection, -10000000.0f, -lateral_distance);
            arrow.transform.rotation = arrow_root.transform.rotation * Quaternion.Euler(0, 0, 0);
        }
        else
        {
            lateral_projection = Mathf.Clamp(lateral_projection, lateral_distance, 10000000.0f);
            arrow.transform.rotation = arrow_root.transform.rotation * Quaternion.Euler(0, 0, 180);
        }

        float midsegment_scale = Mathf.Abs(lateral_projection) - segment_distance * 2.0f;
        float forsegment_scale = Mathf.Clamp(forward_projection - forward_distance, epsilon, 10000000.0f);

        GameObject arrowSeg0 = arrow.transform.GetChild(0).gameObject;
        GameObject arrowSeg1 = arrow.transform.GetChild(1).gameObject;
        GameObject arrowSeg2 = arrow.transform.GetChild(2).gameObject;
        GameObject arrowSeg3 = arrow.transform.GetChild(3).gameObject;
        GameObject arrowSeg4 = arrow.transform.GetChild(4).gameObject;

        arrowSeg1.transform.localScale = new Vector3(segment_distance, segment_distance, midsegment_scale);
        arrowSeg3.transform.localScale = new Vector3(segment_distance, segment_distance, forsegment_scale);

        arrowSeg2.transform.localPosition = new Vector3(-segment_distance * 1.0f - midsegment_scale, 0.0f, segment_distance * 1.0f);
        arrowSeg3.transform.localPosition = new Vector3(-segment_distance * 2.0f - midsegment_scale, 0.0f, segment_distance * 2.0f);
        arrowSeg4.transform.localPosition = new Vector3(-segment_distance * 2.0f - midsegment_scale, 0.0f, segment_distance * 2.0f + forsegment_scale - epsilon);
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
