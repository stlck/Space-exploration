using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerCommandObject : NetworkBehaviour {

    public void SendCommand(NetworkInstanceId nId, int senderId)
    {
        Debug.Log("ServerCommandObject " + nId + " id " + senderId);

        CmdCommand(nId, senderId);
    }

    [Command]
    public void CmdCommand(NetworkInstanceId nId, int senderId)
    {
        Debug.Log("ServerCommandObject receiving " + nId);

        var obj = NetworkServer.FindLocalObject(nId);
        obj.GetComponent<CommandObject>().DoCommand(senderId);
    }
}


/*
 object:
    command object
        click - send command
        can take input
            axis
            mouse

    
     
 * */