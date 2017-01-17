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

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;
using T = Logic_Tabler;

namespace commands
{
    static class Tabler_Outputs
    {
        public static void main(List<T.DrawingArea> fields)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Transaction trans = db.TransactionManager.StartTransaction();
            try
            {
                List<string> blockNames = new List<string>() { "Painutustabel_rida" };
                List<string> layerNames = new List<string>() { "K004" };
                Universal.programInit(blockNames, layerNames, trans);

                fieldHandler(fields, trans);
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

        private static void fieldHandler(List<T.DrawingArea> fields, Transaction trans)
        {
            foreach (T.DrawingArea f in fields)
            {
                if (f.Valid)
                {
                    generateTable(f, trans);
                }
                else
                {
                    Universal.writeCadMessage(f.Reason);
                }
            }
        }

        private static void generateTable(T.DrawingArea field, Transaction trans)
        {
            G.Point insertPoint = field._tableHeads[0].IP;
            double scale = field._tableHeads[0].Scale;

            G.Point currentPoint = field._tableHeads[0].IP;
            double delta = scale * 14;

            currentPoint.X = insertPoint.X;
            currentPoint.Y -= delta;

            foreach (T.TableRow b in field._rows)
            {
                insertRow(currentPoint, b, scale, trans);

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 4 * scale;
            }
        }

        private static void insertRow(G.Point insertion, T.TableRow rowData, double scale, Transaction trans)
        {
            string blockName = "Painutustabel_rida";
            string layerName = "K004";

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
                newBlockReference.TransformBy(Matrix3d.Scaling(scale, insertPointBlock));

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
                                    setRowParameters(attRef, rowData);
                                    newBlockReference.AttributeCollection.AppendAttribute(attRef);
                                    trans.AddNewlyCreatedDBObject(attRef, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void setRowParameters(AttributeReference ar, T.TableRow rowData)
        {
            if (ar != null)
            {
                if (ar.Tag == "Pos") { ar.TextString = rowData.Position.ToString(); }
                else if (ar.Tag == "Klass") { ar.TextString = rowData.Material.ToString(); }
                else if (ar.Tag == "Diam") { ar.TextString = rowData.Diameter.ToString(); }
                else if (ar.Tag == "tk") { ar.TextString = rowData.Count.ToString(); }
                else if (ar.Tag == "Pikkus") { ar.TextString = rowData.Length.ToString(); }
                else if (ar.Tag == "a") { if (rowData.A != -1) ar.TextString = rowData.A.ToString(); }
                else if (ar.Tag == "b") { if (rowData.B != -1) ar.TextString = rowData.B.ToString(); }
                else if (ar.Tag == "c") { if (rowData.C != -1) ar.TextString = rowData.C.ToString(); }
                else if (ar.Tag == "d") { if (rowData.D != -1) ar.TextString = rowData.D.ToString(); }
                else if (ar.Tag == "e") { if (rowData.E != -1) ar.TextString = rowData.E.ToString(); }
                else if (ar.Tag == "f") { if (rowData.F != -1) ar.TextString = rowData.F.ToString(); }
                else if (ar.Tag == "g") { if (rowData.G != -1) ar.TextString = rowData.G.ToString(); }
                else if (ar.Tag == "u") { if (rowData.U != "") ar.TextString = rowData.U; }
                else if (ar.Tag == "v") { if (rowData.V != "") ar.TextString = rowData.V; }
                else if (ar.Tag == "R") { if (rowData.R != -1) ar.TextString = rowData.R.ToString(); }
                else if (ar.Tag == "x") { if (rowData.X != -1) ar.TextString = rowData.X.ToString(); }
                else if (ar.Tag == "y") { if (rowData.Y != -1) ar.TextString = rowData.Y.ToString(); }
            }
        }
    }
}