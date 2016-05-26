//using UnityEngine;
//using UnityEditor;

//[CustomPropertyDrawer(typeof(AI_Behaviour))]
//public class AI_BehaviourEditor : PropertyDrawer
//{
//    // Draw the property inside the given rect
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        // Using BeginProperty / EndProperty on the parent property means that
//        // prefab override logic works on the entire property.
//        EditorGUI.BeginProperty(position, label, property);

//        // Draw label
//        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

//        // Don't make child fields be indented
//        var indent = EditorGUI.indentLevel;
//        EditorGUI.indentLevel = 0;

//        // Calculate rects
//        var behaviourRectLabel = new Rect(position.x, position.y - 20, 20, 20);
//        var behaviourRect = new Rect(position.x + 50, position.y, 100, position.height);
//        //var targetDesRect = new Rect(position.x + 50, position.y + 20, 100, position.height);
//        var durationRect = new Rect(position.x + 50, position.y + 20, 20, position.height);

//        // Draw fields - passs GUIContent.none to each so they are drawn without labels        
//        EditorGUI.PropertyField(behaviourRect, property.FindPropertyRelative("behaviour"), GUIContent.none);
//        //if (property.FindPropertyRelative("behaviour").enumValueIndex == 3)
//        //{
//        //    EditorGUI.PropertyField(targetDesRect, property.FindPropertyRelative("targetDestination"), GUIContent.none);
//        //}
//        EditorGUI.PropertyField(durationRect, property.FindPropertyRelative("duration"), GUIContent.none);
//        EditorGUILayout.Space();

//        // Set indent back to what it was
//        EditorGUI.indentLevel = indent;

//        EditorGUI.EndProperty();
//    }

    //SerializedProperty delayProp;

    //void OnEnable()
    //{
    //    // Required to set a name and find which variable this is
    //    delayProp = serializedObject.FindProperty("delay");
    //}

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();
    //    //AI_Behaviour aiBehaviourScript = (AI_Behaviour)target;

    //    // Displays the Delay-Slider
    //    EditorGUILayout.IntSlider(delayProp, 0, 100, new GUIContent("Delay"));

    //    // Displays a progress bar for the Delay-Slider
    //   // if (!delayProp.hasMultipleDifferentValues)
    //    //{
    //        ProgressBar(delayProp.intValue / 100.0f, "Delay");
    //    //}

    //    serializedObject.ApplyModifiedProperties();
    //}

    //void ProgressBar(float value, string label)
    //{
    //    // Additional GUI custom-inspector
    //    Rect rect = GUILayoutUtility.GetRect(20, 20, "TextField");
    //    EditorGUI.ProgressBar(rect, value, label);
    //    EditorGUILayout.Space();
    //}
//}
