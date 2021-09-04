using System;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Hides the field if a condition is met.
    /// Currently supports <c>bool</c>, <c>enum</c>, and references(references only checks if they are null or not).
    /// </summary>
    /// <inheritdoc cref="BaseIfAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class HideIfAttribute : BaseIfAttribute, IMultipleAttribute
    {
        public bool justDisable { get; private set; }

        /// <inheritdoc cref="HideIfAttribute"/>
        public HideIfAttribute(string comparedPropertyName, bool justDisable = false) : base(comparedPropertyName)
        {
            this.justDisable = justDisable;
        }

        /// <inheritdoc cref="HideIfAttribute"/>
        public HideIfAttribute(string comparedPropertyName, object comparedValue, bool justDisable = false) : base(comparedPropertyName, comparedValue)
        {
            this.justDisable = justDisable;
        }

        /// <inheritdoc cref="HideIfAttribute"/>
        public HideIfAttribute(string methodName, bool serializedObjectMethod, bool justDisable = false) : base(methodName, serializedObjectMethod)
        {
            this.justDisable = justDisable;
        }

#if UNITY_EDITOR
        public override bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            return false;
        }

        public override bool GetPropertyHeight(SerializedProperty property, GUIContent label, out float result)
        {
            result = 0f;
            return false;
        }

        public override bool HidesProperty(SerializedProperty property)
        {
            return !justDisable && CompareValue(property);
        }
#endif
    }
}
