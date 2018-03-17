using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using _SWF = System.Windows.Forms;

//using _Ap = Autodesk.AutoCAD.ApplicationServices;
////using _Br = Autodesk.AutoCAD.BoundaryRepresentation;
//using _Cm = Autodesk.AutoCAD.Colors;
//using _Db = Autodesk.AutoCAD.DatabaseServices;
//using _Ed = Autodesk.AutoCAD.EditorInput;
//using _Ge = Autodesk.AutoCAD.Geometry;
//using _Gi = Autodesk.AutoCAD.GraphicsInterface;
//using _Gs = Autodesk.AutoCAD.GraphicsSystem;
//using _Pl = Autodesk.AutoCAD.PlottingServices;
//using _Brx = Autodesk.AutoCAD.Runtime;
//using _Trx = Autodesk.AutoCAD.Runtime;
//using _Wnd = Autodesk.AutoCAD.Windows;

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

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;
using T = Logic_Tabler;


namespace DMTCommands
{
    static class Summar_Outputs
    {
        public static void main(List<T.DrawingArea> fields)
        {
            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;
            _Ed.Editor ed = doc.Editor;

            _Db.Transaction trans = db.TransactionManager.StartTransaction();
            try
            {
                List<string> blockNames = new List<string>() { "PainutusKokkuvõte" };
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


        private static void fieldHandler(List<T.DrawingArea> fields, _Db.Transaction trans)
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


        private static void generateTable(T.DrawingArea field, _Db.Transaction trans)
        {
            G.Point insertPoint = T.SummarHandler.getSummaryInsertionPoint(field);
            double scale = field._tableHeads[0].Scale;

            G.Point currentPoint = T.SummarHandler.getSummaryInsertionPoint(field);
            double delta = scale * 4;

            currentPoint.X = insertPoint.X;
            currentPoint.Y -= delta;

            foreach (T.TableSummary s in field._summarys)
            {
                insertRow(currentPoint, s, scale, trans);

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 4 * scale;
            }
        }


        private static void insertRow(G.Point insertion, T.TableSummary sumData, double scale, _Db.Transaction trans)
        {
            string blockName = "PainutusKokkuvõte";
            string layerName = "K004";

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            _Db.BlockTable blockTable = trans.GetObject(db.BlockTableId, _Db.OpenMode.ForRead) as _Db.BlockTable;
            _Db.BlockTableRecord curSpace = trans.GetObject(db.CurrentSpaceId, _Db.OpenMode.ForWrite) as _Db.BlockTableRecord;

            _Ge.Point3d insertPointBlock = new _Ge.Point3d(insertion.X, insertion.Y, 0);
            using (_Db.BlockReference newBlockReference = new _Db.BlockReference(insertPointBlock, blockTable[blockName]))
            {
                newBlockReference.Layer = layerName;
                curSpace.AppendEntity(newBlockReference);
                trans.AddNewlyCreatedDBObject(newBlockReference, true);
                newBlockReference.TransformBy(_Ge.Matrix3d.Scaling(scale, insertPointBlock));

                _Db.BlockTableRecord blockBlockTable = trans.GetObject(blockTable[blockName], _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
                if (blockBlockTable.HasAttributeDefinitions)
                {
                    foreach (_Db.ObjectId objID in blockBlockTable)
                    {
                        _Db.DBObject obj = trans.GetObject(objID, _Db.OpenMode.ForRead) as _Db.DBObject;

                        if (obj is _Db.AttributeDefinition)
                        {
                            _Db.AttributeDefinition attDef = obj as _Db.AttributeDefinition;

                            if (!attDef.Constant)
                            {
                                using (_Db.AttributeReference attRef = new _Db.AttributeReference())
                                {
                                    attRef.SetAttributeFromBlock(attDef, newBlockReference.BlockTransform);
                                    attRef.Position = attDef.Position.TransformBy(newBlockReference.BlockTransform);
                                    setRowParameters(attRef, sumData);
                                    newBlockReference.AttributeCollection.AppendAttribute(attRef);
                                    trans.AddNewlyCreatedDBObject(attRef, true);
                                }
                            }
                        }
                    }
                }
            }
        }


        private static void setRowParameters(_Db.AttributeReference ar, T.TableSummary sumData)
        {
            if (ar != null)
            {
                if (ar.Tag == "Diam") { ar.TextString = sumData.Text; }
                else if (ar.Tag == "Klass") { ar.TextString = sumData.Material; }
                else if (ar.Tag == "Mass") { ar.TextString = sumData.Weight.ToString("F2").Replace(",", "."); }
                else if (ar.Tag == "Ühik") { ar.TextString = sumData.Units; }
            }
        }

    }
}