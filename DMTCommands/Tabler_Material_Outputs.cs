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
        public void material_output(List<T.DrawingArea> fields, string tableMaterialRow)
        {
            foreach (T.DrawingArea f in fields)
            {
                if (f.Valid)
                {
                    generateMaterialTable(f, tableMaterialRow);
                }
                else
                {
                    write(f.Reason);
                }
            }
        }


        private void generateMaterialTable(T.DrawingArea field, string tableMaterialRow)
        {
            G.Point insertPoint = field.getSummaryInsertionPoint();
            double scale = field._tableHeads[0].Scale;

            G.Point currentPoint = field.getSummaryInsertionPoint();
            double delta = scale * 4;

            currentPoint.X = insertPoint.X;
            currentPoint.Y -= delta;

            foreach (T.TableMaterialRow s in field._summarys)
            {
                insertRow(currentPoint, s, scale, tableMaterialRow);

                currentPoint.X = insertPoint.X;
                currentPoint.Y -= 4 * scale;
            }
        }


        private void insertRow(G.Point insertion, T.TableMaterialRow sumData, double scale, string tableMaterialRow)
        {
            _Ge.Point3d insertPointBlock = new _Ge.Point3d(insertion.X, insertion.Y, 0);
            using (_Db.BlockReference newBlockReference = new _Db.BlockReference(insertPointBlock, _c.blockTable[tableMaterialRow]))
            {
                newBlockReference.Layer = materialLayer;
                _c.modelSpace.AppendEntity(newBlockReference);
                _c.trans.AddNewlyCreatedDBObject(newBlockReference, true);
                newBlockReference.TransformBy(_Ge.Matrix3d.Scaling(scale, insertPointBlock));

                _Db.BlockTableRecord blockBlockTable = _c.trans.GetObject(_c.blockTable[tableMaterialRow], _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
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
                                    setRowParameters(attRef, sumData);
                                    newBlockReference.AttributeCollection.AppendAttribute(attRef);
                                    _c.trans.AddNewlyCreatedDBObject(attRef, true);
                                }
                            }
                        }
                    }
                }
            }
        }


        private void setRowParameters(_Db.AttributeReference ar, T.TableMaterialRow sumData)
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