using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerState : MonoBehaviour {

    private static PlayerState instance;
    public static PlayerState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("playerState").AddComponent<PlayerState>();
            }
            return instance;
        }
    }

    private States currentState;
    public States CurrentState
    {
        get
        {
            return currentState;
        }
        private set
        {
            currentState = value;
            if(PlayerStateChanged != null)
                PlayerStateChanged.Invoke(currentState);
        }
    }

    public event StateChange PlayerStateChanged;
    public delegate void StateChange(States newState);

    public MyAvatar PlayerPrefab;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        CurrentState = States.Avatar;
    }

    public void ChangeState(States newState)
    {
        CurrentState = newState;
    }
}

public enum States
{
    Avatar,
    OnShip,
    Dead,
}

[System.Serializable]
public class StateFloatValue
{
    public States State;
    public float Value;
}