using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StatBase : MonoBehaviour {

    public float MaxHealth = 10;
    public float CurrentHealth = 10;

    // Update is called once per frame
    //[ServerCallback]
    void Update () {
		if (CurrentHealth <= 0 && MyAvatar.Instance != null && MyAvatar.Instance.isServer)
            NetworkServer.Destroy(gameObject);
	}

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
    }
}
