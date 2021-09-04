using UnityEditor;
using UnityEngine;

namespace Acedia
{
    [CustomPropertyDrawer(typeof(Direction2D), true)]
    public class Pivot2DDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Pivot2DAttribute.DrawGUI(position, property, label);
        }
    }
}
