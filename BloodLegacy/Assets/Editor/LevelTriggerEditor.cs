using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelTrigger))]
public class LevelTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelTrigger trigger = target as LevelTrigger;

        if (Application.isPlaying)
        {
            GUILayout.Label("Live Action Controls");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Enter"))
                trigger.EventTriggerEnter.Invoke();

            if (GUILayout.Button("Stay"))
                trigger.EventTriggerStay.Invoke();

            if (GUILayout.Button("Exit"))
                trigger.EventTriggerExit.Invoke();

            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }

        DrawDefaultInspector();
    }
}