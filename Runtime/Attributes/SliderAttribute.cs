using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Acedia
{
    /// <inheritdoc cref="RangeAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SliderAttribute : PropertyAttribute, IMultipleAttribute
    {
        public float min { get; private set; }
        public float max { get; private set; }
        public bool useInt { get; private set; }

        /// <inheritdoc cref="RangeAttribute(float, float)"/>
        public SliderAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
            useInt = false;
        }

        /// <inheritdoc cref="RangeAttribute(float, float)"/>
        public SliderAttribute(int min, int max)
        {
            this.min = min;
            this.max = max;
            useInt = true;
        }

#if UNITY_EDITOR
        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label) { }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
                Debug.LogErrorFormat("Cannot use Slider for property at path: {0}", property.propertyPath);
                return false;
            }

            if (useInt) EditorGUI.IntSlider(position, property, (int)min, (int)max, label);
            else EditorGUI.Slider(position, property, min, max, label);
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
