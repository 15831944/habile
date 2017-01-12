using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

//ODA
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Teigha.Geometry;

//Bricsys
using Bricscad.ApplicationServices;
using Bricscad.Runtime;
using Bricscad.EditorInput;

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;
using T = Logic_Tabler;

namespace commands
{
    static class Summar_Outputs
    {
        public static void main(List<T.DrawingArea> fields)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            Transaction trans = db.TransactionManager.StartTransaction();
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
                Universal.writeCadMessage("Program stopped with ERROR: " + ex.TargetSite);
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

        private static void insertRow(G.Point insertion, T.TableSummary sumData, double scale, Transaction trans)
        {
            string blockName = "PainutusKokkuvõte";
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

        private static void setRowParameters(AttributeReference ar, T.TableSummary sumData)
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
