#if UNITY_EDITOR
using UnityEngine;

using Handles = UnityEditor.Handles;

namespace AcediaEditor
{
    public static class HandlesExtension
    {
        // TODO HandlesExtension: DrawSolidCapsule2D outlines-only
        public static void DrawSolidCapsule2D(Vector2 center, Vector2 size, CapsuleDirection2D direction, Color color, Color outline)
        {
            bool horizontal = direction == CapsuleDirection2D.Horizontal;

            Vector2 dirSize = new Vector2();
            dirSize.x = horizontal ? size.y : size.x;
            dirSize.y = horizontal ? size.x : size.y;

            float rectH = Mathf.Max(0f, dirSize.y - dirSize.x);
            Vector2 up = horizontal ? Vector2.right : Vector2.up;
            Vector2 down = horizontal ? Vector2.left : Vector2.down;
            Vector2 right = horizontal ? Vector2.down : Vector2.right;

            Rect rect = new Rect();
            rect.width = horizontal ? rectH : dirSize.x;
            rect.height = horizontal ? dirSize.x : rectH;

            rect.position = center - rect.size / 2f;

            Handles.DrawSolidRectangleWithOutline(rect, color, color);

            Handles.color = color;
            Handles.DrawSolidArc(rect.center + up * rectH / 2f, Vector3.back, right, -180f, dirSize.x / 2f);
            Handles.DrawSolidArc(rect.center + down * rectH / 2f, Vector3.back, right, 180f, dirSize.x / 2f);
            Handles.color = Color.white;
        }

        private static bool DrawBoxCollider2D(Collider2D collider, Color fill, Color outline)
        {
            BoxCollider2D box = collider as BoxCollider2D;

            if (box)
            {
                Rect rect = new Rect();
                rect.position = box.offset - box.size / 2f;
                rect.size = box.size;

                Handles.DrawSolidRectangleWithOutline(rect, fill, outline);
                return true;
            }

            return false;
        }

        private static bool DrawCircleCollider2D(Collider2D collider, Color fill, Color outline)
        {
            CircleCollider2D circle = collider as CircleCollider2D;
            if (circle)
            {
                Vector2 center = circle.offset;

                Handles.color = fill;
                Handles.DrawSolidDisc(center, Vector3.back, circle.radius);
                Handles.color = Color.white;

                Handles.color = outline;
                Handles.DrawWireDisc(center, Vector3.back, circle.radius);
                Handles.color = Color.white;
                return true;
            }

            return false;
        }

        private static bool DrawCapsuleCollider2D(Collider2D collider, Color fill, Color outline)
        {
            CapsuleCollider2D capsule = collider as CapsuleCollider2D;
            if (capsule)
            {
                DrawSolidCapsule2D(capsule.offset, capsule.size, capsule.direction, fill, outline);
                return true;
            }

            return false;
        }

        public static void DrawCollider2D(Collider2D collider, Color fill, Color outline)
        {
            Matrix4x4 prevMatrix = Handles.matrix;
            Handles.matrix = collider.transform.localToWorldMatrix;

            if (!DrawBoxCollider2D(collider, fill, outline) &&
                !DrawCircleCollider2D(collider, fill, outline) &&
                !DrawCapsuleCollider2D(collider, fill, outline))
            {
                Debug.LogFormat("Collider {0} cannot be drawn because the collider type hasn't been implemented yet.", collider.name);
            }

            Handles.matrix = prevMatrix;
        }

        public static void DrawCollider2D(Collider2D collider, Color color)
        {
            DrawCollider2D(collider, color, color);
        }
    }
}
#endif
