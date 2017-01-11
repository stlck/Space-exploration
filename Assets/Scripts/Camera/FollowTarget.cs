using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowTarget : MonoBehaviour {

    public Transform CurrentTarget;
    public float ZoomSpeed = 10;
    public List<StateFloatValue> OffsetHeights = new List<StateFloatValue>();

    Dictionary<States, float> heights = new Dictionary<States, float>();
    float cHeight = 10;

    // Use this for initialization
    void Start () {

        foreach (var e in OffsetHeights)
            heights.Add(e.State, e.Value);
    }
	
	// Update is called once per frame
	void Update () {
        if(CurrentTarget == null)
        {
            if (MyAvatar.Instance != null)
                CurrentTarget = MyAvatar.Instance.transform;
        }
        else
        { 
            cHeight = Mathf.Lerp(cHeight, heights[MyAvatar.Instance.CurrentState], ZoomSpeed * Time.deltaTime);
            transform.position = CurrentTarget.position + Vector3.up * cHeight;
        }
    }
}
