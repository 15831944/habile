using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Point : IEquatable<Point>
    {
        public double X;
        public double Y;

        public Point(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Point move(double distance, Vector direction)
        {
            Vector unit_dir = direction / direction.Length();
            Vector dir = distance * unit_dir;

            double new_X = X + dir.X;
            double new_Y = Y + dir.Y;

            Point new_point = new Point(new_X, new_Y);

            return new_point;
        }

        public double distanceTo(Point target)
        {
            double dx = target.X - X;
            double dy = target.Y - Y;

            double len = Math.Sqrt(dx * dx + dy * dy);

            return len;
        }

        //TODO distanceTo (Line)

        public bool Equals(Point other)
        {
            if (other == null) return false;
            double dX = Math.Abs(this.X - other.X);
            double dY = Math.Abs(this.Y - other.Y);

            return (dX < _Variables.EQUALS_TOLERANCE && dY < _Variables.EQUALS_TOLERANCE);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Point);
        }

        public static Point operator +(Point a, Vector b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static bool operator ==(Point a, Point b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !object.Equals(a, b);
        }

        public override string ToString()
        {
            string str = "X: " + X.ToString("f4") + "  Y: " + Y.ToString("f4");

            return str;
        }
    }
}
