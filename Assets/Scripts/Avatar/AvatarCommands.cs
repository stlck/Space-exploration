using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AvatarCommands : NetworkBehaviour
{
    // use for commands to server thats not used on the avatar


    [Command]
    public void CmdAddCredits (int amount)
    {
        TeamStats.Instance.AddCredits(amount);
    }

    [Command]
    public void CmdSpawnShip (string t, string c, int sizex, int sizey, Vector3 position, Quaternion rotation)
    {
        ShipSpawner.FromClientCreateShip(t, c, sizex, sizey, position, rotation);
    }

    [Command]
    public void CmdWarpShip (NetworkInstanceId targetShip, Vector3 target)
    {
        var s = NetworkServer.FindLocalObject(targetShip);
        if (s != null)
            s.GetComponent<Ship>().ServerWarp(target);
    }
}
