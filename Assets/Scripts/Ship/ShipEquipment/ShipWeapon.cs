using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipWeapon : BaseWeapon {

    public BaseProjectile Projectile;
    //public float Cooldown;
    float timer = 0f;
    public Vector3 SpawnPoint;

    void Awake()
    {

    }

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
 //       timer += Time.deltaTime;
	//}

 //   public bool CanFire()
 //   {
 //       return timer >= Cooldown;
 //   }

    public override void FireWeapon()
    {
        base.FireWeapon();
        var p = Instantiate(Projectile, transform.position + transform.forward + transform.TransformPoint(SpawnPoint), transform.rotation);
        p.Owner = this;
        Destroy(p.gameObject, 10);
    }
        
}
