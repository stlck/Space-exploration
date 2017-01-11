using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Location))]
public class LocationEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var t = (Location)target;
        var o = "tiles:\n";
        if (t.MapTiles == null)
        {
            EditorGUILayout.TextField("MapTiles null");
        }
        else
        {
            int i = 0;
            foreach (var tile in t.MapTiles)
            {
                i++;
                o += tile;
                if (i % t.Size == 0)
                    o += "\n";
            }

            EditorGUILayout.TextArea(o);
        }
    }
}
