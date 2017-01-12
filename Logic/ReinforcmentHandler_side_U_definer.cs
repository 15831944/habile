﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using G = Geometry;
using R = Reinforcement;

namespace Logic_Reinf
{
    public partial class ReinforcmentHandler
    {
        private void define_side_U(LineSegment seg)
        {
            G.Line mainLine = seg.getLine();
            G.Line otherLine = seg.getOtherLine();

            if (setLineSegment.Contains(seg)) return;
            if (isLineRight(mainLine) == false && isLineRight(otherLine) == true) return;
            
            G.Line offsetLine = mainLine.Offset(_V_.X_CONCRETE_COVER_1);
            G.Line otherOffsetline = otherLine.Offset(_V_.X_CONCRETE_COVER_1);

            G.Vector d1 = mainLine.getDirectionVector();
            G.Vector o1 = mainLine.getOffsetVector();
            int spacing = _V_.X_REINFORCEMENT_STIRRUP_SPACING;

            double nearEdge = _V_.X_CONCRETE_COVER_1 * 2;
            double equalSpacer = ((mainLine.Length() - 2 * nearEdge) % spacing) / 2;

            double j = nearEdge + equalSpacer;
            double len = mainLine.Length() - nearEdge;

            if ((mainLine.Length() - nearEdge * 2) > spacing)
            {
                R.Raud_Array rauad = new R.Raud_Array(spacing);
                knownArrayReinforcement.Add(rauad);

                while (j < len)
                {
                    G.Point start = offsetLine.Start.move(j, d1);

                    G.Point extended = start.move(_V_.Y_STIRRUP_MAX_LENGTH * 2000, o1);
                    G.Line temp = new G.Line(start, extended);

                    G.Point end = G.Line.getIntersectionPoint(temp, otherOffsetline);

                    U_side_handler(start, end, seg);

                    j = j + spacing;
                }

                U_side_garbage_collector();
            }
        }
    }
}