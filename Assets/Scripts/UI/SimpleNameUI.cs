using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNameUI : MonoBehaviour {

    //public bool IsOn = true;
    public bool IsOn { get; set; }

    public string Label;
    public Vector3 offset;
    public float width = 60;
    public float height = 25;

    void Awake()
    {
        this.enabled = false; return;

        IsOn = true;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        if (!IsOn)
            return;

        var p = Camera.main.WorldToScreenPoint(transform.position + offset);
        GUILayout.BeginArea(new Rect(p.x - width/2, Screen.height - p.y, width, height));
        GUILayout.Label(Label);
        GUILayout.EndArea();
    }
}
