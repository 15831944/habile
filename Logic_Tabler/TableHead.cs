using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;

namespace Logic_Tabler
{
    public class TableHead
    {    
        G.Point _IP;
        double _scale;

        public int height = 14;

        public G.Point IP { get { return _IP; } }
        public double Scale { get { return _scale; } }

        public TableHead(G.Point position, double scale)
        {
            _IP = position;
            _scale = scale;
        }
    }
}
