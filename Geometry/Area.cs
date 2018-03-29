using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Area
    {
        Point _min;
        Point _max;


        public Area(Point min, Point max)
        {
            _min = min;
            _max = max;
        }


        public bool isPointInArea(Point p)
        {
            if (p.X < _min.X) return false;
            if (p.Y < _min.Y) return false;
            if (p.X > _max.X) return false;
            if (p.Y > _max.Y) return false;

            return true;
        }

    }
}
