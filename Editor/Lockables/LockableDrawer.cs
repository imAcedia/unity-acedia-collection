using UnityEditor;
using UnityEngine;

namespace Acedia.Lockables
{
    [CustomPropertyDrawer(typeof(Lockable<>), true)]
    public class LockableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LockableAttribute.DrawGUI(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProp = property.FindPropertyRelative("value");
            return EditorGUI.GetPropertyHeight(valueProp);
        }
    }
}
