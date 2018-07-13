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

        private bool denier(G.Line final)
        {
            foreach (G.Edge eg in allEdges)
            {
                double o = _V_.X_CONCRETE_COVER_1 - _V_.X_DENIER_MINIMUM_DELTA;
                G.Line offsetLine = eg.edgeOffset(o, o, o);
                if (G.Line.hasIntersection(final, offsetLine))
                {
                    return true;
                }
            }
            return false;
        }


        private bool denier(G.Line final, G.Edge e)
        {            
            foreach (G.Edge eg in allEdges)
            {
                if (eg == e) continue;
                
                double o = _V_.X_CONCRETE_COVER_1 - _V_.X_DENIER_MINIMUM_DELTA;
                G.Line offsetLine = eg.edgeOffset(o, o, o);
                
                if (G.Line.hasIntersection(final, offsetLine))
                {
                    return true;
                }
            }
            return false;
        }


        private bool narrow_denier(G.Edge e)
        {
            G.Line main = e.Line.Copy();

            G.Vector v1 = main.getOffsetVector();
            G.Point cp = main.getCenterPoint();
            G.Point ep1 = main.Start.move(_V_.X_CONCRETE_COVER_1 * 2, v1);
            G.Point ep2 = cp.move(_V_.X_CONCRETE_COVER_1 * 2, v1);
            G.Point ep3 = main.End.move(_V_.X_CONCRETE_COVER_1 * 2, v1);
            G.Line testLine1 = new G.Line(main.Start, ep1);
            G.Line testLine2 = new G.Line(cp, ep2);
            G.Line testLine3 = new G.Line(main.End, ep3);
            
            if (denier(testLine1, e) && denier(testLine2, e) && denier(testLine3, e)) return true;

            return false;
        }


        private List<LineSegment> line_segmentator(G.Edge e)
        {
            List<LineSegment> tempList = new List<LineSegment>();
            List<LineSegment> segmentList = new List<LineSegment>();

            G.Line main = e.Line.Offset(G._Variables.EQUALS_TOLERANCE + 0.001);

            G.Vector d1 = e.Line.getDirectionVector();
            G.Vector o1 = e.Line.getOffsetVector();

            double delta = _V_.M_LINE_SEGMENTATOR_STEP;
            double j = 1;
            double len = main.Length() - (j * 2);

            while (j < len)
            {
                G.Point checkStartPoint = main.Start.move(j, d1);
                G.Point checkEndPoint = checkStartPoint.move(_V_.Y_STIRRUP_MAX_LENGTH, o1);
                G.Line checkLine = new G.Line(checkStartPoint, checkEndPoint);

                G.Edge trimmerEdge = null;
                bool check = trimmer_basepoint(checkLine, checkStartPoint, ref trimmerEdge);

                if (tempList.Count == 0)
                {
                    LineSegment temp = new LineSegment(checkStartPoint, e, trimmerEdge);
                    tempList.Add(temp);
                }
                else
                {
                    if (tempList[tempList.Count - 1].compareSegments(trimmerEdge))
                    {
                        tempList[tempList.Count - 1].updateSegment(checkStartPoint);
                    }
                    else
                    {
                        LineSegment temp = new LineSegment(checkStartPoint, e, trimmerEdge);
                        tempList.Add(temp);
                    }
                }

                j = j + delta;
            }

            foreach (LineSegment temp in tempList)
            {
                if (temp.checkValid()) segmentList.Add(temp);
            }

            return segmentList;
        }


        private bool trimmer_basepoint(G.Line extendedLine, G.Point fixedPoint, ref G.Edge trimmer)
        {
            bool trimmed = false;
            G.Line trimmedLine = extendedLine.Copy();

            foreach (G.Edge eg in allEdges)
            {
                G.Line offsetLine = eg.edgeOffset(0, 0, 0);
                if (G.Line.hasIntersection(trimmedLine, offsetLine))
                {
                    G.Line interLine = eg.edgeOffset(0, 0, 0);
                    G.Point ip = G.Line.getIntersectionPoint(trimmedLine, interLine);

                    if (fixedPoint == extendedLine.End)
                    {
                        trimmedLine = new G.Line(ip, extendedLine.End);
                    }
                    else
                    {
                        trimmedLine = new G.Line(extendedLine.Start, ip);
                    }

                    trimmed = true;
                    trimmer = eg;
                }
            }

            return trimmed;
        }


        private G.Line trimLine_basepoint(G.Line extendedLine, G.Point fixedPoint) // this function is used by B vs C handler and does not know what edge it belongs to. Hence the 0.01 tambov
        {
            G.Line trimmedLine = extendedLine.Copy();

            foreach (G.Edge eg in allEdges)
            {
                double o = _V_.X_CONCRETE_COVER_1 - _V_.X_TRIM_MINIMUM_DELTA;
                G.Line offsetLine = eg.edgeOffset(o, o, o);

                if (G.Line.hasIntersection(trimmedLine, offsetLine))
                {
                    G.Point ip = G.Line.getIntersectionPoint(trimmedLine, offsetLine);

                    if (fixedPoint == extendedLine.End)
                    {
                        trimmedLine = new G.Line(ip, extendedLine.End);
                    }
                    else
                    {
                        trimmedLine = new G.Line(extendedLine.Start, ip);
                    }
                }
            }

            return trimmedLine;
        }


        private G.Line trimLine_baseline(G.Line extendedLine, G.Line startLine, double offset, G.Edge e, ref G.Edge trimmer)
        {
            G.Line trimmedLine = extendedLine.Copy();

            foreach (G.Edge eg in allEdges)
            {
                if (eg.Line.getOffsetVector() == e.Line.getOffsetVector()) continue;
                if (e.StartCorner.getOtherEdge(e) == eg && e.StartCorner.Angle > Math.PI) continue;
                if (e.EndCorner.getOtherEdge(e) == eg && e.EndCorner.Angle > Math.PI) continue;
                if (trimmedLine.getDirectionVector() == eg.Line.getDirectionVector()) continue;
                if (trimmedLine.getDirectionVector() == (-1) * eg.Line.getDirectionVector()) continue;

                double o = offset - _V_.X_TRIM_MINIMUM_DELTA;
                double so = _V_.X_CONCRETE_COVER_1 - _V_.X_TRIM_SIDE_MINIMUM_DELTA;

                if (e.StartCorner.getOtherEdge(e) == eg) o = _V_.X_CONCRETE_COVER_1 - _V_.X_TRIM_MINIMUM_DELTA; ;
                if (e.EndCorner.getOtherEdge(e) == eg) o = _V_.X_CONCRETE_COVER_1 - _V_.X_TRIM_MINIMUM_DELTA; ;

                G.Line offsetLine = eg.edgeTrimmer(o, so, so);

                if (G.Line.hasIntersection(trimmedLine, offsetLine))
                {
                    G.Point ip = G.Line.getIntersectionPoint(trimmedLine, offsetLine);
                    
                    if (ip.distanceTo(extendedLine.Start) < ip.distanceTo(extendedLine.End))
                    {
                        trimmedLine = new G.Line(ip, trimmedLine.End);
                    }
                    else
                    {
                        trimmedLine = new G.Line(trimmedLine.Start, ip);
                    }

                    trimmer = eg;
                }
            }
            
            return trimmedLine;
        }
        

        private G.Line trimLine_basepoint(G.Line extendedLine, G.Point fixedPoint, double offset, G.Edge e, ref G.Edge trimmer)
        {
            G.Line trimmedLine = extendedLine.Copy();

            foreach (G.Edge eg in allEdges)
            {
                if (eg.Line.getOffsetVector() == e.Line.getOffsetVector()) continue;
                if (e.StartCorner.getOtherEdge(e) == eg && e.StartCorner.Angle > Math.PI) continue;
                if (e.EndCorner.getOtherEdge(e) == eg && e.EndCorner.Angle > Math.PI) continue;
                if (trimmedLine.getDirectionVector() == eg.Line.getDirectionVector()) continue;
                if (trimmedLine.getDirectionVector() == (-1) * eg.Line.getDirectionVector()) continue;

                double o = offset - _V_.X_TRIM_MINIMUM_DELTA;
                double so = _V_.X_CONCRETE_COVER_1 - _V_.X_TRIM_SIDE_MINIMUM_DELTA;

                if (e.StartCorner.getOtherEdge(e) == eg) o = so;
                if (e.EndCorner.getOtherEdge(e) == eg) o = so;

                G.Line offsetLine = eg.edgeTrimmer(o, so, so);

                if (G.Line.hasIntersection(trimmedLine, offsetLine))
                {
                    G.Point ip = G.Line.getIntersectionPoint(trimmedLine, offsetLine);

                    if (ip == fixedPoint) continue;

                    if (fixedPoint == extendedLine.End)
                    {
                        trimmedLine = new G.Line(ip, extendedLine.End);
                    }
                    else
                    {
                        trimmedLine = new G.Line(extendedLine.Start, ip);
                    }

                    trimmer = eg;
                }
            }
            
            return trimmedLine;
        }


        private void diagonalRotater(G.Point centerPoint, G.Vector v3, double n, double m, ref G.Line tester)
        {
            if (n > 6) return;

            double deg5 = 0.0872665;
            double rot = deg5 * n * m;

            v3 = v3.rotate(rot);
            tester = new G.Line(centerPoint, _V_.X_REINFORCEMENT_DIAGONAL_ANCHOR_LENGTH, v3);

            if (denier(tester))
            {
                n = n + 1;
                m = m * -1;
                diagonalRotater(centerPoint, v3, n, m, ref tester);
            }
            else
            {
                return;
            }
        }


        private G.Point getCornerPoint(G.Edge one, G.Edge two, double cover1, double cover2, ref G.Corner sharedCorner)
        {
            bool realCorner = G.Edge.getSharedCorner(one, two, ref sharedCorner);
            if (realCorner)
            {
                G.Point IP = sharedCorner.getCornerPoint(one, cover1, cover2);
                return IP;
            }
            else
            {
                PseudoCorner pc = new PseudoCorner(one, two);
                G.Point IP = pc.getCornerPoint(one, cover1, cover2);
                return IP;
            }
        }


        private bool isLineRight(G.Line ln)
        {
            G.Vector v = ln.getDirectionVector();

            double absX = Math.Abs(v.X);
            double absY = Math.Abs(v.Y);
            G.Vector absV = new G.Vector(absX, absY);
            G.Polar p = G.Converter.xy_to_la(absV);

            double remain = p.angle % (Math.PI / 4);

            if (remain < 0.1)
            {
                return true;
            }

            return false;
        }

    }
}