using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit
{
    [CustomEditor(typeof(XRExtendedInteractable)), CanEditMultipleObjects]
    internal class XRExtendedInteractableEditor : Editor
    {
        SerializedProperty m_OnHoverEnter;
        SerializedProperty m_OnHoverExit;
        SerializedProperty m_OnSelectEnter;
        SerializedProperty m_OnSelectExit;
        SerializedProperty m_OnActivate;
        SerializedProperty m_OnDeactivate;
        SerializedProperty m_Colliders;
        SerializedProperty m_InteractionLayerMask;
        

        static class Tooltips
        {
            public static readonly GUIContent colliders = new GUIContent("Colliders", "Colliders to include when selecting/interacting with an interactable");
            public static readonly GUIContent interactionLayerMask = new GUIContent("InteractionLayerMask", "Only Interactors with this LayerMask will interact with this Interactable.");

        }

        void OnEnable() {
            m_OnHoverEnter = serializedObject.FindProperty("ExtOnHoverEnter");
            m_OnHoverExit = serializedObject.FindProperty("ExtOnHoverExit");
            m_OnSelectEnter = serializedObject.FindProperty("ExtOnSelectEnter");
            m_OnSelectExit = serializedObject.FindProperty("ExtOnSelectExit");
            m_OnActivate = serializedObject.FindProperty("ExtOnActivate");
            m_OnDeactivate = serializedObject.FindProperty("ExtOnDeactivate");
            m_Colliders = serializedObject.FindProperty("m_Colliders");
            m_InteractionLayerMask = serializedObject.FindProperty("m_InteractionLayerMask");
        }

        public override void OnInspectorGUI() {

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((XRExtendedInteractable)target), typeof(XRExtendedInteractable), false);
            GUI.enabled = true;

            serializedObject.Update();
            
            EditorGUILayout.PropertyField(m_Colliders, Tooltips.colliders, true);

            EditorGUILayout.PropertyField(m_InteractionLayerMask, Tooltips.interactionLayerMask);
            
            EditorGUILayout.PropertyField(m_OnHoverEnter);
            EditorGUILayout.PropertyField(m_OnHoverExit);
            EditorGUILayout.PropertyField(m_OnSelectEnter);
            EditorGUILayout.PropertyField(m_OnSelectExit);
            EditorGUILayout.PropertyField(m_OnActivate);
            EditorGUILayout.PropertyField(m_OnDeactivate);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
