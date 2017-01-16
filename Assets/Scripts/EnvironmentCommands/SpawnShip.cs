using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CommandObject))]
public class SpawnShip : NetworkBehaviour, CmdObj
{
    public GameObject Ship;
    public DockingPoint TargetDock;

    public void doCommand(int senderId)
    {
        Debug.Log("SPAWNING SHIP");

        var s = Instantiate(Ship);
        NetworkHelper.Instance.SpawnObject(s);
        if(TargetDock != null)
        {
            s.transform.position = TargetDock.DockAlign.position;
            s.transform.rotation = TargetDock.DockAlign.rotation;
        }
    }

    public void localCommand()
    {

    }

    public bool canExecuteCommand()
    {
        if(TargetDock == null || !TargetDock.ShipDocked())
            return true;
        return false;
    }
}
