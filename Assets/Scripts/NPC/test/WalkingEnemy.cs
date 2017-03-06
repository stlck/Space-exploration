using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WalkingEnemy : NpcBase{

    public BaseWeapon Weapon;
    public List<Vector3> Route;
    Vector3 currentRouteTarget;
    Vector3 prevPosition;
    BestFirstSearch bfs;

    public override void Start()
    {
        base.Start();
        if(Spawner.BestFirstSearch != null)
        {
            bfs = Spawner.BestFirstSearch;
        }
    }

    public override void Update()
    {
        base.Update();
        //currentTimer += Time.deltaTime;
    }

    public override void Move(Vector3 position)
    {
        base.Move(position);
        Route = bfs.FindPath((int)transform.position.x, (int)transform.position.z, (int)position.x, (int)position.z);
        if (Route.Any())
        {
            prevPosition = transform.position;
            currentRouteTarget = Route.First() + Vector3.up;
        }
        //bfs.FindPath((int)transform.localPosition.x, (int)transform.localPosition.z, (int)position.x, (int)position.z);
    }

    public override void UpdateMove()
    {
        base.UpdateMove();
        transform.position = Vector3.Lerp(prevPosition, currentRouteTarget, currentTimer / currentTime);

        if (currentTimer / currentTime >= 1f)
        {
            Route.RemoveAt(0);
            currentTimer = ChaseTime;
        }
    }

    public override void Attack(Transform target)
    {
        base.Attack(target);
        if (Weapon.CanFire())
            Weapon.FireWeapon();
    }
}
