using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AcediaEditor;
#endif

namespace Acedia
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DrawScriptableObjectAttribute : PropertyAttribute, IMultipleAttribute
    {
#if UNITY_EDITOR
        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label) { }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
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
