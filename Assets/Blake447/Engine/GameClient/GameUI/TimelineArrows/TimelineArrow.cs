using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineArrow : MonoBehaviour
{
    public GameObject ArrowTemplate;
    public GameObject ArrowRoot;
    List<GameObject> Arrows;

    // Start is called before the first frame update
    public void UpdateArrow(int index, Vector3 from, Vector3 to)
    {
        if (index >= 0 && index < Arrows.Count)
        {
            GameObject arrow = Arrows[index];
            PointArrowVector(arrow, from, to, 0.0f);
        }
    }

    public void InitializeArrows()
    {
        Arrows = new List<GameObject>();
        GameObject rootArrow = Instantiate(ArrowTemplate);
        Arrows.Add(rootArrow);
    }
    public void PointArrowVector(GameObject arrow, Vector3 from, Vector3 to, float decrement)
    {
        float main_scale = 1.0f;

        arrow.SetActive(true);
        arrow.transform.position = from;

        Vector3 offset = (to - arrow.transform.position);
        float distance = offset.magnitude / (arrow.transform.localScale.x * main_scale) - decrement / main_scale;

        Quaternion rotation = Quaternion.LookRotation(offset.normalized, Vector3.up);

        arrow.transform.rotation = rotation;
        GameObject arrow_stem = arrow.transform.GetChild(0).gameObject;
        GameObject arrow_tip = arrow.transform.GetChild(1).gameObject;

        if (arrow_stem != null)
        {
            arrow_stem.transform.localScale = new Vector3(25, 25 * (distance * 4.0f), 25);
        }
        if (arrow_tip != null)
        {
            arrow_tip.transform.localPosition = Vector3.forward * distance;
        }
    }

}
