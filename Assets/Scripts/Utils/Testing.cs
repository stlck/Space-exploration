using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Testing : MonoBehaviour {

    public List<NpcBase> Enemies = new List<NpcBase>();
    public InstantiatedLocation Owner;

	// Use this for initialization
	void Start () {
        Enemies = Resources.LoadAll<NpcBase>("Enemies").ToList();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnGUI()
    {
        if(MyAvatar.Instance.isServer)
        { 
            foreach(var e in Enemies)
            if(GUILayout.Button("Spawn " + e.name))
            {
                if (Owner == null)
                    Owner = GameObject.FindObjectOfType<InstantiatedLocation>();
            
                var inst = Instantiate(e, Owner.transform);
                inst.SpawnEnemy(Owner, Owner.FindOpenSpotInLocation());

            }
        }
    }
}
