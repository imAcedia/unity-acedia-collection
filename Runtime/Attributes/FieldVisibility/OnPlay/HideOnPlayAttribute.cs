using System;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Hides the field when the editor is in play-mode.
    /// </summary>
    /// <inheritdoc cref="BaseOnPlayAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class HideOnPlayAttribute : BaseOnPlayAttribute, IMultipleAttribute
    {
        public bool justDisable { get; private set; }

        /// <inheritdoc cref="HideOnPlayAttribute"/>
        public HideOnPlayAttribute(bool justDisable = false) : base()
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
