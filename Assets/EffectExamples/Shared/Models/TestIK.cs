using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIK : MonoBehaviour {

    public Animator Animator;
    public Transform left;
    public Transform right;
    float weight = 0;

    //public Avatar avatar;
    CharacterController avatar;
    MyAvatar myAvatar;
    bool running = false;
	// Use this for initialization
	void Start () {
        avatar = GetComponentInParent<CharacterController>();
        myAvatar = GetComponentInParent<MyAvatar>();

    }

    void Update()
    {
        //running = Mathf.Abs(avatar.vertical) >= .2f || Mathf.Abs(avatar.horizontal) >= .2f;
        running = avatar.velocity.magnitude > .2f;
        Animator.SetBool("Running", running);
        //Animator.SetBool("Grounded", avatar.controller.isGrounded);
    }

    void OnAnimatorIK (int layerIndex)
    {
        if(myAvatar.AvatarWeaponHandler.EquippedWeapon != null)
        { 
            Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            Animator.SetIKPosition(AvatarIKGoal.RightHand, right.position);

            Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            Animator.SetIKRotation(AvatarIKGoal.RightHand, right.rotation);

            Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            Animator.SetIKPosition(AvatarIKGoal.LeftHand, left.position);
        }

    }

}
