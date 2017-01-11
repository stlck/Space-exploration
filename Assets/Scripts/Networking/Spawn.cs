using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spawn : NetworkBehaviour
{

    public GameObject Target;
    public bool OnStart = true;

    // Use this for initialization
    void Start()
    {
        if (OnStart)
            spawnObj();
    }

    void spawnObj()
    {
        var t = Instantiate(Target);
        NetworkServer.Spawn(t);
    }

}
