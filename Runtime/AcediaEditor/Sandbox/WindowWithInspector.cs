#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace AcediaEditor
{
    /// LTODO: Sandbox of WindowWithInspector
    /// <summary> Still in Sandbox-Mode. </summary>
    public class WindowWithInspector : EditorWindow
    {
        private Editor _inspector;
        private Editor inspector
        {
            get => _inspector;
            set
            {
                if (inspector != null)
                    DestroyImmediate(_inspector);

                _inspector = value;
            }
        }

        public static void ShowWindow(Object target, string title = null)
        {
            if (title == null) title = target.name;
            WindowWithInspector window = GetWindow<WindowWithInspector>(title);

            GUIContent titleContent = new GUIContent(title);
            string iconName = string.Format("{0}UnityEditor.InspectorWindow", EditorGUIUtility.isProSkin ? "d_" : "");
            titleContent.image = EditorGUIUtility.IconContent(iconName).image;

            window.titleContent = titleContent;
            window.inspector = Editor.CreateEditor(target);
            window.Show();
        }

        private void OnDisable()
        {
            inspector = null;
        }

        private void OnGUI()
        {
            if (inspector == null)
            {
                Close();
                DestroyImmediate(this);
            }

            if (inspector != null)
                inspector.OnInspectorGUI();
        }
    }
}
#endif
