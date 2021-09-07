using UnityEngine;
using UnityEditor;

namespace Acedia
{
    [CustomPropertyDrawer(typeof(IMultipleAttribute), true)]
    public class MultipleAttributesDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            IMultipleAttribute attr = (IMultipleAttribute)attribute;
            if (attr.HidesProperty(property)) return;

            attr.BeforeOnGUI(position, property, label);

            if (!attr.OnGUI(position, property, label))
                EditorGUI.PropertyField(position, property, label, true);
            
            attr.AfterOnGUI(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            IMultipleAttribute attr = (IMultipleAttribute)attribute;
            if (attr.HidesProperty(property))
                return -EditorGUIUtility.standardVerticalSpacing;

            if (attr.GetPropertyHeight(property, label, out float result))
                return result;

            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
