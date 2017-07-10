using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipEnemy : NpcBase {

    public CharacterController Controller;

    float targetRefreshTimer = 0f;

    // Use this for initialization
    public override void Start () {
        base.Start();
	}

    // Update is called once per frame
    public override void Update () {
        base.Update();

        if (NetworkHelper.Instance.AllPlayers.Any(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange && m.CurrentState != States.Dead))
        {
            var temp = NetworkHelper.Instance.AllPlayers.First(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange).transform;
            //if (Vector3.Distance(temp.position, transform.position) > AttackRange)
            //{
            CurrentTarget = temp;
            //}
        }
        else
            CurrentTarget = null;

        if(CurrentTarget != null)
        {
            var dir = CurrentTarget.position - transform.position;
            dir.Normalize();
            var rot = Vector3.RotateTowards(transform.forward, dir, .4f * Time.deltaTime, 0f);
            rot.z = rot.x = 0f; 
            transform.rotation = Quaternion.LookRotation (rot);
        }
        
        targetRefreshTimer = 0f;
    }
}
