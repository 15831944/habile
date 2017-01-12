using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;

namespace Logic_Tabler
{
    public class ErrorPoint
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
    }
}
