using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Vector : IEquatable<Vector>
    {
        public double X;
        public double Y;

        public Vector(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public double Length()
        {
            double len = Math.Sqrt(X * X + Y * Y);
            return len;
        }

        public Vector rotate(double rad)
        {
            Polar d = Converter.xy_to_la(this);
            Polar new_d = d.rotate(rad);
            Vector new_v = Converter.la_to_xy(new_d);

            return new_v;
        }

        public bool Equals(Vector other)
        {
            if (other == null) return false;

            Polar _this = Converter.xy_to_la(this);
            Polar _other = Converter.xy_to_la(other);

            double dA = Converter.AngleDelta(_this.angle, _other.angle);
            double dL = Math.Abs(_this.L - _other.L);

            if (dA < _Variables.VECTOR_ANGLE_TOLERANCE && dL < _Variables.EQUALS_TOLERANCE) return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Vector);
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !object.Equals(a, b);
        }

        public static Vector operator *(double a, Vector b)
        {
            return new Vector(a * b.X, a * b.Y);
        }

        public static Vector operator /(Vector b, double a)
        {
            return new Vector(b.X / a, b.Y / a);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public override string ToString()
        {
            string str = "X: " + X.ToString("f4") + "  Y: " + Y.ToString("f4") + " L: " + Length();

            return str;
        }
    }
}

