using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acedia
{
    [System.Serializable]
    public struct Pivot2D
    {
        [SerializeField] Direction2D direction;
        [SerializeField] Vector2 pivot;
        [SerializeField] bool useCustomPivot;

        public float X
        {
            get
            {
                if (!useCustomPivot) SetPivot(direction);
                return pivot.x;
            }

            set => SetPivot(new Vector2(value, pivot.y));
        }

        public float Y
        {
            get
            {
                if (!useCustomPivot) SetPivot(direction);
                return pivot.y;
            }

            set => SetPivot(new Vector2(pivot.x, value));
        }

        public Vector2 PivotVector
        {
            get
            {
                if (!useCustomPivot) SetPivot(direction);
                return pivot;
            }

            set => SetPivot(value);
        }

        public Direction2D PivotDirection
        {
            get
            {
                if (!useCustomPivot) SetPivot(pivot);
                return direction;
            }

            set => SetPivot(value);
        }

        public void SetPivot(Vector2 pivot)
        {
            this.pivot = new Vector2(direction.ToX() + 1f, direction.ToY() + 1f) * .5f;
            direction.FromXY(pivot.x * 2f - 1f, pivot.y * 2f - 1f);
            useCustomPivot = true;
        }

        public void SetPivot(Direction2D pivot)
        {
            if (pivot != Direction2D.Middle && !pivot.IsValid())
                throw new System.ArgumentException($"{nameof(pivot)} is not valid(cannot be converted to Vector2).", nameof(pivot));

            direction = pivot;
            this.pivot = new Vector2(pivot.ToX() + 1f, pivot.ToY() + 1f) * .5f;
        }

        public Pivot2D(Direction2D pivot)
        {
            this.pivot = default;
            direction = default;

            useCustomPivot = false;
            SetPivot(pivot);
        }

        public Pivot2D(Vector2 pivot)
        {
            this.pivot = default;
            direction = default;
            
            useCustomPivot = true;
            SetPivot(pivot);
        }

        public static explicit operator Direction2D(Pivot2D pivot)
        {
            return pivot.PivotDirection;
        }

        public static explicit operator Vector2(Pivot2D pivot)
        {
            return pivot.PivotVector;
        }

#if UNITY_EDITOR
        public const string directionField = nameof(direction);
        public const string pivotField = nameof(pivot);
        public const string useCustomPivotField = nameof(useCustomPivot);
#endif
    }

#if UNITY_EDITOR
#endif
}
