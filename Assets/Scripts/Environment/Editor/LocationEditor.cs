using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Location))]
public class LocationEditor : Editor {

    bool showLocEdit = false;

    public override void OnInspectorGUI()
    {
        var t = (Location)target;
        if(GUILayout.Button("Test Spawn"))
        {
            t.TestSpawnLocation();
        }

        base.OnInspectorGUI();

    }
}
