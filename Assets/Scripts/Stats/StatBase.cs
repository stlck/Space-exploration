using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StatBase : MonoBehaviour {

    public float MaxHealth = 10;
    public float CurrentHealth = 10;
    Texture2D hpTex;

    void Awake()
    {
        hpTex = new Texture2D(1,1);
        hpTex.SetPixel(0, 0, Color.red);
        hpTex.Apply();
    }

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

    void OnGUI()
    {
        if(CurrentHealth < MaxHealth && CurrentHealth > 0)
        {
            var p = Camera.main.WorldToScreenPoint(transform.position + Vector3.up);
            //GUI.HorizontalScrollbar(new Rect(p.x,p.y, 30, 5), CurrentHealth / MaxHealth, 5, 0, 1);
            GUI.DrawTexture(new Rect(p.x, Screen.height - p.y, (CurrentHealth / MaxHealth) * 30, 5), hpTex);
        }
    }
}
