using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AcediaEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Attribute to mark that the field will be drawn with multiple PropertDrawer.
    /// This attribute needs to have the highest order to work properly.<br/>
    /// (Note: The other PropertyAttributes must implement the <see cref="IMultipleAttribute"/> interface)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class MultipleDrawerAttribute : PropertyAttribute
    {
        public bool useMultipleOnGUI { get; private set; }
        public IMultipleAttribute[] attributes => attributeArray;
        public void SetAttributes(IMultipleAttribute[] attributes) => attributeArray = attributes;

        private IMultipleAttribute[] attributeArray;

        /// <inheritdoc cref="MultipleDrawerAttribute"/>
        /// <param name="useMultipleOnGUI">Determines if the OnGUI method should be called on every Attribute or only once(only on the highest ordered IMultipleDrawer) or</param>
        public MultipleDrawerAttribute(bool useMultipleOnGUI = false)
        {
            this.useMultipleOnGUI = useMultipleOnGUI;
        }
    }

    public interface IMultipleAttribute
    {
#if UNITY_EDITOR
        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label);
        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label);
        public void AfterOnGUI(Rect position, SerializedProperty property, GUIContent label);
        public bool GetPropertyHeight(SerializedProperty property, GUIContent label, out float result);
        public bool HidesProperty(SerializedProperty property);
#endif
    }
}