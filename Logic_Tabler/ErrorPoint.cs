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
        string _errorMessage;
        double _scale;

        public G.Point IP { get { return _IP; } }
        public string ErrorMessage { get { return _errorMessage; } }
        public double Scale { get { return _scale; } set { _scale = value; } }
        

        public ErrorPoint(G.Point ip, string message)
        {
            _IP = ip;
            _errorMessage = message;
            _scale = 1;
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
