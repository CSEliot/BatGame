using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ToggleThingsInRound))]
public class MyScriptEditor : Editor {

    SerializedProperty toggleScript;

    void OnEnable() {
        toggleScript = serializedObject.FindProperty("toggleScript");
    }

    public override void OnInspectorGUI() {
        ToggleThingsInRound toggleScript = (ToggleThingsInRound)target;

        DrawDefaultInspector();
        if (toggleScript.ChosenToggleType == ToggleThingsInRound.ToggleType.Key) {
            EditorGUILayout.LabelField("Toggle Key");
            toggleScript._ToggleKey = EditorGUILayout.TextField(toggleScript._ToggleKey);
        }
        else {
            EditorGUILayout.LabelField("Toggle Input Button");
            toggleScript._ToggleInput = EditorGUILayout.TextField(toggleScript._ToggleInput);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
