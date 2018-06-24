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


        public Corner(Point p, Edge ln1, Edge ln2)
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

            angle = calcAngle(endEdge, startEdge);
        }


        private double calcAngle(Edge end, Edge start)
        {
            Vector p1 = end.Line.getDirectionVector();
            Vector p2 = start.Line.getDirectionVector();
            if (p1 == p2) return Math.PI;

            Line la = new Line(end.Line.Start, start.Line.End);
            double b = end.Line.Length();
            double c = start.Line.Length();
            double a = la.Length();

            double cosA = (Math.Pow(b, 2) + Math.Pow(c, 2) - Math.Pow(a, 2)) / (2 * b * c);
            double cosAmod = Math.Max(Math.Min(cosA, 0.9999999999999), -0.9999999999999);

            double A = Math.Acos(cosAmod);

            Point ccp = la.getCenterPoint();
            Point ecp = end.Line.Offset(10).getCenterPoint();

            Line aa = new Line(ccp, ecp);
            Line bb = end.Line.extendDouble(10 * start.Line.Length());

            if (Line.hasIntersection(aa, bb))
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


        public bool isReverseCorner()
        {
            if (Angle > Math.PI) return true;

            return false;
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


        public override string ToString()
        {
            //string str = "(Start) " + Start.ToString() + " \n(End) " + End.ToString() + " \n(Length) " + Length().ToString();
            string str = StartEdge.Line.ToString() + " " + EndEdge.Line.ToString();
            return str;
        }

    }
}