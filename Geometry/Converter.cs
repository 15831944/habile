using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public static class Converter
    {
        public static Polar xy_to_la(Vector v)
        {
            double L = Math.Sqrt(v.X * v.X + v.Y * v.Y);
            double a = Math.Atan2(v.Y, v.X);

            Polar d = new Polar(L, a);

            return d;
        }

        public static Vector la_to_xy(Polar d)
        {
            double X = d.L * Math.Cos(d.angle);
            double Y = d.L * Math.Sin(d.angle);

            Vector v = new Vector(X, Y);

            return v;
        }

        public static double Wrap(double value, double max = Math.PI, double min = -Math.PI)
        {
            value -= min;
            max -= min;

            if (max == 0) return min;

            value = value % max;
            value += min;

            while (value < min)
            {
                value += max;
            }

            return value;
        }

        public static double AngleDelta(double a1, double a2)
        {
            double d1 = Math.Abs(a2 - a1);
            double d2 = Math.Abs(a2 - a1 + 2 * Math.PI);
            double d3 = Math.Abs(a2 - a1 - 2 * Math.PI);

            double delta = Math.Min(d1, d2);
            delta = Math.Min(delta, d3);

            return delta;
        }

        public static double AngleDeltaClockwise(Vector v1, Vector v2)
        {
            Polar a1 = Converter.xy_to_la(v1);
            Polar a2 = Converter.xy_to_la(v2);

            double ang1 = Converter.Wrap(a1.angle, Math.PI * 2, 0);
            double ang2 = Converter.Wrap(a2.angle, Math.PI * 2, 0);

            if (ang2 < ang1)
            {
                ang2 += Math.PI * 2;
            }

            double delta = ang2 - ang1;
            return delta;
        }

        public static double ToDeg(double rad)
        {
            double deg = rad * 180 / Math.PI;

            return deg;
        }
    }
}

