using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Location))]
public class LocationEditor : Editor {

    bool showLocEdit = false;

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

        showLocEdit = GUILayout.Toggle(showLocEdit, "showLocEdit");

        if(showLocEdit && t.MapTiles != null)
        {
            int i = 0;
            //foreach (var tile in t.tiles)
            for(int k = 0; k < t.tiles.Length; k++)
            {
                if (i == 0 || i % t.Size == 0)
                    EditorGUILayout.BeginHorizontal();

                int x = i % t.Size;
                int y = i / t.Size;

                t.tiles[i] = EditorGUILayout.IntField(t.tiles[i], GUILayout.Width(20));

                if (i % t.Size == t.Size -1)
                    EditorGUILayout.EndHorizontal();
                i++;                                
            }
            if (GUI.changed)
                t.SetDirty();
        }
    }
}
