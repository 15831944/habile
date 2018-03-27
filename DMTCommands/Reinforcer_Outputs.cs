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
    partial class Reinforcer
    {
        private void output(List<R.Raud> reinf, List<R.Raud_Array> reinf_array, List<R.Raud> unique_reinf, G.Point insertPoint)
        {
            reinfSingleHandler(reinf);
            reinfArrayHandler(reinf_array);
            //bendingHandler(unique_reinf, insertPoint);
        }


        private void reinfSingleHandler(List<R.Raud> reinf)
        {
            foreach (R.Raud re in reinf)
            {
                insertReinforcementSingle(re, false);
                //insertReinforcmentMark(re.ToString(), re.IP);
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

                    //insertReinforcmentMark(rea.ToString(), rea.IP);
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
            
            _Ge.Point3d insertPointLeader = new _Ge.Point3d(IP.X, IP.Y, 0);
            _Ge.Point3d insertPointText = new _Ge.Point3d(IP.X + 7.5 * L._V_.Z_DRAWING_SCALE, IP.Y + 7.5 * L._V_.Z_DRAWING_SCALE, 0);

            _Db.MLeader leader = new _Db.MLeader();
            leader.SetDatabaseDefaults();
            leader.Layer = layerName;

            if (!_c.blockTable.Has("_NONE")) _Ap.Application.SetSystemVariable("DIMBLK", "_NONE");
            leader.ArrowSymbolId = _c.blockTable["_NONE"];

            leader.SetTextAttachmentType(_Db.TextAttachmentType.AttachmentBottomOfTopLine, _Db.LeaderDirectionType.LeftLeader); // Left attachment
            leader.SetTextAttachmentType(_Db.TextAttachmentType.AttachmentBottomOfTopLine, _Db.LeaderDirectionType.RightLeader); // Right attachment
            leader.EnableLanding = false;
            leader.LeaderLineColor = _Cm.Color.FromColorIndex(_Cm.ColorMethod.None, 155);
            leader.LandingGap = 0;
            leader.ContentType = _Db.ContentType.MTextContent;

            _Db.MText mText = new _Db.MText();
            mText.SetDatabaseDefaults();
            mText.TextHeight = 2.5 * L._V_.Z_DRAWING_SCALE;
            mText.Contents = mark;
            mText.Location = insertPointText;

            leader.MText = mText;

            int idx = leader.AddLeaderLine(insertPointLeader);

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

            List<R.Raud> onlyA = unique_reinf.Where(x => x is R.A_Raud).ToList();
            List<R.Raud> onlyB = unique_reinf.Where(x => x is R.B_Raud).ToList();
            List<R.Raud> onlyC = unique_reinf.Where(x => x is R.C_Raud).ToList();
            List<R.Raud> onlyD = unique_reinf.Where(x => x is R.D_Raud).ToList();
            List<R.Raud> onlyE = unique_reinf.Where(x => x is R.E_Raud).ToList();
            List<R.Raud> onlyU = unique_reinf.Where(x => x is R.U_Raud).ToList();

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
                            if (_BETA_.B > 1000) a.Value = _BETA_.V + Math.PI;
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
                        ar.TextString = ((int)G.Converter.ToDeg(_BETA_.U)).ToString();
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
                        ar.TextString = ((int)G.Converter.ToDeg(_BETA_.U)).ToString();
                    }
                    else if (ar.Tag == "V")
                    {
                        ar.TextString = ((int)G.Converter.ToDeg(_BETA_.V)).ToString();
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
                else if (_ALFA_ is R.E_Raud) return "Reinf_E_Raud";
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

    }
}