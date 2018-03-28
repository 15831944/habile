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


        public static double wrapper(double value, double max = 2 * Math.PI, double min = 0)
        {
            //hetkel ei s6ltu max-ist mitte kuidagi
            value -= min;
            value %= (2 * Math.PI);
            while (value < min) value += (2 * Math.PI);
            value += min;

            return value;
        }


        public static double AngleDelta(double a1, double a2)
        {
            double ccw = AngleDeltaCCW(a1, a2);
            double cw = AngleDeltaCW(a1, a2);

            double deltaMin = Math.Min(ccw, cw);
            return deltaMin;
        }


        public static double AngleDeltaCW(double a1, double a2)
        {
            double ang1 = wrapper(a1, 2 * Math.PI, 0);
            double ang2 = wrapper(a2, 2 * Math.PI, 0);

            double delta = ang1 - ang2;
            if (delta < 0) delta += (2 * Math.PI);

            return delta;
        }


        public static double AngleDeltaCCW(double a1, double a2)
        {
            double ang1 = wrapper(a1, 2 * Math.PI, 0);
            double ang2 = wrapper(a2, 2 * Math.PI, 0);

            double delta = ang2 - ang1;
            if (delta < 0) delta += (2 * Math.PI);

            return delta;
        }


        public static double AngleDelta(Vector v1, Vector v2)
        {
            Polar _this = Converter.xy_to_la(v1);
            Polar _other = Converter.xy_to_la(v2);

            return AngleDelta(_this.angle, _other.angle);
        }


        public static double AngleDeltaCW(Vector v1, Vector v2)
        {
            Polar _this = Converter.xy_to_la(v1);
            Polar _other = Converter.xy_to_la(v2);
            
            return AngleDeltaCW(_this.angle, _other.angle);
        }


        public static double AngleDeltaCCW(Vector v1, Vector v2)
        {
            Polar _this = Converter.xy_to_la(v1);
            Polar _other = Converter.xy_to_la(v2);

            return AngleDeltaCCW(_this.angle, _other.angle);
        }

        public static double ToDeg(double rad)
        {
            double deg = rad * 180 / Math.PI;

            return deg;
        }

    }
}

