using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Acedia
{
    // TODO Direction2D: Rework how the Pivot2D and CardinalDirection drawer works

    /// <summary>
    /// Draw the <see cref="Direction2D"/> enum as a cardinal direction dropdown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CardinalDirectionAttribute : PropertyAttribute, IMultipleAttribute
    {
#if UNITY_EDITOR
        public static readonly string[] options = new string[]
        {
            "North",
            "East",
            "South",
            "West",
            "_",
            "North West",
            "North",
            "North East",
            "West",
            "None",
            "East",
            "South West",
            "South",
            "South East",
        };
        public static readonly byte[] secondOptions = new byte[]
        {
            0b1001, // North West
            0b0001, // North
            0b0011, // North East
            0b1000, // West
            0b0000, // None
            0b0010, // East
            0b1100, // South West
            0b0100, // South
            0b0110, // South East
        };

        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label) { }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
                return false;

            DrawGUI(position, property, label);
            return true;
        }

        private void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
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
