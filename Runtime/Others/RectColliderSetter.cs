using UnityEngine;
using Acedia;

#if UNITY_EDITOR
using AcediaEditor;
#endif

namespace Acedia
{
    // TODO RectColliderSetter: Documentations

    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider2D))]
    public class RectColliderSetter : MonoBehaviour
    {
        [MultipleDrawer, Pivot2D]
        public Pivot2D pivot = new Pivot2D(Direction2D.Middle);
        public Vector2 size = Vector2.one;

        private BoxCollider2D boxCollider = default;

        private void Update()
        {
            if (boxCollider == null)
                boxCollider = GetComponent<BoxCollider2D>();

            boxCollider.size = size;
            boxCollider.offset = new Vector2()
            {
                x = -size.x * (pivot.X - .5f),
                y = -size.y * (pivot.Y - .5f),
            };
        }

#if UNITY_EDITOR
        [SerializeField] DrawMode drawGizmos = DrawMode.Selected;

        private void OnDrawGizmos()
        {
            if (drawGizmos == DrawMode.Always)
                DrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (drawGizmos == DrawMode.Selected)
                DrawGizmos();
        }

        private void DrawGizmos()
        {
            if (!enabled) return;
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

            Color fill = Physics2D.colliderAsleepColor;
            fill.a = .1f;
            Color outline = Physics2D.colliderAwakeColor;
            HandlesExtension.DrawCollider2D(boxCollider, fill, outline);
        }
#endif
    }
}
