using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Corner : IEquatable<Corner>
    {
        Point corner;
        Edge startEdge;
        Edge endEdge;
        double angle;

        public Point CP { get { return corner; } }
        public Edge StartEdge { get { return startEdge; } }
        public Edge EndEdge { get { return endEdge; } }
        public double Angle { get { return angle; } }


        public Corner(Point p, Edge ln1, Edge ln2, List<Line> contours)
        {
            corner = p;

            if (ln1.Line.Start == p)
            {
                startEdge = ln1;
                endEdge = ln2;
            }
            else if (ln1.Line.End == p)
            {
                startEdge = ln2;
                endEdge = ln1;
            }

            angle = calcAngle(ln1, ln2, contours);
        }


        private double calcAngle(Edge eb, Edge ec, List<Line> contours)
        {
            double b = eb.Line.Length();
            double c = ec.Line.Length();

            Line la = new Line(eb.Line.Start, ec.Line.End);
            double a = la.Length();

            double cosA = (Math.Pow(b, 2) + Math.Pow(c, 2) - Math.Pow(a, 2)) / (2 * b * c);
            double cosAmod = Math.Max(Math.Min(cosA, 0.9999999999999), -0.9999999999999);

            double A = Math.Acos(cosAmod);

            Point one = la.getCenterPoint();
            Point two = eb.Line.getCenterPoint();

            Line centerLine = new Line(one, two);
            Line centerLine_offset = centerLine.extendEnd(-1 * _Variables.MOVE_DISTANCE);
            Point check = centerLine_offset.End;

            if (!Region_Static.isPointinRegion(check, contours))
            {
                A = 2 * Math.PI - A;
            }

            return A;
        }


        public Edge getOtherEdge(Edge one)
        {
            if (one == StartEdge) return EndEdge;
            else if (one == EndEdge) return StartEdge;
            else throw new EdgeNotInDefinedCornerException();
        }


        public Point getCornerPoint(Edge e, double offset_main, double offset_side, double alfa = 1000)
        {
            Line startLine = e.Line.Offset(offset_main);
            Line otherLine = getOtherEdge(e).Line.Offset(offset_side);

            Line extendedStart = startLine.extendDouble(alfa);
            Line extendedOther = otherLine.extendDouble(alfa);

            Point ip = Line.getIntersectionPoint(extendedStart, extendedOther);

            return ip;
        }


        public bool Equals(Corner other)
        {
            if (other == null) return false;
            return (this.corner == other.corner && this.angle == other.angle);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Corner);
        }


        public static bool operator ==(Corner a, Corner b)
        {
            return object.Equals(a, b);
        }


        public static bool operator !=(Corner a, Corner b)
        {
            return !object.Equals(a, b);
        }

    }
}