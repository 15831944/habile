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
        G.Region r;

        List<G.Edge> allEdges;
        List<G.Corner> allCorners;

        List<G.Edge> emptyEdges;
        List<G.Corner> emptyCorners;

        Dictionary<G.Edge, R.Raud> setEdges;
        Dictionary<G.Corner, R.Raud> setCorners;
        List<LineSegment> setLineSegment;

        List<R.Raud> knownReinforcement;
        List<R.Raud_Array> knownArrayReinforcement;
        List<R.Raud> knownUniqueReinforcement;
        List<G.Line> reinf_geometry_debug;

        public ReinforcmentHandler(List<G.Line> polys)
        {
            r = new G.Region(polys);

            allEdges = new List<G.Edge>(r.edges);
            allCorners = new List<G.Corner>(r.corners);

            setEdges = new Dictionary<G.Edge, R.Raud>();
            setCorners = new Dictionary<G.Corner, R.Raud>();
            setLineSegment = new List<LineSegment>();

            emptyEdges = new List<G.Edge>();
            emptyCorners = new List<G.Corner>();

            knownReinforcement = new List<R.Raud>();
            knownArrayReinforcement = new List<R.Raud_Array>();
            knownUniqueReinforcement = new List<R.Raud>();
            reinf_geometry_debug = new List<G.Line>();
        }

        public void main(ref List<R.Raud> reinf, ref List<R.Raud_Array> reinf_array, ref List<R.Raud> unique_reinf)
        {
            setCalculatedParameters();

            create_all_main_reinforcement();
            create_all_side_reinforcement();

            //Drawing_Box visu2 = new Drawing_Box(r, reinf_geometry_debug);
            //visu2.Show();

            reinf = knownReinforcement;
            reinf_array = knownArrayReinforcement;
            unique_reinf = knownUniqueReinforcement;

            //List<G.Edge> emptyEdgesDebug = allEdges.Where(x => !setEdges.Keys.Contains(x)).ToList();
            //List<G.Corner> emptyCornersDebug = allCorners.Where(x => !setCorners.Keys.Contains(x)).ToList();
            //List<G.Edge> setEdgesDebug = allEdges.Where(x => setEdges.Keys.Contains(x)).ToList();
            //List<G.Corner> setCornerDebug = allCorners.Where(x => setCorners.Keys.Contains(x)).ToList();
            //Drawing_Box visu1 = new Drawing_Box(r, emptyEdgesDebug, emptyCornersDebug, setEdgesDebug, setCornerDebug, reinf_geometry_debug);
            //visu1.Show();
        }

        private void setCalculatedParameters()
        {
            _V_.Y_REINFORCEMENT_MAIN_MIN_LENGTH = _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 2 * 1.2; // A + 2B vs D

            _V_.Y_STIRRUP_MAX_LENGTH = _V_.X_REINFORCEMENT_STIRRUP_CONSTRAINT * _V_.X_ELEMENT_WIDTH;
            _V_.Y_ELEMENT_WIDTH_COVER = _V_.X_ELEMENT_WIDTH - _V_.X_CONCRETE_COVER_1 * 2;

            _V_.Y_CONCRETE_COVER_DELTA = (int)(Math.Ceiling(_V_.X_REINFORCEMENT_MAIN_DIAMETER / 5.0 + 0.01) * 5) + 5;
            _V_.Y_CONCRETE_COVER_2 = _V_.X_CONCRETE_COVER_1 + _V_.Y_CONCRETE_COVER_DELTA;
            _V_.Y_CONCRETE_COVER_3 = _V_.Y_CONCRETE_COVER_2 + _V_.Y_CONCRETE_COVER_DELTA;
        }

        private void create_all_side_reinforcement()
        {
            if (_V_.X_REINFORCEMENT_NUMBER > 1)
            {
                for (int i = allEdges.Count - 1; i >= 0; i--)
                {
                    G.Edge e = allEdges[i];

                    List<LineSegment> segments = line_segmentator(e);

                    foreach (LineSegment cur in segments)
                    {
                        if (cur.hasOtherEdge())
                        {
                            define_side_U(cur);
                        }
                        else
                        {
                            define_side_D(cur);
                        }
                    }
                }
            }
        }

        private void create_all_main_reinforcement()
        {
            create_all_A();
            merge_A();
            remove_short_A();

            create_valid_D();
            create_extended_B(); // Corner > Math.Pi
            merge_B();
            merge_C();

            trimmed_long_A();

            create_valid_D();
            create_extended_B(); // Corner > Math.Pi
            create_long_B();
            merge_A();
            merge_B();
            merge_C();

            trimmed_short_A();

            create_valid_B();
            create_diagonal_A();

            merge_A();
            merge_B();
            merge_C();
        }

        private void create_all_A()
        {
            emptyEdges = allEdges.Where(x => !setEdges.Keys.Contains(x)).ToList();

            for (int i = emptyEdges.Count - 1; i >= 0; i--)
            {
                G.Edge e = emptyEdges[i];
                G.Line main = e.edgeOffset(_V_.X_CONCRETE_COVER_1, _V_.X_CONCRETE_COVER_1, _V_.X_CONCRETE_COVER_1);

                double c1 = _V_.Y_REINFORCEMENT_MAIN_MIN_LENGTH;
                bool c2 = e.StartCorner.Angle > Math.PI;
                bool c3 = e.EndCorner.Angle > Math.PI;

                if (c2) main = main.extendStart(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH);
                if (c3) main = main.extendEnd(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH);

                A_handler(main.Start, main.End, e, null, _V_.X_REINFORCEMENT_MAIN_DIAMETER);
            }
        }

        private void create_valid_D()
        {
            emptyEdges = allEdges.Where(x => !setEdges.Keys.Contains(x)).ToList();
            emptyCorners = allCorners.Where(x => !setCorners.Keys.Contains(x)).ToList();

            for (int i = emptyEdges.Count - 1; i >= 0; i--)
            {
                G.Edge e = emptyEdges[i];
                G.Corner sc = e.StartCorner;
                G.Corner ec = e.EndCorner;

                G.Edge side1Edge = sc.getOtherEdge(e);
                G.Edge side2Edge = ec.getOtherEdge(e);

                bool c1 = emptyCorners.Contains(sc);
                bool c2 = emptyCorners.Contains(ec);
                bool c3 = sc.Angle < Math.PI;
                bool c4 = ec.Angle < Math.PI;
                bool c5 = setEdges.Keys.Contains(side1Edge);
                bool c6 = setEdges.Keys.Contains(side2Edge);

                if (c1 && c2 && c3 && c4 && c5 && c6)
                {
                    G.Edge side1 = sc.getOtherEdge(e);
                    G.Edge side2 = ec.getOtherEdge(e);

                    define_D(e, side1, side2);
                }
            }
        }

        private void create_valid_B()
        {
            emptyEdges = allEdges.Where(x => !setEdges.Keys.Contains(x)).ToList();
            emptyCorners = allCorners.Where(x => !setCorners.Keys.Contains(x)).ToList();

            for (int i = emptyCorners.Count - 1; i >= 0; i--)
            {
                G.Corner ec = emptyCorners[i];
                G.Edge se = ec.StartEdge;
                G.Edge ee = ec.EndEdge;

                bool c1 = setEdges.Keys.Contains(se) && setEdges.Keys.Contains(ee);
                bool c2 = ec.Angle < Math.PI;

                if (c1 && c2)
                {
                    define_B(se, ee);
                }
            }
        }

        private void create_extended_B()
        {
            emptyEdges = allEdges.Where(x => !setEdges.Keys.Contains(x)).ToList();
            emptyCorners = allCorners.Where(x => !setCorners.Keys.Contains(x)).ToList();

            for (int i = emptyEdges.Count - 1; i >= 0; i--)
            {
                G.Edge e = emptyEdges[i];
                G.Corner sc = e.StartCorner;
                G.Corner ec = e.EndCorner;

                bool c1 = sc.Angle > Math.PI;
                bool c2 = ec.Angle > Math.PI;

                if (c1 && c2) continue;
                if (!c1 && !c2) continue;

                if (c1) //startCorner >> math.pi
                {
                    G.Edge otherEdge = ec.getOtherEdge(e);
                    define_B(otherEdge, e);
                }
                else if (c2)//endCorner >> math.pi
                {
                    G.Edge otherEdge = sc.getOtherEdge(e);
                    define_B(e, otherEdge);
                }
            }
        }

        private void create_long_B()
        {
            emptyEdges = allEdges.Where(x => !setEdges.Keys.Contains(x)).ToList();
            emptyCorners = allCorners.Where(x => !setCorners.Keys.Contains(x)).ToList();

            for (int i = emptyEdges.Count - 1; i >= 0; i--)
            {
                G.Edge e = emptyEdges[i];
                G.Corner sc = e.StartCorner;
                G.Corner ec = e.EndCorner;

                bool c1 = setCorners.Keys.Contains(sc);
                bool c2 = setCorners.Keys.Contains(ec);

                if (c1 && c2) continue;
                if (sc.Angle > Math.PI) continue;
                if (ec.Angle > Math.PI) continue;

                if (!c1) //startCorner
                {
                    G.Edge otherEdge = sc.getOtherEdge(e);
                    define_B(e, otherEdge);
                }

                else if (!c2) //endCorner
                {
                    G.Edge otherEdge = ec.getOtherEdge(e);
                    define_B(otherEdge, e);
                }
            }
        }

        private void trimmed_long_A()
        {
            emptyEdges = allEdges.Where(x => !setEdges.Keys.Contains(x)).ToList();
            emptyCorners = allCorners.Where(x => !setCorners.Keys.Contains(x)).ToList();

            for (int i = emptyEdges.Count - 1; i >= 0; i--)
            {
                G.Edge e = emptyEdges[i];

                double c1 = _V_.Y_REINFORCEMENT_MAIN_MIN_LENGTH;
                bool c2 = e.StartCorner.Angle > Math.PI;
                bool c3 = e.EndCorner.Angle > Math.PI;

                G.Line main = e.edgeOffset(_V_.X_CONCRETE_COVER_1, _V_.X_CONCRETE_COVER_1, _V_.X_CONCRETE_COVER_1);

                bool startTrimmed = false;
                G.Edge startTrimmerEdge = null;
                if (c2)
                {
                    G.Line extended = main.extendStart(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH);
                    G.Line trimmed = trimLine_baseline(extended, main, _V_.Y_CONCRETE_COVER_2, ref startTrimmerEdge);
                    if (trimmed.Length() < main.Length() + _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 0.95) startTrimmed = true;
                    main = trimmed;
                }

                bool endTrimmed = false;
                G.Edge endTrimmerEdge = null;
                if (c3)
                {
                    G.Line extended = main.extendEnd(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH);
                    G.Line trimmed = trimLine_baseline(extended, main, _V_.Y_CONCRETE_COVER_2, ref endTrimmerEdge);
                    if (trimmed.Length() < main.Length() + _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 0.95) endTrimmed = true;
                    main = trimmed;
                }

                if (main.Length() > c1)
                {
                    A_handler(main.Start, main.End, e, null, _V_.X_REINFORCEMENT_MAIN_DIAMETER);

                    if (startTrimmed)
                    {
                        bool got_B = define_B(e, startTrimmerEdge);

                        if (got_B == false)
                        {
                            G.Vector v1 = e.Line.getOffsetVector();
                            G.Vector v2 = startTrimmerEdge.Line.getCoolVector(v1);

                            G.Point AP = main.Start.move(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 1.5, v2);
                            G.Line b_line = new G.Line(main.Start, AP);

                            G.Edge sideTrimmerEdge = null;
                            G.Line newMain = trimLine_basepoint(b_line, main.Start, _V_.Y_CONCRETE_COVER_2, ref sideTrimmerEdge);

                            define_D(startTrimmerEdge, sideTrimmerEdge, e);
                        }
                    }

                    if (endTrimmed)
                    {
                        bool got_B = define_B(endTrimmerEdge, e);

                        if (got_B == false)
                        {
                            G.Vector v1 = e.Line.getOffsetVector();
                            G.Vector v2 = endTrimmerEdge.Line.getCoolVector(v1);

                            G.Point AP = main.End.move(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 1.5, v2);
                            G.Line b_line = new G.Line(main.End, AP);

                            G.Edge sideTrimmerEdge = null;
                            G.Line newMain = trimLine_basepoint(b_line, main.End, _V_.Y_CONCRETE_COVER_2, ref sideTrimmerEdge);

                            define_D(endTrimmerEdge, e, sideTrimmerEdge);
                        }
                    }
                }
            }
        }

        private void trimmed_short_A()
        {
            emptyEdges = allEdges.Where(x => !setEdges.Keys.Contains(x)).ToList();
            emptyCorners = allCorners.Where(x => !setCorners.Keys.Contains(x)).ToList();

            for (int i = emptyEdges.Count - 1; i >= 0; i--)
            {
                G.Edge e = emptyEdges[i];

                double c1 = _V_.Y_REINFORCEMENT_MAIN_MIN_LENGTH;
                bool c2 = e.StartCorner.Angle > Math.PI;
                bool c3 = e.EndCorner.Angle > Math.PI;

                G.Line main = e.edgeOffset(_V_.X_CONCRETE_COVER_1, _V_.X_CONCRETE_COVER_1, _V_.X_CONCRETE_COVER_1);

                bool startTrimmed = false;
                G.Edge startTrimmerEdge = null;
                if (c2)
                {
                    G.Line extended = main.extendStart(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH);
                    G.Line trimmed = trimLine_basepoint(extended, main.End, _V_.Y_CONCRETE_COVER_2, ref startTrimmerEdge);
                    if (trimmed.Length() < main.Length() + _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 0.95) startTrimmed = true;
                    main = trimmed;
                }

                bool endTrimmed = false;
                G.Edge endTrimmerEdge = null;
                if (c3)
                {
                    G.Line extended = main.extendEnd(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH);
                    G.Line trimmed = trimLine_basepoint(extended, main.Start, _V_.Y_CONCRETE_COVER_2, ref endTrimmerEdge);
                    if (trimmed.Length() < main.Length() + _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 0.95) endTrimmed = true;
                    main = trimmed;
                }
                
                if (main.Length() <= c1)
                {
                    if (startTrimmed && endTrimmed)
                    {
                        bool got_D = define_D(e, startTrimmerEdge, endTrimmerEdge);

                        if (got_D == false)
                        {
                            G.Vector v1 = main.getOffsetVector();
                            G.Vector v2 = startTrimmerEdge.Line.getCoolVector(v1);
                            G.Vector v3 = endTrimmerEdge.Line.getCoolVector(v1);

                            G.Point side1Point = main.Start.move(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 1.05, v2);
                            G.Point side2Point = main.End.move(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH, v3);

                            G.Line side1 = new G.Line(side1Point, main.Start);
                            G.Line side2 = new G.Line(main.End, side2Point);

                            if (denier(side1) && denier(side2))
                            {
                                G.Point AP = main.Start.move(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 1.5, v2);
                                G.Line b_line = new G.Line(main.Start, AP);

                                G.Edge side1TrimmerEdge = null;
                                G.Line side1Main = trimLine_basepoint(b_line, main.Start, _V_.Y_CONCRETE_COVER_2, ref side1TrimmerEdge);

                                G.Point AP2 = main.End.move(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 1.5, v3);
                                G.Line b_line2 = new G.Line(main.End, AP2);

                                G.Edge side2TrimmerEdge = null;
                                G.Line side2Main = trimLine_basepoint(b_line2, main.End, _V_.Y_CONCRETE_COVER_2, ref side2TrimmerEdge);

                                A_handler(main.Start, main.End, e, null, _V_.X_REINFORCEMENT_MAIN_DIAMETER);
                                define_D(startTrimmerEdge, side1TrimmerEdge, e);
                                define_D(endTrimmerEdge, e, side2TrimmerEdge);
                            }
                            else if (denier(side1))
                            {
                                G.Point AP = main.Start.move(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 1.5, v2);
                                G.Line b_line = new G.Line(main.Start, AP);

                                G.Edge sideTrimmerEdge = null;
                                G.Line newMain = trimLine_basepoint(b_line, main.Start, _V_.Y_CONCRETE_COVER_2, ref sideTrimmerEdge);
                                
                                B_vs_C_handler(main.End, side2Point, main.Start, e, null);
                                define_D(startTrimmerEdge, sideTrimmerEdge, e);
                            }
                            else if (denier(side2))
                            {
                                G.Point AP = main.End.move(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 1.5, v2);
                                G.Line b_line = new G.Line(main.End, AP);

                                G.Edge sideTrimmerEdge = null;
                                G.Line newMain = trimLine_basepoint(b_line, main.End, _V_.Y_CONCRETE_COVER_2, ref sideTrimmerEdge);

                                define_D(endTrimmerEdge, e, sideTrimmerEdge);
                                B_vs_C_handler(main.Start, main.End, side1Point, e, null);
                            }
                        }
                    }
                    else if (startTrimmed)
                    {
                        bool got_B = define_B(e, startTrimmerEdge);

                        if (got_B == false)
                        {
                            G.Vector v1 = e.Line.getOffsetVector();
                            G.Vector v2 = startTrimmerEdge.Line.getCoolVector(v1);

                            G.Point AP = main.Start.move(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 1.5, v2);
                            G.Line b_line = new G.Line(main.Start, AP);

                            G.Edge sideTrimmerEdge = null;
                            G.Line newMain = trimLine_basepoint(b_line, main.Start, _V_.Y_CONCRETE_COVER_2, ref sideTrimmerEdge);

                            A_handler(main.Start, main.End, e, null, _V_.X_REINFORCEMENT_MAIN_DIAMETER);
                            define_D(startTrimmerEdge, sideTrimmerEdge, e);
                        }
                    }
                    else if (endTrimmed)
                    {
                        bool got_B = define_B(endTrimmerEdge, e);

                        if (got_B == false)
                        {
                            G.Vector v1 = e.Line.getOffsetVector();
                            G.Vector v2 = endTrimmerEdge.Line.getCoolVector(v1);

                            G.Point AP = main.End.move(_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH * 1.5, v2);
                            G.Line b_line = new G.Line(main.End, AP);

                            G.Edge sideTrimmerEdge = null;
                            G.Line newMain = trimLine_basepoint(b_line, main.End, _V_.Y_CONCRETE_COVER_2, ref sideTrimmerEdge);

                            A_handler(main.Start, main.End, e, null, _V_.X_REINFORCEMENT_MAIN_DIAMETER);
                            define_D(endTrimmerEdge, e, sideTrimmerEdge);
                        }
                    }
                    else
                    {
                        A_handler(main.Start, main.End, e, null, _V_.X_REINFORCEMENT_MAIN_DIAMETER);
                    }
                }
            }
        }

        private void create_diagonal_A()
        {
            for (int i = emptyCorners.Count - 1; i >= 0; i--)
            {
                G.Corner ec = emptyCorners[i];
                G.Edge se = ec.StartEdge;
                G.Edge ee = ec.EndEdge;

                bool c1 = setEdges.Keys.Contains(se) && setEdges.Keys.Contains(ee);
                bool c2 = ec.Angle > Math.PI;

                if (c1)
                {
                    if (c2)
                    {
                        G.Vector v1 = se.Line.getOffsetVector();
                        G.Vector v2 = ee.Line.getOffsetVector();
                        G.Vector v3 = (v1 + v2).rotate(Math.PI / 2);
                        G.Point centerPoint = ec.getCornerPoint(se, _V_.X_CONCRETE_COVER_1, _V_.X_CONCRETE_COVER_1);
                        G.Line tester = new G.Line(centerPoint, _V_.X_REINFORCEMENT_DIAGONAL_ANCHOR_LENGTH, v3);

                        if (denier(tester))
                        {
                            diagonalRotater(centerPoint, v3, 1, 1, ref tester);
                        }

                        A_handler(tester.Start, tester.End, null, ec, _V_.X_REINFORCEMENT_DIAGONAL_DIAMETER);
                    }
                }
            }
        }

        private void remove_short_A()
        {
            double c1 = _V_.Y_REINFORCEMENT_MAIN_MIN_LENGTH;

            List<R.Raud> onlyA = knownReinforcement.Where(x => x is R.A_Raud).ToList();
            for (int i = onlyA.Count - 1; i >= 0; i--)
            {
                R.A_Raud a = knownReinforcement[i] as R.A_Raud;

                if (a.Length < c1)
                {
                    A_remover(a);
                }
            }
        }

        private void merge_A() // TODO AUTOMATICALLY AFTER EVERY ADDING
        {
            bool restartLoop = true;

            while (restartLoop)
            {
                restartLoop = false;

                List<R.Raud> onlyA = knownReinforcement.Where(x => x is R.A_Raud).ToList();

                foreach (R.A_Raud a in onlyA)
                {
                    List<R.Raud> not = onlyA.Where(x => !ReferenceEquals(a, x)).ToList();
                    List<R.Raud> same = not.Where(x => a.Diameter == x.Diameter).ToList();
                    List<R.Raud> colinear = same.Where(x => G.Line.areLinesCoLinear(a.makeLine(), (x as R.A_Raud).makeLine(), 3)).ToList();

                    if (colinear.Count > 0)
                    {
                        A_handler_replace(a, colinear[0] as R.A_Raud);

                        restartLoop = true;
                        break;
                    }
                }
            }
        }

        private void merge_B() // TODO AUTOMATICALLY AFTER EVERY ADDING
        {
            bool restartLoop = true;

            while (restartLoop)
            {
                restartLoop = false;

                List<R.Raud> onlyB = knownReinforcement.Where(x => x is R.B_Raud).ToList();

                foreach (R.B_Raud a in onlyB)
                {
                    List<R.Raud> not = onlyB.Where(x => !ReferenceEquals(a, x)).ToList();
                    List<R.Raud> same = not.Where(x => a.Diameter == x.Diameter).ToList();
                    List<R.Raud> colinear = same.Where(x => G.Line.areLinesCoLinear(a.makeMainLine(), (x as R.B_Raud).makeSideLine(), 3)).ToList();

                    if (colinear.Count > 0)
                    {
                        B_handler_replace(a, colinear[0] as R.B_Raud);

                        restartLoop = true;
                        break;
                    }
                }
            }
        }

        private void merge_C() // TODO AUTOMATICALLY AFTER EVERY ADDING
        {
            bool restartLoop = true;

            while (restartLoop)
            {
                restartLoop = false;

                List<R.Raud> onlyC = knownReinforcement.Where(x => x is R.C_Raud).ToList();

                foreach (R.C_Raud a in onlyC)
                {
                    List<R.Raud> not = onlyC.Where(x => !ReferenceEquals(a, x)).ToList();
                    List<R.Raud> same = not.Where(x => a.Diameter == x.Diameter).ToList();
                    List<R.Raud> colinear = same.Where(x => G.Line.areLinesCoLinear(a.makeMainLine(), (x as R.C_Raud).makeSideLine(), 3)).ToList();

                    if (colinear.Count > 0)
                    {
                        C_handler_replace(a, colinear[0] as R.C_Raud);

                        restartLoop = true;
                        break;
                    }
                }
            }
        }

    }
}