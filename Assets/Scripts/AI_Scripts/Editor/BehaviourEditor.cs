using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Behaviour))]
[CanEditMultipleObjects]
public class BehaviourEditor : Editor
{
    SerializedProperty delayProp;

    void OnEnable()
    {
        // Required to set a name and find which variable this is
        delayProp = serializedObject.FindProperty("delay");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Displays the Delay-Slider
        EditorGUILayout.IntSlider(delayProp, 0, 100, new GUIContent("Delay"));

        // Displays a progress bar for the Delay-Slider
        //if (!delayProp.hasMultipleDifferentValues)
        //{
            ProgressBar(delayProp.intValue / 100.0f, "Delay");
        //}

        serializedObject.ApplyModifiedProperties();
    }	

    void ProgressBar(float value, string label)
    {
        // Additional GUI custom-inspector
        Rect rect = GUILayoutUtility.GetRect(20, 20, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}
