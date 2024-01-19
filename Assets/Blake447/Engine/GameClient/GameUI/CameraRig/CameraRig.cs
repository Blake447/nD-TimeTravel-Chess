using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{

    [SerializeField]
    Camera Camera;

    [SerializeField]
    GameObject CameraObject;

    [SerializeField]
    GameObject CameraTarget;

    public float mov_speed = 2.0f;
    public float foc_speed = 0.5f;
    public float rot_speed = 0.1f;

    public float hsense = 1.0f;
    public float vsense = 1.0f;

    public float hmove = 1.0f;
    public float vmove = 1.0f;
    private bool hasWarned = false;

    float curZoom = 20.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 50.0f;
    public float spdZoom = 5.0f;



    bool didRaycastMiss = false;
    bool isLocked;
    public void Lock()
    {
        isLocked = true;
    }
    public void Unlock()
    {
        isLocked = false;
    }
    public void SetRaycastMissed()
    {
        didRaycastMiss = true;
    }

    public Camera GetCamera()
    {
        return Camera;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetTarget(Vector3 position)
    {
        if (CameraTarget != null)
        {
            CameraTarget.transform.position = position;
        }
    }

    Vector3 startPosition;
    void GetMouseInput()
    {

        curZoom -= spdZoom * Input.mouseScrollDelta.y * Time.deltaTime * 60.0f;
        curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);
        Camera.gameObject.transform.localPosition = Vector3.forward * curZoom;

        if (didRaycastMiss)
        {
            if (!Input.GetMouseButton(0))
            {
                didRaycastMiss = false;
            }
        }
        if (Input.GetMouseButton(1))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f);
                Vector3 camera_right = Camera.main.transform.right;
                Vector3 camera_up = Camera.main.transform.up;
                CameraTarget.transform.position = CameraTarget.transform.position - camera_right * hmove * mouseDelta.x * curZoom * 0.005f;
                CameraTarget.transform.position = CameraTarget.transform.position - camera_up * vmove * mouseDelta.y * curZoom * 0.005f;
            }
            else
            {
                Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f);
                float offset_x = mouseDelta.x * hsense;
                float offset_y = mouseDelta.y * vsense;

                Quaternion xrotation = Quaternion.AngleAxis(offset_x, Vector3.up);
                CameraTarget.transform.rotation = xrotation * CameraTarget.transform.rotation;

                Quaternion yrotation = Quaternion.AngleAxis(offset_y, CameraTarget.transform.right);
                CameraTarget.transform.rotation = yrotation * CameraTarget.transform.rotation;
            }
        }
        if (didRaycastMiss && !isLocked)
        {
            Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f);
            Vector3 camera_right = Camera.main.transform.right;
            Vector3 camera_up = Camera.main.transform.up;
            CameraTarget.transform.position = CameraTarget.transform.position - camera_right * hmove * mouseDelta.x * curZoom * 0.005f;
            CameraTarget.transform.position = CameraTarget.transform.position - camera_up * vmove * mouseDelta.y * curZoom * 0.005f;
        }


        
        //if (Input.GetMouseButtonDown(1))
        //{
        //    startPosition = Input.mousePosition;
        //}

        //if (Input.GetMouseButton(1))
        //{
        //    Vector3 mouseDelta = Input.mousePosition - startPosition;
        //    if (mouseDelta.magnitude > 100.0f)
        //    {
        //        float offset_x = mouseDelta.x * hsense;
        //        float offset_y = mouseDelta.y * vsense;

        //        Quaternion xrotation = Quaternion.AngleAxis(offset_x, Vector3.up);
        //        CameraTarget.transform.rotation = xrotation * CameraTarget.transform.rotation;

        //        Quaternion yrotation = Quaternion.AngleAxis(offset_y, CameraTarget.transform.right);
        //        CameraTarget.transform.rotation = yrotation * CameraTarget.transform.rotation;
        //    }

        //}
    }
    void RotatePivotToTarget()
    {
        if (CameraTarget != null && CameraObject != null)
        {
            CameraObject.transform.position = Vector3.MoveTowards(CameraObject.transform.position, CameraTarget.transform.position, mov_speed * Time.deltaTime);
            CameraObject.transform.rotation = Quaternion.RotateTowards(CameraObject.transform.rotation, CameraTarget.transform.rotation, rot_speed * Time.deltaTime);
        }
        else
        {
            if (!hasWarned)
            {
                Debug.LogWarning("Camera target not assigned");
                hasWarned = true;
            }
        }
    }

    void GetKeyInput()
    {
        if (!isLocked)
        {
            float hdir = 0.0f;
            float vdir = 0.0f;
            float time_offset = 20.0f;
            float mv_offset = 40.0f;

            Vector3 t_offset = Vector3.right;
            Vector3 m_offset = Vector3.forward;
            Vector3 camera_rt = Camera.main.transform.right;
            Vector3 camera_fw = Camera.main.transform.forward;

            Vector3[] tm_dirs = new Vector3[]
            {
                m_offset,
                t_offset,
                -m_offset,
                -t_offset
            };

            float[] tm_scales = new float[]
            {
                -mv_offset,
                time_offset,
                -mv_offset,
                time_offset
            };

            int max = 0;
            float max_dot = -10.0f;
            for (int i = 0; i < tm_dirs.Length; i++)
            {
                float dot = Vector3.Dot(camera_fw, tm_dirs[i]);
                if (dot > max_dot)
                {
                    max = i;
                    max_dot = dot;
                }
            }

            int offset = max;
            int cam_right = Input.GetKey(KeyCode.D) ? 1 : 0;
            int cam_forward = Input.GetKey(KeyCode.W) ? 1 : 0;
            int cam_left = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q)) ? 1 : 0;
            int cam_back = Input.GetKey(KeyCode.S) ? 1 : 0;

            int[] weights = new int[] { cam_right, cam_forward, cam_left, cam_back };

            Vector3 offset_dir = Vector3.zero;
            for (int i = 0; i < tm_dirs.Length; i++)
            {
                offset_dir += tm_dirs[i] * tm_scales[i] * weights[(offset + i + 3) % weights.Length];
            }

            Vector3 CameraForward = Vector3.ProjectOnPlane(cam_forward*camera_fw - cam_back * camera_fw, Vector3.up);
            Vector3 CameraRight   = Vector3.ProjectOnPlane(cam_right * camera_rt - cam_left * camera_rt, Vector3.up);
            Vector3 CameraOffset = CameraForward + CameraRight;




            CameraTarget.transform.position += CameraOffset *Time.deltaTime*90.0f * (curZoom / maxZoom);

            //CameraTarget.transform.position += offset_dir*0.0025f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //DriveCameraLocalPosition();
        GetMouseInput();
        GetKeyInput();
        RotatePivotToTarget();
    }
}
