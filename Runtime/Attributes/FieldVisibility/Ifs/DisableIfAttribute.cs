using System;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Disables the field if a condition is met.
    /// Currently supports <c>bool</c>, <c>enum</c>, and references(references only checks if they are null or not).
    /// </summary>
    /// <inheritdoc cref="BaseIfAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class DisableIfAttribute : BaseIfAttribute, IMultipleAttribute
    {
        public bool justHide { get; private set; }

        /// <inheritdoc cref="DisableIfAttribute"/>
        public DisableIfAttribute(string comparedPropertyName, bool justHide = false) : base(comparedPropertyName)
        {
            this.justHide = justHide;
        }

        /// <inheritdoc cref="DisableIfAttribute"/>
        public DisableIfAttribute(string comparedPropertyName, object comparedValue, bool justHide = false) : base(comparedPropertyName, comparedValue)
        {
            this.justHide = justHide;
        }

        /// <inheritdoc cref="DisableIfAttribute"/>
        public DisableIfAttribute(string methodName, bool serializedObjectMethod, bool justHide = false) : base(methodName, serializedObjectMethod)
        {
            this.justHide = justHide;
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
            return justHide && CompareValue(property);
        }
#endif
    }
}
