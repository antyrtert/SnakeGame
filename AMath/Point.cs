namespace AMath
{
    public interface IPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string ToString() => $"{X}; {Y}";
    }

    public class Point : IPoint
    {
        public int X, Y;
        public static readonly Point Zero = new Point(0, 0);

        int IPoint.X { get => X; set => X = value; }
        int IPoint.Y { get => Y; set => Y = value; }

        public Point(int x, int y) =>
            (X, Y) = (x, y);

        public static implicit operator System.Windows.Point(Point p) =>
            new System.Windows.Point(p.X, p.Y);

        public static implicit operator Point(System.Windows.Point p) =>
            new Point((int)p.X, (int)p.Y);

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
            System.Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));
    }
}
