#define BRX_APP
//#define ARX_APP

using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using _SWF = System.Windows.Forms;


#if BRX_APP
    using _Ap = Bricscad.ApplicationServices;
    //using _Br = Teigha.BoundaryRepresentation;
    using _Cm = Teigha.Colors;
    using _Db = Teigha.DatabaseServices;
    using _Ed = Bricscad.EditorInput;
    using _Ge = Teigha.Geometry;
    using _Gi = Teigha.GraphicsInterface;
    using _Gs = Teigha.GraphicsSystem;
    using _Gsk = Bricscad.GraphicsSystem;
    using _Pl = Bricscad.PlottingServices;
    using _Brx = Bricscad.Runtime;
    using _Trx = Teigha.Runtime;
    using _Wnd = Bricscad.Windows;
    //using _Int = Bricscad.Internal;
#elif ARX_APP
    using _Ap = Autodesk.AutoCAD.ApplicationServices;
    //using _Br = Autodesk.AutoCAD.BoundaryRepresentation;
    using _Cm = Autodesk.AutoCAD.Colors;
    using _Db = Autodesk.AutoCAD.DatabaseServices;
    using _Ed = Autodesk.AutoCAD.EditorInput;
    using _Ge = Autodesk.AutoCAD.Geometry;
    using _Gi = Autodesk.AutoCAD.GraphicsInterface;
    using _Gs = Autodesk.AutoCAD.GraphicsSystem;
    using _Pl = Autodesk.AutoCAD.PlottingServices;
    using _Brx = Autodesk.AutoCAD.Runtime;
    using _Trx = Autodesk.AutoCAD.Runtime;
    using _Wnd = Autodesk.AutoCAD.Windows;
#endif

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;
using T = Logic_Tabler;


namespace DMTCommands
{
    partial class REINFORCE_command
    {
        private void output(List<R.Raud> reinf, List<R.Raud_Array> reinf_array, List<R.Raud> unique_reinf, G.Point insertPoint)
        {
            reinfSingleHandler(reinf);
            reinfArrayHandler(reinf_array);
            bendingHandler(unique_reinf, insertPoint);
        }


        private void reinfSingleHandler(List<R.Raud> reinf)
        {
            foreach (R.Raud re in reinf)
            {
                insertReinforcementSingle(re, false);
                insertReinforcmentMark(re.ToString(), re.IP);
            }
        }


        private void reinfArrayHandler(List<R.Raud_Array> reinf)
        {
            foreach (R.Raud_Array rea in reinf)
            {
                if (rea.array.Count > 0)
                {
                    foreach (R.Raud re in rea.array)
                    {
                        insertReinforcementSingle(re, true);
                    }

                    insertReinforcmentMark(rea.ToString(), rea.IP);
                }
            }
        }


        private void insertReinforcementSingle(R.Raud _ALFA_, bool arrayReinf)
        {
            string layerName = "Armatuur";
            string blockName = getReinforcementBlockName(_ALFA_, arrayReinf);

            _Ge.Point3d insertPointBlock = new _Ge.Point3d(_ALFA_.StartPoint.X, _ALFA_.StartPoint.Y, 0);
            using (_Db.BlockReference newBlockReference = new _Db.BlockReference(insertPointBlock, _c.blockTable[blockName]))
            {
                newBlockReference.Rotation = _ALFA_.Rotation;
                newBlockReference.Layer = layerName;
                _c.modelSpace.AppendEntity(newBlockReference);
                _c.trans.AddNewlyCreatedDBObject(newBlockReference, true);

                setReinforcementBlockParameters(newBlockReference, _ALFA_);
            }
        }


        private void insertReinforcmentMark(string mark, G.Point IP)
        {
            string layerName = "K023TL";
            string styleName = "dmt_M" + (int)Math.Round(L._V_.Z_DRAWING_SCALE);

            textStyleHandler();
            blockHandler();
            leaderStyleHandler(styleName, (int)Math.Round(L._V_.Z_DRAWING_SCALE));

            _Db.DBDictionary mleaderStyleTable = _c.trans.GetObject(_c.db.MLeaderStyleDictionaryId, _Db.OpenMode.ForWrite) as _Db.DBDictionary;

            _Ge.Point3d insertPointLeader = new _Ge.Point3d(IP.X, IP.Y, 0);
            _Ge.Point3d insertPointText = new _Ge.Point3d(IP.X + 7.5 * L._V_.Z_DRAWING_SCALE, IP.Y + 7.5 * L._V_.Z_DRAWING_SCALE, 0);

            _Db.MText mText = new _Db.MText();
            mText.SetDatabaseDefaults();
            mText.Contents = mark;

            _Db.MLeader leader = new _Db.MLeader();
            leader.SetDatabaseDefaults();
            leader.ContentType = _Db.ContentType.MTextContent;
            leader.MText = mText;
            leader.AddLeaderLine(insertPointLeader);
            leader.TextLocation = insertPointText;
            leader.MLeaderStyle = mleaderStyleTable.GetAt(styleName);
            
            leader.Layer = layerName;

            _c.modelSpace.AppendEntity(leader);
            _c.trans.AddNewlyCreatedDBObject(leader, true);
        }


            private void insertBending(R.Raud _ALFA_, G.Point insertion)
        {
            string layerName = "K023TL";
            string blockName = getBendingBlockName(_ALFA_);

            _Ge.Point3d insertPointBlock = new _Ge.Point3d(insertion.X, insertion.Y, 0);

            using (_Db.BlockReference newBlockReference = new _Db.BlockReference(insertPointBlock, _c.blockTable[blockName]))
            {
                newBlockReference.Layer = layerName;
                _c.modelSpace.AppendEntity(newBlockReference);
                _c.trans.AddNewlyCreatedDBObject(newBlockReference, true);

                newBlockReference.TransformBy(_Ge.Matrix3d.Scaling(L._V_.Z_DRAWING_SCALE, insertPointBlock));

                _Db.BlockTableRecord blockBlockTable = _c.trans.GetObject(_c.blockTable[blockName], _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
                if (blockBlockTable.HasAttributeDefinitions)
                {
                    foreach (_Db.ObjectId objID in blockBlockTable)
                    {
                        _Db.DBObject obj = _c.trans.GetObject(objID, _Db.OpenMode.ForRead) as _Db.DBObject;

                        if (obj is _Db.AttributeDefinition)
                        {
                            _Db.AttributeDefinition attDef = obj as _Db.AttributeDefinition;

                            if (!attDef.Constant)
                            {
                                using (_Db.AttributeReference attRef = new _Db.AttributeReference())
                                {
                                    attRef.SetAttributeFromBlock(attDef, newBlockReference.BlockTransform);
                                    attRef.Position = attDef.Position.TransformBy(newBlockReference.BlockTransform);
                                    setBendingBlockParameters(attRef, _ALFA_);
                                    newBlockReference.AttributeCollection.AppendAttribute(attRef);
                                    _c.trans.AddNewlyCreatedDBObject(attRef, true);
                                }
                            }
                        }
                    }
                }
            }
        }


        private void bendingHandler(List<R.Raud> unique_reinf, G.Point insertPoint)
        {
            G.Point currentPoint = new G.Point(insertPoint.X, insertPoint.Y);

            List<R.Raud> onlyA = unique_reinf.Where(x => x is R.A_Raud).OrderBy(b => b.Length).Reverse().ToList();
            List<R.Raud> onlyB = unique_reinf.Where(x => x is R.B_Raud).OrderBy(b => b.Length).Reverse().ToList();
            List<R.Raud> onlyC = unique_reinf.Where(x => x is R.C_Raud).OrderBy(b => b.Length).Reverse().ToList();
            List<R.Raud> onlyD = unique_reinf.Where(x => x is R.D_Raud).OrderBy(b => b.Length).Reverse().ToList();
            List<R.Raud> onlyE = unique_reinf.Where(x => x is R.E_Raud).OrderBy(b => b.Length).Reverse().ToList();
            List<R.Raud> onlyU = unique_reinf.Where(x => x is R.U_Raud).OrderBy(b => b.Length).Reverse().ToList();

            if (onlyA.Count > 0)
            {
                foreach (R.Raud re in onlyA)
                {
                    insertBending(re, currentPoint);
                    currentPoint.X += 17 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 20 * L._V_.Z_DRAWING_SCALE;
            }

            if (onlyB.Count > 0)
            {
                foreach (R.Raud re in onlyB)
                {
                    insertBending(re, currentPoint);
                    currentPoint.X += 17 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 23 * L._V_.Z_DRAWING_SCALE;
            }

            if (onlyD.Count > 0)
            {
                foreach (R.Raud re in onlyD)
                {
                    insertBending(re, currentPoint);
                    currentPoint.X += 17 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 23 * L._V_.Z_DRAWING_SCALE;
            }

            if (onlyC.Count > 0)
            {
                foreach (R.Raud re in onlyC)
                {
                    insertBending(re, currentPoint);
                    currentPoint.X += 22 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 23 * L._V_.Z_DRAWING_SCALE;
            }

            if (onlyE.Count > 0)
            {
                foreach (R.Raud re in onlyE)
                {
                    insertBending(re, currentPoint);
                    currentPoint.X += 40 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 23 * L._V_.Z_DRAWING_SCALE;
            }

            if (onlyU.Count > 0)
            {
                foreach (R.Raud re in onlyU)
                {
                    insertBending(re, currentPoint);
                    currentPoint.X += 17 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 23 * L._V_.Z_DRAWING_SCALE;
            }
        }


        private void setReinforcementBlockParameters(_Db.BlockReference newBlockReference, R.Raud _ALFA_)
        {
            _Db.DynamicBlockReferencePropertyCollection aa = newBlockReference.DynamicBlockReferencePropertyCollection;
            foreach (_Db.DynamicBlockReferenceProperty a in aa)
            {
                if (a != null)
                {
                    if (_ALFA_ is R.A_Raud)
                    {
                        R.A_Raud _BETA_ = _ALFA_ as R.A_Raud;
                        if (a.PropertyName == "A") a.Value = _BETA_.A;
                    }

                    else if (_ALFA_ is R.B_Raud)
                    {
                        R.B_Raud _BETA_ = _ALFA_ as R.B_Raud;
                        if (a.PropertyName == "A") a.Value = _BETA_.A;
                        else if (a.PropertyName == "B") a.Value = _BETA_.B;
                    }

                    else if (_ALFA_ is R.C_Raud)
                    {
                        R.C_Raud _BETA_ = _ALFA_ as R.C_Raud;
                        if (a.PropertyName == "A") a.Value = _BETA_.A;
                        else if (a.PropertyName == "B") a.Value = _BETA_.B;
                        else if (a.PropertyName == "U") a.Value = Math.PI - _BETA_.U; // HERE
                    }

                    else if (_ALFA_ is R.D_Raud)
                    {
                        R.D_Raud _BETA_ = _ALFA_ as R.D_Raud;
                        if (a.PropertyName == "A") a.Value = _BETA_.A;
                        else if (a.PropertyName == "B") a.Value = _BETA_.B;
                        else if (a.PropertyName == "C") a.Value = _BETA_.C;

                        else if (a.PropertyName == "A/C") a.Value = _BETA_.A;
                    }

                    else if (_ALFA_ is R.E_Raud)
                    {
                        R.E_Raud _BETA_ = _ALFA_ as R.E_Raud;

                        if (a.PropertyName == "A") a.Value = _BETA_.A;
                        else if (a.PropertyName == "B") a.Value = _BETA_.B;
                        else if (a.PropertyName == "C") a.Value = _BETA_.C;
                        else if (a.PropertyName == "U") a.Value = Math.PI - _BETA_.U; // HERE
                        else if (a.PropertyName == "V")
                        {
                            if (_BETA_.B > 6000) a.Value = _BETA_.V + Math.PI;
                            else a.Value = _BETA_.V;
                        }
                        else if (a.PropertyName == "X") a.Value = _BETA_.X;
                        else if (a.PropertyName == "Y") a.Value = _BETA_.Y;
                    }
                    else if (_ALFA_ is R.U_Raud)
                    {
                        R.U_Raud _BETA_ = _ALFA_ as R.U_Raud;
                        if (a.PropertyName == "A") a.Value = _BETA_.A; 
                        else if (a.PropertyName == "B") a.Value = _BETA_.B;
                        else if (a.PropertyName == "C") a.Value = _BETA_.C;
                    }
                }
            }
        }


        private void setBendingBlockParameters(_Db.AttributeReference ar, R.Raud _ALFA_)
        {
            if (ar != null)
            {
                if (ar.Tag == "Teraseklass")
                {
                    ar.TextString = _ALFA_.Materjal.ToString();
                }
                if (ar.Tag == "Positsioon")
                {
                    ar.TextString = _ALFA_.ToStringNoCount();
                }

                if (_ALFA_ is R.A_Raud)
                {
                    R.A_Raud _BETA_ = _ALFA_ as R.A_Raud;
                    if (ar.Tag == "A")
                    {
                        ar.TextString = _BETA_.A.ToString();
                    }
                }

                if (_ALFA_ is R.B_Raud)
                {
                    R.B_Raud _BETA_ = _ALFA_ as R.B_Raud;
                    if (ar.Tag == "A")
                    {
                        ar.TextString = _BETA_.A.ToString();
                    }
                    else if (ar.Tag == "B")
                    {
                        ar.TextString = _BETA_.B.ToString();
                    }
                }

                if (_ALFA_ is R.C_Raud)
                {
                    R.C_Raud _BETA_ = _ALFA_ as R.C_Raud;
                    if (ar.Tag == "A")
                    {
                        ar.TextString = _BETA_.A.ToString();
                    }
                    else if (ar.Tag == "B")
                    {
                        ar.TextString = _BETA_.B.ToString();
                    }
                    else if (ar.Tag == "U")
                    {
                        ar.TextString = ((int)Math.Round(G.Converter.ToDeg(_BETA_.U), 0)).ToString();
                    }
                }

                if (_ALFA_ is R.D_Raud)
                {
                    R.D_Raud _BETA_ = _ALFA_ as R.D_Raud;
                    if (ar.Tag == "A")
                    {
                        ar.TextString = _BETA_.A.ToString();
                    }
                    else if (ar.Tag == "B")
                    {
                        ar.TextString = _BETA_.B2.ToString(); // parand magic
                    }
                    else if (ar.Tag == "C")
                    {
                        ar.TextString = _BETA_.C.ToString();
                    }
                }

                if (_ALFA_ is R.E_Raud)
                {
                    R.E_Raud _BETA_ = _ALFA_ as R.E_Raud;
                    if (ar.Tag == "A")
                    {
                        ar.TextString = _BETA_.A.ToString();
                    }
                    else if (ar.Tag == "B")
                    {
                        ar.TextString = _BETA_.B2.ToString(); // parand magic
                    }
                    else if (ar.Tag == "C")
                    {
                        ar.TextString = _BETA_.C.ToString();
                    }
                    else if (ar.Tag == "U")
                    {
                        ar.TextString = ((int)Math.Round(G.Converter.ToDeg(_BETA_.U), 0)).ToString();
                    }
                    else if (ar.Tag == "V")
                    {
                        ar.TextString = ((int)Math.Round(G.Converter.ToDeg(_BETA_.V), 0)).ToString();
                    }
                    else if (ar.Tag == "X")
                    {
                        ar.TextString = ((int)_BETA_.X).ToString();
                    }
                    else if (ar.Tag == "Y")
                    {
                        ar.TextString = ((int)_BETA_.Y).ToString();
                    }
                }

                if (_ALFA_ is R.U_Raud)
                {
                    R.U_Raud _BETA_ = _ALFA_ as R.U_Raud;
                    if (ar.Tag == "A")
                    {
                        ar.TextString = _BETA_.A.ToString();
                    }
                    else if (ar.Tag == "B")
                    {
                        ar.TextString = _BETA_.B.ToString();
                    }
                    else if (ar.Tag == "C")
                    {
                        ar.TextString = _BETA_.C.ToString();
                    }
                }
            }
        }


        private string getReinforcementBlockName(R.Raud _ALFA_, bool arrayReinf)
        {
            if (arrayReinf == false)
            {
                if (_ALFA_ is R.A_Raud) return "Reinf_A_Raud";
                else if (_ALFA_ is R.B_Raud) return "Reinf_B_Raud";
                else if (_ALFA_ is R.C_Raud) return "Reinf_C_Raud";
                else if (_ALFA_ is R.D_Raud) return "Reinf_D_Raud";
                else if (_ALFA_ is R.E_Raud) return "Reinf_E_Raud2";
            }
            else
            {
                if (_ALFA_ is R.A_Raud) return "Reinf_A_Raud";
                else if (_ALFA_ is R.D_Raud) return "Reinf_D_Raud_Side";
                else if (_ALFA_ is R.U_Raud) return "Reinf_U_Raud_Side";
            }

            return "Reinf_A_Raud";
        }


        private string getBendingBlockName(R.Raud _ALFA_)
        {
            if (_ALFA_ is R.A_Raud) return "Raud_A";
            else if (_ALFA_ is R.B_Raud) return "Raud_B";
            else if (_ALFA_ is R.C_Raud) return "Raud_C";
            else if (_ALFA_ is R.D_Raud) return "Raud_D";
            else if (_ALFA_ is R.E_Raud) return "Raud_E";
            else if (_ALFA_ is R.U_Raud) return "Raud_U";
            else return "Raud_A";
        }


        private void textStyleHandler()
        {
            _Db.TextStyleTable txtStyleTable = _c.trans.GetObject(_c.db.TextStyleTableId, _Db.OpenMode.ForWrite) as _Db.TextStyleTable;

            if (!txtStyleTable.Has("Stommest"))
            {
                _Db.TextStyleTableRecord newStyle = new _Db.TextStyleTableRecord();
                newStyle.Name = "Stommest";

                newStyle.FileName = "ARIALN.TTF";
                newStyle.FlagBits = 0;
                newStyle.Font = new _Gi.FontDescriptor("Arial Narrow", false, false, 0, 34);
                newStyle.IsVertical = false;
                newStyle.ObliquingAngle = 0;
                newStyle.TextSize = 0;
                newStyle.XScale = 1;

                txtStyleTable.Add(newStyle);
                _c.trans.AddNewlyCreatedDBObject(newStyle, true);
                write("[OUTPUT] TextStyle 'Stommest' created");
            }
        }


        private void blockHandler()
        {
            if (!_c.blockTable.Has("_NONE"))
            {
                _Ap.Application.SetSystemVariable("DIMBLK", "_NONE");

                write("[OUTPUT] Block '_NONE' created");
            }
        }


        private void leaderStyleHandler(string styleName, int scale)
        {
            _Db.DBDictionary mleaderStyleTable = _c.trans.GetObject(_c.db.MLeaderStyleDictionaryId, _Db.OpenMode.ForWrite) as _Db.DBDictionary;
            _Db.TextStyleTable txtStyleTable = _c.trans.GetObject(_c.db.TextStyleTableId, _Db.OpenMode.ForWrite) as _Db.TextStyleTable;

            if (!mleaderStyleTable.Contains(styleName))
            {
                _Db.MLeaderStyle newStyle = new _Db.MLeaderStyle();

                newStyle.Annotative = _Db.AnnotativeStates.False;
                newStyle.ArrowSize = 3.0;
                newStyle.ArrowSymbolId = _c.blockTable["_NONE"];
                //newStyle.BlockColor=; //BYBLOCK
                newStyle.BlockConnectionType = _Db.BlockConnectionType.ConnectExtents;
                newStyle.BlockId = _Db.ObjectId.Null;
                newStyle.BlockRotation = 0;
                newStyle.BlockScale = new _Ge.Scale3d(1, 1, 1);
                newStyle.BreakSize = 0;
                newStyle.ContentType = _Db.ContentType.MTextContent;
                newStyle.DefaultMText = new _Db.MText();
                newStyle.DoglegLength = 8;
                newStyle.DrawLeaderOrderType = _Db.DrawLeaderOrderType.DrawLeaderHeadFirst;
                newStyle.DrawMLeaderOrderType = _Db.DrawMLeaderOrderType.DrawLeaderFirst;
                newStyle.EnableBlockRotation = true;
                newStyle.EnableBlockScale = true;
                newStyle.EnableDogleg = false;
                newStyle.EnableFrameText = false;
                newStyle.EnableLanding = true;
                newStyle.FirstSegmentAngleConstraint = _Db.AngleConstraint.DegreesAny;
                newStyle.LandingGap = 1;
                newStyle.LeaderLineColor = _Cm.Color.FromColorIndex(_Cm.ColorMethod.None, 142);
                newStyle.LeaderLineType = _Db.LeaderType.StraightLeader;
                //newStyle.LeaderLineTypeId=; //BYBLOCK
                //newStyle.LeaderLineWeight=; //BYBLOCK
                newStyle.MaxLeaderSegmentsPoints = 2;
                newStyle.Scale = scale;
                newStyle.SecondSegmentAngleConstraint = _Db.AngleConstraint.DegreesAny;
                newStyle.TextAlignAlwaysLeft = false;
                newStyle.TextAlignmentType = _Db.TextAlignmentType.LeftAlignment;
                newStyle.TextAngleType = _Db.TextAngleType.HorizontalAngle;
                //newStyle.TextColor=; //BYBLOCK
                newStyle.TextHeight = 2.5;
                newStyle.TextStyleId = txtStyleTable["Stommest"];

                newStyle.SetTextAttachmentType(_Db.TextAttachmentType.AttachmentBottomOfTopLine, _Db.LeaderDirectionType.LeftLeader); // Left attachment
                newStyle.SetTextAttachmentType(_Db.TextAttachmentType.AttachmentBottomOfTopLine, _Db.LeaderDirectionType.RightLeader); // Right attachment

                newStyle.PostMLeaderStyleToDb(_c.db, styleName);
                _c.trans.AddNewlyCreatedDBObject(newStyle, true);
                write("[OUTPUT] MLeader style '" + styleName + "' created");
            }
        }

    }
}