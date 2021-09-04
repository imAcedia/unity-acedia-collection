using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using AcediaEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Base class for attributes that uses comparison functionality.
    /// Currently supports <c>bool</c>, <c>enum</c>, and references(references only checks if they are null or not).
    /// </summary>
    /// <param name="comparedPropertyName">The name of the property that is used to compare with <paramref name="comparedValue"/> (case sensitive).</param>
    /// <param name="comparedValue">The value that is used to compare with <paramref name="comparedPropertyName"/> property.</param>
    /// <param name="justDisable">Should this attribute completely hide the field or just disables it.</param>
    /// <param name="justHide">Should this attribute completely hide the field or just disables it.</param>
    /// <param name="methodName">The name of the method to compare from. Needs to return boolean and doesn't have parameters</param>
    /// <param name="serializedObjectMethod">Does the method exist on the outer-most object (ie. the containing Component, ScriptableObject, etc) instead of the current class?</param>
    /// <param name="invert">Should this attribute invert the comparison result.</param>
    /// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
    [AttributeUsage(AttributeTargets.All)]
    public abstract class BaseIfAttribute : PropertyAttribute, IMultipleAttribute
    {
        public string comparedPropertyName { get; private set; }
        public object comparedValue { get; private set; }
        public string methodName { get; private set; }
        public bool serializedObjectMethod { get; private set; }

        /// <summary>Should the comparison result be inverted.</summary>
        public bool invert { get; set; }


        /// <inheritdoc cref="BaseIfAttribute"/>
        public BaseIfAttribute(string comparedPropertyName)
        {
            this.comparedPropertyName = comparedPropertyName;
        }

        /// <inheritdoc cref="BaseIfAttribute"/>
        public BaseIfAttribute(string comparedPropertyName, object comparedValue)
        {
            this.comparedPropertyName = comparedPropertyName;
            this.comparedValue = comparedValue;
        }

        /// <inheritdoc cref="BaseIfAttribute"/>
        public BaseIfAttribute(string methodName, bool serializedObjectMethod)
        {
            this.methodName = methodName;
            this.serializedObjectMethod = serializedObjectMethod;
        }

#if UNITY_EDITOR
        public virtual bool CompareValue(SerializedProperty property)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                return CompareProperty(property);

            return CompareMethod(property);

            bool CompareProperty(SerializedProperty property)
            {
                string path = EditorHelper.GetSiblingPropertyPath(property, comparedPropertyName);
                SerializedProperty comparedProperty = property.serializedObject.FindProperty(path);

                if (comparedProperty == null)
                {
                    Debug.LogError($"Cannot find property at path: {path}");
                    return false; // Don't hide the field if an error occured
                }

                bool result;
                try
                {
                    result = EditorHelper.ComparePropertyValue(comparedProperty, comparedValue);
                }
                catch (Exception e)
                {
                    if (e is ArgumentNullException || e is NotImplementedException)
                    {
                        Debug.LogException(e);
                        return false;
                    }

                    throw;
                }

                return result != invert;
            }

            bool CompareMethod(SerializedProperty property)
            {
                try
                {
                    GetMethodAction(property);
                }
                catch (MissingMethodException e)
                {
                    Debug.LogException(e);
                    return false;
                }

                if (methodAction == null) return false;
                return methodAction() != invert;
            }
        }

        private Func<bool> methodAction = default;
        private Func<bool> GetMethodAction(SerializedProperty property)
        {
            SerializedProperty parentProperty = EditorHelper.GetParentProperty(property);
            object target = !serializedObjectMethod && parentProperty != null ?
                EditorHelper.GetTargetObject(parentProperty) :
                property.serializedObject.targetObject;

            if (methodAction != null && methodAction.Target == target)
                return methodAction;

            Type type = target.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo mi = type.GetMethod(methodName, flags, null, Type.EmptyTypes, null);

            if (mi == null)
            {
                throw new MissingMethodException($"Cannot find method \"{methodName}\" on class {type.FullName}");
            }

            if (mi.ReturnType != typeof(bool))
            {
                throw new MissingMethodException($"Method \"{methodName}\" on class {type.FullName} doesn't return a boolean.");
            }

            methodAction = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), target, mi);
            return methodAction;
        }

        protected bool prevEnabled = false;

        public virtual bool DisablesGUI(SerializedProperty property)
            => CompareValue(property);

        public virtual void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            prevEnabled = GUI.enabled;
            GUI.enabled = !DisablesGUI(property);
        }
        public virtual void AfterOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = prevEnabled;
        }

        public abstract bool OnGUI(Rect position, SerializedProperty property, GUIContent label);

        public abstract bool GetPropertyHeight(SerializedProperty property, GUIContent label, out float result);

        public abstract bool HidesProperty(SerializedProperty property);
#endif
    }
}
