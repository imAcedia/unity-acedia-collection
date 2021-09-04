using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Acedia
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class MustInheritAttribute : PropertyAttribute, IMultipleAttribute
    {
        public Type baseType { get; private set; }

        public MustInheritAttribute(Type baseType)
        {
            this.baseType = baseType;
        }

#if UNITY_EDITOR
        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
                return false;

            EditorGUI.ObjectField(position, property, baseType, label);
            //property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, baseType, true);
            return true;
        }

        public void AfterOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

        }

        public bool GetPropertyHeight(SerializedProperty property, GUIContent label, out float result)
        {
            result = EditorGUIUtility.singleLineHeight;
            return property.propertyType != SerializedPropertyType.ObjectReference;
        }

        public bool HidesProperty(SerializedProperty property)
        {
            return false;
        }
#endif
    }
}
