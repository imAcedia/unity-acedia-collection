#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Acedia;

namespace AcediaEditor
{
    // TODO EditorHelper: Cleanup
    public static partial class EditorHelper
    {
        public static readonly float fieldHeight = EditorGUIUtility.singleLineHeight;
        public static readonly float fieldSpacing = EditorGUIUtility.standardVerticalSpacing;
        public static readonly float defaultTabSize = 15f;

        public static float VerticalFieldDistance => fieldHeight + fieldSpacing;

        public static void CreateFolderDeep(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            if (AssetDatabase.IsValidFolder(path)) return;

            string parent = Path.GetDirectoryName(path);
            if (!AssetDatabase.IsValidFolder(parent))
                CreateFolderDeep(parent);

            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        public static string GetParentPropertyPath(SerializedProperty property)
        {
            string path = property.propertyPath;
            if (!path.Contains(".")) return "";

            string[] paths = path.Split('.');
            int joinCount = paths.Length - 1;

            if (paths.Length >= 3 && paths[paths.Length - 2].Equals("Array"))
                joinCount -= 1;

            path = string.Join(".", paths, 0, joinCount);
            return path;
        }

        public static string GetSiblingPropertyPath(SerializedProperty property, string siblingPath)
        {
            string parentPath = GetParentPropertyPath(property);
            SerializedProperty parent = property.serializedObject.FindProperty(parentPath);
            
            string path;
            if (string.IsNullOrEmpty(parentPath))
                path = siblingPath;

            else if (parent.isArray)
                path = parentPath.Contains(".") ? Path.ChangeExtension(parentPath, siblingPath) : siblingPath;

            else path = $"{parentPath}.{siblingPath}";

            return path;
        }

        public static SerializedProperty GetParentProperty(SerializedProperty property)
        {
            string parentPath = GetParentPropertyPath(property);
            return property.serializedObject.FindProperty(parentPath);
        }

        public static SerializedProperty GetSiblingProperty(SerializedProperty property, string siblingPath)
        {
            string path = GetSiblingPropertyPath(property, siblingPath);
            return property.serializedObject.FindProperty(path);
        }

        public static object GetTargetObject(SerializedProperty property)
        {
            if (property == null) return null;

            string path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            string[] elements = path.Split('.');
        
            foreach (string element in elements)
            {
                int indexStart = element.IndexOf('[');
                if (indexStart >= 0)
                {
                    string elementName = element.Substring(0, indexStart);
                    int index = Convert.ToInt32(element.Substring(indexStart + 1, element.Length - indexStart - 2));
                    obj = GetObjectValue(obj, elementName, index);
                }

                else obj = GetObjectValue(obj, element);
            }

            return obj;
        }

        public static Type GetPropertyFieldType(SerializedProperty property)
        {
            if (property == null) return null;

            string path = property.propertyPath.Replace(".Array.data[", "[");
            Type objType = property.serializedObject.targetObject.GetType();

            string[] elements = path.Split('.');
            FieldInfo fi = null;
            foreach (string element in elements)
            {
                if (fi == null)
                {
                    if (objType.IsArray) fi = objType.GetElementType().GetField(element);
                    else fi = objType.GetField(element, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                }

                if (fi == null) return null;
                objType = fi.FieldType;
            }

            return objType;
        }

        public static FieldInfo GetPropertyFieldInfo(SerializedProperty property)
        {
            if (property == null) return null;

            string path = property.propertyPath.Replace(".Array.data[", "[");
            Type objType = property.serializedObject.targetObject.GetType();

            string[] elements = path.Split('.');
            FieldInfo fi = null;
            foreach (string element in elements)
            {
                if (fi == null)
                {
                    if (objType.IsArray) fi = objType.GetElementType().GetField(element);
                    else fi = objType.GetField(element, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                }

                if (fi == null) return null;
                objType = fi.FieldType;
            }

            return fi;
        }

        public static Type GetPropertyDrawerFieldType(PropertyDrawer drawer)
        {
            Type type = drawer.fieldInfo.FieldType;
            if (type.IsArray) type = type.GetElementType();
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) type = type.GetGenericArguments()[0];
            return type;
        }

        /// <summary> Used for IfAttributes? </summary>
        public static bool ComparePropertyValue(SerializedProperty property, object comparedValue)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            bool result;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    result = comparedValue == null ? property.boolValue : property.boolValue.Equals(comparedValue);
                    return result;
                case SerializedPropertyType.Enum:
                    result = comparedValue != null && property.enumValueIndex.Equals((int)comparedValue);
                    return result;
                case SerializedPropertyType.ObjectReference:
                    if (comparedValue != null) Debug.LogWarning($"{nameof(comparedValue)} will be ignored when comparing object reference value, since it will only check if the reference is null or not.");
                    result = property.objectReferenceValue != null;
                    return result;
                default:
                    throw new NotImplementedException($"{property.type} is not supported. Property path: {property.propertyPath}");
            }
        }

        #region Reflections
        public static List<Type> GetDerivedTypes(Type baseType, Assembly assembly = null, bool onlyExportedTypes = false)
        {
            if (assembly == null) assembly = Assembly.GetAssembly(baseType);

            Type[] allTypes;
            if (onlyExportedTypes) allTypes = assembly.GetTypes();
            else allTypes = assembly.GetExportedTypes();

            List<Type> results = new List<Type>(allTypes.Length);
            for (int i = 0; i < allTypes.Length; i++)
            {
                Type type = allTypes[i];
                if (type.IsSubclassOf(baseType) && !type.IsAbstract && !type.IsGenericType)
                    results.Add(type);
            }

            return results;
        }

        public static bool IsSubclassOfGenericDefinition(this Type sub, Type generic)
        {
            Type checkedType = sub;
            while (checkedType != null && checkedType != typeof(object))
            {
                Type t = checkedType.IsGenericType ? checkedType.GetGenericTypeDefinition() : checkedType;
                if (generic == t) return true;
                checkedType = checkedType.BaseType;
            }

            return false;
        }

        public static object GetObjectValue(object source, string name)
        {
            if (source == null) return null;

            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null) return f.GetValue(source);

                PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null) return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        public static object GetObjectValue(object source, string name, int index)
        {
            IEnumerable enumerable = GetObjectValue(source, name) as IEnumerable;
            if (enumerable == null) return null;
            
            IEnumerator enumerator = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
                if (!enumerator.MoveNext()) return null;

            return enumerator.Current;
        }
        #endregion

        #region IMGUI
        public static bool TextFieldSubmitted(int textFieldId, Rect textFieldRect)
        {
            if (!EditorGUIUtility.editingTextField || GUIUtility.keyboardControl != textFieldId)
                return true;

            Event e = Event.current;

            if (e.isKey)
            {
                switch (e.keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                    case KeyCode.Escape:
                        e.Use();
                        return true;
                }
            }

            if (e.type == EventType.MouseDown && !textFieldRect.Contains(e.mousePosition))
                return true;

            return false;
        }

        public static int GetLastControlID()
        {
            Type type = typeof(EditorGUIUtility);
            FieldInfo field = type.GetField("s_LastControlID", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            return (int)field.GetValue(null);
        }

        public static void DebugRect(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(1f, 0f, 0f, .2f));
        }

        // UNDONE EditorHelper: Finish EnumFlagField
        public static void EnumFlagField(Rect position, SerializedProperty property, GUIContent label, bool alwaysOpen, Action<int> OnValidate = null)
        {
            throw new NotImplementedException();
            #pragma warning disable CS0162 // Unreachable code detected
            EditorGUI.BeginProperty(position, label, property);
            #pragma warning restore CS0162 // Unreachable code detected

            bool isExpanded = true;
            int value = property.intValue;

            if (alwaysOpen)
            {
                EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);
            }
            else
            {
                isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);
                property.isExpanded = isExpanded;
            }

            if (isExpanded)
            {
                // ATODO: Custom Enum Flags Field
            }

            property.intValue = value;
        }

        #region Rect Indent
        public static void UnIndentRect(ref Rect rect)
            => IndentRect(ref rect, -defaultTabSize);

        public static void UnIndentRect(ref Rect rect, float tabSize)
            => IndentRect(ref rect, -tabSize);

        public static void IndentRect(ref Rect rect)
            => IndentRect(ref rect, defaultTabSize);

        public static void IndentRect(ref Rect rect, float tabSize)
        {
            EditorGUIUtility.labelWidth -= tabSize;
            rect.width -= tabSize;
            rect.x += tabSize;
        }

        public static Rect UnIndentRect(Rect rect)
            => IndentRect(rect, -defaultTabSize);

        public static Rect UnIndentRect(Rect rect, float tabSize)
            => IndentRect(rect, -tabSize);

        public static Rect IndentRect(Rect rect)
            => IndentRect(rect, defaultTabSize);

        public static Rect IndentRect(Rect rect, float tabSize)
        {
            EditorGUIUtility.labelWidth -= tabSize;
            rect.width -= tabSize;
            rect.x += tabSize;

            return rect;
        }
        #endregion
        #endregion

        #region UIElements

        #region Mesh Drawing
        public static void DrawArrow(MeshGenerationContext context, Vector2 pointA, Vector2 pointB, Color color, float size = 1f, float headPosNormalized = .5f)
        {
            float angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x);

            #region DrawLine
            MeshWriteData mesh = context.Allocate(4, 6);
            float offsetX = size / 2f * Mathf.Sin(angle);
            float offsetY = size / 2f * Mathf.Cos(angle);

            // Top Right
            // Bottom Right
            // Bottom Left
            // Top Left
            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ),
                tint = color
            });
            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(pointB.x + offsetX, pointB.y - offsetY, Vertex.nearZ),
                tint = color
            });
            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(pointB.x - offsetX, pointB.y + offsetY, Vertex.nearZ),
                tint = color
            });
            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(pointA.x - offsetX, pointA.y + offsetY, Vertex.nearZ),
                tint = color
            });

            mesh.SetNextIndex(0);
            mesh.SetNextIndex(1);
            mesh.SetNextIndex(2);
            mesh.SetNextIndex(2);
            mesh.SetNextIndex(3);
            mesh.SetNextIndex(0);
            #endregion

            mesh = context.Allocate(3, 3);

            float r = 4f + size;
            Vector2 center = (pointA + pointB) * headPosNormalized;
            Vector2 p1 = center + (Vector2.right * r).Rotated(angle);
            Vector2 p2 = center + (Vector2.right * r).Rotated(angle + 120f * Mathf.Deg2Rad);
            Vector2 p3 = center + (Vector2.right * r).Rotated(angle + 240f * Mathf.Deg2Rad);

            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(p1.x, p1.y, Vertex.nearZ),
                tint = color
            });
            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(p2.x, p2.y, Vertex.nearZ),
                tint = color
            });
            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(p3.x, p3.y, Vertex.nearZ),
                tint = color
            });
            mesh.SetNextIndex(0);
            mesh.SetNextIndex(1);
            mesh.SetNextIndex(2);
            return;
        }

        public static void DrawLine(MeshGenerationContext context, Vector2 pointA, Vector2 pointB, Color color, float thickness = 1f)
        {
            MeshWriteData mesh = context.Allocate(4, 6);

            float angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x);
            float offsetX = thickness / 2f * Mathf.Sin(angle);
            float offsetY = thickness / 2f * Mathf.Cos(angle);

            // Top Right
            // Bottom Right
            // Bottom Left
            // Top Left
            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ),
                tint = color
            });
            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(pointB.x + offsetX, pointB.y - offsetY, Vertex.nearZ),
                tint = color
            });
            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(pointB.x - offsetX, pointB.y + offsetY, Vertex.nearZ),
                tint = color
            });
            mesh.SetNextVertex(new Vertex()
            {
                position = new Vector3(pointA.x - offsetX, pointA.y + offsetY, Vertex.nearZ),
                tint = color
            });

            mesh.SetNextIndex(0);
            mesh.SetNextIndex(1);
            mesh.SetNextIndex(2);
            mesh.SetNextIndex(2);
            mesh.SetNextIndex(3);
            mesh.SetNextIndex(0);
        }

        public static void DrawLines(MeshGenerationContext context, Vector2[] points, Color color, float thickness)
        {
            if (points.Length <= 1) throw new System.ArgumentException("Not enough points to make a single line", "points");

            Vertex[] vertices = new Vertex[(points.Length) * 2];
            ushort[] indices = new ushort[(points.Length - 1) * 6];
            MeshWriteData mesh = context.Allocate(vertices.Length, indices.Length);

            Vector2 ab = Vector2.zero;
            Vector2 p = Vector2.zero;

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 pointA = points[i];
                Vector2 pointB = i < points.Length - 1 ? points[i + 1] : Vector2.zero;
                int vertexStart = i * 2;

                if (i == 0 || i >= points.Length - 1)
                {
                    if (i == 0) ab = (pointB - pointA).normalized;
                    p = new Vector2(-ab.y, ab.x) * thickness;
                }
                else
                {
                    Vector2 cb = (pointA - pointB).normalized;
                    Vector2 bisector = (ab + cb).normalized;

                    if (bisector.sqrMagnitude > 0f)
                    {
                        float a = Mathf.Atan2(Utilities.Cross2D(bisector, ab), Vector2.Dot(bisector, ab));
                        float sin = Mathf.Sin(a);
                        float offset = thickness / sin;
                        p = bisector * offset;
                        ab = -cb;
                    }
                }

                vertices[vertexStart + 0] = new Vertex()
                {
                    position = new Vector3(pointA.x + p.x, pointA.y + p.y, Vertex.nearZ),
                    tint = color
                };
                vertices[vertexStart + 1] = new Vertex()
                {
                    position = new Vector3(pointA.x - p.x, pointA.y - p.y, Vertex.nearZ),
                    tint = color
                };

                if (i > 0)
                {
                    ushort tl = (ushort)(vertexStart - 2);
                    ushort bl = (ushort)(vertexStart - 1);
                    ushort tr = (ushort)vertexStart;
                    ushort br = (ushort)(vertexStart + 1);

                    mesh.SetNextIndex(tl);
                    mesh.SetNextIndex(bl);
                    mesh.SetNextIndex(tr);
                    mesh.SetNextIndex(tr);
                    mesh.SetNextIndex(bl);
                    mesh.SetNextIndex(br);
                }
            }

            mesh.SetAllVertices(vertices);
        }
        #endregion

        #endregion
    }
}
#endif
