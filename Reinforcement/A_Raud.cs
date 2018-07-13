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
    //A

    public class A_Raud : Raud
    {
        double _A;
        
        public double A { get { return _A; } }


        public A_Raud(G.Line main, int nr, int d, string teras) : base (main, nr, d, teras)
        {
            _A = shorter(main.Length());

            _IP = main.getCenterPoint();
            _Length = _A;
        }


        public override string ToString()
        {
            string str = Number.ToString() + "A" + Diameter.ToString() + A.ToString();
            return str;
        }


        public override string ToStringNoCount()
        {
            string str = "A" + Diameter.ToString() + A.ToString();
            return str;
        }


        public G.Line makeLine()
        {
            G.Point a = new G.Point(_StartPoint.X, _StartPoint.Y);
            G.Point b = new G.Point(_EndPoint.X, _EndPoint.Y);
            G.Line rebarLine = new G.Line(a, b);
            return rebarLine;
        }


        public static A_Raud mergeTwoRebar(A_Raud one, A_Raud two)
        {
            G.Line a = one.makeLine();
            G.Line b = two.makeLine();
            G.Line new_line = G.Line.merge(a, b);

            A_Raud raud = new A_Raud(new_line, one.Number, one.Diameter, one.Materjal);
            return raud;
        }


        public bool Equals(A_Raud other)
        {
            if (other == null) return false;
            return (this.A == other.A && 
                    this.Diameter == other.Diameter && 
                    this.Materjal == other.Materjal);
        }


        public override bool Equals(Raud obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as A_Raud);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as A_Raud);
        }


        public static bool operator ==(A_Raud a, A_Raud b)
        {
            return object.Equals(a, b);
        }


        public static bool operator !=(A_Raud a, A_Raud b)
        {
            return !object.Equals(a, b);
        }

    }
}
