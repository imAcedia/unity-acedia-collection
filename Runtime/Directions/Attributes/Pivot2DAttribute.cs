using System;
using UnityEngine;

#if UNITY_EDITOR
using AcediaEditor;
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Draw the <see cref="Direction2D"/> enum as a pivot dropdown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class Pivot2DAttribute : PropertyAttribute, IMultipleAttribute
    {
#if UNITY_EDITOR
        public static readonly string[] options = new string[]
        {
            "Top",
            "Right",
            "Bottom",
            "Left",
            "_",
            "Top Left",
            "Top Center",
            "Top Right",
            "Middle Left",
            "Middle Center",
            "Middle Right",
            "Bottom Left",
            "Bottom Center",
            "Bottom Right",
        };
        public static readonly byte[] secondOptions = new byte[]
        {
            0b1001, // Top Left
            0b0001, // Top Center
            0b0011, // Top Right
            0b1000, // Middle Left
            0b0000, // Middle Center
            0b0010, // Middle Right
            0b1100, // Bottom Left
            0b0100, // Bottom Center
            0b0110, // Bottom Right
        };

        public static readonly GUIContent customButton = new GUIContent()
        {
            image = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_AvatarPivot@2x" : "AvatarPivot@2x").image,
            tooltip = "Use custom pivot.",
        };

        public static readonly GUIStyle customButtonStyle = new GUIStyle("Button")
        {
            padding = new RectOffset(2, 2, 2, 3),
        };

        public static bool PivotStructGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Pivot2D pivot = default;
            try
            {
                pivot = (Pivot2D)EditorHelper.GetTargetObject(property);
            }
            catch (InvalidCastException e)
            {
                Debug.LogException(e);
                return false;
            }

            SerializedProperty directionProp = property.FindPropertyRelative(Pivot2D.directionField);
            SerializedProperty pivotProp = property.FindPropertyRelative(Pivot2D.pivotField);
            SerializedProperty useCustomPivotProp = property.FindPropertyRelative(Pivot2D.useCustomPivotField);
            bool useCustom = useCustomPivotProp.boolValue;

            EditorGUI.BeginProperty(position, label, property);
            Rect rect = position;
            rect.height = EditorHelper.fieldHeight;

            if (useCustom)
            {
                rect.width -= EditorHelper.fieldHeight;
                Rect vRect = EditorGUI.PrefixLabel(rect, label);
                pivotProp.vector2Value = EditorGUI.Vector2Field(vRect, GUIContent.none, pivotProp.vector2Value);
            }
            else
            {
                rect.width -= EditorHelper.fieldHeight;
                PivotDirectionGUI(rect, directionProp, label);
            }

            rect.x += rect.width;
            rect.width = EditorHelper.fieldHeight + EditorHelper.fieldSpacing;
            EditorGUI.BeginChangeCheck();
            useCustomPivotProp.boolValue = GUI.Toggle(rect, useCustom, customButton, customButtonStyle);
            if (EditorGUI.EndChangeCheck())
            {
                if (useCustom) pivot.SetPivot(pivot.PivotVector);
                else pivot.SetPivot(pivot.PivotDirection);

                pivotProp.vector2Value = pivot.PivotVector;
                directionProp.intValue = (int)pivot.PivotDirection;
            }

            EditorGUI.EndProperty();
            return true;
        }

        public static void PivotDirectionGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int value = property.intValue;
            int mask = value;

            int secondSelection;
            {
                secondSelection = 3; // Start On Middle
                if ((value & 1 << 0) != 0) secondSelection -= 3; // Goto Top
                if ((value & 1 << 2) != 0) secondSelection += 3; // Goto Bottom

                secondSelection++; // Start On Center
                if ((value & 1 << 3) != 0) secondSelection--; // Goto Left
                if ((value & 1 << 1) != 0) secondSelection++; // Goto Right

                secondSelection += 5; // Give firstSelection offset
                mask |= 1 << secondSelection;
            }

            //EditorGUI.MaskField(position, label, mask, options);
            Rect rect = EditorGUI.PrefixLabel(position, label);
            bool open = EditorGUI.DropdownButton
            (
                rect,
                new GUIContent(options[secondSelection]),
                FocusType.Passive
            );
            if (open)
            {
                OpenMenu(rect, property, mask);
            }
            return;

            static void OpenMenu(Rect position, SerializedProperty property, int mask)
            {
                GenericMenu menu = new GenericMenu();
                menu.allowDuplicateNames = true;

                for (int i = 0; i < options.Length; i++)
                {
                    if (i <= 3) // First Selection
                    {
                        menu.AddItem
                        (
                            new GUIContent(options[i]),
                            (mask & 1 << i) != 0,
                            FirstSelection,
                            new SelectionContext()
                            {
                                index = i,
                                property = property,
                            }
                        );
                    }
                    if (i == 4) // Separator
                    {
                        menu.AddSeparator("");
                    }
                    if (i >= 5) // Second Selection
                    {
                        int index = i - 5;

                        menu.AddItem
                        (
                            new GUIContent(options[i]),
                            (mask & 1 << i) != 0,
                            SecondSelection,
                            new SelectionContext()
                            {
                                index = index,
                                property = property,
                            }
                        );
                    }
                }

                menu.DropDown(position);
                return;

                void FirstSelection(object ctx)
                {
                    SelectionContext selectionContext = (SelectionContext)ctx;
                    SerializedProperty property = selectionContext.property;
                    int index = selectionContext.index;
                    int value = property.intValue;
                    int optionMask = 1 << index;

                    value &= ~(1 << ((index + 2) % 4)); // turn off Complement
                    value ^= optionMask; // xor with current

                    property.intValue = value;
                    property.serializedObject.ApplyModifiedProperties();

                    /// Example Table (Top & Bottom only)
                    /// 01 o 00 = 01
                    /// 01 o 10 = 01
                    /// 01 o 01 = 00
                    /// 10 o 01 = 10
                    /// 10 o 00 = 10
                    /// 10 o 10 = 00

                    //if (index == 0) // if Top
                    //{
                    //    value &= ~1 & 1 << 2; // turn off Bottom
                    //    value ^= optionMask; // xor with current Top
                    //}

                    //if (index == 1) // if Right
                    //{
                    //    value &= ~1 & 1 << 3; // turn off Left
                    //    value ^= optionMask; // xor with current Right
                    //}

                    //if (index == 2) // if Bottom
                    //{
                    //    value &= ~1 & 1 << 0; // turn off Top
                    //    value ^= optionMask; // xor with current Bottom
                    //}

                    //if (index == 3) // if Left
                    //{
                    //    value &= ~1 & 1 << 1; // turn off Right
                    //    value ^= optionMask; // xor with current Left
                    //}
                }

                void SecondSelection(object ctx)
                {
                    SelectionContext selectionContext = (SelectionContext)ctx;
                    SerializedProperty property = selectionContext.property;
                    int index = selectionContext.index;

                    property.intValue = secondOptions[index];
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label) { }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                PivotStructGUI(position, property, label);
            }

            else PivotDirectionGUI(position, property, label);
            return true;
        }

        public void AfterOnGUI(Rect position, SerializedProperty property, GUIContent label) { }

        public bool GetPropertyHeight(SerializedProperty property, GUIContent label, out float result)
        {
            result = 0f;
            return false;
        }

        public bool HidesProperty(SerializedProperty property)
        {
            return false;
        }

        public class SelectionContext
        {
            public int index = -1;
            public SerializedProperty property;

            public override string ToString()
            {
                return $"{index}. {Convert.ToString(property.intValue, 2)}";
            }
        }
#endif
    }
}
