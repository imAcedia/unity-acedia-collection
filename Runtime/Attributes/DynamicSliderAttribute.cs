using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AcediaEditor;
#endif

namespace Acedia
{
    /// <inheritdoc cref="RangeAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DynamicSliderAttribute : PropertyAttribute, IMultipleAttribute
    {
        public float min { get; private set; }
        public float max { get; private set; }

        public string minPropertyPath { get; private set; } = null;
        public string maxPropertyPath { get; private set; } = null;

        public bool minInt { get; private set; } = false;
        public bool maxInt { get; private set; } = false;

        /// <inheritdoc cref="DynamicSliderAttribute"/>
        /// <param name="minProperty">The property name of the minimum allowed value</param>
        /// <param name="maxProperty">The property name of the maximum allowed value</param>
        public DynamicSliderAttribute(string minProperty, string maxProperty)
        {
            this.minPropertyPath = minProperty;
            this.maxPropertyPath = maxProperty;
        }

        /// <inheritdoc cref="SliderAttribute(float, float)"/>
        /// <inheritdoc cref="DynamicSliderAttribute(string, string)"/>
        public DynamicSliderAttribute(float min, string maxProperty)
        {
            this.min = min;
            this.maxPropertyPath = maxProperty;
        }

        /// <inheritdoc cref="SliderAttribute(float, float)"/>
        /// <inheritdoc cref="DynamicSliderAttribute(string, string)"/>
        public DynamicSliderAttribute(int min, string maxProperty)
        {
            this.min = min;
            this.maxPropertyPath = maxProperty;
            minInt = true;
        }

        /// <inheritdoc cref="SliderAttribute(float, float)"/>
        /// <inheritdoc cref="DynamicSliderAttribute(string, string)"/>
        public DynamicSliderAttribute(string minProperty, float max)
        {
            this.minPropertyPath = minProperty;
            this.max = max;
        }

        /// <inheritdoc cref="SliderAttribute(float, float)"/>
        /// <inheritdoc cref="DynamicSliderAttribute(string, string)"/>
        public DynamicSliderAttribute(string minProperty, int max)
        {
            this.minPropertyPath = minProperty;
            this.max = max;
            maxInt = true;
        }

#if UNITY_EDITOR
        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label) { }

        // IMPORTANT: Add Begin/EndProperty so it works on prefabs
        // IMPORTANT: Add Begin/EndProperty so it works on prefabs
        // IMPORTANT: Add Begin/EndProperty so it works on prefabs
        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
                Debug.LogError($"Cannot use {nameof(DynamicSliderAttribute)} for property at path: {property.propertyPath}");
                return false;
            }

            float min = this.min;
            float max = this.max;

            if (minPropertyPath != null)
            {
                SerializedProperty minProperty = EditorHelper.GetSiblingProperty(property, minPropertyPath);
                if (minProperty == null)
                {
                    Debug.LogError($"Cannot find min property at path: {minPropertyPath}");
                    return false;
                }
                minInt = minProperty.propertyType == SerializedPropertyType.Integer;
                min = minInt ? minProperty.intValue : minProperty.floatValue;
            }

            if (maxPropertyPath != null)
            {
                SerializedProperty maxProperty = EditorHelper.GetSiblingProperty(property, maxPropertyPath);
                if (maxProperty == null)
                {
                    Debug.LogError($"Cannot find max property at path: {maxPropertyPath}");
                    return false;
                }
                maxInt = maxProperty.propertyType == SerializedPropertyType.Integer;
                max = maxInt ? maxProperty.intValue : maxProperty.floatValue;
            }

            bool useInt = minInt && maxInt;

            if (!useInt)
            {
                EditorGUI.Slider(position, property, min, max, label);
            }
            else
            {
                EditorGUI.IntSlider(position, property, (int)min, (int)max, label);
            }

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
