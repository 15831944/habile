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
        //DEBUG
        private bool A_handler_debug(G.Point mainPoint, G.Point mainEnd)
        {
            G.Line main = new G.Line(mainPoint, mainEnd);

            reinf_geometry_debug.Add(main);

            R.A_Raud reinf = new R.A_Raud(main, _V_.X_REINFORCEMENT_NUMBER, _V_.X_REINFORCEMENT_MAIN_DIAMETER, _V_.X_REINFORCEMENT_MARK);
            keep(reinf, null, null, null);

            return true;
        }


        //A HANDLE
        public bool A_handler(G.Point mainPoint, G.Point mainEnd, G.Edge e, G.Corner c, int d)
        {
            G.Line main = new G.Line(mainPoint, mainEnd);

            //reinf_geometry_debug.Add(main);

            R.A_Raud reinf = new R.A_Raud(main, _V_.X_REINFORCEMENT_NUMBER, d, _V_.X_REINFORCEMENT_MARK);
            if (denier(reinf.makeLine())) return false;
            keep(reinf, e, c, null);

            return true;
        }


        private bool A_handler_replace(R.A_Raud a, R.A_Raud b)
        {
            R.A_Raud new_reinf = R.A_Raud.mergeTwoRebar(a, b);
            if (denier(new_reinf.makeLine())) return false;
            keep_replace(new_reinf, a, b);

            return true;
        }


        private void A_remover(R.A_Raud a)
        {
            keep_remove(a);
        }


        //B HANDLE
        private bool B_vs_C_handler(G.Point mainPoint, G.Point mainEnd, G.Point sideStart, G.Edge e, G.Corner oc)
        {
            G.Line main = new G.Line(mainPoint, mainEnd);
            G.Line side = new G.Line(sideStart, mainPoint);
            
            G.Line temp1 = trimLine_basepoint(main, mainPoint);
            if (temp1.Length() > main.Length() * _V_.M_TRIM_TOLERANCE) main = temp1.Copy();
            if (denier(main)) return false;

            G.Line temp2 = trimLine_basepoint(side, mainPoint);
            if (temp2.Length() > side.Length() * _V_.M_TRIM_TOLERANCE) side = temp2.Copy();
            if (denier(side)) return false;

            if (main.Length() < _V_.Y_REINFORCEMENT_MAIN_RADIUS) return false;
            if (side.Length() < _V_.Y_REINFORCEMENT_MAIN_RADIUS) return false;

            G.Vector v1 = main.getDirectionVector();
            G.Vector v2 = side.getDirectionVector();

            double ang = G.Converter.AngleDeltaCW(v1, v2);
            
            if (Math.Abs(ang - 3 * Math.PI / 2) < _V_.M_B_BAR_TOLERANCE)
            {
                R.B_Raud reinf = new R.B_Raud(main, side, _V_.X_REINFORCEMENT_NUMBER, _V_.X_REINFORCEMENT_MAIN_DIAMETER, _V_.X_REINFORCEMENT_MARK);
                keep(reinf, e, oc, null);
            }
            else
            {
                R.C_Raud reinf = new R.C_Raud(main, side, _V_.X_REINFORCEMENT_NUMBER, _V_.X_REINFORCEMENT_MAIN_DIAMETER, _V_.X_REINFORCEMENT_MARK);
                keep(reinf, e, oc, null);
            }

            return true;
        }


        private bool B_handler_replace(R.B_Raud a, R.B_Raud b)
        {
            R.D_Raud new_reinf_long = R.B_Raud.mergeTwoRebar_long(a, b);

            bool longD = true;
            if (denier(new_reinf_long.makeMainLine())) longD = false;
            if (denier(new_reinf_long.makeSide1Line())) longD = false;
            if (denier(new_reinf_long.makeSide2Line())) longD = false;

            if (new_reinf_long.A < _V_.Y_REINFORCEMENT_MAIN_RADIUS) longD = false;
            if (new_reinf_long.B2 < _V_.Y_REINFORCEMENT_MAIN_RADIUS * 1.99) longD = false;
            if (new_reinf_long.C < _V_.Y_REINFORCEMENT_MAIN_RADIUS) longD = false;
            if (new_reinf_long.Length > _V_.X_REINFORCEMENT_MAIN_MAX_D_LENGTH) longD = false;

            if (longD == true)
            {
                keep_replace(new_reinf_long, a, b);
                return true;
            }

            R.D_Raud new_reinf_normal = R.B_Raud.mergeTwoRebar(a, b);

            if (denier(new_reinf_normal.makeMainLine())) return false;
            if (denier(new_reinf_normal.makeSide1Line())) return false;
            if (denier(new_reinf_normal.makeSide2Line())) return false;

            if (new_reinf_normal.A < _V_.Y_REINFORCEMENT_MAIN_RADIUS) return false;
            if (new_reinf_normal.B2 < _V_.Y_REINFORCEMENT_MAIN_RADIUS * 1.99) return false;
            if (new_reinf_normal.C < _V_.Y_REINFORCEMENT_MAIN_RADIUS) return false;
            if (new_reinf_long.Length > _V_.X_REINFORCEMENT_MAIN_MAX_D_LENGTH) return false;

            keep_replace(new_reinf_normal, a, b);

            return true;
        }


        private bool C_handler_replace(R.C_Raud a, R.C_Raud b)
        {
            R.D_Raud new_reinf_long = R.C_Raud.mergeTwoRebar_long(a, b);

            bool longD = true;
            if (denier(new_reinf_long.makeMainLine())) longD = false;
            if (denier(new_reinf_long.makeSide1Line())) longD = false;
            if (denier(new_reinf_long.makeSide2Line())) longD = false;

            if (new_reinf_long.A < _V_.Y_REINFORCEMENT_MAIN_RADIUS) longD = false;
            if (new_reinf_long.B2 < _V_.Y_REINFORCEMENT_MAIN_RADIUS * 1.99) longD = false;
            if (new_reinf_long.C < _V_.Y_REINFORCEMENT_MAIN_RADIUS) longD = false;
            if (new_reinf_long.Length > _V_.X_REINFORCEMENT_MAIN_MAX_D_LENGTH) longD = false;

            if (longD == true)
            {
                keep_replace(new_reinf_long, a, b);
                return true;
            }

            R.E_Raud new_reinf = R.C_Raud.mergeTwoRebar(a, b);

            if (denier(new_reinf.makeMainLine())) return false;
            if (denier(new_reinf.makeSide1Line())) return false;
            if (denier(new_reinf.makeSide2Line())) return false;

            if (new_reinf.A < _V_.Y_REINFORCEMENT_MAIN_RADIUS) return false;
            if (new_reinf.B2 < _V_.Y_REINFORCEMENT_MAIN_RADIUS * 1.99) return false;
            if (new_reinf.C < _V_.Y_REINFORCEMENT_MAIN_RADIUS) return false;
            if (new_reinf_long.Length > _V_.X_REINFORCEMENT_MAIN_MAX_D_LENGTH) return false;

            keep_replace(new_reinf, a, b);

            return true;
        }


        //D HANDLE
        private bool D_vs_E_handler(G.Point mainPoint, G.Point mainEnd, G.Point side1Start, G.Point side2End, G.Edge e, G.Corner c1, G.Corner c2, int parand, G.Edge other = null, G.Edge other2 = null)
        {
            G.Line main = new G.Line(mainPoint, mainEnd);
            G.Line side1 = new G.Line(side1Start, mainPoint);
            G.Line side2 = new G.Line(mainEnd, side2End);

            if (main.Length() < _V_.Y_REINFORCEMENT_MAIN_RADIUS * 1.99) return false;

            //reinf_geometry_debug.Add(main);
            //reinf_geometry_debug.Add(side1);
            //reinf_geometry_debug.Add(side2);

            G.Vector v1 = main.getDirectionVector();
            G.Vector v2 = side1.getDirectionVector();
            G.Vector v3 = side2.getDirectionVector();

            double ang1 = G.Converter.AngleDeltaCW(v1, v2);
            double ang2 = G.Converter.AngleDeltaCW(v1, v3);

            bool d1 = Math.Abs(ang1 - 3 * Math.PI / 2) < _V_.M_B_BAR_TOLERANCE;
            bool d2 = Math.Abs(ang2 - Math.PI / 2) < _V_.M_B_BAR_TOLERANCE;
            if (d1 && d2)
            {
                R.D_Raud reinf = new R.D_Raud(main, side1, side2, _V_.X_REINFORCEMENT_NUMBER, _V_.X_REINFORCEMENT_MAIN_DIAMETER, _V_.X_REINFORCEMENT_MARK, parand);

                if (denier(reinf.makeMainLine())) return false;
                if (denier(reinf.makeSide1Line())) return false;
                if (denier(reinf.makeSide2Line())) return false;

                keep(reinf, e, c1, c2);
                keep_double(reinf, other);
                keep_double(reinf, other2);
            }
            else
            {
                R.E_Raud reinf = new R.E_Raud(main, side1, side2, _V_.X_REINFORCEMENT_NUMBER, _V_.X_REINFORCEMENT_MAIN_DIAMETER, _V_.X_REINFORCEMENT_MARK, parand);

                if (denier(reinf.makeMainLine())) return false;
                if (denier(reinf.makeSide1Line())) return false;
                if (denier(reinf.makeSide2Line())) return false;

                keep(reinf, e, c1, c2);
                keep_double(reinf, other);
                keep_double(reinf, other2);
            }

            return true;
        }


        //definer
        private bool D_side_handler(G.Point start, G.Point end, int parand)
        {
            G.Line side = new G.Line(start, end);

            if (denier(side)) return false;

            //reinf_geometry_debug.Add(side);

            G.Vector o1 = side.getDirectionVector();
            double absX = Math.Abs(o1.X);
            double absY = Math.Abs(o1.Y);
            G.Vector absV = new G.Vector(absX, absY);
            G.Polar p = G.Converter.xy_to_la(absV);

            R.D_Raud cur;
            if (p.angle < Math.PI / 4)
            {
                if (_V_.X_REINFORCEMENT_SIDE_D_CREATE == 1)
                {
                    cur = new R.D_Raud(side, _V_.Y_ELEMENT_WIDTH_COVER + parand, 1, _V_.X_REINFORCEMENT_SIDE_D_DIAMETER, _V_.X_REINFORCEMENT_MARK);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (_V_.X_REINFORCEMENT_TOP_D_CREATE == 1)
                {
                    cur = new R.D_Raud(side, _V_.Y_ELEMENT_WIDTH_COVER + parand, 1, _V_.X_REINFORCEMENT_TOP_D_DIAMETER, _V_.X_REINFORCEMENT_MARK);
                }
                else
                {
                    return false;
                }
            }

            int currentIndex = knownArrayReinforcement.Count - 1;
            knownArrayReinforcement[currentIndex].add_one(cur);
            keep_array(cur, null);

            return true;
        }


        private void D_side_garbage_collector()
        {
            int currentIndex = knownArrayReinforcement.Count - 1;

            if (knownArrayReinforcement[currentIndex].array.Count == 0)
            {
                knownArrayReinforcement.RemoveAt(currentIndex);
            }
        }


        private bool U_side_handler(G.Point start, G.Point end, LineSegment ls)
        {
            G.Line side = new G.Line(start, end);
            if (side.Length() < _V_.Y_REINFORCEMENT_STIRRUP_RADIUS * 2) return false;

            if (denier(side)) return false;

            //reinf_geometry_debug.Add(side);

            R.U_Raud cur = new R.U_Raud(side, _V_.Y_ELEMENT_WIDTH_COVER, 1, _V_.X_REINFORCEMENT_STIRRUP_DIAMETER, _V_.X_REINFORCEMENT_MARK);

            int currentIndex = knownArrayReinforcement.Count - 1;
            knownArrayReinforcement[currentIndex].add_one(cur);
            keep_array(cur, ls);

            return true;
        }


        private void U_side_garbage_collector()
        {
            int currentIndex = knownArrayReinforcement.Count - 1;

            if (knownArrayReinforcement[currentIndex].array.Count == 0)
            {
                knownArrayReinforcement.RemoveAt(currentIndex);
            }
        }

    }
}