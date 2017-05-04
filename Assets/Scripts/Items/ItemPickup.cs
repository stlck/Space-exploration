using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

    public BaseItem Item;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter (Collision collision)
    {
        if(collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<MyAvatar>().isLocalPlayer)
        {
            MyAvatar.Instance.InventoryItems.Add(Item);
            Destroy(gameObject);
        }
    }
}
