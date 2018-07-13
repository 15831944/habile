using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;
using R = Reinforcement;

namespace Logic_Reinf
{
    public class PseudoCorner
    {
        G.Edge edge1;
        G.Edge edge2;

        public G.Edge StartEdge { get { return edge1; } }
        public G.Edge EndEdge { get { return edge2; } }


        public PseudoCorner(G.Edge ln1, G.Edge ln2)
        {
            edge1 = ln1;
            edge2 = ln2;
        }


        public G.Edge getOtherEdge(G.Edge one)
        {
            if (one == edge1) return edge2;
            else if (one == edge2) return edge1;
            else throw new G.EdgeNotInDefinedCornerException();
        }


        public G.Point getCornerPoint(G.Edge e, double offset_main, double offset_side, double alfa = 100000)
        {
            G.Line startLine = e.Line.Offset(offset_main);
            G.Line otherLine = getOtherEdge(e).Line.Offset(offset_side);

            G.Line extendedStart = startLine.extendDouble(alfa);
            G.Line extendedOther = otherLine.extendDouble(alfa);

            G.Point ip = G.Line.getIntersectionPoint(extendedStart, extendedOther);

            return ip;
        }

    }
}