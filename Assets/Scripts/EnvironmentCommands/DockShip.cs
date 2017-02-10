using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class DockShip : NetworkBehaviour, CmdObj, IShipSpawnObject
{
    Ship ship;
    void Awake()
    {
        ship = GetComponentInParent<Ship>();
    }

    public bool canExecuteCommand()
    {
        if(DockingPoint.DockingPoints.Any(m => Vector3.Distance(transform.position, m.transform.position) < 10))
            return true;

        return false;
    }

    public void doCommand(int senderId)
    {
        if (DockingPoint.DockingPoints.Any(m => Vector3.Distance(transform.position, m.transform.position) < 10))
        {
            var dockingTarget = DockingPoint.DockingPoints.First(m => Vector3.Distance(transform.position, m.transform.position) < 10);
            ship.AlignToTarget = dockingTarget.DockAlign;
            ship.Docking = true;
            maxTime = 5;
        }
    }

    public void localCommand()
    {
        // de-/ex-pand drawbridge
    }

    void setAlignToTarget()
    {
        // depending on free tile
    }

    public Transform AlignTo;
    public float maxTime = 0;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (MyAvatar.Instance.isServer)
        {
            if (maxTime > 0)
                maxTime -= Time.deltaTime;
            else if (ship.Docking)
                ship.Docking = false;
        }
    }

    public List<Vector2Int> TileConfig()
    {
        var mustHaveAZero = new List<Vector2Int>();
        mustHaveAZero.Add(new Vector2Int(-1, 0));
        mustHaveAZero.Add(new Vector2Int(1, 0));
        
        return mustHaveAZero;
    }
}
