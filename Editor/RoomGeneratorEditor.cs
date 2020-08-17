using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomGenerator))]
public class RoomGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoomGenerator roomGenerator = (RoomGenerator)target;

        DrawDefaultInspector();
        EditorGUILayout.LabelField("Room Distance", roomGenerator.roomDistance.ToString());

        if (GUILayout.Button("Generate"))
        {
            roomGenerator.GenerateLevel();
        }

        if (GUILayout.Button("Clear"))
        {
            roomGenerator.ClearRooms();
        }
    }
}
