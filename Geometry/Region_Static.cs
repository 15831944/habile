using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    static class Region_Static
    {
        public static bool isPointinRegion(Point pa, Vector v, List<Line> contours)
        {
            double min_X = double.MaxValue;
            double max_X = double.MinValue;
            double min_Y = double.MaxValue;
            double max_Y = double.MinValue;

            find_boundries(contours, ref min_X, ref max_X, ref min_Y, ref max_Y);

            if (pa.X < min_X) return false;
            if (pa.X > max_X) return false;
            if (pa.Y < min_Y) return false;
            if (pa.Y > max_Y) return false;

            double dX = Math.Abs(max_X - min_X);
            double dY = Math.Abs(max_Y - min_Y);
            double dL = (dX + dY)*2;

            double new_X = (pa.X + dX) * 5;
            double new_Y = pa.Y;
            
            Point pe = pa.move(dL, v);
            Line testLine = new Line(pa, pe);

            int i = 0;

            foreach (Line contour in contours)
            {
                bool inter = Line.hasIntersection(testLine, contour);
                if (inter) i++;
            }

            if (i == 0) return false;
            bool answer = (i % 2 != 0);
            return answer;
        }


        private static void find_boundries(List<Line> contours, ref double min_X, ref double max_X, ref double min_Y, ref double max_Y)
        {
            foreach (Line ln in contours)
            {
                if (ln.Start.X < min_X) min_X = ln.Start.X;
                if (ln.Start.X > max_X) max_X = ln.Start.X;
                if (ln.Start.Y < min_Y) min_Y = ln.Start.Y;
                if (ln.Start.Y > max_Y) max_Y = ln.Start.Y;

                if (ln.End.X < min_X) min_X = ln.End.X;
                if (ln.End.X > max_X) max_X = ln.End.X;
                if (ln.End.Y < min_Y) min_Y = ln.End.Y;
                if (ln.End.Y > max_Y) max_Y = ln.End.Y;
            }
        }

    }
}
