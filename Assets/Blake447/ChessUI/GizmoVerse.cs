using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GizmoNode
{
    public int iFrom;
    public int tStart;
    public int tEnd;
};
public class GizmoVerse : MonoBehaviour
{
    public PurpleArrow purpleArrowTemplate;
    public GameObject purplerArrowRoot;

    List<PurpleArrow> purpleArrows = new List<PurpleArrow>();

    List<GizmoNode> timelineList = new List<GizmoNode>();
    MVTime mvtime;
    float offset_x = 1.0f;
    float offset_y = -1.0f;
    float offset_z = 1.0f;

    public void ClearGizmoVerse()
    {
        foreach (PurpleArrow arrow in purpleArrows)
        {
            Destroy(arrow.gameObject);
        }
        purpleArrows.Clear();

        timelineList.Clear();
        GizmoNode rootNode = new GizmoNode();
        rootNode.iFrom = 0;
        rootNode.tStart = 0;
        rootNode.tEnd = 0;
        timelineList.Add(rootNode);
    }

    public void AddMoveToHistory(Move move)
    {
        Command command = move.head;
        int[] from = (int[])command.from.Clone();
        int[] to = (int[])command.to.Clone();

        int mFrom = from[from.Length - 2];
        int mTo = to[to.Length - 2];
        
        int tFrom = from[from.Length - 1];
        int tTo = to[to.Length - 1];

        int iFrom = TimelineToIndex(mFrom);
        int iTo = TimelineToIndex(mTo);

        //Debug.Log("Adding move with iFrom: " + iFrom + " and iTo: " + iTo);

        if (iFrom == iTo && tFrom == tTo)
        {
            GizmoNode node = timelineList[iTo];
            node.tEnd++;
        }
        else
        {
            GizmoNode nodeFrom = timelineList[iFrom];
            GizmoNode nodeTo = timelineList[iTo];
            if (tTo < nodeTo.tEnd)
            {
                int color = (command.pfrom / 32);
                bool isBlack = color == 1;
                int maxWhite = 0;
                int maxBlack = 0;
                
                for (int i = 0; i < timelineList.Count; i++)
                {
                    GizmoNode node = timelineList[i];
                    int m = IndexToTimeline(i);
                    if (node.tStart != -1)
                    {
                        // if we have a valid node
                        maxWhite = Mathf.Max(maxWhite, m);
                        maxBlack = Mathf.Min(maxBlack, m);
                    }
                }
                int target_wi = TimelineToIndex(maxWhite + 1);
                int target_bi = TimelineToIndex(maxBlack - 1);
                int count = timelineList.Count;
                while (target_wi >= timelineList.Count || target_bi >= timelineList.Count)
                {
                    //Debug.Log("Target White: " + target_wi + " - Target Black: " + target_bi + " - timelineList.Count: " + timelineList.Count);
                    GizmoNode empty = new GizmoNode();
                    empty.iFrom = -1;
                    empty.tStart = -1;
                    empty.tEnd = -1;
                    timelineList.Add(empty);
                    count++;
                }
                int target = isBlack ? target_bi : target_wi;
                GizmoNode targetGizmo = timelineList[target];
                targetGizmo.iFrom = iTo;
                targetGizmo.tStart = tTo + 1;
                targetGizmo.tEnd = tTo + 1;

                nodeFrom.tEnd++;
            }
            else
            {
                nodeFrom.tEnd++;
                nodeTo.tEnd++;
            }
        }
    }

    Vector3 CalculatePosition(float mv, float t)
    {
        Vector3 boardOffset = Vector3.zero;
        if (mvtime == null)
            mvtime = GetComponent<MVTime>();
        if (mvtime != null)
        {
            Vector2 offsets = mvtime.GetOffsets();
            offset_x = offsets.x;
            offset_z = -offsets.y;
            boardOffset = mvtime.GetBoardOffset();
        }


        return offset_x * -Vector3.right * t + offset_z * Vector3.forward * mv + boardOffset - Vector3.up * 0.8f;
    }



    public void PrintTimelines()
    {
        for (int i = 0; i < timelineList.Count; i++)
        {
            GizmoNode node = timelineList[i];
            //Debug.Log("node " + i.ToString("00") + " - iFrom: " + node.iFrom + " - tStart: " + node.tStart + " - tEnd: " + node.tEnd);
        }
        SetArrows();
    }

    public void SetArrows()
    {
        while (purpleArrows.Count < timelineList.Count)
        {
            purpleArrows.Add(Instantiate(purpleArrowTemplate));
        }
        for (int i = 0; i < timelineList.Count; i++)
        {
            if (timelineList[i].tStart == -1)
                purpleArrows[i].gameObject.SetActive(false);


            GizmoNode node = timelineList[i];
            int mv_start = IndexToTimeline(node.iFrom);
            int t_start = node.tStart;

            int mv_end = IndexToTimeline(i);
            int t_end = node.tEnd;


            

            Vector3 start = CalculatePosition(mv_start, t_start);
            Vector3 end = CalculatePosition(mv_end, t_end);
            if (i == 0) start -= Vector3.forward * offset_z * 0.75f;
            start += Vector3.right * offset_x * 0.5f;
            end -= Vector3.right * offset_x * 0.5f;

            purpleArrows[i].PointArrow(start, end);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int TimelineToIndex(int m)
    {
        int offset = m < 0 ? -1 : 0;
        return Mathf.Abs(m) * 2 + offset;
    }
    int IndexToTimeline(int m)
    {
        int sign = (m % 2) == 1 ? -1 : 1;
        return ((m + 1) / 2) * sign;
    }
}
