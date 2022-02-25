using System;

namespace AMath
{
    [Serializable]
    public struct Vector : IVec2
    {
        public int X, Y;
        public static readonly Vector
            Left  = new Vector(-1, 0),
            Right = new Vector(1, 0),
            Up    = new Vector(0, -1),
            Down  = new Vector(0, 1),
            Zero  = new Vector(0, 0);

        int IVec2.X { get => X; set => X = value; }
        int IVec2.Y { get => Y; set => Y = value; }

        public Vector(int x, int y) =>
            (X, Y) = (x, y);

        public static Vector operator +(Vector Left, Vector Right) =>
            new Vector(Left.X + Right.X, Left.Y + Right.Y);

        public static Vector operator -(Vector Left, Vector Right) =>
            new Vector(Left.X - Right.X, Left.Y - Right.Y);

        public static Vector operator -(Vector vector) =>
            new Vector(-vector.X, -vector.Y);

        public static Vector operator *(Vector Left, int Right) =>
            new Vector(Left.X * Right, Left.Y * Right);

        public static Vector operator /(Vector Left, int Right) =>
            new Vector(Left.X / Right, Left.Y / Right);

        public static bool operator ==(Vector Left, Vector Right) => Vector.Equals(Left, Right);
        public static bool operator !=(Vector Left, Vector Right) => !Vector.Equals(Left, Right);

        public static bool Equals(Vector Left, Vector Right)
        {
            if (Left is Vector left && Right is Vector right)
                return left.X == right.X && left.Y == right.Y;
            return false;
        }

        public override bool Equals(object obj) => Vector.Equals(this, (Vector)obj);

        public override int GetHashCode() => base.GetHashCode();
    }
}
