using System;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Enables the field only when the editor is in play-mode.
    /// </summary>
    /// <inheritdoc cref="BaseOnPlayAttribute"/>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnableOnPlayAttribute : BaseOnPlayAttribute, IMultipleAttribute
    {
        /// <inheritdoc cref="EnableOnPlayAttribute"/>
        public EnableOnPlayAttribute() : base()
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
            return false;
        }
#endif
    }
}
