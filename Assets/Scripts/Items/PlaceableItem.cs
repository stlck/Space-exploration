using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlaceableItem : BaseItem {

    public BaseItem Item;
    public bool Active = false;
    public Transform ActiveVisual;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(ActiveVisual.gameObject.activeInHierarchy != Active)
            ActiveVisual.gameObject.SetActive(Active);
	}

    public void PlaceItem()
    {
        Active = false;

        NetworkHelper.Instance.SpawnItem(Item, ActiveVisual.transform.position);
    }

}
