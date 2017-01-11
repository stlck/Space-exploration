using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ReadInput : NetworkBehaviour
{
    public delegate void DualAxisInput(float ver, float hor);
    public delegate void LookingAtInput(Vector3 t);
    public delegate void MouseBtnClicked(int index, bool isown);

    [SyncVar]
    public float lastHorizontal;
    [SyncVar]
    public float lastVertical;

    public event DualAxisInput EventAxisOut;

    [SyncVar]
    public Vector3 lookingAtPosition;
    public event LookingAtInput EventLookingAtPosition;

    [SyncVar]
    public bool isMouseDown;
    public event MouseBtnClicked EventMouseDown;

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

                getLookingAt();

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

                getLookingAt();

                if (Input.GetMouseButtonUp(1))
                {
                    isMouseDown = false;
                    EventMouseDown(1, false);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    isMouseDown = true;
                    EventMouseDown(1, true);
                }
            }
        }

        if (EventAxisOut != null)
            EventAxisOut(lastVertical, lastHorizontal);
    }

    void getLookingAt()
    {
        var t = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - transform.position.y));

        if (isServer)
        {
            lookingAtPosition = t;
        }

        if (isClient)
            CmdSendLookingAt(t);

        EventLookingAtPosition(lookingAtPosition);
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
        EventLookingAtPosition(lookingAtPosition);
    }

    [Command]
    void CmdSendMouseBtnClick(int index, bool down)
    {
        isMouseDown = down;
        EventMouseDown(index, down);
    }
}
