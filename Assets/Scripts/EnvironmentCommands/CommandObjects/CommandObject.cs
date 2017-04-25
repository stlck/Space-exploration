using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class CommandObject : NetworkBehaviour
{
    public UnityEvent OnCommand;
    [Header("Command object")]
    public MonoBehaviour CommandTarget;
    public NetworkIdentity Identity;
    public float RequiredRange = 3;


    public void OnMouseUp()
    {
        if (Vector3.Distance(transform.position, MyAvatar.Instance.transform.position) < RequiredRange)
        {
            if (CommandTarget != null && (CommandTarget as CmdObj).canExecuteCommand())
                MyAvatar.Instance.ServerCommands.SendCommand(this.netId, MyAvatar.Instance.PlayerId);
        }
        else
            RangeIndicator.Instance.TurnOn(transform.position, RequiredRange);
    }

    public void DoCommand(int senderId)
    {
        OnCommand.Invoke();
        (CommandTarget as CmdObj).localCommand();
        (CommandTarget as CmdObj).doCommand(senderId);
    }
}

interface CmdObj
{
    bool canExecuteCommand();
    void localCommand();
    void doCommand(int senderId);
}