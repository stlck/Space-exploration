using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingPoint : MonoBehaviour
{
    public static List<DockingPoint> DockingPoints = new List<DockingPoint>();
    public Transform DockAlign;

    void Awake()
    {
        DockingPoints.Add(this);
    }

    /// <summary>
    ///  returns true if there's a ship nearby
    /// </summary>
    /// <returns></returns>
    public bool ShipDocked()
    {
        var close = (Physics.OverlapSphere(transform.position, 10));
        foreach (var c in close)
            if (c.GetComponent<Ship>() != null)
                return true;
        return false;
    }
}
