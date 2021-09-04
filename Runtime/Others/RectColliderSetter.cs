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
        public Vector2 size = Vector2.one;

        [MultipleDrawer, Pivot2D]
        [HideIf(nameof(useCustomPivot))]
        public Direction2D _pivot = Direction2D.Middle;
        [ShowIf(nameof(useCustomPivot))]
        public Vector2 pivot;
        [OnInspectorChangeMethod(nameof(OnToggleCustom))]
        public bool useCustomPivot = false;
        private void OnToggleCustom()
        {
            if (!useCustomPivot) ToPivot2D();
            else FromPivot2D();
        }

        public Vector2 Pivot => useCustomPivot ? pivot : FromPivot2D();
        private Vector2 FromPivot2D() => pivot = new Vector2(_pivot.ToX() + 1f, _pivot.ToY() + 1f) * .5f;
        private Direction2D ToPivot2D() { _pivot.FromXY(pivot.x * 2f - 1f, pivot.y * 2f - 1f); return _pivot; }

        private BoxCollider2D boxCollider = default;

        private void Update()
        {
            if (boxCollider == null)
                boxCollider = GetComponent<BoxCollider2D>();

            boxCollider.size = size;
            boxCollider.offset = new Vector2()
            {
                x = -size.x * (Pivot.x - .5f),
                y = -size.y * (Pivot.y - .5f),
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
