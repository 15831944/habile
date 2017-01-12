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
        public bool define_B(G.Edge startEdge, G.Edge endEdge)
        {
            bool startSet = setEdges.Keys.Contains(startEdge);
            bool endSet = setEdges.Keys.Contains(endEdge);

            double cover1 = _V_.X_CONCRETE_COVER_1;
            double cover2 = _V_.X_CONCRETE_COVER_1;

            double mainDist = _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH;
            double sideDist = _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH;

            if (startSet == true && endSet == true)
            {
                cover1 = cover1 + _V_.Y_CONCRETE_COVER_DELTA;
                cover2 = cover2 + _V_.Y_CONCRETE_COVER_DELTA;

                //mainDist == default
                //sideDist == default
            }
            else if (startSet == false && endSet == false)
            {
                double startLineLength = startEdge.Line.Length();
                double endLineLength = endEdge.Line.Length();

                if (startEdge.EndCorner.Angle > Math.PI)
                {
                    startLineLength = startLineLength + _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH;
                }

                if (endEdge.StartCorner.Angle > Math.PI)
                {
                    endLineLength = endLineLength + _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH;
                }

                if (startLineLength > endLineLength)
                {
                    endSet = true;

                    //cover1 == default
                    cover2 = cover2 + _V_.Y_CONCRETE_COVER_DELTA;

                    mainDist = startEdge.edgeOffset(cover1, cover1, cover2).Length();
                    //sideDist == default

                    if (startEdge.EndCorner.Angle > Math.PI)
                    {
                        mainDist += _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH;
                    }
                }
                else
                {
                    startSet = true;

                    cover1 = cover1 + _V_.Y_CONCRETE_COVER_DELTA;
                    //cover2 == default

                    //mainDist == default
                    sideDist = endEdge.edgeOffset(cover1, cover1, cover2).Length();

                    if (endEdge.StartCorner.Angle > Math.PI)
                    {
                        sideDist += +_V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH;
                    }
                }
            }
            else if (startSet == false && endSet == true)
            {
                //cover1 == default
                cover2 = cover2 + _V_.Y_CONCRETE_COVER_DELTA;

                mainDist = startEdge.edgeOffset(cover1, cover1, cover2).Length();
                //sideDist == default

                if (startEdge.EndCorner.Angle > Math.PI)
                {
                    mainDist += _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH;
                }
            }
            else if (startSet == true && endSet == false)
            {
                cover1 = cover1 + _V_.Y_CONCRETE_COVER_DELTA;
                //cover2 == defualt

                //mainDist == default
                sideDist = endEdge.edgeOffset(cover1, cover1, cover2).Length();

                if (endEdge.StartCorner.Angle > Math.PI)
                {
                    sideDist += + _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH;
                }
            }

            mainDist = Math.Max(mainDist, _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH);
            sideDist = Math.Max(sideDist, _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH);

            G.Corner sharedCorner = null;
            G.Point IP = getCornerPoint(startEdge, endEdge, cover1, cover2, ref sharedCorner);

            G.Vector v1 = endEdge.Line.getOffsetVector();
            G.Vector v2 = startEdge.Line.getCoolVector(v1);

            G.Vector v3 = startEdge.Line.getOffsetVector();
            G.Vector v4 = endEdge.Line.getCoolVector(v3);

            G.Point mainEndPoint = IP.move(mainDist, v2);
            G.Point sidePoint = IP.move(sideDist, v4);

            //A_handler_debug(IP, mainEndPoint);
            //A_handler_debug(sidePoint, IP);

            bool success = false;

            if (startSet == false)
            {
                success = B_vs_C_handler(IP, mainEndPoint, sidePoint, startEdge, sharedCorner);
            }
            else if (endSet == false)
            {
                success = B_vs_C_handler(IP, mainEndPoint, sidePoint, endEdge, sharedCorner);
            }
            else
            {
                success = B_vs_C_handler(IP, mainEndPoint, sidePoint, null, sharedCorner);
            }
                
            return success;
        }

    }
}