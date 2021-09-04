#if UNITY_EDITOR
using UnityEditor;

using Object = UnityEngine.Object;

namespace AcediaEditor
{
    // TODO SubEditor<,>: Documentations
    public class SubEditor<TTarget, TEditor>
            where TTarget : Object
            where TEditor : Editor
    {
        public TEditor editor { get; private set; }
        public TTarget target { get; private set; }

        public SubEditor(TTarget target)
        {
            if (target == null) return;
            this.target = target;
            editor = (TEditor)Editor.CreateEditor(target);
        }

        public void Draw()
        {
            if (target == null) return;
            editor.OnInspectorGUI();
        }

        public void Destroy()
        {
            if (editor == null) return;
            Object.DestroyImmediate(editor);
        }
    }
}
#endif
