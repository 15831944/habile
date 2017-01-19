using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class Edge : IEquatable<Edge>
    {
        Line line;
        Corner startCorner;
        Corner endCorner;

        public Line Line { get { return line; } }
        public Corner StartCorner { get { return startCorner; } }
        public Corner EndCorner { get { return endCorner; } }

        public Edge(Line line)
        {
            this.line = line;
        }

        public void setCorner(Corner one)
        {
            if (one.CP == line.Start)
            {
                startCorner = one;
            }
            else if (one.CP == line.End)
            {
                endCorner = one;
            }
            else
            {
                throw new CornerNotInDefinedEdgeException();
            }
        }

        public Corner getOtherCorner(Corner one)
        {
            if (one == startCorner) return endCorner;
            else if (one == endCorner) return startCorner;
            else throw new CornerNotInDefinedEdgeException();
        }

        public Line edgeOffset(double offset_main, double offset_side1, double offset_side2)
        {
            Point startPoint = StartCorner.getCornerPoint(this, offset_main, offset_side1);
            Point endPoint = EndCorner.getCornerPoint(this, offset_main, offset_side2);

            Line offset_line = new Line(startPoint, endPoint);

            return offset_line;
        }

        public Line edgeTrimmer(double offset_main, double offset_side1, double offset_side2)
        {
            if (StartCorner.Angle < Math.PI) offset_side1 = -offset_side1;
            if (EndCorner.Angle < Math.PI) offset_side2 = -offset_side2;

            Point startPoint = StartCorner.getCornerPoint(this, offset_main, offset_side1);
            Point endPoint = EndCorner.getCornerPoint(this, offset_main, offset_side2);

            Line offset_line = new Line(startPoint, endPoint);

            return offset_line;
        }

        public static bool getSharedCorner(Edge e1, Edge e2, ref Corner c)
        {
            if (e1.StartCorner == e2.EndCorner)
            {
                c = e1.StartCorner;
                return true;
            }
            else if (e1.EndCorner == e2.StartCorner)
            {
                c = e1.EndCorner;
                return true;
            }
            else if (e1.StartCorner == e2.StartCorner)
            {
                c = e1.StartCorner;
                return true;
            }
            else if (e1.EndCorner == e2.EndCorner)
            {
                c = e1.EndCorner;
                return true;
            }

            c = null;
            return false;
        }

        public static bool haveSharedCorner(Edge e1, Edge e2)
        {
            if (e1.StartCorner == e2.EndCorner) return true;
            else if (e1.EndCorner == e2.StartCorner) return true;
            else if (e1.StartCorner == e2.StartCorner) return true;
            else if (e1.EndCorner == e2.EndCorner) return true;

            return false;
        }

        public bool Equals(Edge other)
        {
            if (other == null) return false;
            return (this.line == other.line);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Edge);
        }

        public static bool operator ==(Edge a, Edge b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(Edge a, Edge b)
        {
            return !object.Equals(a, b);
        }
    }
}
