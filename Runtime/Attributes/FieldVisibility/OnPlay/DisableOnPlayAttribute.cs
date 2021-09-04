using System;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Disables the field when the editor is in play-mode.
    /// </summary>
    /// <inheritdoc cref="BaseIfAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DisableOnPlayAttribute : BaseOnPlayAttribute, IMultipleAttribute
    {
        public bool justHide { get; private set; }

        /// <inheritdoc cref="DisableOnPlayAttribute"/>
        public DisableOnPlayAttribute(bool justHide = false) : base()
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
