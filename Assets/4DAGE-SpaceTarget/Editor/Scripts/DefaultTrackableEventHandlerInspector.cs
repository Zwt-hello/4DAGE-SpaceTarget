using UnityEditor;
using UnityEngine;

namespace SpaceTarget.EditorClasses
{
    [CustomEditor(typeof(DefaultTrackableEventHandler))]
    [CanEditMultipleObjects]
    public class DefaultTrackableEventHandlerInspector : Editor
    {
        SerializedProperty mOnTargetFoundProp;
        SerializedProperty mOnTargetLostProp;

        void OnEnable()
        {
            // Setup the SerializedProperties.
            mOnTargetFoundProp = serializedObject.FindProperty("OnTargetFound");
            mOnTargetLostProp = serializedObject.FindProperty("OnTargetLost");
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            // render the standard script selector so that users can find the DefaultTrackableEventHandler
            // to customize it:
            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true);
            GUI.enabled = true;

            GUILayout.Label("Event(s) when target is found:");
            EditorGUILayout.PropertyField(mOnTargetFoundProp);

            GUILayout.Label("Event(s) when target is lost:");
            EditorGUILayout.PropertyField(mOnTargetLostProp);

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
}