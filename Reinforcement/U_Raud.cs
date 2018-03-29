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

    public class U_Raud : Raud
    {
        double _A;
        double _B;
        double _C;
        

        public double A { get { return _A; } }
        public double B { get { return _B; } }
        public double C { get { return _C; } }


        public U_Raud(G.Line side1, int z_dist, int nr, int d, string teras) : base(side1, nr, d, teras)
        {
            if (d > 16)
            {
                _A = shorter(d * 14.5);
            }
            else
            {
                _A = shorter(d * 13);
            }
                        
            _B = shorter(z_dist);
            _C = shorter(side1.Length());

            _IP = side1.getCenterPoint();
            _Length = _A + _B + _C;
        }

        public override string ToString()
        {
            string str = Number.ToString() + "U" + Diameter.ToString() + ((int)(B / 10)).ToString() + ((int)(C / 10)).ToString();
            return str;
        }

        public override string ToStringNoCount()
        {
            string str = "U" + Diameter.ToString() + ((int)(B / 10)).ToString() + ((int)(C / 10)).ToString();
            return str;
        }

        public bool Equals(U_Raud other)
        {
            if (other == null) return false;
            return (this.A == other.A &&
                    this.B == other.B &&
                    this.C == other.C &&
                    this.Diameter == other.Diameter &&
                    this.Materjal == other.Materjal);
        }

        public override bool Equals(Raud obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as U_Raud);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as U_Raud);
        }

        public static bool operator ==(U_Raud a, U_Raud b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(U_Raud a, U_Raud b)
        {
            return !object.Equals(a, b);
        }
    }
}
