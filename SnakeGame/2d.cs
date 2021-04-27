using System;

namespace SnakeGame
{
    public class Point
    {
        public int X, Y;
        public static readonly Point Zero = new Point(0, 0);

        public Point() { }

        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static implicit operator System.Windows.Point(Point p) =>
            new System.Windows.Point(p.X, p.Y);

        public static implicit operator Point(System.Windows.Point p) =>
            new Point((int)p.X, (int)p.Y);

        public static explicit operator Point(Vector vector) =>
            new Point(vector.X, vector.Y);

        public static Point operator +(Point Left, Vector Right) =>
            new Point(Left.X + Right.X, Left.Y + Right.Y);

        public static Point operator -(Point Left, Vector Right) =>
            new Point(Left.X - Right.X, Left.Y - Right.Y);

        public static Vector operator -(Point Left, Point Right) =>
            new Vector(Left.X - Right.X, Left.Y - Right.Y);

        public static bool operator ==(Point Left, Point Right) => Point.Equals(Left, Right);
        public static bool operator !=(Point Left, Point Right) => !Point.Equals(Left, Right);

        public override int GetHashCode() => base.GetHashCode();

        public static bool Equals(Point Left, Point Right)
        {
            if (Left is Point left && Right is Point right)
                return left.X == right.X && left.Y == right.Y;
            return false;
        }

        public override bool Equals(object obj) => Point.Equals(this, (Point)obj);

        public double DistanceTo(Point other) =>
            Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));

        public override string ToString() => $"({X}; {Y})";
    }

    public class Vector
    {
        public int X, Y;
        public static readonly Vector
            Left = new Vector(-1, 0),
            Right = new Vector(1, 0),
            Up = new Vector(0, -1),
            Down = new Vector(0, 1),
            Zero = new Vector(0, 0);

        public Vector(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static explicit operator Vector(Point point) =>
            new Vector(point.X, point.Y);

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

        public override string ToString() => $"{{{X}; {Y}}}";

        public override int GetHashCode() => base.GetHashCode();
    }
}
