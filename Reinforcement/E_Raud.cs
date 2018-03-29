using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using G = Geometry;

namespace Reinforcement
{
    //Positioon
    //Teraseklass
    //A - side1
    //B - main
    //C - side2
    //U - side1 / main angle delta
    //V - main / side2 angle delta
    //X - side1 / main height
    //Y - main / side2 height

    public class E_Raud : Raud
    {
        internal G.Point _Side1Point;
        internal G.Point _Side2Point;

        double _A;
        double _B;
        double _B2; // parand magic
        double _C;
        double _U;
        double _V;
        double _X;
        double _Y;
        

        public double A { get { return _A; } }
        public double B { get { return _B; } }
        public double B2 { get { return _B2; } } // parand magic
        public double C { get { return _C; } }
        public double U { get { return _U; } }
        public double V { get { return _V; } }
        public double X { get { return _X; } }
        public double Y { get { return _Y; } }


        public E_Raud(G.Line main, G.Line side1, G.Line side2, int nr, int d, string teras, int parand = 0) : base(main, nr, d, teras)
        {
            G.Vector mainDir = main.getDirectionVector();
            G.Polar mainPol = G.Converter.xy_to_la(mainDir);

            G.Vector sideDir1 = side1.getDirectionVector();
            G.Polar sidePol1 = G.Converter.xy_to_la(sideDir1);

            G.Vector sideDir2 = side2.getDirectionVector();
            G.Polar sidePol2 = G.Converter.xy_to_la(sideDir2);

            _A = shorter(side1.Length());
            _B = shorter(main.Length());
            _B2 = shorter(main.Length() + parand); // parand magic
            _C = shorter(side2.Length());

            G.Vector perpendVector = mainDir.rotate(-Math.PI / 2);
            double maxDist = Math.Max(_A, _C) * 2;
            G.Line mainExtended = main.extendDouble(maxDist);

            G.Point movePoint1 = side1.Start.move(maxDist, perpendVector);
            G.Line perpendLine1 = new G.Line(side1.Start, movePoint1);
            G.Point interPoint1 = G.Line.getIntersectionPoint(mainExtended, perpendLine1);
            G.Line XLine = new G.Line(side1.Start, interPoint1);

            G.Point movePoint2 = side2.End.move(maxDist, perpendVector);
            G.Line perpendLine2 = new G.Line(side2.End, movePoint2);
            G.Point interPoint2 = G.Line.getIntersectionPoint(mainExtended, perpendLine2);
            G.Line YLine = new G.Line(side2.End, interPoint2);

            _U = G.Converter.AngleDelta(mainPol.angle, sidePol1.angle);
            _V = G.Converter.AngleDelta(mainPol.angle, sidePol2.angle);
            _X = XLine.Length();
            _Y = YLine.Length();

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
                str = Number.ToString() + "E" + Diameter.ToString() + ((int)(B2 / 10)).ToString() + ((int)(C / 10)).ToString(); // parand magic
            }
            else
            {
                str = Number.ToString() + "E" + Diameter.ToString() + ((int)(B2 / 10)).ToString() + ((int)(A / 10)).ToString() + ((int)(C / 10)).ToString(); // parand magic
            }
            return str;
        }


        public override string ToStringNoCount()
        {
            string str = "";
            if (A == C)
            {
                str = "E" + Diameter.ToString() + ((int)(B2 / 10)).ToString() + ((int)(C / 10)).ToString(); // parand magic
            }
            else
            {
                str = "E" + Diameter.ToString() + ((int)(B2 / 10)).ToString() + ((int)(A / 10)).ToString() + ((int)(C / 10)).ToString(); // parand magic
            }
            return str;
        }


        public string ToStringDebug()
        {
            string str = "";
            str = "A: " + _A.ToString() + ", B:" + _B.ToString() + ", B2: " + _B2.ToString() + ", C: " + _C.ToString() + ", U: " + _U.ToString() + ", V: " + _V.ToString() + ", X: " + _X.ToString() + ", Y: " + _Y.ToString();

            return str;
        }


        public bool Equals(E_Raud other)
        {
            if (other == null) return false;

            bool alfa = (this.A == other.A &&
                        this.B2 == other.B2 && // parand magic
                        this.C == other.C &&
                        this.Diameter == other.Diameter &&
                        this.Materjal == other.Materjal);

            bool a1 = (int)G.Converter.ToDeg(this.U) == (int)G.Converter.ToDeg(other.U) && (int)G.Converter.ToDeg(this.V) == (int)G.Converter.ToDeg(other.V);
            bool a2 = (int)G.Converter.ToDeg(this.U) == (int)G.Converter.ToDeg(other.V) && (int)G.Converter.ToDeg(this.V) == (int)G.Converter.ToDeg(other.U);

            bool tot = alfa && (a1 || a2);

            return tot;
        }


        public override bool Equals(Raud obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as E_Raud);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as E_Raud);
        }


        public static bool operator ==(E_Raud a, E_Raud b)
        {
            return object.Equals(a, b);
        }


        public static bool operator !=(E_Raud a, E_Raud b)
        {
            return !object.Equals(a, b);
        }

    }
}