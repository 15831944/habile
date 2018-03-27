using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;

namespace Reinforcement
{
    //Positioon
    //Teraseklass
    //A - side1
    //B - main
    //U - delta angle

    public class C_Raud : Raud
    {
        internal G.Point _SidePoint;

        double _A;
        double _B;
        double _U;

        double _Length;

        public double A { get { return _A; } }
        public double B { get { return _B; } }
        public double U { get { return _U; } }
        public double Length { get { return _Length; } }


        public C_Raud(G.Line main, G.Line side, int nr, int d, string teras) : base(main, nr, d, teras)
        {
            G.Vector mainDir = main.getDirectionVector();
            G.Polar mainPol = G.Converter.xy_to_la(mainDir);

            G.Vector sideDir = side.getDirectionVector();
            G.Polar sidePol = G.Converter.xy_to_la(sideDir);

            _A = shorter(side.Length());
            _B = shorter(main.Length());
            _U = G.Converter.AngleDelta(mainPol.angle, sidePol.angle);

            _IP = main.Start;
            _Length = _A + _B;

            //OVERRIDE
            G.Vector dir = main.getDirectionVector();
            double shorterLength = shorter(main.Length());
            _StartPoint = main.Start;
            _EndPoint = _StartPoint.move(shorterLength, dir);
            //OVERRIDE

            G.Vector v1 = -1 * side.getDirectionVector();
            _SidePoint = _StartPoint.move(_A, v1);
        }


        public G.Line makeMainLine()
        {
            G.Point a = new G.Point(_StartPoint.X, _StartPoint.Y);
            G.Point b = new G.Point(_EndPoint.X, _EndPoint.Y);
            G.Line rebarLine = new G.Line(a, b);
            return rebarLine;
        }


        public G.Line makeSideLine()
        {
            G.Point a = new G.Point(_SidePoint.X, _SidePoint.Y);
            G.Point b = new G.Point(_StartPoint.X, _StartPoint.Y);
            G.Line rebarLine = new G.Line(a, b);
            return rebarLine;
        }


        public static E_Raud mergeTwoRebar(C_Raud one, C_Raud two)
        {
            G.Point a = one._SidePoint;
            G.Point b = one.StartPoint;
            G.Point c = two.StartPoint;
            G.Point d = two._EndPoint;

            G.Line side1 = new G.Line(a, b);
            G.Line main = new G.Line(b, c);
            G.Line side2 = new G.Line(c, d);

            E_Raud raud = new E_Raud(main, side1, side2, one.Number, one.Diameter, one.Materjal);
            return raud;
        }


        public static D_Raud mergeTwoRebar_long(C_Raud one, C_Raud two)
        {
            G.Point a = one._SidePoint;
            G.Point b = one.StartPoint;
            G.Point c = two.StartPoint;
            G.Point d = two._EndPoint;

            G.Line temp1 = new G.Line(a, b);
            G.Line main = new G.Line(b, c);
            G.Line temp2 = new G.Line(c, d);

            double s1 = temp1.Length();
            double s2 = temp2.Length();
            double max = Math.Max(s1, s2);

            G.Vector v1 = (-1) * temp1.getDirectionVector();
            G.Vector v2 = temp2.getDirectionVector();

            G.Point new_a = b.move(max, v1);
            G.Point new_d = c.move(max, v2);

            G.Line side1 = new G.Line(new_a, b);
            G.Line side2 = new G.Line(c, new_d);

            D_Raud raud = new D_Raud(main, side1, side2, one.Number, one.Diameter, one.Materjal);
            return raud;
        }


        public override string ToString()
        {
            string str = Number.ToString() + "C" + Diameter.ToString() + ((int)(A / 10)).ToString() + ((int)(B / 10)).ToString();
            return str;
        }


        public override string ToStringNoCount()
        {
            string str =  "C" + Diameter.ToString() + ((int)(A / 10)).ToString() + ((int)(B / 10)).ToString();
            return str;
        }


        public bool Equals(C_Raud other)
        {
            if (other == null) return false;
            return (this.A == other.A &&
                    this.B == other.B &&
                    (int)G.Converter.ToDeg(this.U) == (int)G.Converter.ToDeg(other.U) &&
                    this.Diameter == other.Diameter &&
                    this.Materjal == other.Materjal);
        }


        public override bool Equals(Raud obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as C_Raud);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as C_Raud);
        }


        public static bool operator ==(C_Raud a, C_Raud b)
        {
            return object.Equals(a, b);
        }


        public static bool operator !=(C_Raud a, C_Raud b)
        {
            return !object.Equals(a, b);
        }

    }
}
