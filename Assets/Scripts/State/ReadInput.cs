using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ReadInput : NetworkBehaviour
{
    public delegate void DualAxisInput(float ver, float hor);
    public delegate void LookingAtInput(Vector3 t);

    [SyncVar]
    public float lastHorizontal;
    [SyncVar]
    public float lastVertical;

    public event DualAxisInput AxisOut;

    [SyncVar]
    public Vector3 lookingAtPosition;
    public event LookingAtInput LookingAtPosition;

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

                //transform.LookAt(mousePos);
                getLookingAt();
            }
            //Vertical.Invoke(lastVertical);
            //Horizontal.Invoke(lastHorizontal);
        }

        if(isServer)
        {
            if (isLocalPlayer)
            {
                lastHorizontal = Input.GetAxis("Horizontal");
                lastVertical = Input.GetAxis("Vertical");

                getLookingAt();
            }
        }

        if (AxisOut != null)
            AxisOut(lastVertical, lastHorizontal);
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

        LookingAtPosition(lookingAtPosition);
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

        //Vertical.Invoke(lastVertical);
        //Horizontal.Invoke(lastHorizontal);
    }

    [Command]
    void CmdSendLookingAt(Vector3 target)
    {
        lookingAtPosition = target;
        LookingAtPosition(lookingAtPosition);
    }
}
