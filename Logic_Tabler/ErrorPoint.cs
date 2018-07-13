using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;

namespace Logic_Tabler
{
    public class ErrorPoint : IEquatable<ErrorPoint>
    {
        G.Point _IP;
        string _ErrorMessage;
        double _Scale;

        public G.Point IP { get { return _IP; } }
        public string ErrorMessage { get { return _ErrorMessage; } }
        public double Scale { get { return _Scale; } }


        public ErrorPoint(G.Point ip, string message, double scale)
        {
            _IP = ip;
            _ErrorMessage = message;
            _Scale = scale;
        }


        public bool Equals(ErrorPoint other)
        {
            if (other == null) return false;
            return (this.IP == other.IP &&
                    this.ErrorMessage == other.ErrorMessage);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as ErrorPoint);
        }


        public static bool operator ==(ErrorPoint a, ErrorPoint b)
        {
            return object.Equals(a, b);
        }


        public static bool operator !=(ErrorPoint a, ErrorPoint b)
        {
            return !object.Equals(a, b);
        }

    }
}
