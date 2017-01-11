using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class CommandObject : NetworkBehaviour {

    public UnityEvent OnCommand;
    [Header("Command object")]
    public MonoBehaviour CommandTarget;
    public NetworkIdentity Identity;

    public void OnMouseUp()
    {
        Debug.Log("Sending command " + this.netId);
        if(CommandTarget != null && (CommandTarget as CmdObj).canExecuteCommand())
            MyAvatar.Instance.ServerCommands.SendCommand(this.netId, MyAvatar.Instance.PlayerId);
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