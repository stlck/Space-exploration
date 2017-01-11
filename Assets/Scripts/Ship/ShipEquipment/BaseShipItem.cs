using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShipItem : BaseItem
{
    public ShipItemType Type;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public enum ShipItemType
{
    Weapon,
    Shields,
    Engine,
}