using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayersOnState : MonoBehaviour {

    public LayerMask CullingOnAvatar;
    public LayerMask CullingOnShip;
    public LayerMask CullingOnDeath;
    Camera main;

    // Use this for initialization
    void Start () {
        main = GetComponent<Camera>();

        StartCoroutine(stupidWait());
    }

    IEnumerator stupidWait()
    {
        while(MyAvatar.Instance == null)
            yield return new WaitForSeconds(.2f);

        MyAvatar.Instance.EventPlayerStateChanged += Instance_EventPlayerStateChanged;
        Instance_EventPlayerStateChanged(MyAvatar.Instance.CurrentState);
    }

    private void Instance_EventPlayerStateChanged(States newState)
    {
        switch(newState)
        {
            case States.Avatar:
                main.cullingMask = CullingOnAvatar;
                break;
            case States.Dead:
                main.cullingMask = CullingOnDeath;
                break;
            case States.OnShip:
                main.cullingMask = CullingOnShip;
                break;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
