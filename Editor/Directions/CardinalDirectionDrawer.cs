using UnityEditor;
using UnityEngine;

namespace Acedia
{
    [CustomPropertyDrawer(typeof(Direction2D), true)]
    public class CardinalDirectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CardinalDirectionAttribute.DrawGUI(position, property, label);
        }
    }
}
