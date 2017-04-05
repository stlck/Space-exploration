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
            if(GUILayout.Button("Spawn Enemy"))
            {
                if (Owner == null)
                    Owner = GameObject.FindObjectOfType<InstantiatedLocation>();
            
                var e = Instantiate(Enemies[0], Owner.transform);
                e.SpawnEnemy(Owner, Owner.FindOpenSpotInLocation());

            }
        }
    }
}
