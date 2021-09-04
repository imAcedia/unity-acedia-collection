using UnityEngine;
using UnityEditor;
using System;

namespace Acedia
{
    /// <summary>
    /// Disables the field from the the inspector.
    /// </summary>
    /// <inheritdoc cref="BaseIfAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DisableInInspectorAttribute : BaseIfAttribute, IMultipleAttribute
    {
        public bool justHide { get; private set; }

        /// <inheritdoc cref="DisableInInspectorAttribute"/>
        public DisableInInspectorAttribute(bool justHide = false) : base("")
        {
            this.justHide = justHide;
        }

#if UNITY_EDITOR
        public override bool CompareValue(SerializedProperty property)
        {
            return true;
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
            return justHide;
        }
#endif
    }
}
