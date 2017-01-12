using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Polar : IEquatable<Polar>
    {
        public double L;
        public double angle;

        public Polar(double L, double ang)
        {
            this.L = L;
            this.angle = Converter.Wrap(ang);
        }

        public Polar rotate(double ang)
        {
            double new_ang = Converter.Wrap(this.angle + ang);
            Polar new_d = new Polar(L, new_ang);

            return new_d;
        }

        public bool Equals(Polar other)
        {
            if (other == null) return false;
            return (this.L == other.L) && (this.angle == other.angle);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Polar);
        }

        public static bool operator ==(Polar a, Polar b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(Polar a, Polar b)
        {
            return !object.Equals(a, b);
        }

        public override string ToString()
        {
            string str = "L: " + L.ToString("f2") + "  ang (rad): " + angle.ToString("f4");

            return str;
        }

    }
}
