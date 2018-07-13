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
    //C - side2

    public class D_Raud : Raud
    {
        internal G.Point _Side1Point;
        internal G.Point _Side2Point;

        double _A;
        double _B;
        double _B2; // magic
        double _C;
        

        public double A { get { return _A; } }
        public double B { get { return _B; } }
        public double B2 { get { return _B2; } } // parand magic
        public double C { get { return _C; } }


        public D_Raud(G.Line side1, int z_dist, int nr, int d, string teras) : base(side1, nr, d, teras)
        {
            _A = shorter(side1.Length());
            _B = shorter(z_dist); // parand magic
            _B2 = _B; // parand magic
            _C = shorter(side1.Length());

            _IP = side1.getCenterPoint();
            _Length = _A + _B + _C;
        }


        public D_Raud(G.Line main, G.Line side1, G.Line side2, int nr, int d, string teras, int parand = 0) : base(main, nr, d, teras)
        {
            _A = shorter(side1.Length());
            _B = shorter(main.Length());
            _B2 = shorter(main.Length() + parand); // parand magic
            _C = shorter(side2.Length());

            _IP = main.getCenterPoint();
            _Length = _A + _B2 + _C; // parand magic

            G.Vector v1 = -1 * side1.getDirectionVector();
            _Side1Point = _StartPoint.move(_A, v1);

            G.Vector v2 = side2.getDirectionVector();
            _Side2Point = _EndPoint.move(_C, v2);
        }


        public G.Line makeMainLine()
        {
            G.Point a = new G.Point(_StartPoint.X, _StartPoint.Y);
            G.Point b = new G.Point(_EndPoint.X, _EndPoint.Y);
            G.Line rebarLine = new G.Line(a, b);
            return rebarLine;
        }


        public G.Line makeSide1Line()
        {
            G.Point a = new G.Point(_Side1Point.X, _Side1Point.Y);
            G.Point b = new G.Point(_StartPoint.X, _StartPoint.Y);
            G.Line rebarLine = new G.Line(a, b);
            return rebarLine;
        }


        public G.Line makeSide2Line()
        {
            G.Point a = new G.Point(_EndPoint.X, _EndPoint.Y);
            G.Point b = new G.Point(_Side2Point.X, _Side2Point.Y);
            G.Line rebarLine = new G.Line(a, b);
            return rebarLine;
        }


        public override string ToString()
        {
            string str = "";
            if (A == C)
            {
                str = Number.ToString() + "D" + Diameter.ToString() + ((int)(B2 / 10)).ToString() + ((int)(C / 10)).ToString(); // parand magic
            }
            else
            {
                str = Number.ToString() + "D" + Diameter.ToString() + ((int)(B2 / 10)).ToString() + ((int)(A / 10)).ToString() + ((int)(C / 10)).ToString(); // parand magic
            }

            return str;
        }


        public override string ToStringNoCount()
        {
            string str = "";
            if (A == C)
            {
                str = "D" + Diameter.ToString() + ((int)(B2 / 10)).ToString() + ((int)(C / 10)).ToString(); // parand magic
            }
            else
            {
                str = "D" + Diameter.ToString() + ((int)(B2 / 10)).ToString() + ((int)(A / 10)).ToString() + ((int)(C / 10)).ToString(); // parand magic
            }
            return str;
        }


        public bool Equals(D_Raud other)
        {
            if (other == null) return false;
            return (this.A == other.A &&
                    this.B2 == other.B2 && // parand magic
                    this.C == other.C &&
                    this.Diameter == other.Diameter &&
                    this.Materjal == other.Materjal);
        }


        public override bool Equals(Raud obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as D_Raud);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as D_Raud);
        }


        public static bool operator ==(D_Raud a, D_Raud b)
        {
            return object.Equals(a, b);
        }


        public static bool operator !=(D_Raud a, D_Raud b)
        {
            return !object.Equals(a, b);
        }

    }
}