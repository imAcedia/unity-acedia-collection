using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Disables the field when in play-mode. NOTE: Still not function properly
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredAttribute : PropertyAttribute, IMultipleAttribute
    {
        /// <inheritdoc cref="RequiredAttribute"/>
        public RequiredAttribute() { }

#if UNITY_EDITOR
        public void Check(SerializedProperty property)
        {
            if (property.objectReferenceValue == null)
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    Debug.LogException(new NullReferenceException($"Field {property.displayName} on {property.serializedObject.targetObject.name} is required."), property.serializedObject.targetObject);

                if (EditorApplication.isPlaying)
                    EditorApplication.isPaused = true;

                else if (EditorApplication.isPlayingOrWillChangePlaymode)
                    EditorApplication.isPlaying = false;
            }
        }

        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Check(property);
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            return false;
        }

        public void AfterOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
        }

        public bool GetPropertyHeight(SerializedProperty property, GUIContent label, out float result)
        {
            result = 0f;
            return false;
        }

        public bool HidesProperty(SerializedProperty property)
        {
            return false;
        }
#endif
    }
}
