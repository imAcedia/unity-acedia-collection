namespace Acedia
{
    // TODO Direction2D: Documentations
    [System.Flags]
    public enum Direction2D
    {
        Up = 1 << 0,
        Right = 1 << 1,
        Down = 1 << 2,
        Left = 1 << 3,

        North = 1 << 0,
        East = 1 << 1,
        South = 1 << 2,
        West = 1 << 3,

        Center = 0,
        Middle = 0,
        Top = 1 << 0,
        Bottom = 1 << 2,

        Vertical = Up | Down,
        Horizontal = Left | Right,
        All = ~0,
        None = 0,
    }

    static class Direction2DExtensions
    {
        public static bool IsValid(this Direction2D d)
        {
            return !((d & Direction2D.Horizontal) == Direction2D.Horizontal || (d & Direction2D.Vertical) == Direction2D.Vertical || d == Direction2D.None);
        }

        public static bool FlagActive(this Direction2D d, Direction2D flag, bool checkAllFlags = false)
        {
            if (checkAllFlags) return (d & flag) == flag;
            return ((d & flag) != 0);
        }

        public static bool HasOnly1Flag(this Direction2D d)
        {
            return (d != 0 && (d & (d - 1)) == 0);
        }

        public static int ActiveFlagCount(this Direction2D d)
        {
            int count = 0; int n = (int)d;
            while (n != 0) { n = n & (n - 1); count++; }
            return count;
        }

        public static int ToX(this Direction2D d)
        {
            if (!d.IsValid()) { return 0; }
            if (!d.FlagActive(Direction2D.Horizontal)) { return 0; }

            return d.FlagActive(Direction2D.Left) ? -1 : 1;
        }

        public static int ToY(this Direction2D d)
        {
            if (!d.IsValid()) return 0;
            if (!d.FlagActive(Direction2D.Vertical)) return 0;

            return d.FlagActive(Direction2D.Down) ? -1 : 1;
        }

        public static void ToXY(this Direction2D d, out int x, out int y)
        {
            x = y = 0;

            if (d.IsValid())
            {
                if (d.FlagActive(Direction2D.Horizontal))
                    x = d.FlagActive(Direction2D.Left) ? -1 : 1;

                if (d.FlagActive(Direction2D.Vertical))
                    y = d.FlagActive(Direction2D.Down) ? -1 : 1;
            }
        }

        public static void ToXY(this Direction2D d, out float x, out float y)
        {
            x = y = 0f;

            if (d.IsValid())
            {
                if (d.FlagActive(Direction2D.Horizontal))
                    x = d.FlagActive(Direction2D.Left) ? -1f : 1f;

                if (d.FlagActive(Direction2D.Vertical))
                    y = d.FlagActive(Direction2D.Down) ? -1f : 1f;
            }
        }

        public static (int, int) ToXYTuple(this Direction2D d)
        {
            int x = 0, y = 0;

            if (d.IsValid())
            {
                if (d.FlagActive(Direction2D.Horizontal))
                    x = d.FlagActive(Direction2D.Left) ? -1 : 1;

                if (d.FlagActive(Direction2D.Vertical))
                    y = d.FlagActive(Direction2D.Down) ? -1 : 1;
            }

            return (x, y);
        }

        public static void FromXY(this ref Direction2D d, int x, int y)
        {
            d = Direction2D.None;

            if (x < 0) d |= Direction2D.Left;
            else if (x > 0) d |= Direction2D.Right;

            if (y > 0) d |= Direction2D.Up;
            else if (y < 0) d |= Direction2D.Down;
        }

        public static void FromXY(this ref Direction2D d, float x, float y)
        {
            d = Direction2D.None;

            if (x < 0f) d |= Direction2D.Left;
            else if (x > 0f) d |= Direction2D.Right;

            if (y > 0f) d |= Direction2D.Up;
            else if (y < 0f) d |= Direction2D.Down;
        }

        public static Direction2D Inverted(this Direction2D d)
        {
            if (!d.IsValid()) return Direction2D.None;

            Direction2D invert = Direction2D.None;

            if (d.FlagActive(Direction2D.Up)) invert |= Direction2D.Down;
            if (d.FlagActive(Direction2D.Down)) invert |= Direction2D.Up;
            if (d.FlagActive(Direction2D.Left)) invert |= Direction2D.Right;
            if (d.FlagActive(Direction2D.Right)) invert |= Direction2D.Left;

            return invert;
        }
    }
}