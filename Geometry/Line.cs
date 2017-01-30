using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using MoreLinq;

namespace Geometry
{
    public class Line : IEquatable<Line>
    {
        public Point Start;
        public Point End;

        public Line(Point Start, Point End)
        {
            if (Start == End)
            {
                throw new LineSamePointException();
            }

            this.Start = Start;
            this.End = End;
        }

        public Line(Point Center, double distance, Vector v1)
        {
            this.Start = Center.move(distance, v1);
            this.End = Center.move(distance, -1 * v1);

            if (this.Start == this.End)
            {
                throw new LineSamePointException();
            }
        }

        public Line Copy()
        {
            Point new_start = new Point(Start.X, Start.Y);
            Point new_end = new Point(End.X, End.Y);
            Line new_line = new Line(new_start, new_end);

            return new_line;
        }

        public double Length()
        {
            double dx = End.X - Start.X;
            double dy = End.Y - Start.Y;

            double len = Math.Sqrt(dx * dx + dy * dy);

            return len;
        }

        public Point getCenterPoint()
        {
            double cX = Start.X + ((End.X - Start.X) / 2);
            double cY = Start.Y + ((End.Y - Start.Y) / 2);

            Point center = new Point(cX, cY);

            return center;
        }

        public Line extendStart(double dist)
        {
            Vector dir = -1 * getDirectionVector();
            Point new_start = Start.move(dist, dir);
            Point new_end = new Point(End.X, End.Y);
            Line new_line = new Line(new_start, new_end);

            return new_line;
        }

        public Line extendEnd(double dist)
        {
            Vector dir = getDirectionVector();
            Point new_end = End.move(dist, dir);
            Point new_start = new Point(Start.X, Start.Y);
            Line new_line = new Line(new_start, new_end);

            return new_line;
        }
        
        public Line extendDouble(double dist)
        {
            Vector dir = getDirectionVector();
            Point new_start = Start.move(dist, -1 * dir);
            Point new_end = End.move(dist, dir);
            Line new_line = new Line(new_start, new_end);

            return new_line;
        }

        public Vector getDirectionVector()
        {
            double dx = End.X - Start.X;
            double dy = End.Y - Start.Y;

            double dL = Length();

            dx = dx / dL;
            dy = dy / dL;

            Vector unit = new Vector(dx, dy);

            return unit;
        }

        public Vector getTopVector(Vector one)
        {
            Vector other = getDirectionVector();

            if (Converter.AngleDeltaClockwise(one, other) > Math.PI)
            {
                other = other.rotate(Math.PI);
            }

            return other;
        }

        public Vector getBottomVector(Vector one)
        {
            Vector other = getDirectionVector();

            if (Converter.AngleDeltaClockwise(other, one) > Math.PI)
            {
                other = other.rotate(Math.PI);
            }

            return other;
        }

        public Vector getCoolVector(Vector one)
        {
            Polar main = Converter.xy_to_la(one);
            Polar other1 = Converter.xy_to_la(getDirectionVector());
            Polar other2 = other1.rotate(Math.PI);

            if (Converter.AngleDelta(main.angle, other1.angle) < Converter.AngleDelta(main.angle, other2.angle))
            {
                Vector v1 = Converter.la_to_xy(other1);
                return v1;
            }
            else
            {
                Vector v1 = Converter.la_to_xy(other2);
                return v1;
            }
        }

        public Vector getOffsetVector()
        {
            Vector unit = getDirectionVector();
            Vector u90 = unit.rotate(Math.PI / 2);

            return u90;
        }

        public Point getOtherPoint(Point one)
        {
            if (one == Start) return End;
            else return Start;
        }

        public Line Offset(double offset)
        {
            Vector u90 = getOffsetVector();
            Vector v90 = offset * u90;

            Point new_Start = Start + v90;
            Point new_End = End + v90;

            Line new_line = new Line(new_Start, new_End);

            return new_line;
        }

        public Line rotation(Point startPoint, double angle)
        {
            Vector a_s = new Vector(Start.X - startPoint.X, Start.Y - startPoint.Y);
            a_s = a_s.rotate(angle);
            Vector a_e = new Vector(End.X - startPoint.X, End.Y - startPoint.Y);
            a_e = a_e.rotate(angle);

            Point new_start = new Point(a_s.X, a_s.Y);
            Point new_end = new Point(a_e.X, a_e.Y);
            Line projection = new Line(new_start, new_end);

            return projection;
        }

        public static Line merge(Line line_a, Line line_b)
        {
            Vector v1 = line_a.getDirectionVector();
            Polar pol = Converter.xy_to_la(v1);

            Line m = line_a.rotation(line_a.Start, pol.angle);
            Line n = line_b.rotation(line_a.Start, pol.angle);

            Dictionary<Point, Point> keeper = new Dictionary<Point, Point>();
            keeper[m.Start] = line_a.Start;
            keeper[m.End] = line_a.End;
            keeper[n.Start] = line_b.Start;
            keeper[n.End] = line_b.End;

            List<Point> pointList = new List<Point>();
            pointList.Add(m.Start);
            pointList.Add(m.End);
            pointList.Add(n.Start);
            pointList.Add(n.End);

            pointList = pointList.OrderBy(b => b.X).ToList();

            //Point rotated_start = pointList.MinBy(x => x.X);
            //pointList.Remove(rotated_start);
            //Point rotated_end = pointList.MaxBy(x => x.X);

            Point rotated_start = pointList[0];
            Point rotated_end = pointList[pointList.Count - 1];

            Point normal_start = keeper[rotated_start];
            Point normal_end = keeper[rotated_end];

            Line merged_line = new Line(normal_start, normal_end);
            return merged_line;
        }

        public static bool areLinesOnSameLine(Line line_a, Line line_b, double X_TOLERANCE = 1)
        {
            Vector v1 = line_a.getDirectionVector();
            Polar p1 = Converter.xy_to_la(v1);

            Line a_x = line_a.rotation(line_a.Start, -p1.angle);
            Line b_x = line_b.rotation(line_a.Start, -p1.angle);

            double dY = Math.Abs(b_x.Start.Y - b_x.End.Y);
            if (dY > X_TOLERANCE) return false;

            double dYstart = Math.Abs(b_x.Start.Y - a_x.Start.Y);
            if (dYstart > X_TOLERANCE) return false;

            double dYend = Math.Abs(b_x.End.Y - a_x.Start.Y);
            if (dYend > X_TOLERANCE) return false;

            return true;
        }

        public static bool areLinesCoLinear(Line line_a, Line line_b, double X_TOLERANCE = 1)
        {
            Vector v1 = line_a.getDirectionVector();
            Polar p1 = Converter.xy_to_la(v1);

            Line a_x = line_a.rotation(line_a.Start, -p1.angle);
            Line b_x = line_b.rotation(line_a.Start, -p1.angle);

            if (areLinesOnSameLine(line_a, line_b, X_TOLERANCE))
            {
                if ((a_x.Start.X < b_x.Start.X) && (b_x.Start.X < a_x.End.X)) return true;
                if ((a_x.Start.X < b_x.End.X) && (b_x.End.X < a_x.End.X)) return true;
                if ((b_x.Start.X < a_x.Start.X) && (a_x.Start.X < b_x.End.X)) return true;
                if ((b_x.Start.X < a_x.End.X) && (a_x.End.X < b_x.End.X)) return true;
                if ((a_x.Start.X == b_x.Start.X) || (b_x.Start.X == a_x.End.X)) return true;
                if ((a_x.Start.X == b_x.End.X) || (b_x.End.X == a_x.End.X)) return true;
            }

            return false;
        }

        public static bool hasIntersection(Line line_a, Line line_b)
        {
            if (line_a == line_b)
            {
                throw new LineSameLineException();
            }

            Point a = line_a.Start;
            Point b = line_a.End;
            Point c = line_b.Start;
            Point d = line_b.End;
            
            if (a == c || a == d)
            {
                return true;
            }
        
            if (b == c || b == d)
            {
                return true;
            }

            int s1 = side(a, b, c);
            int s2 = side(a, b, d);
            
            if (s1 == 0 && s2 == 0) //co-linear treated as FALSE
            {
                return false;
            }
            
            if (s1 == s2)
            {
                return false;
            }

            s1 = side(c, d, a);
            s2 = side(c, d, b);
            
            if (s1 == s2)
            {
                return false;
            }

            return true;
        }

        private static int side(Point a, Point b, Point c)
        {
            double dd = (c.Y - a.Y) * (b.X - a.X) - (b.Y - a.Y) * (c.X - a.X);

            int r = 0;

            if (dd > 0) r = 1;
            else if (dd < 0) r = -1;
            else r = 0;

            return r;
        }

        public static Point getIntersectionPoint(Line line_a, Line line_b)
        {
            if (hasIntersection(line_a, line_b))
            {
                Point a = line_a.Start;
                Point b = line_a.End;
                if (line_a.End.X < line_a.Start.X)
                {
                    a = line_a.End;
                    b = line_a.Start;
                }

                Point c = line_b.Start;
                Point d = line_b.End;
                if (line_b.End.X < line_b.Start.X)
                {
                    c = line_b.End;
                    d = line_b.Start;
                }

                if (a == c) return c;
                else if (a == d) return d;
                else if (b == c) return c;
                else if (b == d) return d;

                double A1 = b.Y - a.Y;
                double B1 = a.X - b.X;
                double C1 = (A1 * a.X) + (B1 * a.Y);

                double A2 = d.Y - c.Y;
                double B2 = c.X - d.X;
                double C2 = (A2 * c.X) + (B2 * c.Y);

                double det = A1 * B2 - A2 * B1;

                double new_x = (B2 * C1 - B1 * C2) / det;
                double new_y = (A1 * C2 - A2 * C1) / det;

                Point inter = new Point(new_x, new_y);
                return inter;
            }
            else
            {
                throw new LineNoIntersectionException();
            }

        }

        public bool Equals(Line other)
        {
            if (other == null) return false;
            return (this.Start == other.Start && this.End == other.End);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Line);
        }

        public static bool operator ==(Line a, Line b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(Line a, Line b)
        {
            return !object.Equals(a, b);
        }

        public override string ToString()
        {
            string str = "(Start) " + Start.ToString() + " \n(End) " + End.ToString() + "\n(Length) " + Length().ToString();

            return str;
        }

    }
}
