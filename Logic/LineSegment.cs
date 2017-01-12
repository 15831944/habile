using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;
using R = Reinforcement;

namespace Logic_Reinf
{
    public class LineSegment : IEquatable<LineSegment>
    {
        G.Point _start;
        G.Point _end;

        G.Edge _parent;
        G.Edge _other;

        public LineSegment(G.Point start, G.Edge parent, G.Edge other = null)
        {
            _start = start;
            _end = start;

            _parent = parent;
            _other = other;
        }

        public void updateSegment(G.Point end)
        {
            _end = end;
        }

        public bool compareSegments(G.Edge other)
        {
            if (_other == null && other == null) return true;
            if (_other == null) return false;
            if (other == null) return false;

            if (_other == other) return true;

            return false;
        }

        public G.Line getLine()
        {
            return new G.Line(_start, _end);
        }

        public G.Line getOtherLine()
        {
            return new G.Line(_other.Line.Start, _other.Line.End);
        }

        public bool hasOtherEdge()
        {
            if (_other == null) return false;
            else return true;
        }

        public bool Equals(LineSegment other)
        {
            if (other == null) return false;

            bool one = (this._parent == other._parent) && (this._other == other._other);
            bool two = (this._parent == other._other) && (this._other == other._parent);
            bool eq = one || two;
            return eq;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as LineSegment);
        }

        public static bool operator ==(LineSegment a, LineSegment b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(LineSegment a, LineSegment b)
        {
            return !object.Equals(a, b);
        }
    }
}
