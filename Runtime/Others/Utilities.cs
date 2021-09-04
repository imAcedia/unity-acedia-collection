using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using System.IO;

namespace Acedia
{
    // TODO Utilities: Cleanup
    public static class Utilities
    {
        public static Vector2 Rotated(this Vector2 v, float a)
        {
            float cos = Mathf.Cos(a);
            float sin = Mathf.Sin(a);
            Vector2 res = new Vector2()
            {
                x = v.x * cos - v.y * sin,
                y = v.x * sin + v.y * cos
            };

            return res;
        }

        public static void Rotate(this ref Vector2 v, float a)
        {
            float cos = Mathf.Cos(a);
            float sin = Mathf.Sin(a);
            v = new Vector2()
            {
                x = v.x * cos - v.y * sin,
                y = v.x * sin + v.y * cos
            };
        }

        public static bool IntersectRayWithBounds(this Rect rect, Ray ray, out float distance)
        {
            Bounds bounds = new Bounds(rect.center, rect.size);
            return bounds.IntersectRay(ray, out distance);
        }

        // TODO Utilities: IntersectRay QA
        public static bool IntersectRay(this Rect rect, Ray2D ray, out float distance)
        {
            // t = ray mesti di kali berapa biar kena boundary nya rect
            // ray.origin + t * ray.dir = rect.min

            float invDirX = ray.direction.x == 0f ? -Mathf.Infinity : 1f / ray.direction.x;
            float invDirY = ray.direction.y == 0f ? -Mathf.Infinity : 1f / ray.direction.y;

            float t0 = (rect.xMin - ray.origin.x) * invDirX;
            float t1 = (rect.xMax - ray.origin.x) * invDirX;
            if (t0 > t1) Swap(ref t0, ref t1);

            float t0y = (rect.yMin - ray.origin.y) * invDirY;
            float t1y = (rect.yMax - ray.origin.y) * invDirY;
            if (t0y > t1y) Swap(ref t0y, ref t1y);

            if (t0y > t0) t0 = t0y;
            if (t1y < t1) t1 = t1y;

            //distance = t0 > 0 ? t0 : t1;
            distance = t0;
            return t0 <= t1;
        }

        public static void Swap(ref float a, ref float b)
        {
            float temp = a;
            a = b; b = temp;
        }

        public static bool PathEquals(this string thisPath, string path)
        {
            return Path.GetFullPath(thisPath).Equals(Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase);
        }

        public static Rect GetLocalBounds(this Collider2D collider)
        {
            if (TryCast(collider, out BoxCollider2D box))
            {
                return new Rect()
                {
                    size = box.size,
                    center = box.offset
                };
            }

            else if (TryCast(collider, out CircleCollider2D circle))
            {
                return new Rect()
                {
                    size = Vector2.one * circle.radius * 2f,
                    center = circle.offset,
                };
            }

            else if (TryCast(collider, out CapsuleCollider2D capsule))
            {
                CapsuleDirection2D direction = capsule.direction;
                bool vert = direction == CapsuleDirection2D.Vertical;
                float radius = vert ? capsule.size.x : capsule.size.y;
                float height = vert ? capsule.size.y : capsule.size.x;
                Vector2 size = new Vector2(radius, Mathf.Max(radius, height));
                if (!vert) Swap(ref size.x, ref size.y);

                return new Rect()
                {
                    size = size,
                    center = capsule.offset
                };
            }

            // LTODO: Add other colliders such as Composites, Polygon, Edge, etc
            else throw new NotImplementedException();
        }

        public static bool TryCast<TBase, TDerived>(TBase from, out TDerived result)
                where TBase : class
                where TDerived : class, TBase
        {
            result = from as TDerived;
            return result != null;
        }

        public static float Cross2D(this Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }

        public static void QuickRemoveAt<T>(this List<T> list, int index)
        {
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }
    }
}
