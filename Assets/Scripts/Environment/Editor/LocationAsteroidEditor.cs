using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocationAsteroid))]
public class LocationAsteroidEditor : LocationEditor
{
    bool showLocEdit = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
