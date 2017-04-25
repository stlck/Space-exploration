using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeamStats : NetworkBehaviour
{
    private static TeamStats instance;
    public static TeamStats Instance
    {
        get
        {
            return instance;
        }
    }

    [SyncVar]
    public int Credits = 0;
    public int StartCredits = 200;
    // weapons
    // storage

    // Use this for initialization
    void Start()
    {
        instance = this;

    }

    public override void OnStartLocalPlayer()
    {

    }

    public override void OnStartServer()
    {
        NetworkServer.Spawn(gameObject);
        Credits = StartCredits;
    }

    public void AddCredits(int amount)
    {
        if (isServer)
            Credits += amount;
        else
            MyAvatar.Instance.MyCommands.CmdAddCredits(amount);
    }
}
