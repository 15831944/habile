using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Region
    {
        public List<Edge> edges;
        public List<Corner> corners;

        private double min_X = double.MaxValue;
        private double max_X = double.MinValue;
        private double min_Y = double.MaxValue;
        private double max_Y = double.MinValue;

        public double Min_X { get { return min_X; } }
        public double Max_X { get { return max_X; } }
        public double Min_Y { get { return min_Y; } }
        public double Max_Y { get { return max_Y; } }

        public Region(List<Line> contours)
        {
            if (contours.Count < 3)
            {
                throw new RegionNotValidException();
            }

            edges = new List<Edge>();
            
            setBoundies(contours);
            setEdges(contours);

            while (true)
            {
                corners = new List<Corner>();
                setCorners(contours);
                bool stop = insanityCheck();
                if (stop) break;
            }

            
        }

        private void setBoundies(List<Line> contours)
        {
            foreach (Line ln in contours)
            {
                if (ln.Start.X < Min_X) min_X = ln.Start.X;
                if (ln.Start.X > max_X) max_X = ln.Start.X;
                if (ln.Start.Y < min_Y) min_Y = ln.Start.Y;
                if (ln.Start.Y > max_Y) max_Y = ln.Start.Y;

                if (ln.End.X < Min_X) min_X = ln.End.X;
                if (ln.End.X > max_X) max_X = ln.End.X;
                if (ln.End.Y < min_Y) min_Y = ln.End.Y;
                if (ln.End.Y > max_Y) max_Y = ln.End.Y;
            }
        }

        private void setEdges(List<Line> contours)
        {
            foreach (Line line in contours)
            {
                Point center = line.getCenterPoint();
                Vector offset = line.getOffsetVector();
                Point p = center.move(_Variables.MOVE_DISTANCE, offset);
                bool inSide = Region_Static.isPointinRegion(p, contours);

                if (inSide)
                {
                    Edge e = new Edge(line);
                    edges.Add(e);
                }
                else
                {
                    Line new_line = new Line(line.End, line.Start);
                    center = new_line.getCenterPoint();
                    offset = new_line.getOffsetVector();
                    p = center.move(_Variables.MOVE_DISTANCE, offset);
                    inSide = Region_Static.isPointinRegion(p, contours);

                    if (inSide)
                    {
                        Edge e = new Edge(new_line);
                        edges.Add(e);
                    }
                    else
                    {
                        throw new RegionLineNotInRegionException();
                    }
                }
            }
        }

        private void setCorners(List<Line> contours)
        {
            foreach (Edge e1 in edges)
            {
                foreach (Edge e2 in edges)
                {
                    if (e1 == e2)
                    {
                        continue;
                    }
                    else if (e1.Line.End == e2.Line.Start)
                    {
                        if (setCorner(e1.Line.End, e1, e2, contours)) break;
                    }
                }
            }
        }

        private bool setCorner(Point point, Edge e1, Edge e2, List<Line> contours)
        {
            foreach (Corner oc in corners)
            {
                if (oc.StartEdge == e1 && oc.EndEdge == e2)
                {
                    return false;
                }
            }

            Corner c = new Corner(point, e1, e2, contours);
            e1.setCorner(c);
            e2.setCorner(c);
            corners.Add(c);
            return true;
        }

        public bool isPointinRegion(Point pa)
        {
            if (pa.X < min_X) return false;
            if (pa.X > max_X) return false;
            if (pa.Y < min_Y) return false;
            if (pa.Y > max_Y) return false;

            double dX = Math.Abs(max_X - min_X);
            double dY = Math.Abs(max_Y - min_Y);

            double new_X = (pa.X + dX) * 1.9;
            double new_Y = (pa.Y + dY) * 2.1;
            Point pe = new Point(new_X, new_Y);

            Line testLine = new Line(pa, pe);
            int i = 0;

            foreach (Edge edge in edges)
            {
                bool inter = Line.hasIntersection(testLine, edge.Line);
                if (inter) i++;
            }

            if (i == 0) return false;

            bool answer = (i % 2 != 0);

            return answer;
        }

        private bool insanityCheck()
        {
            for (int i = corners.Count - 1; i >= 0; i--)
            {
                Corner c = corners[i];

                if (Math.Abs(c.Angle - Math.PI) < _Variables.FLAT_CORNER_TOLERANCE)
                {
                    Edge start = c.StartEdge;
                    Edge end = c.EndEdge;

                    Line new_line = new Line(end.Line.Start, start.Line.End);
                    Point center = new_line.getCenterPoint();
                    Vector offset = new_line.getOffsetVector();
                    Point p = center.move(_Variables.MOVE_DISTANCE, offset);
                    bool inSide = isPointinRegion(p);

                    if (inSide)
                    {
                        Edge e = new Edge(new_line);
                        edges.Add(e);
                        edges.Remove(start);
                        edges.Remove(end);

                        return false;
                    }
                    else
                    {
                        throw new RegionLineNotInRegionException();
                    }
                }
            }

            return true;
        }
    }
}


