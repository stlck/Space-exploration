using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Linq;

public class MyAvatar : NetworkBehaviour
{
    private static MyAvatar instance;
    public static MyAvatar Instance
    {
        get
        {
            return instance;
        }
    }

    public NetworkIdentity Identity;
    public ReadInput MyInput;
    public MoveAvator Movement;
    public MovementBase CurrentMovementBase;
    public ServerCommandObject ServerCommands;
    public AvatarWeaponHandler AvatarWeaponHandler;
    public StatBase MyStats;
    public List<BaseItem> InventoryItems = new List<BaseItem>();
    
    public CreateEnvironment EnvironmentCreator;

    [SyncVar]
    public int PlayerId;

    [SyncVar]
    public bool CanMove = true;

    [SyncVar]
    public States CurrentState;

    [SyncEvent]
    public event StateChange EventPlayerStateChanged;
    public delegate void StateChange(States newState);

    [SyncVar]
    public string test = "not set";

    public UnityEvent IsLocal;

    // Use this for initialization
    void Awake()
    {
        MyStats = GetComponent<StatBase>();
        InventoryItems = new List<BaseItem>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        instance = this;
        //MyInput.EventAxisOut += updateMovement;
        //MyInput.EventLookingAtPosition += updateLookingAt;

        IsLocal.Invoke();
    }

    void Start()
    {
        if (Identity.isServer)
        {
            CurrentState = States.Avatar;
            NetworkHelper.Instance.AllPlayers.Add(this);
            PlayerId = NetworkHelper.Instance.AllPlayers.Count;
            NetworkServer.SpawnObjects();
            transform.position = Vector3.up * .51f;

            if(!isLocalPlayer)
            {
                IsLocal.Invoke();
                //MyInput.EventAxisOut += updateMovement;
                //MyInput.EventLookingAtPosition += updateLookingAt;
            }
        }
    }
    /*
    void updateMovement(float v, float h)
    {
        if(isClient && CanMove)//CurrentState == States.Avatar)
        { 
            Movement.SetInput(v, h);
        }
        else if(isServer)
        {

            if (CurrentMovementBase != null)
                CurrentMovementBase.SetInput(v, h);
            else if( CanMove)
                Movement.SetInput(v, h);
        }
    }

    void updateLookingAt(Vector3 t)
    {
        if (isClient && CanMove)//CurrentState == States.Avatar)
        {
            Movement.SetLookingAt(t);
        }
        else if (isServer)
        {
            if (CurrentMovementBase != null)
                CurrentMovementBase.SetLookingAt(t);
            else if (CanMove)
                Movement.SetLookingAt(t);
        }

    }*/
    
    void Update()
    {
        // set parent on new collider
        RaycastHit hit;
        if( Physics.Raycast(transform.position, transform.up * -1, out hit, 1, Movement.walkable))
        {
            if (hit.transform.root != transform.parent)
                SetParent(hit.transform.root);
        }

        if(isLocalPlayer)
        {
            if(CurrentState == States.OnShip && Input.GetKeyDown(KeyCode.Escape) && CurrentMovementBase != null)
            {
                CmdDefaultMovementInput();
                //SetMovementInput(CurrentMovementBase);
                //CmdReleaseShip();
            }
        }
        if(isServer)
        {
            Debug.Log("Is server, handling movement");
            if(CurrentMovementBase != null)
            { 
                CurrentMovementBase.SetMouseDown(MyInput.isMouseDown);
                CurrentMovementBase.SetLookingAt(MyInput.lookingAtPosition);
                CurrentMovementBase.SetInput(MyInput.lastVertical, MyInput.lastHorizontal);
            }
            else
            {
                Movement.SetMouseDown(MyInput.isMouseDown);
                Movement.SetLookingAt(MyInput.lookingAtPosition);
                Movement.SetInput(MyInput.lastVertical, MyInput.lastHorizontal);
            }
        }
        else if (isClient && isLocalPlayer && CurrentState == States.Avatar)
        {
            Movement.SetLookingAt(MyInput.lookingAtPosition);
            Movement.SetInput(MyInput.lastVertical, MyInput.lastHorizontal);
        }
        if (isServer && CurrentState == States.Avatar && MyInput.isMouseDown)
        {
            AvatarWeaponHandler.ServerFire();
        }
    }

    public void SetParent(Transform newP)
    {
        transform.SetParent(newP, true);
    }

    public void SetMovementInput(MovementBase s)
    {
        CmdControlTarget(s.GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    public void CmdDefaultMovementInput()
    {
        if (CurrentMovementBase != null)
        {
            removeCurrentMovement();
        }
    }

    void removeCurrentMovement()
    {
        CurrentMovementBase.ReleaseControl();
        CurrentMovementBase.SetInput(0, 0);
        CurrentMovementBase.SetLookingAt(Vector3.zero);
        CurrentMovementBase = null;
        ChangeState(States.Avatar);
    }

    [Command]
    void CmdControlTarget(NetworkInstanceId id)
    {
        var mov = NetworkServer.FindLocalObject(id).GetComponent<MovementBase>();
        if(mov != CurrentMovementBase)
        {
            CurrentMovementBase = NetworkServer.FindLocalObject(id).GetComponent<MovementBase>();
            ChangeState(States.OnShip);
            CurrentMovementBase.TakeControl();
        }
        else
            removeCurrentMovement();
    }

    public void ChangeState(States newState)
    {
        CurrentState = newState;
        EventPlayerStateChanged(newState);
    }

    [Command]
    public void CmdAddCredits(int amount)
    {
        TeamStats.Instance.AddCredits(amount);
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