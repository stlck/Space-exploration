using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StatBase : MonoBehaviour {

    public float MaxHealth = 10;
    public float CurrentHealth = 10;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(MyAvatar.Instance.isServer && CurrentHealth <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
	}

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
    }
}
