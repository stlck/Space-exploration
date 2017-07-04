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
        if (Item == null)
        {
            Debug.Log("No Item To Pickup", gameObject);
            return;
        }

        if (collision.gameObject.tag == "Player" /*&& collision.gameObject.GetComponent<MyAvatar>().isLocalPlayer*/)
        {
            var avatarToPickup = collision.transform.root.GetComponent<MyAvatar>();
            if (avatarToPickup == MyAvatar.Instance)
                MyAvatar.Instance.InventoryItems.Add(Item);
            //Destroy(gameObject);

            if (MyAvatar.Instance.isServer)
                NetworkHelper.Instance.SrvDestroyObject(gameObject);
        }
    }
}
