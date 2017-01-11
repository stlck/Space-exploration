using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class RotateToMouse : NetworkBehaviour {

    public Vector3 mousePos;

	// Update is called once per frame
	void Update () {

        if (isLocalPlayer)
        { 
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - transform.position.y));
            //transform.LookAt(mousePos);
            CmdSetRot(mousePos);
        }

        //if(isServer)
        //    transform.LookAt(mousePos);
    }

    [Command]
    public void CmdSetRot(Vector3 p)
    {
        transform.LookAt(p);
    }
}
