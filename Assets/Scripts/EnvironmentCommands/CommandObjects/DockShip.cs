using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class DockShip : NetworkBehaviour, CmdObj, IShipSpawnObject
{

    public Transform AlignTo;
    public float maxTime = 0;
    [SyncVar]
    public int TilePositionX;
    [SyncVar]
    public int TilePositionY;
    public Vector3 AlignPosition;

    Ship ship;

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        checkParent();

        setAlignToTarget();
    }

    void checkParent()
    {
        ship = GetComponentInParent<Ship>();
        if (AlignTo == null)
        {
            AlignTo = new GameObject("AlignTArget").transform;
            AlignTo.transform.SetParent(transform);
            AlignTo.transform.localPosition = Vector3.zero;
        }
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
            checkParent();

            var dockingTarget = DockingPoint.DockingPoints.First(m => Vector3.Distance(transform.position, m.transform.position) < 10);
            
            // Use alignposition if a bridge is required!
            ship.DocingPosition = dockingTarget.DockAlign.position - transform.localPosition * 2;// - AlignPosition;
            ship.DocingRotation = dockingTarget.DockAlign.rotation;
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
        if(ship.tiles[TilePositionX +1, TilePositionY] == 0)
        {
            AlignPosition = /* transform.position +*/ Vector3.right * ship.ShipScale;
        }
        else if(ship.tiles[TilePositionX -1, TilePositionY] == 0)
        {
            AlignPosition =/* transform.position + */Vector3.left * ship.ShipScale;
        }
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

    public void SetTilePosition(Vector2Int position)
    {
        TilePositionX = position.x;
        TilePositionY = position.y;
    }
}
