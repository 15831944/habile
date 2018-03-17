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
    static class Checker_Outputs
    {
        public static void main(List<T.ErrorPoint> errors)
        {
            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;
            _Db.Transaction trans = db.TransactionManager.StartTransaction();

            try
            {
                errorHandler(errors, trans);

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


        private static void errorHandler(List<T.ErrorPoint> errors, _Db.Transaction trans)
        {
            Universal.writeCadMessage(" ");
            Universal.writeCadMessage("----- VIGADE LOETELU ALGUS -----");
            foreach (T.ErrorPoint e in errors)
            {
                double scale = e.Scale;
                _Ge.Point3d insertPoint = new _Ge.Point3d(e.IP.X, e.IP.Y, 0);
                createCircle(5 * scale, insertPoint, trans);
                createCircle(40 * scale, insertPoint, trans);

                Universal.writeCadMessage(e.ErrorMessage);
            }
            Universal.writeCadMessage("----- VIGADE LOETELU LÕPP -----");
            Universal.writeCadMessage(" ");
            Universal.writeCadMessage("VIGADE ARV - " + errors.Count.ToString());
        }


        private static void createCircle(double radius, _Ge.Point3d ip, _Db.Transaction trans)
        {
            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;
            _Ed.Editor ed = doc.Editor;

            _Db.BlockTableRecord curSpace = trans.GetObject(db.CurrentSpaceId, _Db.OpenMode.ForWrite) as _Db.BlockTableRecord;
            using (_Db.Circle circle = new _Db.Circle())
            {
                circle.Center = ip;
                circle.Radius = radius;
                circle.ColorIndex = 1;
                curSpace.AppendEntity(circle);
                trans.AddNewlyCreatedDBObject(circle, true);
            }
        }

    }
}