using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Acedia
{
    /// <summary>
    /// Draw the <see cref="Direction2D"/> enum as a pivot dropdown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CardinalDirectionAttribute : PropertyAttribute, IMultipleAttribute
    {
#if UNITY_EDITOR
        public static readonly string[] options = new string[]
        {
            "None",        // 0. 0
            "North West",  // 1. 9
            "North",       // 2. 1
            "North East",  // 3. 3
            "East",        // 4. 2
            "South East",  // 5. 6
            "South",       // 6. 4
            "South West",  // 7. 12
            "West",        // 8. 8
        };
        public static readonly byte[] secondSelections = new byte[]
        {
            0b1001, // 1. North West
            0b0011, // 3. North East
            0b0110, // 5. South East
            0b1100, // 7. South West
        };

        public static void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int value = property.intValue;
            Direction2D dir = (Direction2D)value;

            int mask = 0;
            int shownIndex = 0;
            {
                //if (dir.FlagActive(Direction2D.Vertical))
                //{
                //    int flipper = 1;
                //    if (dir.FlagActive(Direction2D.North)) shownIndex = 2;
                //    if (dir.FlagActive(Direction2D.South)) { shownIndex = 6; flipper = -1; }
                //    if (dir.FlagActive(Direction2D.East)) shownIndex += flipper;
                //    if (dir.FlagActive(Direction2D.West)) shownIndex += -flipper;
                //}
                //else
                //{
                //    if (dir.FlagActive(Direction2D.East)) shownIndex = 4;
                //    if (dir.FlagActive(Direction2D.West)) shownIndex = 8;
                //}
            }

            for (int i = 0; i < 4; i++)
            {
                // Start from North(2), East(4), South(6), West(8)
                int option = (i + 1) * 2;

                Direction2D d = (Direction2D)(1 << i);
                if (dir.FlagActive(d))
                {
                    mask |= 1 << option;

                    if (shownIndex != 0)
                        shownIndex += (option - shownIndex) > 2 ? -1 : 1;
                    else shownIndex = option;
                }
            }

            mask |= 1 << shownIndex;

            //EditorGUI.MaskField(position, label, mask, options);
            Rect rect = EditorGUI.PrefixLabel(position, label);
            bool open = EditorGUI.DropdownButton
            (
                rect,
                new GUIContent(options[shownIndex]),
                FocusType.Passive
            );
            if (open)
            {
                OpenMenu(rect, property, mask);
            }
        }

        private static void OpenMenu(Rect position, SerializedProperty property, int mask)
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < options.Length; i++)
            {
                if (i % 2 == 0) // First Selection
                {
                    menu.AddItem
                    (
                        new GUIContent(options[i]),
                        (mask & 1 << i) != 0,
                        FirstSelection,
                        new SelectionContext()
                        {
                            index = i / 2,
                            property = property,
                        }
                    );
                }
                else // Second Selection
                {
                    menu.AddItem
                    (
                        new GUIContent(options[i]),
                        (mask & 1 << i) != 0,
                        SecondSelection,
                        new SelectionContext()
                        {
                            index = (i - 1) / 2,
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
                if (index-- == 0)
                {
                    property.intValue = 0;
                    property.serializedObject.ApplyModifiedProperties();
                    return;
                }

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

                property.intValue = secondSelections[index];
                property.serializedObject.ApplyModifiedProperties();
            }
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

        public void BeforeOnGUI(Rect position, SerializedProperty property, GUIContent label) { }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawGUI(position, property, label);
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
#endif
    }
}
