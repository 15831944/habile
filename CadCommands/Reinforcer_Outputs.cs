using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using SWF = System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using L = Logic_Reinf;
using R = Reinforcement;
using G = Geometry;

namespace commands
{
    static class Reinforcer_Outputs
    {
        public static void main(List<R.Raud> reinf, List<R.Raud_Array> reinf_array, List<R.Raud> unique_reinf, G.Point insertPoint)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction trans = db.TransactionManager.StartTransaction();

            try
            {
                List<string> blockNames = new List<string>() { "Raud_A", "Raud_B", "Raud_C", "Raud_D", "Raud_E", "Reinf_A_Raud", "Reinf_B_Raud", "Reinf_C_Raud", "Reinf_D_Raud", "Reinf_E_Raud" };
                List<string> layerNames = new List<string>() { "K023TL", "Armatuur" };
                Universal.programInit(blockNames, layerNames, trans);

                reinfHandler(reinf, trans);
                reinfArrayHandler(reinf_array, trans);
                bendingHandler(unique_reinf, insertPoint, trans);

                trans.Commit();
            }
            catch (System.Exception ex)
            {
                Universal.writeCadMessage("Program stopped with ERROR:\n" + ex.Message + "\n" + ex.TargetSite);
            }
            finally
            {
                trans.Dispose();
            }
        }

        private static void reinfHandler(List<R.Raud> reinf, Transaction trans)
        {
            foreach (R.Raud re in reinf)
            {
                insertReinforcementSingle(re, false, trans);
                insertReinforcmentMark2(re.ToString(), re.IP, trans);
            }
        }

        private static void reinfArrayHandler(List<R.Raud_Array> reinf, Transaction trans)
        {
            foreach (R.Raud_Array rea in reinf)
            {
                if (rea.array.Count > 0)
                {
                    foreach (R.Raud re in rea.array)
                    {
                        insertReinforcementSingle(re, true, trans);
                    }

                    insertReinforcmentMark2(rea.ToString(), rea.IP, trans);
                }
            }
        }

        private static void insertReinforcementSingle(R.Raud _ALFA_, bool arrayReinf, Transaction trans)
        {
            string blockName = getReinforcementBlockName(_ALFA_, arrayReinf);
            string layerName = "Armatuur";

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord curSpace = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

            Point3d insertPointBlock = new Point3d(_ALFA_.StartPoint.X, _ALFA_.StartPoint.Y, 0);
            using (BlockReference newBlockReference = new BlockReference(insertPointBlock, blockTable[blockName]))
            {
                newBlockReference.Rotation = _ALFA_.Rotation;
                newBlockReference.Layer = layerName;
                curSpace.AppendEntity(newBlockReference);
                trans.AddNewlyCreatedDBObject(newBlockReference, true);

                setReinforcementBlockParameters(newBlockReference, _ALFA_);
            }
        }

        private static void insertReinforcmentMark2(string mark, G.Point IP, Transaction trans)
        {
            string layerName = "K023TL";

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            BlockTableRecord curSpace = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

            Point3d insertPointLeader = new Point3d(IP.X, IP.Y, 0);
            Point3d insertPointText = new Point3d(IP.X + 7.5 * L._V_.Z_DRAWING_SCALE, IP.Y + 7.5 * L._V_.Z_DRAWING_SCALE, 0);

            MLeader leader = new MLeader();
            leader.SetDatabaseDefaults();
            leader.Layer = layerName;

            BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            if (!blockTable.Has("_NONE"))
            {
                Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("DIMBLK", "_NONE");
            }
            leader.ArrowSymbolId = blockTable["_NONE"];

            leader.SetTextAttachmentType(TextAttachmentType.AttachmentBottomOfTopLine, LeaderDirectionType.LeftLeader); // Left attachment
            leader.SetTextAttachmentType(TextAttachmentType.AttachmentBottomOfTopLine, LeaderDirectionType.RightLeader); // Right attachment
            leader.EnableLanding = false;
            leader.LeaderLineColor = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.None, 155);
            leader.LandingGap = 0;
            leader.ContentType = ContentType.MTextContent;

            MText mText = new MText();
            mText.SetDatabaseDefaults();
            mText.TextHeight = 2.5 * L._V_.Z_DRAWING_SCALE;
            mText.Contents = mark;
            mText.Location = insertPointText;

            leader.MText = mText;

            int idx = leader.AddLeaderLine(insertPointLeader);

            curSpace.AppendEntity(leader);
            trans.AddNewlyCreatedDBObject(leader, true);
        }

        private static void insertBending(R.Raud _ALFA_, G.Point insertion, Transaction trans)
        {
            string blockName = getBendingBlockName(_ALFA_);
            string layerName = "K023TL";

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord curSpace = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

            Point3d insertPointBlock = new Point3d(insertion.X, insertion.Y, 0);
            using (BlockReference newBlockReference = new BlockReference(insertPointBlock, blockTable[blockName]))
            {
                newBlockReference.Layer = layerName;
                curSpace.AppendEntity(newBlockReference);
                trans.AddNewlyCreatedDBObject(newBlockReference, true);

                newBlockReference.TransformBy(Matrix3d.Scaling(L._V_.Z_DRAWING_SCALE, insertPointBlock));

                BlockTableRecord blockBlockTable = trans.GetObject(blockTable[blockName], OpenMode.ForRead) as BlockTableRecord;
                if (blockBlockTable.HasAttributeDefinitions)
                {
                    foreach (ObjectId objID in blockBlockTable)
                    {
                        DBObject obj = trans.GetObject(objID, OpenMode.ForRead) as DBObject;

                        if (obj is AttributeDefinition)
                        {
                            AttributeDefinition attDef = obj as AttributeDefinition;

                            if (!attDef.Constant)
                            {
                                using (AttributeReference attRef = new AttributeReference())
                                {
                                    attRef.SetAttributeFromBlock(attDef, newBlockReference.BlockTransform);
                                    attRef.Position = attDef.Position.TransformBy(newBlockReference.BlockTransform);
                                    setBendingBlockParameters(attRef, _ALFA_);
                                    newBlockReference.AttributeCollection.AppendAttribute(attRef);
                                    trans.AddNewlyCreatedDBObject(attRef, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void bendingHandler(List<R.Raud> unique_reinf, G.Point insertPoint, Transaction trans)
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
                    insertBending(re, currentPoint, trans);
                    currentPoint.X += 17 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 20 * L._V_.Z_DRAWING_SCALE;
            }

            if (onlyB.Count > 0)
            {
                foreach (R.Raud re in onlyB)
                {
                    insertBending(re, currentPoint, trans);
                    currentPoint.X += 17 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 23 * L._V_.Z_DRAWING_SCALE;
            }

            if (onlyD.Count > 0)
            {
                foreach (R.Raud re in onlyD)
                {
                    insertBending(re, currentPoint, trans);
                    currentPoint.X += 17 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 23 * L._V_.Z_DRAWING_SCALE;
            }

            if (onlyC.Count > 0)
            {
                foreach (R.Raud re in onlyC)
                {
                    insertBending(re, currentPoint, trans);
                    currentPoint.X += 22 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 23 * L._V_.Z_DRAWING_SCALE;
            }

            if (onlyE.Count > 0)
            {
                foreach (R.Raud re in onlyE)
                {
                    insertBending(re, currentPoint, trans);
                    currentPoint.X += 40 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 23 * L._V_.Z_DRAWING_SCALE;
            }

            if (onlyU.Count > 0)
            {
                foreach (R.Raud re in onlyU)
                {
                    insertBending(re, currentPoint, trans);
                    currentPoint.X += 17 * L._V_.Z_DRAWING_SCALE;
                }

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 23 * L._V_.Z_DRAWING_SCALE;
            }
        }

        private static void setReinforcementBlockParameters(BlockReference newBlockReference, R.Raud _ALFA_)
        {
            DynamicBlockReferencePropertyCollection aa = newBlockReference.DynamicBlockReferencePropertyCollection;
            foreach (DynamicBlockReferenceProperty a in aa)
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

        private static void setBendingBlockParameters(AttributeReference ar, R.Raud _ALFA_)
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

        private static string getReinforcementBlockName(R.Raud _ALFA_, bool arrayReinf)
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

        private static string getBendingBlockName(R.Raud _ALFA_)
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