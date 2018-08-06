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
    partial class TABLE_command
    {
        public void bending_output(List<T.DrawingArea> fields, string tableBendingRow)
        {
            foreach (T.DrawingArea f in fields)
            {
                if (f.Valid)
                {
                    generateBendingTable(f, tableBendingRow);
                }
                else
                {
                    write(f.Reason);
                }
            }
        }


        private void generateBendingTable(T.DrawingArea field, string tableBendingRow)
        {
            G.Point insertPoint = field._tableHeads[0].IP;
            double scale = field._tableHeads[0].Scale;

            G.Point currentPoint = field._tableHeads[0].IP;
            double delta = scale * 14;

            currentPoint.X = insertPoint.X;
            currentPoint.Y -= delta;

            foreach (T.ErrorPoint e in field._errors)
            {
                write(e.ErrorMessage);
            }
            
            foreach (T.TableBendingRow b in field._rows)
            {
                insertRow(currentPoint, b, scale, tableBendingRow);

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 4 * scale;
            }
        }


        private void insertRow(G.Point insertion, T.TableBendingRow rowData, double scale, string tableBendingRow)
        {
            _Ge.Point3d insertPointBlock = new _Ge.Point3d(insertion.X, insertion.Y, 0);
            using (_Db.BlockReference newBlockReference = new _Db.BlockReference(insertPointBlock, _c.blockTable[tableBendingRow]))
            {
                newBlockReference.Layer = bendingLayer;
                _c.modelSpace.AppendEntity(newBlockReference);
                _c.trans.AddNewlyCreatedDBObject(newBlockReference, true);
                newBlockReference.TransformBy(_Ge.Matrix3d.Scaling(scale, insertPointBlock));

                _Db.BlockTableRecord blockBlockTable = _c.trans.GetObject(_c.blockTable[tableBendingRow], _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
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
                                    setRowParameters(attRef, rowData);
                                    newBlockReference.AttributeCollection.AppendAttribute(attRef);
                                    _c.trans.AddNewlyCreatedDBObject(attRef, true);
                                }
                            }
                        }
                    }
                }
            }
        }


        private void setRowParameters(_Db.AttributeReference ar, T.TableBendingRow rowData)
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