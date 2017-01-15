using System;
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
        private void define_side_D(LineSegment seg)
        {
            G.Line ln = seg.getLine();
            G.Line offsetLine = ln.Offset(_V_.X_CONCRETE_COVER_1);

            G.Vector d1 = ln.getDirectionVector();
            G.Vector o1 = ln.getOffsetVector();

            double absX = Math.Abs(o1.X);
            double absY = Math.Abs(o1.Y);
            G.Vector absV = new G.Vector(absX, absY);
            G.Polar p = G.Converter.xy_to_la(absV);

            int spacing;
            int distance;
            int parand; // parand magic
            if (p.angle < Math.PI / 4)
            {
                spacing = _V_.X_REINFORCEMENT_SIDE_D_SPACING;
                distance = _V_.X_REINFORCEMENT_SIDE_D_ANCHOR_LENGTH;
                parand = _V_.X_REINFORCEMENT_SIDE_D_FIX;
            }
            else
            {
                spacing = _V_.X_REINFORCEMENT_TOP_D_SPACING;
                distance = _V_.X_REINFORCEMENT_TOP_D_ANCHOR_LENGTH;
                parand = _V_.X_REINFORCEMENT_TOP_D_FIX;
            }

            double lineTrim = _V_.X_CONCRETE_COVER_1 * 2.5;
            double equalSpacer = ((ln.Length() - 2 * lineTrim) % spacing) / 2;

            double j = lineTrim + equalSpacer;
            double len = ln.Length();

            if (len > spacing)
            {
                R.Raud_Array rauad = new R.Raud_Array(spacing);
                knownArrayReinforcement.Add(rauad);

                while (j < len)
                {
                    G.Point start = offsetLine.Start.move(j, d1);
                    G.Point end = start.move(distance, o1);

                    D_side_handler(start, end, parand); // parand magic
                    j = j + spacing;
                }

                D_side_garbage_collector();
            }
        }
    }        
}