#define BRX_APP
//#define ARX_APP

using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
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
    class CENTER_command
    {
        _CONNECTION _c;

        string blockName = "Raskuskese";


        public CENTER_command(ref _CONNECTION c)
        {
            _c = c;
        }


        public void run()
        {
            _Db.DBObjectCollection polys = getGeometry();
            if (polys.Count == 0) throw new DMTException("Valida tuleb polyline tüüpi objekte");
            
            try
            {
                _Db.Region reg = createRegion(polys);
                _Ge.Point3d center = getCenter(reg);

                write("Centroid: " + center.X.ToString() + "," + center.Y.ToString());
                createBlock(center);
            }
            catch
            {
                throw new DMTException("Vigane geomeetria");
            }            
        }
        

        private _Db.DBObjectCollection getGeometry()
        {
            _Db.DBObjectCollection geometry = new _Db.DBObjectCollection();

            _Ed.PromptSelectionOptions opts = new _Ed.PromptSelectionOptions();
            opts.MessageForAdding = "\nSelect Geometry: ";
            _Ed.PromptSelectionResult userSelection = _c.doc.Editor.GetSelection(opts);
            if (userSelection.Status != _Ed.PromptStatus.OK) throw new DMTException("[ERROR] Geometry - cancelled");

            _Ed.SelectionSet selectionSet = userSelection.Value;

            foreach (_Ed.SelectedObject currentObject in selectionSet)
            {
                if (currentObject == null) continue;

                _Db.Entity currentEntity = _c.trans.GetObject(currentObject.ObjectId, _Db.OpenMode.ForRead) as _Db.Entity;
                if (currentEntity == null) continue;

                if (currentEntity is _Db.Curve)
                {
                    geometry.Add(currentEntity);
                }
            }

            return geometry;
        }


        private _Db.Region createRegion(_Db.DBObjectCollection polys)
        {
            _Db.DBObjectCollection regions = new _Db.DBObjectCollection();
            regions = _Db.Region.CreateFromCurves(polys);

            double area = 0;
            int index = 0;

            if (regions.Count > 1)
            {
                for (int i = 0; i < regions.Count; i++)
                {
                    _Db.Region cur = regions[i] as _Db.Region;
                    if (area < cur.Area)
                    {
                        index = i;
                        area = cur.Area;
                    }
                }
            }


            _Db.Region bigRegion = regions[index] as _Db.Region;

            for (int i = 0; i < regions.Count; i++)
            {
                if (i == index) continue;
                _Db.Region cur = regions[i] as _Db.Region;
                bigRegion.BooleanOperation(_Db.BooleanOperationType.BoolSubtract, cur);
            }
            
            return bigRegion;
        }


        private _Ge.Point3d getCenter(_Db.Region reg)
        {
            _Db.Solid3d solid = new _Db.Solid3d();
            solid.Extrude(reg, 2.0, 0.0);
            _Ge.Point3d solidCentroid = solid.MassProperties.Centroid;

            return new _Ge.Point3d(solidCentroid.X, solidCentroid.Y, 0);
        }


        private void createBlock(_Ge.Point3d center)
        {
            if (!_c.blockTable.Has(blockName))
            {
                _Db.BlockTableRecord btr = new _Db.BlockTableRecord();
                btr.Name = blockName;

                _Db.ObjectId btrId = _c.blockTable.Add(btr);
                _c.trans.AddNewlyCreatedDBObject(btr, true);

                createX(btr, 1.5, new _Ge.Point3d(0, 0, 0));
                createCircle(btr, 1, new _Ge.Point3d(0, 0, 0));

                _Db.BlockReference newBlockReference = new _Db.BlockReference(center, btrId);
                newBlockReference.TransformBy(_Ge.Matrix3d.Scaling(40, center));

                _c.modelSpace.AppendEntity(newBlockReference);
                _c.trans.AddNewlyCreatedDBObject(newBlockReference, true);
                write("Create missing block");
            }
            else
            {
                using (_Db.BlockReference newBlockReference = new _Db.BlockReference(center, _c.blockTable[blockName]))
                {
                    newBlockReference.TransformBy(_Ge.Matrix3d.Scaling(40, center));
                    _c.modelSpace.AppendEntity(newBlockReference);
                    _c.trans.AddNewlyCreatedDBObject(newBlockReference, true);
                }
            }
        }


        private void createX(_Db.BlockTableRecord btr, double radius, _Ge.Point3d ip)
        {
            _Ge.Point2d p1 = new _Ge.Point2d(ip.X - radius, ip.Y);
            _Ge.Point2d p2 = new _Ge.Point2d(ip.X + radius, ip.Y);
            _Ge.Point2d p3 = new _Ge.Point2d(ip.X, ip.Y - radius);
            _Ge.Point2d p4 = new _Ge.Point2d(ip.X, ip.Y + radius);

            using (_Db.Polyline poly = new _Db.Polyline())
            {
                poly.AddVertexAt(0, p1, 0, 0, 0);
                poly.AddVertexAt(1, p2, 0, 0, 0);
                poly.ColorIndex = 2;

                btr.AppendEntity(poly);
                _c.trans.AddNewlyCreatedDBObject(poly, true);
            }

            using (_Db.Polyline poly = new _Db.Polyline())
            {
                poly.AddVertexAt(0, p3, 0, 0, 0);
                poly.AddVertexAt(1, p4, 0, 0, 0);
                poly.ColorIndex = 2;

                btr.AppendEntity(poly);
                _c.trans.AddNewlyCreatedDBObject(poly, true);
            }
        }


        private void createCircle(_Db.BlockTableRecord btr, double radius, _Ge.Point3d ip)
        {
            using (_Db.Circle circle = new _Db.Circle())
            {
                circle.Center = ip;
                circle.Radius = radius;
                circle.ColorIndex = 2;
                btr.AppendEntity(circle);
                _c.trans.AddNewlyCreatedDBObject(circle, true);
            }
        }


        private void write(string message)
        {
            _c.ed.WriteMessage("\n" + message);
        }

    }
}