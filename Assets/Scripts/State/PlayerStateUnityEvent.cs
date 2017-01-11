﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlayerStateUnityEvent : MonoBehaviour
{
    public BoolUpdate PlayerStateUpdated;
    public States TargetState;
    public bool ReverseOnOtherStates = true;
    public bool UseReversedValue = false;
    
    public void OnEnable()
    {
        MyAvatar.Instance.EventPlayerStateChanged += SendUpdate;
    }

    public void SendUpdate(States newState)
    {
        var b = (newState == TargetState);

        if (UseReversedValue)
            b = !b;

        if(ReverseOnOtherStates || newState == TargetState)
        PlayerStateUpdated.Invoke(b);
    }
    
    public void OnDisable()
    {
        MyAvatar.Instance.EventPlayerStateChanged -= SendUpdate;
    }
}

[System.Serializable]
public class PlayerStateUpdate : UnityEvent<States> { }


[System.Serializable]
public class BoolUpdate : UnityEvent<bool> { }