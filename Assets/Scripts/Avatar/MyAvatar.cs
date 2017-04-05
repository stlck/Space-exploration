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
            }
        }
    }
    
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
            if(CurrentState == States.OnShip && Input.GetKeyDown(KeyCode.Escape))// && CurrentMovementBase != null)
            {
                CmdDefaultMovementInput();
            }
        }
        if(isServer)
        {
            if (transform.position.y < 0f)
                transform.position += Vector3.up * Mathf.Abs(transform.position.y);

            //Debug.Log("Is server, handling movement");
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

    [Command]
    public void CmdSpawnShip(string t, string c, int sizex, int sizey, Vector3 position, Quaternion rotation)
    {
        ShipSpawner.FromClientCreateShip(t, c, sizex, sizey, position, rotation);
    }

    [Command]
    public void CmdWarpShip(NetworkInstanceId targetShip, Vector3 target)
    {
        var s = NetworkServer.FindLocalObject(targetShip);
        if (s != null)
            s.GetComponent<Ship>().ServerWarp(target);
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