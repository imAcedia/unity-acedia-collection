using AcediaEditor;
using UnityEditor;
using UnityEngine;

namespace Acedia
{
    [CustomPropertyDrawer(typeof(Pivot2D))]
    public class Pivot2DDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Pivot2DAttribute.PivotStructGUI(position, property, label);
        }
    }
}
