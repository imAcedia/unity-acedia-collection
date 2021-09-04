using UnityEditor;
using UnityEngine;

namespace Acedia
{
    [CustomPropertyDrawer(typeof(Direction2D), true)]
    public class Direction2DDrawer : PropertyDrawer
    {
        public string[] options = new string[]
        {
            "Up",
            "Right",
            "Down",
            "Left",
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            int result = EditorGUI.MaskField(position, property.intValue, options);
            property.intValue = result;
            EditorGUI.EndProperty();
        }
    }
}
