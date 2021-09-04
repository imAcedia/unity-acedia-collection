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
    /// Calls a specified method from a component when the field is changed in the inspector.<br/>
    /// </summary>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="serializedObjectMethod">Does the method exist on the outer-most object (ie. the containing Component/ScriptableObject, etc) instead of the current class?</param>
    /// <param name="optional">Should the attribute throws an exception when the method is not found.</param>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class OnInspectorChangeMethodAttribute : PropertyAttribute, IMultipleAttribute
    {
        public string methodName { get; private set; }
        public bool serializedObjectMethod { get; private set; }
        public bool optional { get; private set; }

        /// <inheritdoc cref="OnInspectorChangeMethodAttribute"/>
        public OnInspectorChangeMethodAttribute(string methodName, bool serializedObjectMethod = false, bool optional = false)
        {
            this.methodName = methodName;
            this.serializedObjectMethod = serializedObjectMethod;
            this.optional = optional;
        }

#if UNITY_EDITOR
        private Action methodAction;
        private void GetMethodAction(SerializedProperty property)
        {
            SerializedProperty parentProperty = EditorHelper.GetParentProperty(property);
            object target = !serializedObjectMethod && parentProperty != null ? 
                EditorHelper.GetTargetObject(parentProperty):
                property.serializedObject.targetObject;

            Type type = target.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo mi = type.GetMethod(methodName, flags, null, Type.EmptyTypes, null);

            if (mi == null)
            {
                if (!optional)
                {
                    throw new MissingMethodException($"Cannot find method \"{methodName}\" on class {type.FullName}");
                    //Debug.LogError($"Cannot find method \"{methodName}\" on class {type.Name}");
                }

                return;
            }

            methodAction = (Action)Delegate.CreateDelegate(typeof(Action), target, mi);
        }

        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            return false;
        }

        public void AfterOnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorGUI.EndChangeCheck())
            {
                if (methodAction == null) GetMethodAction(property);
                if (methodAction == null) return;

                property.serializedObject.ApplyModifiedProperties();
                methodAction.Invoke();
                property.serializedObject.UpdateIfRequiredOrScript();
            }
        }

        public bool GetPropertyHeight(SerializedProperty property, GUIContent label, out float result)
        {
            result = 0f;
            return false;
        }

        public bool HidesProperty(SerializedProperty property)
        {
            return false;
        }
#endif
    }
}
