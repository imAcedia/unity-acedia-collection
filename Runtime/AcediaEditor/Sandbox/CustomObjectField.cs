#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Object = UnityEngine.Object;

namespace AcediaEditor.UIElements
{
    /// LTODO CustomObjectField: Sandbox
    /// <summary> Still in Sandbox-Mode. </summary>
    public class CustomObjectField : ObjectField
    {
        public string emptyText = "Empty";
        public string emptyTooltipText;
        public string tooltipText;

        public Label objectLabel { get; private set; }

        public static new readonly string ussClassName = "custom-object-field";
        public static new readonly string labelUssClassName = ussClassName + "__label";
        public static readonly string emptyUssClassName = ussClassName + "--empty";

        public CustomObjectField() : this(null) { }
        public CustomObjectField(Object value) : base()
        {
            // LNOTE: Be careful when using Manipulator because it has a weird implementation, Read more...
            //   At the time of writing, Manipulator implementation seems really weird and the
            //   UIElements team is working on either removing or changing it.
            //   Refer to the forum post below for more information
            //   https://forum.unity.com/threads/why-are-there-multiple-event-metaphors-in-uielements.688552/
            new ContextualMenuManipulator(PopulateContextMenu_Event).target = this;
            RegisterCallback<ChangeEvent<Object>>(e => UpdateField());
            AddToClassList(ussClassName);

            objectLabel = new Label();
            objectLabel.AddToClassList(labelUssClassName);
            objectLabel.pickingMode = PickingMode.Ignore;

            // LNOTE: ObjectFieldDisplay is private, can't get ussClassName
            this.Q<VisualElement>(className: "unity-object-field-display").Add(objectLabel);

            if (value != null)
                this.value = value;

            UpdateField();
        }

        public void UpdateField()
        {
            if (value == null)
            {
                AddToClassList(emptyUssClassName);
                tooltip = emptyTooltipText;
                objectLabel.text = emptyText;
            }
            else
            {
                //string name = value.name;
                string name = EditorGUIUtility.ObjectContent(value, value.GetType()).text;

                RemoveFromClassList(emptyUssClassName);
                tooltip = tooltipText;
                objectLabel.text = name;
            }
        }

        #region Context Menu
        private void PopulateContextMenu_Event(ContextualMenuPopulateEvent e)
        {
            bool objectExist = value != null;

            if (objectExist) e.menu.AppendAction("Set Null", SetNull);

            e.menu.AppendSeparator();
            e.menu.AppendAction("Select Asset", SelectAsset);
            e.menu.AppendAction("Rename Asset", RenameAsset);

            return;
            void RenameAsset(DropdownMenuAction obj) => this.RenameAsset();
            void SelectAsset(DropdownMenuAction obj) => this.SelectAsset();
            void SetNull(DropdownMenuAction obj)
            {
                value = null;
                UpdateField();
            }
        }
        #endregion

        private void RenameAsset()
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = value;
            EditorWindow.focusedWindow.SendEvent(new Event()
            {
                keyCode = KeyCode.F2,
                type = EventType.KeyDown
            });
        }

        private void SelectAsset()
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = value;
            EditorGUIUtility.PingObject(value);
        }
    }
}
#endif
