using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StatBase : MonoBehaviour {

    public float MaxHealth = 10;
    public float CurrentHealth = 10;
    public int CreditsOnKill = 0;

    //Texture2D hpTex;

    public Transform EffectOnDeath;
    public Transform EffectOnSpawn;
    public Transform EffectOnHit;

    void Awake()
    {
        //hpTex = new Texture2D(1,1);
        //hpTex.SetPixel(0, 0, Color.red);
        //hpTex.Apply();
    }

    void Start()
    {
        if(EffectOnSpawn != null)
        {
            Instantiate(EffectOnSpawn, transform.position, EffectOnSpawn.rotation);
        }
    }

    void OnDestroy()
    {
        if(Application.isPlaying && EffectOnDeath != null)
        {
            Instantiate(EffectOnDeath, transform.position, EffectOnDeath.rotation);
        }
    }

    // Update is called once per frame
    //[ServerCallback]
    void Update () {
        if (CurrentHealth <= 0 && MyAvatar.Instance != null && MyAvatar.Instance.isServer && GetComponent<NetworkIdentity>() != null)
            KillObject();

    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 directionFrom)
    {
        CurrentHealth -= amount;
        if(EffectOnHit != null)
        {
            Instantiate(EffectOnHit, hitPoint, Quaternion.Euler(-directionFrom));
        }
    }

   /* void OnGUI()
    {
        if(CurrentHealth < MaxHealth && CurrentHealth > 0)
        {
            var p = Camera.main.WorldToScreenPoint(transform.position + Vector3.up);
            //GUI.HorizontalScrollbar(new Rect(p.x,p.y, 30, 5), CurrentHealth / MaxHealth, 5, 0, 1);
            GUI.DrawTexture(new Rect(p.x, Screen.height - p.y, (CurrentHealth / MaxHealth) * 30, 5), hpTex);
        }
    }*/

    public void KillObject()
    {
        if(MyAvatar.Instance.isServer)
        {
            TeamStats.Instance.AddCredits(CreditsOnKill);
            if (gameObject.GetComponent<MyAvatar>() == null)
                NetworkServer.Destroy(gameObject);
            else
                gameObject.GetComponent<MyAvatar>().RpcSetDead();
        }
    }
}
