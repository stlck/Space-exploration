using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CommandObject))]
public class SpawnShip : NetworkBehaviour, CmdObj
{
    public GameObject Ship;

    public void doCommand(int senderId)
    {
        Debug.Log("SPAWNING SHIP");

        var s = Instantiate(Ship);
        NetworkHelper.Instance.SpawnObject(s);
        //NetworkServer.Spawn(s);
        //NetworkServer.SpawnObjects();
    }

    public void localCommand()
    {

    }

    public bool canExecuteCommand()
    {
        return true;
    }
}
