using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Draws a boolean field as a toggle button
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ToggleButtonAttribute : PropertyAttribute, IMultipleAttribute
    {
        /// <inheritdoc cref="ToggleButtonAttribute"/>
        public ToggleButtonAttribute() { }

#if UNITY_EDITOR
        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label) { }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                Debug.LogErrorFormat("Cannot use ToggleButton for property at path: {0}", property.propertyPath);
                return false;
            }

            property.boolValue = GUI.Toggle(position, property.boolValue, label, "Button");
            return true;
        }

        public void AfterOnGUI(Rect position, SerializedProperty property, GUIContent label) { }

        public bool GetPropertyHeight(SerializedProperty property, GUIContent label, out float result)
        {
            result = EditorGUIUtility.singleLineHeight;
            return true;
        }

        public bool HidesProperty(SerializedProperty property)
        {
            return false;
        }
#endif
    } 
}
