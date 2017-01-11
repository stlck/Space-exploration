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

    
}
