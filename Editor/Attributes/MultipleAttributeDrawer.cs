using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Acedia
{
    [CustomPropertyDrawer(typeof(MultipleDrawerAttribute), true)]
    public class MultipleAttributeDrawer : PropertyDrawer
    {
        private IMultipleAttribute[] UpdateAttributes(MultipleDrawerAttribute mAttribute)
        {
            // Get the attribute list sorted by PropertyAttribute.order
            if (mAttribute.attributes == null)
            {
                object[] attributes = fieldInfo.GetCustomAttributes(typeof(IMultipleAttribute), false);
                Array.Sort(attributes, OrderComparer);

                mAttribute.SetAttributes(new IMultipleAttribute[attributes.Length]);
                Array.Copy(attributes, mAttribute.attributes, attributes.Length);
            }

            return mAttribute.attributes;

            int OrderComparer(object x, object y)
            {
                PropertyAttribute xAttribute = x as PropertyAttribute;
                PropertyAttribute yAttribute = y as PropertyAttribute;
                int xOrder = xAttribute == null ? int.MaxValue : xAttribute.order;
                int yOrder = yAttribute == null ? int.MaxValue : yAttribute.order;

                return xOrder.CompareTo(yOrder);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MultipleDrawerAttribute mAttribute = (MultipleDrawerAttribute)attribute;
            IMultipleAttribute[] attributes = UpdateAttributes(mAttribute);

            for (int i = 0; i < attributes.Length; i++)
                if (attributes[i].HidesProperty(property)) return;

            for (int i = 0; i < attributes.Length; i++)
                attributes[i].BeforeOnGUI(position, property, label);

            bool drawn = false;
            for (int i = attributes.Length - 1; i >= 0; i--)
            {
                drawn |= attributes[i].OnGUI(position, property, label);
                if (drawn && !mAttribute.useMultipleOnGUI) break;
            }

            if (!drawn) EditorGUI.PropertyField(position, property, label, true);

            for (int i = attributes.Length - 1; i >= 0; i--)
                attributes[i].AfterOnGUI(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            MultipleDrawerAttribute mAttribute = (MultipleDrawerAttribute)attribute;
            IMultipleAttribute[] attributes = UpdateAttributes(mAttribute);

            for (int i = 0; i < attributes.Length; i++)
                if (attributes[i].HidesProperty(property))
                    return -EditorGUIUtility.standardVerticalSpacing;

            float height = 0f;
            bool drawn = false;
            for (int i = attributes.Length - 1; i >= 0; i--)
            {
                if (attributes[i].GetPropertyHeight(property, label, out float newHeight))
                {
                    height = Mathf.Max(height, newHeight);
                    drawn = true;
                }

                if (drawn && !mAttribute.useMultipleOnGUI) break;
            }

            if (!drawn) height = base.GetPropertyHeight(property, label);
            return height;
        }
    }
}
