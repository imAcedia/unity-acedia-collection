#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace AcediaEditor
{
    // TODO ReorderableListWrapper: Documentations
    public class ReorderableListWrapper
    {
        private readonly Dictionary<Object, ReorderableList> lists = new Dictionary<Object, ReorderableList>();

        private ReorderableList CreateList(SerializedProperty property)
        {
            ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, true);

            list.drawHeaderCallback = r => EditorGUI.LabelField(r, list.serializedProperty.displayName);
            list.elementHeightCallback = i => EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(i));
            list.drawElementCallback = (r, i, a, f) => EditorGUI.PropertyField(r, list.serializedProperty.GetArrayElementAtIndex(i), true);

            return list;
        }

        public ReorderableList GetList(SerializedProperty property, Object key)
        {
            if (!lists.ContainsKey(key))
                lists.Add(key, CreateList(property));

            lists[key].serializedProperty = property;
            return lists[key];
        }
    }
}
#endif
