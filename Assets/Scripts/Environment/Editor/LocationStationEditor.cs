using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocationStation))]
public class LocationStationEditor : LocationEditor
{

    bool showLocEdit = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var t = (LocationStation)target;

        /*if (GUILayout.Button("FILL"))
            t.FillMap();
        if (GUILayout.Button("Set row 0 to 1"))
        {
            for (int i = 0; i < t.Size; i++)
            {
                t.tiles[i * t.Size] = 1;
                if (i > 0)
                    t.tiles[i * t.Size - 1] = 1;
            }
            for (int j = 0; j < t.Size; j++)
            {
                t.tiles[j] = 1;
                if (j > 0)
                    t.tiles[t.Size * t.Size - j] = 1;
            }

            Debug.Log("tiles set to 1");
        }

        showLocEdit = GUILayout.Toggle(showLocEdit, "showLocEdit");

        if (showLocEdit && t.tiles != null)
        {
            int i = 0;
            //foreach (var tile in t.tiles)
            for (int k = 0; k < t.tiles.Length; k++)
            {
                if (i == 0 || i % t.Size == 0)
                    EditorGUILayout.BeginHorizontal();

                int x = i % t.Size;
                int y = i / t.Size;

                t.tiles[i] = EditorGUILayout.IntField(t.tiles[i], GUILayout.Width(20));

                if (i % t.Size == t.Size - 1)
                    EditorGUILayout.EndHorizontal();
                i++;
            }
        }*/

        if (GUI.changed)
            t.SetDirty();
    }
}
