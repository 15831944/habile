//using System;
//using System.Text;
//using System.Collections;
//using System.Linq;
//using System.IO;
//using System.Diagnostics;
//using System.Collections.Generic;
//using SWF = System.Windows.Forms;
//using DR = System.Drawing;

//using Autodesk.AutoCAD.Runtime;
//using Autodesk.AutoCAD.ApplicationServices;
//using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.Geometry;
//using Autodesk.AutoCAD.EditorInput;

//using R = Reinforcement;
//using G = Geometry;
//using L = Logic_Reinf;
//using T = Logic_Tabler;

<<<<<<< HEAD
//namespace commands
//{
//    static class Checker_Outputs
//    {
//        public static void writeCadMessage(string errorMessage)
//        {
//            Document doc = Application.DocumentManager.MdiActiveDocument;
//            Editor ed = doc.Editor;

//            ed.WriteMessage("\n" + errorMessage);
//        }

//        public static void main(List<T.ErrorPoint> errors)
//        {
//            Document doc = Application.DocumentManager.MdiActiveDocument;
//            Database db = doc.Database;
=======
namespace commands
{
    static class Checker_Outputs
    {
        public static void main(List<T.ErrorPoint> errors)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
>>>>>>> f9e896c1ac9c93a01b3c4525c361f2086cbb152b

//            Transaction trans = db.TransactionManager.StartTransaction();
//            try
//            {
//                errorHandler(errors, trans);

<<<<<<< HEAD
//                trans.Commit();
//            }
//            catch (System.Exception ex)
//            {
//                writeCadMessage("Program stopped with ERROR: " + ex.TargetSite);
//            }
//            finally
//            {
//                trans.Dispose();
//            }
//        }

//        private static void errorHandler(List<T.ErrorPoint> errors, Transaction trans)
//        {
//            writeCadMessage(" ");
//            writeCadMessage("----- VIGADE LOETELU ALGUS -----");
//            foreach (T.ErrorPoint e in errors)
//            {
//                double scale = e.Scale;
//                Point3d insertPoint = new Point3d(e.IP.X, e.IP.Y, 0);
//                createCircle(5 * scale, insertPoint, trans);
//                createCircle(40 * scale, insertPoint, trans);

//                writeCadMessage(e.ErrorMessage);
//            }
//            writeCadMessage("----- VIGADE LOETELU LÕPP -----");
//            writeCadMessage(" ");
//            writeCadMessage("VIGADE ARV - " + errors.Count.ToString());
//        }
=======
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

        private static void errorHandler(List<T.ErrorPoint> errors, Transaction trans)
        {
            Universal.writeCadMessage(" ");
            Universal.writeCadMessage("----- VIGADE LOETELU ALGUS -----");
            foreach (T.ErrorPoint e in errors)
            {
                double scale = e.Scale;
                Point3d insertPoint = new Point3d(e.IP.X, e.IP.Y, 0);
                createCircle(5 * scale, insertPoint, trans);
                createCircle(40 * scale, insertPoint, trans);

                Universal.writeCadMessage(e.ErrorMessage);
            }
            Universal.writeCadMessage("----- VIGADE LOETELU LÕPP -----");
            Universal.writeCadMessage(" ");
            Universal.writeCadMessage("VIGADE ARV - " + errors.Count.ToString());
        }
>>>>>>> f9e896c1ac9c93a01b3c4525c361f2086cbb152b

//        private static void createCircle(double radius, Point3d ip, Transaction trans)
//        {
//            Document doc = Application.DocumentManager.MdiActiveDocument;
//            Database db = doc.Database;
//            Editor ed = doc.Editor;

//            BlockTableRecord curSpace = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
//            using (Circle circle = new Circle())
//            {
//                circle.Center = ip;
//                circle.Radius = radius;
//                circle.ColorIndex = 1;
//                curSpace.AppendEntity(circle);
//                trans.AddNewlyCreatedDBObject(circle, true);
//            }
//        }
//    }
//}