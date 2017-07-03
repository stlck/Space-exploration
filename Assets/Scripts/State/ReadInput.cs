using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ReadInput : NetworkBehaviour
{
    [SyncVar]
    public float lastHorizontal;
    [SyncVar]
    public float lastVertical;

    [SyncVar]
    public Vector3 lookingAtPosition;

    [SyncVar]
    public bool isMouseDown;

    public Transform CurrentlyLookingAt;

    void Start()
    {
        lastHorizontal = 0f;
        lastVertical = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isClient)
        {
            if(isLocalPlayer)
            {
                float myValHor = Input.GetAxis("Horizontal");
                float myValVer = Input.GetAxis("Vertical");

                CmdSendInputToServer(myValVer, myValHor);

                lookAtMouse();

                if (Input.GetMouseButtonUp(1))
                    CmdSendMouseBtnClick(1, false);
                else if(Input.GetMouseButtonDown(1))
                    CmdSendMouseBtnClick(1, true);
            }
        }

        if(isServer)
        {
            if (isLocalPlayer)
            {
                lastHorizontal = Input.GetAxis("Horizontal");
                lastVertical = Input.GetAxis("Vertical");

                lookAtMouse();

                if (Input.GetMouseButtonUp(1))
                {
                    isMouseDown = false;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    isMouseDown = true;
                }
            }
        }
    }

    void getLookingAt()
    {
        Vector3 t = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - transform.position.y));

        var tUp = t + Vector3.up * 20;
        Ray r = new Ray(t,t);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 40))
        {
            CurrentlyLookingAt = hit.transform;
        }

        if (isServer)
            lookingAtPosition = t;
        if (isClient)
            CmdSendLookingAt(t);
    }

    void lookAtMouse ()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position + Vector3.up);

        // Generate a ray from the cursor position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float hitdist = 0.0f;
        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(ray, out hitdist))
        {
            bool FoundTarget = false;
            Vector3 targetPoint = Vector3.zero;
            RaycastHit info;
            if (Physics.Raycast(ray, out info, 100, Camera.main.cullingMask))
            {

                switch (info.collider.tag)
                {
                    case "Block":
                        FoundTarget = true;
                        // if y < player add V3.up
                        // else aim at point
                        targetPoint = info.point;
                        if (info.point.y <= transform.position.y)
                            targetPoint.y += 1f;

                        break;
                    case "Player":
                        FoundTarget = true;
                        targetPoint = info.point;
                        break;
                    default:
                        break;
                }
            }

            // Get the point along the ray that hits the calculated distance.
            if (!FoundTarget)
            {
                targetPoint = ray.GetPoint(hitdist);
                targetPoint.y = transform.position.y;
            }
            var transformlookat = targetPoint;
            transformlookat.y = transform.position.y;
            //transform.LookAt(transformlookat);
            if (isServer)
                lookingAtPosition = (targetPoint);
            else if (isClient)
                CmdSendLookingAt(targetPoint);

            //IKRight.LookAt(targetPoint);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);
    }

    [Command]
    void CmdSendInputToServer(float ver, float hor)
    {
        lastHorizontal = hor;
        lastVertical = ver;
    }

    [Command]
    void CmdSendLookingAt(Vector3 target)
    {
        lookingAtPosition = target;
    }

    [Command]
    void CmdSendMouseBtnClick(int index, bool down)
    {
        isMouseDown = down;
    }
}
