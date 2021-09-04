using System;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Shows the field only if a condition is met.
    /// Currently supports <c>bool</c>, <c>enum</c>, and references(references only checks if they are null or not).
    /// </summary>
    /// <inheritdoc cref="BaseIfAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ShowIfAttribute : BaseIfAttribute, IMultipleAttribute
    {
        /// <inheritdoc cref="ShowIfAttribute"/>
        public ShowIfAttribute(string comparedPropertyName) : base(comparedPropertyName)
        {

        }

        /// <inheritdoc cref="ShowIfAttribute"/>
        public ShowIfAttribute(string comparedPropertyName, object comparedValue) : base(comparedPropertyName, comparedValue)
        {

        }

        /// <inheritdoc cref="ShowIfAttribute"/>
        public ShowIfAttribute(string methodName, bool serializedObjectMethod) : base(methodName, serializedObjectMethod)
        {

        }

#if UNITY_EDITOR
        public override bool DisablesGUI(SerializedProperty property)
        {
            return !CompareValue(property);
        }

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
            return !CompareValue(property);
        }
#endif
    }
}
