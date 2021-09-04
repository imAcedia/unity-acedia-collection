using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AcediaEditor;
#endif

namespace Acedia.Lockables
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class LockableAttribute : PropertyAttribute, IMultipleAttribute
    {
#if UNITY_EDITOR
        public void AfterOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            return DrawGUI(position, property, label);
        }

        private static bool CheckType(SerializedProperty property)
        {
            Type lockableType = typeof(Lockable<>);
            Type propertyType = EditorHelper.GetPropertyFieldType(property);
            if (propertyType.IsGenericType) propertyType = propertyType.GetGenericTypeDefinition();

            if (propertyType == lockableType) return true;
            if (propertyType.IsSubclassOfGenericDefinition(lockableType))
                return true;

            return false;
        }

        public static bool DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!CheckType(property))
            {
                Debug.LogError($"{nameof(LockableAttribute)} is not supported for field \"{property.displayName}\".");
                return false;
            }

            SerializedProperty valueProp = property.FindPropertyRelative("value");
            SerializedProperty lockedProp = property.FindPropertyRelative("locked");

            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };

            bool guiEnabled = GUI.enabled;
            if (lockedProp.boolValue) GUI.enabled = false;

            rect.width -= EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, valueProp, label, false);
            if (valueProp.isExpanded)
            {
                rect.y += rect.height;
                rect.width += EditorGUIUtility.singleLineHeight;
                DrawChildren(rect, valueProp);
                rect.width -= EditorGUIUtility.singleLineHeight;
                rect.y -= rect.height;
            }

            GUI.enabled = guiEnabled;

            rect.x += rect.width;
            rect.width = EditorGUIUtility.singleLineHeight;
            lockedProp.boolValue = EditorGUI.Toggle(rect, GUIContent.none, lockedProp.boolValue, "IN LockButton");

            EditorGUI.EndProperty();
            return true;

            // Wrote this because we need to draw the children without decreasing the width for the lock
            static void DrawChildren(Rect position, SerializedProperty property)
            {
                EditorHelper.IndentRect(ref position);

                property = property.Copy();
                int startingDepth = property.depth;

                // foreach each property inside the parent
                property.NextVisible(true);
                while (property.depth > startingDepth)
                {
                    EditorGUI.PropertyField(position, property, true);
                    property.NextVisible(false);
                    position.y += position.height;
                    position.y += EditorGUIUtility.standardVerticalSpacing;
                }

                EditorHelper.UnIndentRect(ref position);
            }
        }

        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
        }

        public bool GetPropertyHeight(SerializedProperty property, GUIContent label, out float result)
        {
            if (!CheckType(property))
            {
                result = 0f;
                return false;
            }

            result = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"));
            return true;
        }

        public bool HidesProperty(SerializedProperty property) => false;
#endif
    }
}
